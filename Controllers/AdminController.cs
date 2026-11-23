using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

// Toda la administración se protege en el servidor; ocultar botones por sí solo no sería un control de acceso.
[Authorize(Roles = "Admin")]
public class AdminController(ApplicationDbContext db, IWebHostEnvironment environment, HeroConfigurationService heroConfigurations, WorldPoemService poems) : Controller
{
    private static readonly Dictionary<string, string> Titles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["heroes"] = "Héroes", ["monsters"] = "Monstruos", ["equipment"] = "Equipos y Artefactos",
        ["miscellanea"] = "Miscelánea", ["abilities"] = "Habilidades", ["spells"] = "Hechizos del Terror",
        ["poems"] = "Versos y Leyendas"
    };

    [HttpGet]
    public Task<IActionResult> Index(string type) => BuildLibraryPage(type);

    [HttpGet]
    public IActionResult Archive(string type) => RedirectToAction(nameof(Index), new { type });

    private async Task<IActionResult> BuildLibraryPage(string type)
    {
        if (!Titles.TryGetValue(type, out var title)) return BadRequest();
        var rows = await LoadCatalogRowsAsync(type.ToLowerInvariant());
        var comparer = StringComparer.Create(System.Globalization.CultureInfo.GetCultureInfo("es-ES"), ignoreCase: true);
        var active = rows.Where(row => !row.IsArchived).OrderBy(row => row.Id).ToList();
        var archived = rows.Where(row => row.IsArchived).OrderBy(row => row.Id).ToList();
        var allRows = active.Concat(archived).ToList();
        var createLabel = type.ToLowerInvariant() switch
        {
            "heroes" => "Añadir héroe", "monsters" => "Añadir monstruo",
            "equipment" => "Añadir equipo o artefacto", "miscellanea" => "Añadir objeto",
            "abilities" => "Añadir habilidad", "spells" => "Añadir hechizo", "poems" => "Añadir verso",
            _ => "Añadir ficha"
        };
        var classifications = allRows.Select(row => row.Classification);
        if (type.Equals("heroes", StringComparison.OrdinalIgnoreCase))
        {
            // El filtro ofrece todos los jugadores creados, aunque alguno todavía no tenga héroe asignado.
            classifications = classifications.Concat(await db.PlayerProfiles.AsNoTracking().Select(player => player.Name).ToListAsync());
        }

        ViewBag.LibraryGuardian = await poems.FindAsync("library-guardian");
        return View("Index", new CatalogListViewModel
        {
            Type = type.ToLowerInvariant(), Title = title, CreateLabel = createLabel,
            Items = active, ArchivedItems = archived,
            ExpansionOptions = allRows.SelectMany(row => row.Expansion.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .Distinct(comparer).OrderBy(value => value, comparer).ToList(),
            ClassificationLabel = type.Equals("heroes", StringComparison.OrdinalIgnoreCase) ? "Jugador" : "Categoría / clase",
            ClassificationOptions = classifications.Where(value => !string.IsNullOrWhiteSpace(value)).SelectMany(value => value.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .Distinct(comparer).OrderBy(value => value, comparer).ToList()
        });
    }

    // Cada tabla se convierte a una ficha de biblioteca común, conservando texto para búsqueda y filtros.
    private async Task<List<CatalogListItem>> LoadCatalogRowsAsync(string type)
    {
        switch (type)
        {
            case "heroes":
                var playerNamesByHero = await db.HeroConfigurations.AsNoTracking().Where(configuration => configuration.IsActive)
                    .Select(configuration => new { configuration.HeroId, configuration.PlayerProfile.Name }).ToListAsync();
                var groupedPlayers = playerNamesByHero.GroupBy(item => item.HeroId)
                    .ToDictionary(group => group.Key, group => string.Join('|', group.Select(item => item.Name).Distinct(StringComparer.OrdinalIgnoreCase)));
                return (await db.Heroes.AsNoTracking().ToListAsync()).Select(hero =>
                    new CatalogListItem(hero.Id, hero.Name, $"{hero.Hp} HP · D{hero.DicesMind} MND · D{hero.DicesAttack} ATK · D{hero.DicesDefend} DFC", hero.IsArchived,
                        $"{hero.Name} {hero.CharacterTypeClass} {groupedPlayers.GetValueOrDefault(hero.Id)} {hero.Description} {hero.Spells} {hero.Appears.DisplayName()}",
                        hero.Appears.DisplayName(), groupedPlayers.GetValueOrDefault(hero.Id) ?? "")).ToList();
            case "monsters":
                return (await db.Monsters.AsNoTracking().ToListAsync()).Select(monster =>
                    new CatalogListItem(monster.Id, monster.Name, $"{monster.Hp} HP · D{monster.DicesMind} MND · D{monster.DicesAttack} ATK · D{monster.DicesDefend} DFC", monster.IsArchived,
                        $"{monster.Name} {monster.Faction} {monster.Ability} {monster.Description} {monster.Appears.DisplayName()}",
                        monster.Appears.DisplayName(), monster.Faction)).ToList();
            case "equipment":
                return (await db.EquipmentsAndArtifacts.AsNoTracking().ToListAsync()).Select(item =>
                    new CatalogListItem(item.Id, item.Name, EquipmentDetails(item), item.IsArchived,
                        $"{item.Name} {item.Category} {item.Type} {item.Description} {item.Weight} {item.Appears.DisplayName()}",
                        item.Appears.DisplayName(), item.Category.Equals("Artifact", StringComparison.OrdinalIgnoreCase) ? "Artefacto" : "Equipo")).ToList();
            case "miscellanea":
                return (await db.Miscellanea.AsNoTracking().ToListAsync()).Select(item =>
                    new CatalogListItem(item.Id, item.Name, MiscellaneaDetails(item), item.IsArchived,
                        $"{item.Name} {item.Description} {item.Appears.DisplayName()} {item.Category.DisplayName()}", item.Appears.DisplayName(), item.Category.DisplayName())).ToList();
            case "abilities":
                return (await db.MonsterAbilities.AsNoTracking().ToListAsync()).Select(item =>
                    new CatalogListItem(item.Id, item.Name, item.Appears.DisplayName(), item.IsArchived,
                        $"{item.Name} {item.Description} {item.Appears.DisplayName()}", item.Appears.DisplayName(), "Habilidad")).ToList();
            case "spells":
                return (await db.SpellsOfTerror.AsNoTracking().ToListAsync()).Select(item =>
                    new CatalogListItem(item.Id, item.Name, item.Type, item.IsArchived,
                        $"{item.Name} {item.Type} {item.Description} {item.Appears.DisplayName()}", item.Appears.DisplayName(), item.Type)).ToList();
            case "poems":
                return (await db.WorldPoems.AsNoTracking().ToListAsync()).Select(poem =>
                    new CatalogListItem(poem.Id, poem.Title, poem.Key, poem.IsArchived,
                        $"{poem.Title} {poem.Key} {poem.Text}", "", "Verso")).ToList();
            default:
                return [];
        }
    }

    private static string EquipmentDetails(EquipmentsAndArtifacts item)
    {
        var effect = item.WeaponAttackDice is int attack ? $"D{attack} ATK" : item.DefenseBonus != 0 ? $"{(item.DefenseBonus > 0 ? "+" : "")}{item.DefenseBonus} DFC" : item.Type;
        return item.Price > 0 ? $"{effect} · {item.Price} monedas" : effect;
    }

    private static string MiscellaneaDetails(MiscellaneaItem item)
    {
        var effect = item.WeaponAttackDice is int attack ? $"D{attack} ATK" : item.DefenseBonus != 0 ? $"{(item.DefenseBonus > 0 ? "+" : "")}{item.DefenseBonus} DFC"
            : item.HealthBonus != 0 ? $"{(item.HealthBonus > 0 ? "+" : "")}{item.HealthBonus} HP" : item.MindBonus != 0 ? $"{(item.MindBonus > 0 ? "+" : "")}{item.MindBonus} MND" : item.Category.DisplayName();
        return item.Price > 0 ? $"{effect} · {item.Price} monedas" : effect;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string type, int? id, int? configurationId)
    {
        if (!Titles.ContainsKey(type)) return BadRequest();
        CatalogEditViewModel? model;
        if (type.Equals("heroes", StringComparison.OrdinalIgnoreCase) && id is not null)
        {
            var hero = await db.Heroes.FindAsync(id.Value);
            if (hero is null) return NotFound();
            var configuration = configurationId is null ? null : await db.HeroConfigurations.Include(item => item.PlayerProfile)
                .FirstOrDefaultAsync(item => item.Id == configurationId.Value && item.HeroId == id.Value);
            if (configurationId is not null && configuration is null) return NotFound();
            model = await heroConfigurations.BuildEditorAsync(hero, configuration, personal: false);
        }
        else model = id is null ? new CatalogEditViewModel { Type = type.ToLowerInvariant() } : await LoadEditModel(type, id.Value);
        if (model is null) return NotFound();
        if (model.Type == "heroes" && model.Id is int heroId && model.ConfigurationTabs.Count == 0)
        {
            model.ConfigurationTabs = await db.HeroConfigurations.AsNoTracking().Where(item => item.HeroId == heroId)
                .Include(item => item.PlayerProfile).OrderBy(item => item.PlayerProfile.Name)
                .Select(item => new HeroConfigurationTab(item.Id, item.PlayerProfile.Name, item.CharacterName, item.IsActive)).ToListAsync();
        }
        await PopulateEditorOptionsAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(CatalogEditViewModel model)
    {
        if (!Titles.ContainsKey(model.Type)) return BadRequest();
        if (!ModelState.IsValid) return await ReturnEditorAsync(model);
        if (model.Type == "equipment" && NormalizeCategory(model.Category).Length == 0)
        {
            ModelState.AddModelError(nameof(model.Category), "Elige Equipo o Artefacto.");
            return await ReturnEditorAsync(model);
        }
        if (!await ValidateSelectionsAsync(model.Type, "AppearsOptions", model.AppearsOptions)
            || (model.AbilityOptions.Length > 0 && !await ValidateSelectionsAsync(model.Type, "AbilityOptions", model.AbilityOptions))
            || (model.Type == "miscellanea" && model.MiscellaneaCategoryOptions.Length > 0 && !await ValidateSelectionsAsync(model.Type, "MiscellaneaCategoryOptions", model.MiscellaneaCategoryOptions)))
        {
            ModelState.AddModelError(string.Empty, "Elige los valores desde las sugerencias del catálogo.");
            return await ReturnEditorAsync(model);
        }
        if (model.Type == "heroes")
        {
            var ruleHero = new Heroe { CharacterTypeClass = model.CharacterTypeClass, Description = model.Description };
            var loadoutError = await heroConfigurations.ValidateLoadoutAsync(ruleHero, model.WeaponOptions, model.ArmorOptions, model.ItemOptions);
            if (loadoutError is not null) { ModelState.AddModelError(string.Empty, loadoutError); return await ReturnEditorAsync(model); }
        }
        // El texto de expansiones se convierte aquí a banderas del enum antes de guardar la entidad.
        var appearance = TypeAppearsParser.Parse(string.Join(", ", model.AppearsOptions));
        var isNewRecord = model.Id is null;

        if (model.Type.Equals("poems", StringComparison.OrdinalIgnoreCase)
            && (string.IsNullOrWhiteSpace(model.Key) || model.Key.Length > 64
                || !System.Text.RegularExpressions.Regex.IsMatch(model.Key, "^[a-z0-9-]+$")))
        {
            ModelState.AddModelError(nameof(model.Key), "Usa una clave breve con letras minúsculas, números y guiones.");
            return await ReturnEditorAsync(model);
        }

        switch (model.Type)
        {
            case "heroes":
                var hero = model.Id is null ? new Heroe() : await db.Heroes.FindAsync(model.Id.Value) ?? new Heroe();
                if (model.Id is null) { hero.Id = await NextCharacterId(isHero: true); db.Heroes.Add(hero); }
                hero.Name = model.Name.Trim(); hero.CharacterTypeClass = model.CharacterTypeClass;
                hero.Hp = model.Hp; hero.DicesMind = model.DicesMind; hero.DicesAttack = model.DicesAttack; hero.DicesDefend = model.DicesDefend;
                hero.DicesRedMovement = model.DicesRedMovement; hero.Description = model.Description; hero.Appears = appearance;
                hero.Weapon = HeroConfigurationService.JoinNames(model.WeaponOptions.Length > 0 ? model.WeaponOptions : HeroConfigurationService.SplitNames(model.Weapon));
                hero.Armor = HeroConfigurationService.JoinNames(model.ArmorOptions.Length > 0 ? model.ArmorOptions : HeroConfigurationService.SplitNames(model.Armor));
                hero.Spells = HeroConfigurationService.JoinNames(model.AbilityOptions);
                hero.Items = HeroConfigurationService.JoinNames(model.ItemOptions.Length > 0 ? model.ItemOptions : HeroConfigurationService.SplitNames(model.Items));
                hero.Gold = model.Gold; hero.QuestsCompleted = model.QuestsCompleted; hero.IsArchived = model.IsArchived; break;
            case "monsters":
                var monster = model.Id is null ? new Monster() : await db.Monsters.FindAsync(model.Id.Value) ?? new Monster();
                if (model.Id is null) { monster.Id = await NextCharacterId(isHero: false); db.Monsters.Add(monster); }
                monster.Name = model.Name.Trim(); monster.CharacterTypeClass = model.Name.Trim(); monster.Hp = model.Hp;
                monster.DicesMind = model.DicesMind; monster.DicesAttack = model.DicesAttack; monster.DicesDefend = model.DicesDefend;
                monster.MaxMovement = model.MaxMovement; monster.Faction = model.Faction; monster.Ability = string.Join(", ", model.AbilityOptions);
                monster.Description = model.Description; monster.Appears = appearance; monster.IsArchived = model.IsArchived;
                monster.Attack = model.DicesAttack; monster.Defense = model.DicesDefend; break;
            case "equipment":
                var item = model.Id is null ? new EquipmentsAndArtifacts(0, "", "", 0, "", TypeAppears.None, "", "Equipment") : await db.EquipmentsAndArtifacts.FindAsync(model.Id.Value) ?? new EquipmentsAndArtifacts(0, "", "", 0, "", TypeAppears.None, "", "Equipment");
                if (model.Id is null) { item.Id = await db.EquipmentsAndArtifacts.Select(x => (int?)x.Id).MaxAsync() + 1 ?? 1; db.EquipmentsAndArtifacts.Add(item); }
                item.Name = model.Name.Trim(); item.Description = model.Description; item.Price = model.Price; item.Type = model.ItemType;
                item.Weight = model.Weight; item.Category = NormalizeCategory(model.Category); item.Appears = appearance; item.IsArchived = model.IsArchived; EquipmentEffectParser.Apply(item); break;
            case "miscellanea":
                var entry = model.Id is null ? new MiscellaneaItem(0, "", "", 0, TypeAppears.None) : await db.Miscellanea.FindAsync(model.Id.Value) ?? new MiscellaneaItem(0, "", "", 0, TypeAppears.None);
                if (model.Id is null) { entry.Id = await db.Miscellanea.Select(x => (int?)x.Id).MaxAsync() + 1 ?? 1; db.Miscellanea.Add(entry); }
                entry.Name = model.Name.Trim(); entry.Description = model.Description; entry.Price = model.Price; entry.Appears = appearance; entry.Category = ParseMiscellaneaCategory(model.MiscellaneaCategoryOptions); entry.IsArchived = model.IsArchived; EquipmentEffectParser.Apply(entry); break;
            case "abilities":
                var ability = model.Id is null ? new MonsterAbility(0, "", "", TypeAppears.None) : await db.MonsterAbilities.FindAsync(model.Id.Value) ?? new MonsterAbility(0, "", "", TypeAppears.None);
                if (model.Id is null) { ability.Id = await db.MonsterAbilities.Select(x => (int?)x.Id).MaxAsync() + 1 ?? 1; db.MonsterAbilities.Add(ability); }
                ability.Name = model.Name.Trim(); ability.Description = model.Description; ability.Appears = appearance; ability.IsArchived = model.IsArchived; break;
            case "spells":
                var spell = model.Id is null ? new SpellOfTerror(0, "", "", "", TypeAppears.None) : await db.SpellsOfTerror.FindAsync(model.Id.Value) ?? new SpellOfTerror(0, "", "", "", TypeAppears.None);
                if (model.Id is null) { spell.Id = await db.SpellsOfTerror.Select(x => (int?)x.Id).MaxAsync() + 1 ?? 1; db.SpellsOfTerror.Add(spell); }
                spell.Name = model.Name.Trim(); spell.Description = model.Description; spell.Type = model.SpellType; spell.Appears = appearance; spell.IsArchived = model.IsArchived; break;
            case "poems":
                var poem = model.Id is null ? new WorldPoem() : await db.WorldPoems.FindAsync(model.Id.Value) ?? new WorldPoem();
                var poemKey = model.Key.Trim().ToLowerInvariant();
                if (await db.WorldPoems.AnyAsync(x => x.Key == poemKey && x.Id != (model.Id ?? 0)))
                {
                    ModelState.AddModelError(nameof(model.Key), "Ya existe un verso con esa clave.");
                    return await ReturnEditorAsync(model);
                }
                if (model.Id is null) db.WorldPoems.Add(poem);
                poem.Key = poemKey; poem.Title = model.Name.Trim(); poem.Text = model.Description; poem.IsArchived = model.IsArchived; break;
        }

        await db.SaveChangesAsync();
        TempData["GuardianNotice"] = await poems.MessageAsync(isNewRecord ? "library-added" : "library-edited", model.Name);
        return RedirectToAction(nameof(Index), new { type = model.Type });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateField(string type, int id, string field, string? value, string[]? values)
    {
        if (type.Equals("hero-configuration", StringComparison.OrdinalIgnoreCase))
        {
            var configuration = await db.HeroConfigurations.FirstOrDefaultAsync(item => item.Id == id);
            if (configuration is null) return NotFound();
            var error = await heroConfigurations.UpdateAsync(configuration, field, value, values ?? []);
            if (error is not null) return BadRequest(error);
            var hero = await db.Heroes.FindAsync(configuration.HeroId);
            if (hero is null) return NotFound();
            var editor = await heroConfigurations.BuildEditorAsync(hero, configuration, personal: false);
            return Json(new
            {
                success = true,
                message = await poems.MessageAsync("library-edited", configuration.CharacterName),
                stats = new { hp = editor.DisplayHp, mind = editor.DisplayMind, attack = editor.DisplayAttack, defense = editor.DisplayDefense, movement = editor.DicesRedMovement },
                marks = editor.ComparisonMarks
            });
        }
        // Cada solicitud solo puede actualizar un campo incluido en esta lista explícita.
        var allowed = GetEditableFields(type);
        if (allowed is null || !allowed.TryGetValue(field, out var propertyName)) return BadRequest();
        var submittedChoices = field is "AppearsOptions" or "AbilityOptions" or "MiscellaneaCategoryOptions" ? values ?? [] : [value ?? string.Empty];
        if (!await ValidateSelectionsAsync(type, field, submittedChoices)) return BadRequest("Selecciona un valor de la lista sugerida.");

        object? entity = type.ToLowerInvariant() switch
        {
            "heroes" => await db.Heroes.FindAsync(id),
            "monsters" => await db.Monsters.FindAsync(id),
            "equipment" => await db.EquipmentsAndArtifacts.FindAsync(id),
            "miscellanea" => await db.Miscellanea.FindAsync(id),
            "abilities" => await db.MonsterAbilities.FindAsync(id),
            "spells" => await db.SpellsOfTerror.FindAsync(id),
            "poems" => await db.WorldPoems.FindAsync(id),
            _ => null
        };
        if (entity is null) return NotFound();

        var property = entity.GetType().GetProperty(propertyName);
        if (property is null || !property.CanWrite) return BadRequest();
        if (propertyName == "Appears") property.SetValue(entity, TypeAppearsParser.Parse(string.Join(", ", values ?? [])));
        else if (field == "AbilityOptions") property.SetValue(entity, string.Join(", ", values ?? []));
        else if (field is "WeaponOptions" or "ArmorOptions" or "ItemOptions")
        {
            if (entity is not Heroe heroEntity) return BadRequest();
            var weapons = field == "WeaponOptions" ? values ?? [] : HeroConfigurationService.SplitNames(heroEntity.Weapon);
            var armors = field == "ArmorOptions" ? values ?? [] : HeroConfigurationService.SplitNames(heroEntity.Armor);
            var items = field == "ItemOptions" ? values ?? [] : HeroConfigurationService.SplitNames(heroEntity.Items);
            var validation = await heroConfigurations.ValidateLoadoutAsync(heroEntity, weapons, armors, items);
            if (validation is not null) return BadRequest(validation);
            property.SetValue(entity, HeroConfigurationService.JoinNames(values ?? []));
        }
        else if (field == "MiscellaneaCategoryOptions" && entity is MiscellaneaItem miscellaneaCategoryItem) miscellaneaCategoryItem.Category = ParseMiscellaneaCategory(values ?? []);
        else if (property.PropertyType == typeof(int))
        {
            if (!int.TryParse(value, out var number) || number < 0) return BadRequest("El valor debe ser un número positivo.");
            property.SetValue(entity, number);
        }
        else if (property.PropertyType == typeof(MiscellaneaCategory))
        {
            property.SetValue(entity, ParseMiscellaneaCategory(values ?? []));
        }
        else if (property.PropertyType == typeof(string))
        {
            var text = (value ?? string.Empty).Trim();
            if (field == "Key")
            {
                text = text.ToLowerInvariant();
                if (text.Length is 0 or > 64 || !System.Text.RegularExpressions.Regex.IsMatch(text, "^[a-z0-9-]+$")
                    || await db.WorldPoems.AnyAsync(poem => poem.Key == text && poem.Id != id)) return BadRequest("La clave debe ser única y usar minúsculas, números o guiones.");
            }
            if (field == "Category") text = NormalizeCategory(text);
            if (field == "Category" && text.Length == 0) return BadRequest();
            if (text.Length > 2000 || (field == "Name" && (text.Length == 0 || text.Length > 120))) return BadRequest();
            property.SetValue(entity, text);
            if (entity is Monster monster && field == "Name") monster.CharacterTypeClass = text;
            if (entity is EquipmentsAndArtifacts equipment && field == "Description") EquipmentEffectParser.Apply(equipment);
            if (entity is MiscellaneaItem miscellaneaItem && field == "Description") EquipmentEffectParser.Apply(miscellaneaItem);
        }
        else return BadRequest();

        await db.SaveChangesAsync();
        var recordName = entity switch
        {
            Heroe hero => hero.Name, Monster monster => monster.Name, EquipmentsAndArtifacts equipment => equipment.Name,
            MiscellaneaItem item => item.Name, MonsterAbility ability => ability.Name, SpellOfTerror spell => spell.Name,
            WorldPoem poem => poem.Title, _ => "la ficha"
        };
        return Json(new { success = true, message = await poems.MessageAsync("library-edited", recordName) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetCharacterDefaults(string type, int id)
    {
        if (type.Equals("heroes", StringComparison.OrdinalIgnoreCase))
        {
            var hero = await db.Heroes.FindAsync(id);
            if (hero is null) return NotFound();
            var source = FindCsvRecord("HeroQuest_Heroes.csv", "Name", hero.SourceName ?? hero.Name);
            if (source is null) return NotFound("No existe una ficha de referencia para este héroe.");

            hero.Name = CsvRecordReader.GetText(source, "Name");
            hero.CharacterTypeClass = CsvRecordReader.GetText(source, "CharacterTypeClass");
            hero.Hp = CsvRecordReader.GetInteger(source, "Hp");
            hero.DicesMind = CsvRecordReader.GetInteger(source, "DicesMind");
            hero.DicesAttack = CsvRecordReader.GetInteger(source, "DicesAttack");
            hero.DicesDefend = CsvRecordReader.GetInteger(source, "DicesDefend");
            hero.DicesRedMovement = CsvRecordReader.GetInteger(source, "DicesRedMovement");
            hero.Description = CsvRecordReader.GetText(source, "Description");
            hero.Weapon = CsvRecordReader.GetText(source, "Weapon");
            hero.Armor = CsvRecordReader.GetText(source, "Armor");
            hero.Spells = CsvRecordReader.GetText(source, "Spells");
            hero.Items = CsvRecordReader.GetText(source, "Items");
            hero.Gold = CsvRecordReader.GetInteger(source, "Gold");
            hero.QuestsCompleted = CsvRecordReader.GetInteger(source, "QuestsCompleted");
            hero.Appears = TypeAppearsParser.Parse(CsvRecordReader.GetText(source, "Appearances"));
        }
        else if (type.Equals("monsters", StringComparison.OrdinalIgnoreCase))
        {
            var monster = await db.Monsters.FindAsync(id);
            if (monster is null) return NotFound();
            var source = FindCsvRecord("HeroQuest_MonstersCategoriesAbilities.csv", "Nombre", monster.SourceName ?? monster.Name);
            if (source is null) return NotFound("No existe una ficha de referencia para este monstruo.");

            monster.Name = CsvRecordReader.GetText(source, "Nombre");
            monster.CharacterTypeClass = monster.Name;
            monster.MaxMovement = CsvRecordReader.GetInteger(source, "Movimiento");
            monster.DicesAttack = monster.Attack = CsvRecordReader.GetInteger(source, "Ataque");
            monster.DicesDefend = monster.Defense = CsvRecordReader.GetInteger(source, "Defensa");
            monster.Hp = CsvRecordReader.GetInteger(source, "Cuerpo");
            monster.DicesMind = CsvRecordReader.GetInteger(source, "Mente");
            monster.Ability = MonsterCsvParsing.CleanAbilityNames(CsvRecordReader.GetText(source, "Habilidad"));
            monster.Faction = CsvRecordReader.GetText(source, "Categoria");
            monster.Appears = TypeAppearsParser.Parse(CsvRecordReader.GetText(source, "Apariciones"));
        }
        else return BadRequest();

        await db.SaveChangesAsync();
        TempData["CharacterResetMessage"] = "Se han restaurado los valores de referencia del personaje desde el CSV oficial del proyecto.";
        return RedirectToAction(nameof(Edit), new { type, id });
    }

    private static Dictionary<string, string>? GetEditableFields(string type) => type.ToLowerInvariant() switch
    {
        "heroes" => new() { ["Name"] = "Name", ["Description"] = "Description", ["CharacterTypeClass"] = "CharacterTypeClass", ["Hp"] = "Hp", ["DicesMind"] = "DicesMind", ["DicesAttack"] = "DicesAttack", ["DicesDefend"] = "DicesDefend", ["DicesRedMovement"] = "DicesRedMovement", ["WeaponOptions"] = "Weapon", ["ArmorOptions"] = "Armor", ["AbilityOptions"] = "Spells", ["ItemOptions"] = "Items", ["Gold"] = "Gold", ["QuestsCompleted"] = "QuestsCompleted", ["AppearsOptions"] = "Appears" },
        "hero-configuration" => new() { ["Name"] = "CharacterName", ["WeaponOptions"] = "Weapons", ["ArmorOptions"] = "Armors", ["ItemOptions"] = "Items", ["AbilityOptions"] = "Abilities", ["MovementDice"] = "MovementDice", ["Gold"] = "Gold", ["QuestsCompleted"] = "QuestsCompleted" },
        "monsters" => new() { ["Name"] = "Name", ["Description"] = "Description", ["Faction"] = "Faction", ["AbilityOptions"] = "Ability", ["Hp"] = "Hp", ["DicesMind"] = "DicesMind", ["DicesAttack"] = "DicesAttack", ["DicesDefend"] = "DicesDefend", ["MaxMovement"] = "MaxMovement", ["AppearsOptions"] = "Appears" },
        "equipment" => new() { ["Name"] = "Name", ["Description"] = "Description", ["Price"] = "Price", ["ItemType"] = "Type", ["Weight"] = "Weight", ["Category"] = "Category", ["AppearsOptions"] = "Appears" },
        "miscellanea" => new() { ["Name"] = "Name", ["Description"] = "Description", ["Price"] = "Price", ["AppearsOptions"] = "Appears", ["MiscellaneaCategoryOptions"] = "Category" },
        "abilities" => new() { ["Name"] = "Name", ["Description"] = "Description", ["AppearsOptions"] = "Appears" },
        "spells" => new() { ["Name"] = "Name", ["Description"] = "Description", ["SpellType"] = "Type", ["AppearsOptions"] = "Appears" },
        "poems" => new() { ["Name"] = "Title", ["Description"] = "Text", ["Key"] = "Key" },
        _ => null
    };

    private static string NormalizeCategory(string category) => category.Trim() switch
    {
        "Equipo" or "Equipment" => "Equipment",
        "Artefacto" or "Artifact" => "Artifact",
        _ => string.Empty
    };

    private static MiscellaneaCategory ParseMiscellaneaCategory(IEnumerable<string> names)
    {
        // La ficha admite varias etiquetas y usa tienda normal sólo cuando no se ha marcado otra procedencia.
        var selected = names.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var category = Enum.GetValues<MiscellaneaCategory>().Where(flag => flag != 0 && selected.Contains(flag.DisplayName())).Aggregate((MiscellaneaCategory)0, (result, flag) => result | flag);
        return category == 0 ? MiscellaneaCategory.NormalShop : category;
    }

    private async Task<bool> ValidateSelectionsAsync(string type, string field, string[] values)
    {
        IEnumerable<string>? choices = field switch
        {
            "MiscellaneaCategoryOptions" => Enum.GetValues<MiscellaneaCategory>().Where(flag => flag != 0).Select(flag => flag.DisplayName()),
            "AppearsOptions" => Enum.GetValues<TypeAppears>().Where(value => value != TypeAppears.None).Select(value => value.DisplayName()),
            "AbilityOptions" => await db.MonsterAbilities.AsNoTracking().Select(item => item.Name)
                .Concat(db.SpellsOfTerror.AsNoTracking().Select(item => item.Name)).ToListAsync(),
            "CharacterTypeClass" => (await db.Heroes.AsNoTracking().Select(hero => hero.CharacterTypeClass).Distinct().ToListAsync())
                .Concat(["Bárbaro", "Elfo", "Enano", "Mago"]),
            "Faction" => await db.Monsters.AsNoTracking().Select(monster => monster.Faction).Distinct().ToListAsync(),
            "Category" => ["Equipo", "Artefacto", "Equipment", "Artifact"],
            "ItemType" => await db.EquipmentsAndArtifacts.AsNoTracking().Select(item => item.Type).Distinct().ToListAsync(),
            "SpellType" => await db.SpellsOfTerror.AsNoTracking().Select(spell => spell.Type).Distinct().ToListAsync(),
            _ => null
        };
        if (choices is null) return true;
        var allowed = choices.Where(choice => !string.IsNullOrWhiteSpace(choice)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return values.All(value => allowed.Contains(value.Trim()));
    }

    private IDictionary<string, object>? FindCsvRecord(string fileName, string nameHeader, string name)
    {
        var path = Path.Combine(environment.ContentRootPath, "Data", "HeroQuest_csv", fileName);
        if (!System.IO.File.Exists(path)) return null;
        return CsvRecordReader.ReadRecords(path).FirstOrDefault(record =>
            string.Equals(CsvRecordReader.GetText(record, nameHeader), name, StringComparison.OrdinalIgnoreCase));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(string type, int id, bool inArchive = false)
    {
        if (!Titles.ContainsKey(type)) return BadRequest();
        var recordName = await GetCatalogNameAsync(type, id);
        if (recordName is null) return NotFound();
        var archived = type.ToLowerInvariant() switch
        {
            "heroes" => await SetArchive(db.Heroes, id), "monsters" => await SetArchive(db.Monsters, id),
            "equipment" => await SetArchive(db.EquipmentsAndArtifacts, id), "miscellanea" => await SetArchive(db.Miscellanea, id),
            "abilities" => await SetArchive(db.MonsterAbilities, id), "spells" => await SetArchive(db.SpellsOfTerror, id),
            "poems" => await SetArchive(db.WorldPoems, id), _ => false
        };
        if (!archived) return NotFound();
        await db.SaveChangesAsync();
        TempData["GuardianNotice"] = await poems.MessageAsync(inArchive ? "library-restored" : "library-archived", recordName);
        return RedirectToAction(nameof(Index), new { type, archived = !inArchive });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string type, int id, bool archived = false)
    {
        if (!Titles.ContainsKey(type)) return BadRequest();
        var recordName = await GetCatalogNameAsync(type, id);
        if (recordName is null) return NotFound();
        var deleted = type.ToLowerInvariant() switch
        {
            "heroes" => await Remove(db.Heroes, id), "monsters" => await Remove(db.Monsters, id),
            "equipment" => await Remove(db.EquipmentsAndArtifacts, id), "miscellanea" => await Remove(db.Miscellanea, id),
            "abilities" => await Remove(db.MonsterAbilities, id), "spells" => await Remove(db.SpellsOfTerror, id),
            "poems" => await Remove(db.WorldPoems, id), _ => false
        };
        if (!deleted) return NotFound();
        await db.SaveChangesAsync();
        TempData["GuardianNotice"] = await poems.MessageAsync("library-destroyed", recordName);
        return RedirectToAction(nameof(Index), new { type, archived });
    }

    private async Task<string?> GetCatalogNameAsync(string type, int id) => type.ToLowerInvariant() switch
    {
        "heroes" => await db.Heroes.Where(item => item.Id == id).Select(item => item.Name).FirstOrDefaultAsync(),
        "monsters" => await db.Monsters.Where(item => item.Id == id).Select(item => item.Name).FirstOrDefaultAsync(),
        "equipment" => await db.EquipmentsAndArtifacts.Where(item => item.Id == id).Select(item => item.Name).FirstOrDefaultAsync(),
        "miscellanea" => await db.Miscellanea.Where(item => item.Id == id).Select(item => item.Name).FirstOrDefaultAsync(),
        "abilities" => await db.MonsterAbilities.Where(item => item.Id == id).Select(item => item.Name).FirstOrDefaultAsync(),
        "spells" => await db.SpellsOfTerror.Where(item => item.Id == id).Select(item => item.Name).FirstOrDefaultAsync(),
        "poems" => await db.WorldPoems.Where(item => item.Id == id).Select(item => item.Title).FirstOrDefaultAsync(),
        _ => null
    };

    private async Task<int> NextCharacterId(bool isHero)
    {
        // Elegimos siempre una clave libre con la paridad reservada para ese tipo de personaje.
        var usedIds = await db.Heroes.Select(hero => hero.Id)
            .Concat(db.Monsters.Select(monster => monster.Id))
            .ToHashSetAsync();
        var candidate = isHero ? 1 : 2;
        while (usedIds.Contains(candidate)) candidate += 2;
        return candidate;
    }

    // El archivado conserva las claves foráneas y permite recuperar el registro después.
    private static async Task<bool> SetArchive<TEntity>(DbSet<TEntity> set, int id) where TEntity : class, IArchivable
    {
        var entity = await set.FindAsync(id);
        if (entity is null) return false;
        entity.IsArchived = !entity.IsArchived;
        return true;
    }

    private static async Task<bool> Remove<TEntity>(DbSet<TEntity> set, int id) where TEntity : class
    {
        var entity = await set.FindAsync(id);
        if (entity is null) return false;
        set.Remove(entity);
        return true;
    }

    private async Task<IActionResult> ReturnEditorAsync(CatalogEditViewModel model)
    {
        await PopulateEditorOptionsAsync(model);
        return View("Edit", model);
    }

    // Los desplegables se construyen con enums, catálogos del juego y valores ya guardados para no perder opciones existentes.
    private async Task PopulateEditorOptionsAsync(CatalogEditViewModel model)
    {
        ViewBag.MiscellaneaCategoryOptions = Enum.GetValues<MiscellaneaCategory>().Where(flag => flag != 0).Select(flag => flag.DisplayName()).ToArray();
        ViewBag.AppearanceOptions = Enum.GetValues<TypeAppears>()
            .Where(value => value != TypeAppears.None).Select(value => value.DisplayName()).ToArray();
        ViewBag.HeroClassOptions = await db.Heroes.AsNoTracking().Select(hero => hero.CharacterTypeClass).Distinct().ToListAsync();
        ViewBag.HeroClassOptions = ((IEnumerable<string>)ViewBag.HeroClassOptions)
            .Concat(["Bárbaro", "Elfo", "Enano", "Mago", model.CharacterTypeClass])
            .Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value).ToArray();
        ViewBag.FactionOptions = await db.Monsters.AsNoTracking().Select(monster => monster.Faction).Distinct().ToListAsync();
        ViewBag.FactionOptions = ((IEnumerable<string>)ViewBag.FactionOptions).Append(model.Faction)
            .Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value).ToArray();
        var abilityOptions = await db.MonsterAbilities.AsNoTracking().Select(ability => ability.Name)
            .Concat(db.SpellsOfTerror.AsNoTracking().Select(spell => spell.Name)).Distinct().ToListAsync();
        ViewBag.AbilityCatalogOptions = abilityOptions.Concat(model.AbilityOptions)
            .Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value).ToArray();
        ViewBag.EquipmentTypeOptions = await db.EquipmentsAndArtifacts.AsNoTracking().Select(item => item.Type).Distinct().ToListAsync();
        ViewBag.EquipmentTypeOptions = ((IEnumerable<string>)ViewBag.EquipmentTypeOptions).Append(model.ItemType)
            .Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value).ToArray();
        ViewBag.SpellTypeOptions = await db.SpellsOfTerror.AsNoTracking().Select(spell => spell.Type).Distinct().ToListAsync();
        ViewBag.SpellTypeOptions = ((IEnumerable<string>)ViewBag.SpellTypeOptions).Append(model.SpellType)
            .Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value).ToArray();

        if (model.Type == "heroes" && model.Id is int heroId && await db.Heroes.FindAsync(heroId) is { } hero)
        {
            ViewBag.WeaponOptions = (await heroConfigurations.GetWeaponOptionsAsync(hero)).Concat(model.WeaponOptions).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value).ToArray();
            ViewBag.ArmorOptions = (await heroConfigurations.GetArmorOptionsAsync(hero)).Concat(model.ArmorOptions).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value).ToArray();
            ViewBag.ItemOptions = (await heroConfigurations.GetItemOptionsAsync(hero)).Concat(model.ItemOptions).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value).ToArray();
        }
        else if (model.Type == "heroes")
        {
            ViewBag.WeaponOptions = await db.EquipmentsAndArtifacts.AsNoTracking().Where(item => !item.IsArchived && item.Type == "Arma").Select(item => item.Name).Distinct().OrderBy(value => value).ToArrayAsync();
            ViewBag.ArmorOptions = (await db.EquipmentsAndArtifacts.AsNoTracking().Where(item => !item.IsArchived && item.Type == "Armadura").Select(item => item.Name).ToListAsync())
                .Concat(await db.Miscellanea.AsNoTracking().Where(item => !item.IsArchived && (item.EquipmentSlot == "Armadura" || item.EquipmentSlot == "Escudo" || item.EquipmentSlot == "Cabeza")).Select(item => item.Name).ToListAsync()).Distinct().OrderBy(value => value).ToArray();
            ViewBag.ItemOptions = await db.Miscellanea.AsNoTracking().Where(item => !item.IsArchived).Select(item => item.Name).Distinct().OrderBy(value => value).ToArrayAsync();
        }
    }

    private static string[] SplitOptions(string? values)
        => values?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() ?? [];

    private static string[] SelectedAppearances(TypeAppears appears)
        => appears.DisplayName().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    private async Task<CatalogEditViewModel?> LoadEditModel(string type, int id)
    {
        var model = new CatalogEditViewModel { Type = type.ToLowerInvariant(), Id = id };
        switch (model.Type)
        {
            case "heroes": if (await db.Heroes.FindAsync(id) is { } h) { model.Name=h.Name; model.SourceName=h.SourceName; model.CharacterTypeClass=h.CharacterTypeClass; model.Hp=h.Hp; model.DicesMind=h.DicesMind; model.DicesAttack=h.DicesAttack; model.DicesDefend=h.DicesDefend; model.DicesRedMovement=h.DicesRedMovement; model.Description=h.Description??""; model.AppearsOptions=SelectedAppearances(h.Appears); model.AbilityOptions=HeroConfigurationService.SplitNames(h.Spells); model.Weapon=h.Weapon; model.Armor=h.Armor??""; model.Items=h.Items??""; model.WeaponOptions=HeroConfigurationService.SplitNames(h.Weapon); model.ArmorOptions=HeroConfigurationService.SplitNames(h.Armor); model.ItemOptions=HeroConfigurationService.SplitNames(h.Items); model.Spells=h.Spells??""; model.Gold=h.Gold; model.QuestsCompleted=h.QuestsCompleted; model.IsArchived=h.IsArchived; model.DisplayHp=h.Hp; model.DisplayMind=h.DicesMind; model.DisplayAttack=h.DicesAttack; model.DisplayDefense=h.DicesDefend; } else return null; break;
            case "monsters": if (await db.Monsters.FindAsync(id) is { } m) { model.Name=m.Name; model.SourceName=m.SourceName; model.Hp=m.Hp; model.DicesMind=m.DicesMind; model.DicesAttack=m.DicesAttack; model.DicesDefend=m.DicesDefend; model.MaxMovement=m.MaxMovement; model.Faction=m.Faction; model.Ability=m.Ability; model.AbilityOptions=SplitOptions(m.Ability); model.Description=m.Description??""; model.AppearsOptions=SelectedAppearances(m.Appears); model.IsArchived=m.IsArchived; } else return null; break;
            case "equipment": if (await db.EquipmentsAndArtifacts.FindAsync(id) is { } e) { model.Name=e.Name; model.Description=e.Description; model.Price=e.Price; model.ItemType=e.Type; model.Weight=e.Weight; model.Category=e.Category; model.AppearsOptions=SelectedAppearances(e.Appears); model.IsArchived=e.IsArchived; } else return null; break;
            case "miscellanea": if (await db.Miscellanea.FindAsync(id) is { } i) { model.Name=i.Name; model.Description=i.Description; model.Price=i.Price; model.AppearsOptions=SelectedAppearances(i.Appears); model.MiscellaneaCategoryOptions=Enum.GetValues<MiscellaneaCategory>().Where(flag => flag != 0 && i.Category.HasFlag(flag)).Select(flag => flag.DisplayName()).ToArray(); model.IsArchived=i.IsArchived; } else return null; break;
            case "abilities": if (await db.MonsterAbilities.FindAsync(id) is { } a) { model.Name=a.Name; model.Description=a.Description; model.AppearsOptions=SelectedAppearances(a.Appears); model.IsArchived=a.IsArchived; } else return null; break;
            case "spells": if (await db.SpellsOfTerror.FindAsync(id) is { } s) { model.Name=s.Name; model.Description=s.Description; model.SpellType=s.Type; model.AppearsOptions=SelectedAppearances(s.Appears); model.IsArchived=s.IsArchived; } else return null; break;
            case "poems": if (await db.WorldPoems.FindAsync(id) is { } p) { model.Key=p.Key; model.Name=p.Title; model.Description=p.Text; model.IsArchived=p.IsArchived; } else return null; break;
        }
        return model;
    }
}
