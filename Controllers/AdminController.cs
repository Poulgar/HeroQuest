using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

// Toda la administración se protege en el servidor; ocultar botones por sí solo no sería un control de acceso.
[Authorize(Roles = "Admin")]
public class AdminController(ApplicationDbContext db) : Controller
{
    private static readonly Dictionary<string, string> Titles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["heroes"] = "Héroes", ["monsters"] = "Monstruos", ["equipment"] = "Equipos y Artefactos",
        ["miscellanea"] = "Miscelánea", ["abilities"] = "Habilidades", ["spells"] = "Hechizos del Terror",
        ["poems"] = "Versos y Leyendas"
    };

    [HttpGet]
    public Task<IActionResult> Index(string type, bool archived = false) => BuildCatalogPage(type, archived, viewName: "Index");

    [HttpGet]
    public IActionResult Archive(string type) => RedirectToAction(nameof(Index), new { type, archived = true });

    private async Task<IActionResult> BuildCatalogPage(string type, bool archived, string viewName)
    {
        if (!Titles.TryGetValue(type, out var title)) return BadRequest();
        var rows = type.ToLowerInvariant() switch
        {
            "heroes" => await db.Heroes.Where(x => x.IsArchived == archived).Select(x => new CatalogListItem(x.Id, x.Name, $"{x.Hp} HP · {x.CharacterTypeClass}", x.IsArchived)).ToListAsync(),
            "monsters" => await db.Monsters.Where(x => x.IsArchived == archived).Select(x => new CatalogListItem(x.Id, x.Name, $"{x.Hp} HP · {x.Faction}", x.IsArchived)).ToListAsync(),
            "equipment" => await db.EquipmentsAndArtifacts.Where(x => x.IsArchived == archived).Select(x => new CatalogListItem(x.Id, x.Name, $"{x.Category} · {x.Price} mo", x.IsArchived)).ToListAsync(),
            "miscellanea" => await db.Miscellanea.Where(x => x.IsArchived == archived).Select(x => new CatalogListItem(x.Id, x.Name, $"{x.Price} mo", x.IsArchived)).ToListAsync(),
            "abilities" => await db.MonsterAbilities.Where(x => x.IsArchived == archived).Select(x => new CatalogListItem(x.Id, x.Name, x.Appears.ToString(), x.IsArchived)).ToListAsync(),
            "spells" => await db.SpellsOfTerror.Where(x => x.IsArchived == archived).Select(x => new CatalogListItem(x.Id, x.Name, x.Type, x.IsArchived)).ToListAsync(),
            "poems" => await db.WorldPoems.Where(x => x.IsArchived == archived).Select(x => new CatalogListItem(x.Id, x.Title, x.Key, x.IsArchived)).ToListAsync(),
            _ => []
        };
        ViewData["IsArchive"] = archived;
        return View(viewName, new CatalogListViewModel { Type = type.ToLowerInvariant(), Title = title, Items = rows.OrderBy(x => x.Id).ToList() });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string type, int? id)
    {
        if (!Titles.ContainsKey(type)) return BadRequest();
        var model = id is null ? new CatalogEditViewModel { Type = type.ToLowerInvariant() } : await LoadEditModel(type, id.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(CatalogEditViewModel model)
    {
        if (!Titles.ContainsKey(model.Type)) return BadRequest();
        if (!ModelState.IsValid) return View("Edit", model);
        // El texto de expansiones se convierte aquí a banderas del enum antes de guardar la entidad.
        var appearance = TypeAppearsParser.Parse(model.Appears);

        if (model.Type.Equals("poems", StringComparison.OrdinalIgnoreCase)
            && (string.IsNullOrWhiteSpace(model.Key) || model.Key.Length > 64
                || !System.Text.RegularExpressions.Regex.IsMatch(model.Key, "^[a-z0-9-]+$")))
        {
            ModelState.AddModelError(nameof(model.Key), "Usa una clave breve con letras minúsculas, números y guiones.");
            return View("Edit", model);
        }

        switch (model.Type)
        {
            case "heroes":
                var hero = model.Id is null ? new Heroe() : await db.Heroes.FindAsync(model.Id.Value) ?? new Heroe();
                if (model.Id is null) { hero.Id = await NextCharacterId(isHero: true); db.Heroes.Add(hero); }
                hero.Name = model.Name.Trim(); hero.CharacterTypeClass = model.CharacterTypeClass;
                hero.Hp = model.Hp; hero.DicesMind = model.DicesMind; hero.DicesAttack = model.DicesAttack; hero.DicesDefend = model.DicesDefend;
                hero.DicesRedMovement = model.DicesRedMovement; hero.Description = model.Description; hero.Appears = appearance;
                hero.Player = model.Player; hero.Weapon = model.Weapon; hero.Armor = model.Armor; hero.Spells = model.Spells; hero.Items = model.Items;
                hero.Gold = model.Gold; hero.QuestsCompleted = model.QuestsCompleted; hero.IsArchived = model.IsArchived; break;
            case "monsters":
                var monster = model.Id is null ? new Monster() : await db.Monsters.FindAsync(model.Id.Value) ?? new Monster();
                if (model.Id is null) { monster.Id = await NextCharacterId(isHero: false); db.Monsters.Add(monster); }
                monster.Name = model.Name.Trim(); monster.CharacterTypeClass = model.Name.Trim(); monster.Hp = model.Hp;
                monster.DicesMind = model.DicesMind; monster.DicesAttack = model.DicesAttack; monster.DicesDefend = model.DicesDefend;
                monster.MaxMovement = model.MaxMovement; monster.Faction = model.Faction; monster.Ability = model.Ability;
                monster.Description = model.Description; monster.Appears = appearance; monster.IsArchived = model.IsArchived;
                monster.Attack = model.DicesAttack; monster.Defense = model.DicesDefend; break;
            case "equipment":
                var item = model.Id is null ? new EquipmentsAndArtifacts(0, "", "", 0, "", TypeAppears.None, "", "Equipment") : await db.EquipmentsAndArtifacts.FindAsync(model.Id.Value) ?? new EquipmentsAndArtifacts(0, "", "", 0, "", TypeAppears.None, "", "Equipment");
                if (model.Id is null) { item.Id = await db.EquipmentsAndArtifacts.Select(x => (int?)x.Id).MaxAsync() + 1 ?? 1; db.EquipmentsAndArtifacts.Add(item); }
                item.Name = model.Name.Trim(); item.Description = model.Description; item.Price = model.Price; item.Type = model.ItemType;
                item.Weight = model.Weight; item.Category = model.Category; item.Appears = appearance; item.IsArchived = model.IsArchived; break;
            case "miscellanea":
                var entry = model.Id is null ? new MiscellaneaItem(0, "", "", 0, TypeAppears.None) : await db.Miscellanea.FindAsync(model.Id.Value) ?? new MiscellaneaItem(0, "", "", 0, TypeAppears.None);
                if (model.Id is null) { entry.Id = await db.Miscellanea.Select(x => (int?)x.Id).MaxAsync() + 1 ?? 1; db.Miscellanea.Add(entry); }
                entry.Name = model.Name.Trim(); entry.Description = model.Description; entry.Price = model.Price; entry.Appears = appearance; entry.IsArchived = model.IsArchived; break;
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
                    return View("Edit", model);
                }
                if (model.Id is null) db.WorldPoems.Add(poem);
                poem.Key = poemKey; poem.Title = model.Name.Trim(); poem.Text = model.Description; poem.IsArchived = model.IsArchived; break;
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { type = model.Type });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(string type, int id, bool inArchive = false)
    {
        if (!Titles.ContainsKey(type)) return BadRequest();
        var archived = type.ToLowerInvariant() switch
        {
            "heroes" => await SetArchive(db.Heroes, id), "monsters" => await SetArchive(db.Monsters, id),
            "equipment" => await SetArchive(db.EquipmentsAndArtifacts, id), "miscellanea" => await SetArchive(db.Miscellanea, id),
            "abilities" => await SetArchive(db.MonsterAbilities, id), "spells" => await SetArchive(db.SpellsOfTerror, id),
            "poems" => await SetArchive(db.WorldPoems, id), _ => false
        };
        if (!archived) return NotFound();
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { type, archived = !inArchive });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string type, int id, bool archived = false)
    {
        if (!Titles.ContainsKey(type)) return BadRequest();
        var deleted = type.ToLowerInvariant() switch
        {
            "heroes" => await Remove(db.Heroes, id), "monsters" => await Remove(db.Monsters, id),
            "equipment" => await Remove(db.EquipmentsAndArtifacts, id), "miscellanea" => await Remove(db.Miscellanea, id),
            "abilities" => await Remove(db.MonsterAbilities, id), "spells" => await Remove(db.SpellsOfTerror, id),
            "poems" => await Remove(db.WorldPoems, id), _ => false
        };
        if (!deleted) return NotFound();
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { type, archived });
    }

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

    private async Task<CatalogEditViewModel?> LoadEditModel(string type, int id)
    {
        var model = new CatalogEditViewModel { Type = type.ToLowerInvariant(), Id = id };
        switch (model.Type)
        {
            case "heroes": if (await db.Heroes.FindAsync(id) is { } h) { model.Name=h.Name; model.CharacterTypeClass=h.CharacterTypeClass; model.Hp=h.Hp; model.DicesMind=h.DicesMind; model.DicesAttack=h.DicesAttack; model.DicesDefend=h.DicesDefend; model.DicesRedMovement=h.DicesRedMovement; model.Description=h.Description??""; model.Appears=h.Appears.ToString(); model.Player=h.Player; model.Weapon=h.Weapon; model.Armor=h.Armor??""; model.Spells=h.Spells??""; model.Items=h.Items??""; model.Gold=h.Gold; model.QuestsCompleted=h.QuestsCompleted; model.IsArchived=h.IsArchived; } else return null; break;
            case "monsters": if (await db.Monsters.FindAsync(id) is { } m) { model.Name=m.Name; model.Hp=m.Hp; model.DicesMind=m.DicesMind; model.DicesAttack=m.DicesAttack; model.DicesDefend=m.DicesDefend; model.MaxMovement=m.MaxMovement; model.Faction=m.Faction; model.Ability=m.Ability; model.Description=m.Description??""; model.Appears=m.Appears.ToString(); model.IsArchived=m.IsArchived; } else return null; break;
            case "equipment": if (await db.EquipmentsAndArtifacts.FindAsync(id) is { } e) { model.Name=e.Name; model.Description=e.Description; model.Price=e.Price; model.ItemType=e.Type; model.Weight=e.Weight; model.Category=e.Category; model.Appears=e.Appears.ToString(); model.IsArchived=e.IsArchived; } else return null; break;
            case "miscellanea": if (await db.Miscellanea.FindAsync(id) is { } i) { model.Name=i.Name; model.Description=i.Description; model.Price=i.Price; model.Appears=i.Appears.ToString(); model.IsArchived=i.IsArchived; } else return null; break;
            case "abilities": if (await db.MonsterAbilities.FindAsync(id) is { } a) { model.Name=a.Name; model.Description=a.Description; model.Appears=a.Appears.ToString(); model.IsArchived=a.IsArchived; } else return null; break;
            case "spells": if (await db.SpellsOfTerror.FindAsync(id) is { } s) { model.Name=s.Name; model.Description=s.Description; model.SpellType=s.Type; model.Appears=s.Appears.ToString(); model.IsArchived=s.IsArchived; } else return null; break;
            case "poems": if (await db.WorldPoems.FindAsync(id) is { } p) { model.Key=p.Key; model.Name=p.Title; model.Description=p.Text; model.IsArchived=p.IsArchived; } else return null; break;
        }
        return model;
    }
}

