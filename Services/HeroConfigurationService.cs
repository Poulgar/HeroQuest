using HeroQuest.Data;
using HeroQuest.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Services;

/// <summary>Centraliza carga, restricciones, cálculo y comparación de una ficha personal con su ficha maestra.</summary>
public sealed class HeroConfigurationService(ApplicationDbContext db)
{
    public async Task<HeroConfiguration> CreateAsync(PlayerProfile player, Heroe hero)
    {
        var current = await db.HeroConfigurations.FirstOrDefaultAsync(item => item.PlayerProfileId == player.Id && item.HeroId == hero.Id);
        if (current is not null) { current.IsActive = true; return current; }

        var configuration = new HeroConfiguration
        {
            PlayerProfile = player,
            Hero = hero,
            CharacterName = string.IsNullOrWhiteSpace(player.Nickname) ? player.Name : player.Nickname,
            Weapons = hero.Weapon,
            Armors = hero.Armor ?? string.Empty,
            Items = hero.Items ?? string.Empty,
            Abilities = hero.Spells ?? string.Empty,
            MovementDice = hero.DicesRedMovement,
            Gold = hero.Gold,
            QuestsCompleted = hero.QuestsCompleted
        };
        db.HeroConfigurations.Add(configuration);
        return configuration;
    }

    public async Task<CatalogEditViewModel> BuildEditorAsync(Heroe hero, HeroConfiguration? configuration, bool personal)
    {
        var model = new CatalogEditViewModel
        {
            Type = "heroes", Id = hero.Id, ConfigurationId = configuration?.Id,
            IsPersonalConfiguration = personal,
            Name = configuration?.CharacterName ?? hero.Name,
            CharacterTypeClass = hero.CharacterTypeClass,
            Description = hero.Description ?? string.Empty, SourceName = hero.SourceName,
            AppearsOptions = hero.Appears.DisplayName().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
            IsArchived = hero.IsArchived,
            Hp = hero.Hp, DicesMind = hero.DicesMind, DicesAttack = hero.DicesAttack, DicesDefend = hero.DicesDefend,
            DicesRedMovement = configuration?.MovementDice ?? hero.DicesRedMovement,
            Weapon = configuration?.Weapons ?? hero.Weapon,
            Armor = configuration?.Armors ?? hero.Armor ?? string.Empty,
            Items = configuration?.Items ?? hero.Items ?? string.Empty,
            Spells = configuration?.Abilities ?? hero.Spells ?? string.Empty,
            Gold = configuration?.Gold ?? hero.Gold,
            QuestsCompleted = configuration?.QuestsCompleted ?? hero.QuestsCompleted,
            WeaponOptions = SplitNames(configuration?.Weapons ?? hero.Weapon),
            ArmorOptions = SplitNames(configuration?.Armors ?? hero.Armor ?? string.Empty),
            ItemOptions = SplitNames(configuration?.Items ?? hero.Items ?? string.Empty),
            AbilityOptions = SplitNames(configuration?.Abilities ?? hero.Spells ?? string.Empty)
        };

        var tabs = await db.HeroConfigurations.AsNoTracking().Where(item => item.HeroId == hero.Id)
            .Include(item => item.PlayerProfile).OrderBy(item => item.PlayerProfile.Name).ToListAsync();
        model.ConfigurationTabs = tabs.Select(item => new HeroConfigurationTab(item.Id, item.PlayerProfile.Name, item.CharacterName, item.IsActive)).ToList();

        var equippedWeapons = await FindEquipmentAsync(model.WeaponOptions);
        var defaultWeapons = await FindEquipmentAsync(SplitNames(hero.Weapon));
        var equippedArmor = await FindEquipmentAsync(model.ArmorOptions);
        var defaultArmor = await FindEquipmentAsync(SplitNames(hero.Armor ?? string.Empty));
        var carriedItems = await FindMiscellaneaAsync(model.ItemOptions);
        var defaultWeaponAttack = defaultWeapons.Select(item => item.WeaponAttackDice ?? 0).DefaultIfEmpty().Max();
        var intrinsicDefense = Math.Max(1, hero.DicesDefend - defaultArmor.Select(item => item.DefenseBonus).DefaultIfEmpty().Max());
        var weaponAttack = equippedWeapons.Select(item => item.WeaponAttackDice ?? 0).DefaultIfEmpty().Max();
        var objectAttack = carriedItems.Select(item => item.WeaponAttackDice ?? 0).DefaultIfEmpty().Max();
        var armorDefense = equippedArmor.Select(item => item.DefenseBonus).DefaultIfEmpty().Max();

        model.DisplayHp = Math.Max(0, hero.Hp + carriedItems.Sum(item => item.HealthBonus));
        model.DisplayMind = Math.Max(0, hero.DicesMind + carriedItems.Sum(item => item.MindBonus));
        // Se suma la diferencia respecto al arma original: cambiar una espada de 3 dados por un hacha de 4 deja 4, no 7.
        model.DisplayAttack = Math.Max(1, hero.DicesAttack + Math.Max(weaponAttack, objectAttack) - defaultWeaponAttack);
        model.DisplayDefense = Math.Max(1, intrinsicDefense + armorDefense + carriedItems.Sum(item => item.DefenseBonus));
        var movementOverride = equippedArmor.Where(item => item.MovementDiceOverride.HasValue).Select(item => item.MovementDiceOverride!.Value).DefaultIfEmpty(model.DicesRedMovement).Min();
        model.DicesRedMovement = movementOverride;

        if (configuration is not null)
        {
            SetMark(model, "Hp", hero.Hp, model.DisplayHp);
            SetMark(model, "DicesMind", hero.DicesMind, model.DisplayMind);
            SetMark(model, "DicesAttack", hero.DicesAttack, model.DisplayAttack);
            SetMark(model, "DicesDefend", hero.DicesDefend, model.DisplayDefense);
            SetMark(model, "DicesRedMovement", hero.DicesRedMovement, model.DicesRedMovement);
            model.ComparisonMarks["WeaponOptions"] = CompareSets(SplitNames(hero.Weapon), model.WeaponOptions);
            model.ComparisonMarks["ArmorOptions"] = CompareSets(SplitNames(hero.Armor ?? string.Empty), model.ArmorOptions);
            model.ComparisonMarks["ItemOptions"] = CompareSets(SplitNames(hero.Items ?? string.Empty), model.ItemOptions);
            model.ComparisonMarks["AbilityOptions"] = CompareSets(SplitNames(hero.Spells ?? string.Empty), model.AbilityOptions);
            // Cuando una sustitución mantiene el mismo valor calculado, se marca como cambio sin ventaja neta.
            if (model.ComparisonMarks["DicesAttack"].Length == 0 && (model.ComparisonMarks["WeaponOptions"] != string.Empty || model.ComparisonMarks["ItemOptions"] != string.Empty)) model.ComparisonMarks["DicesAttack"] = "changed";
            if (model.ComparisonMarks["DicesDefend"].Length == 0 && (model.ComparisonMarks["ArmorOptions"] != string.Empty || model.ComparisonMarks["ItemOptions"] != string.Empty)) model.ComparisonMarks["DicesDefend"] = "changed";
            if (model.ComparisonMarks["DicesMind"].Length == 0 && model.ComparisonMarks["ItemOptions"] != string.Empty) model.ComparisonMarks["DicesMind"] = "changed";
            if (model.ComparisonMarks["DicesRedMovement"].Length == 0 && model.ComparisonMarks["ArmorOptions"] != string.Empty) model.ComparisonMarks["DicesRedMovement"] = "changed";
            if (model.ComparisonMarks["Hp"].Length == 0 && model.ComparisonMarks["ItemOptions"] != string.Empty) model.ComparisonMarks["Hp"] = "changed";
        }
        return model;
    }

    public async Task<string[]> GetWeaponOptionsAsync(Heroe hero)
    {
        var items = await db.EquipmentsAndArtifacts.AsNoTracking().Where(item => !item.IsArchived && item.Type == "Arma").ToListAsync();
        return items.Where(item => IsAllowed(hero, item.Name, item.Description, item.AllowedClasses, item.ExcludedClasses, item.IsRangedWeapon, "Arma", item.EquipmentSlot, item.Weight))
            .Select(item => item.Name).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item).ToArray();
    }

    public async Task<string[]> GetArmorOptionsAsync(Heroe hero)
    {
        var items = await db.EquipmentsAndArtifacts.AsNoTracking().Where(item => !item.IsArchived && item.Type == "Armadura").ToListAsync();
        var misc = await db.Miscellanea.AsNoTracking().Where(item => !item.IsArchived && (item.EquipmentSlot == "Armadura" || item.EquipmentSlot == "Escudo" || item.EquipmentSlot == "Cabeza")).ToListAsync();
        var candidates = items.Select(item => (Name: item.Name, Description: item.Description, Allowed: item.AllowedClasses, Excluded: item.ExcludedClasses, Ranged: item.IsRangedWeapon, Type: item.Type, Slot: item.EquipmentSlot, Weight: item.Weight))
            .Concat(misc.Select(item => (Name: item.Name, Description: item.Description, Allowed: item.AllowedClasses, Excluded: item.ExcludedClasses, Ranged: item.IsRangedWeapon, Type: "Armadura", Slot: item.EquipmentSlot, Weight: string.Empty)));
        return candidates
            .Where(item => IsAllowed(hero, item.Name, item.Description, item.Allowed, item.Excluded, item.Ranged, item.Type, item.Slot, item.Weight))
            .Select(item => item.Name).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item).ToArray();
    }

    public async Task<string[]> GetItemOptionsAsync(Heroe hero)
    {
        var items = await db.Miscellanea.AsNoTracking().Where(item => !item.IsArchived).ToListAsync();
        return items.Where(item => !new[] { "Armadura", "Escudo", "Cabeza", "Arma" }.Contains(item.EquipmentSlot)
            && IsAllowed(hero, item.Name, item.Description, item.AllowedClasses, item.ExcludedClasses, item.IsRangedWeapon, "Objeto", item.EquipmentSlot, string.Empty))
            .Select(item => item.Name).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(item => item).ToArray();
    }

    public async Task<string?> ValidateLoadoutAsync(Heroe hero, string[] weapons, string[] armors, string[] items)
    {
        var restrictions = hero.Description ?? string.Empty;
        if (restrictions.Contains("no puede usar ni armas ni armaduras", StringComparison.OrdinalIgnoreCase)
            && (weapons.Length > 0 || armors.Length > 0)) return "Este héroe no puede llevar armas ni armaduras.";
        if (hero.CharacterTypeClass.Equals("Valquiria", StringComparison.OrdinalIgnoreCase) && weapons.Length > 1)
            return "La Valquiria solo puede llevar un arma.";

        var allowedWeapons = await GetWeaponOptionsAsync(hero);
        if (weapons.Except(allowedWeapons, StringComparer.OrdinalIgnoreCase).Any()) return "Una de las armas no está permitida para esta clase.";
        var allowedArmor = await GetArmorOptionsAsync(hero);
        if (armors.Except(allowedArmor, StringComparer.OrdinalIgnoreCase).Any()) return "Una de las armaduras no está permitida para esta clase.";
        var allowedItems = await GetItemOptionsAsync(hero);
        if (items.Except(allowedItems, StringComparer.OrdinalIgnoreCase).Any()) return "Uno de los objetos no está permitido para esta clase.";

        var equippedNames = weapons.Concat(armors).Concat(items).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var equipmentNames = equippedNames.ToArray();
        var incompatibilities = await db.EquipmentsAndArtifacts.AsNoTracking().Where(item => equipmentNames.Contains(item.Name))
            .Select(item => new { item.Name, item.CannotCombineWith }).ToListAsync();
        var miscIncompatibilities = await db.Miscellanea.AsNoTracking().Where(item => equipmentNames.Contains(item.Name))
            .Select(item => new { item.Name, item.CannotCombineWith }).ToListAsync();
        var conflict = incompatibilities.Concat(miscIncompatibilities)
            .Any(item => HeroConfigurationService.SplitNames(item.CannotCombineWith).Any(equippedNames.Contains));
        if (conflict) return "Dos piezas elegidas no pueden usarse juntas según sus descripciones.";
        return null;
    }

    public async Task<string?> UpdateAsync(HeroConfiguration configuration, string field, string? value, string[] values)
    {
        var hero = await db.Heroes.FindAsync(configuration.HeroId);
        if (hero is null) return "No se encuentra el héroe de esta configuración.";
        var weapons = field == "WeaponOptions" ? values : SplitNames(configuration.Weapons);
        var armors = field == "ArmorOptions" ? values : SplitNames(configuration.Armors);
        var items = field == "ItemOptions" ? values : SplitNames(configuration.Items);
        if (field is "WeaponOptions" or "ArmorOptions" or "ItemOptions")
        {
            var error = await ValidateLoadoutAsync(hero, weapons, armors, items);
            if (error is not null) return error;
        }

        switch (field)
        {
            case "Name":
                if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > 120) return "El nombre debe tener entre 1 y 120 caracteres.";
                configuration.CharacterName = value.Trim(); break;
            case "WeaponOptions": configuration.Weapons = JoinNames(weapons); break;
            case "ArmorOptions": configuration.Armors = JoinNames(armors); break;
            case "ItemOptions": configuration.Items = JoinNames(items); break;
            case "AbilityOptions":
                var knownAbilities = await db.MonsterAbilities.Select(item => item.Name).Concat(db.SpellsOfTerror.Select(item => item.Name)).ToListAsync();
                if (values.Except(knownAbilities, StringComparer.OrdinalIgnoreCase).Any()) return "Elige habilidades y hechizos del catálogo.";
                configuration.Abilities = JoinNames(values); break;
            case "MovementDice":
                if (!int.TryParse(value, out var movement) || movement is < 0 or > 20) return "El movimiento debe estar entre 0 y 20 dados.";
                configuration.MovementDice = movement; break;
            case "Gold":
                if (!int.TryParse(value, out var gold) || gold < 0) return "El oro no puede ser negativo.";
                configuration.Gold = gold; break;
            case "QuestsCompleted":
                if (!int.TryParse(value, out var quests) || quests < 0) return "Las misiones completadas no pueden ser negativas.";
                configuration.QuestsCompleted = quests; break;
            default: return "Ese campo no se puede cambiar desde una ficha de jugador.";
        }
        await db.SaveChangesAsync();
        return null;
    }

    public static string[] SplitNames(string? values)
        => values?.Split(['|', ','], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToArray() ?? [];

    public static string JoinNames(IEnumerable<string> values) => string.Join('|', values.Select(value => value.Trim()).Where(value => value.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase));

    private async Task<List<EquipmentsAndArtifacts>> FindEquipmentAsync(IEnumerable<string> names)
    {
        var lookup = names.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return lookup.Count == 0 ? [] : await db.EquipmentsAndArtifacts.AsNoTracking().Where(item => lookup.Contains(item.Name)).ToListAsync();
    }

    private async Task<List<MiscellaneaItem>> FindMiscellaneaAsync(IEnumerable<string> names)
    {
        var lookup = names.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return lookup.Count == 0 ? [] : await db.Miscellanea.AsNoTracking().Where(item => lookup.Contains(item.Name)).ToListAsync();
    }

    private static bool IsAllowed(Heroe hero, string name, string description, string allowed, string excluded, bool ranged, string type, string slot, string weight)
    {
        var heroClass = hero.CharacterTypeClass;
        var isMage = heroClass is "Mago" or "Hechicera";
        if (!string.IsNullOrWhiteSpace(allowed) && !allowed.Split('|').Any(value => ClassMatches(heroClass, value))) return false;
        if (excluded.Split('|', StringSplitOptions.RemoveEmptyEntries).Any(value => ClassMatches(heroClass, value))) return false;
        if (heroClass.Equals("Berserker", StringComparison.OrdinalIgnoreCase) && ranged) return false;
        if (heroClass.Equals("Valquiria", StringComparison.OrdinalIgnoreCase)
            && new[] { "Hacha de Batalla", "Coraza", "Escudo" }.Any(value => name.Contains(value, StringComparison.OrdinalIgnoreCase))) return false;
        if (isMage)
        {
            if (type == "Armadura" && !name.Contains("Capa de Mago", StringComparison.OrdinalIgnoreCase)) return false;
            if (type == "Arma" && (weight.Equals("Pesado", StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrWhiteSpace(allowed) && !allowed.Split('|').Any(value => ClassMatches(heroClass, value))))) return false;
        }
        if (type == "Armadura" && (heroClass is "Druida" or "Explorador")
            && (description.Contains("metal", StringComparison.OrdinalIgnoreCase) || weight.Equals("Pesado", StringComparison.OrdinalIgnoreCase))) return false;
        if ((heroClass is "Santa" or "Guerrero Sangriento") && (type is "Arma" or "Armadura")) return false;
        return true;
    }

    private static bool ClassMatches(string heroClass, string restriction)
        => heroClass.Equals(restriction, StringComparison.OrdinalIgnoreCase)
           || ((heroClass is "Mago" or "Hechicera") && restriction.Equals("Mago", StringComparison.OrdinalIgnoreCase));

    private static void SetMark(CatalogEditViewModel model, string field, int baseline, int configured)
        => model.ComparisonMarks[field] = configured > baseline ? "up" : configured < baseline ? "down" : string.Empty;

    private static string CompareSets(IEnumerable<string> baseline, IEnumerable<string> configured)
    {
        var before = baseline.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var after = configured.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var added = after.Except(before).Any();
        var removed = before.Except(after).Any();
        return added && removed ? "changed" : added ? "up" : removed ? "down" : string.Empty;
    }
}
