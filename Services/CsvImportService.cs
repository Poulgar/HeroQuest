using HeroQuest.Data;
using HeroQuest.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Services;

// Servicio para importar datos desde archivos CSV a la base de datos
public class CsvImportService
{
    private readonly ApplicationDbContext _context;

    public CsvImportService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Importa todos los archivos CSV ignorando duplicados si ya existen
    public async Task ImportAllAsync(string csvFolderPath)
    {
        var monsterCategoriesAbilitiesPath = Path.Combine(csvFolderPath, "HeroQuest_MonstersCategoriesAbilities.csv");
        var miscellaneaPath = Path.Combine(csvFolderPath, "HeroQuest_Miscellanea.csv");
        var equipmentPath = Path.Combine(csvFolderPath, "HeroQuest_Equipment.csv");
        var artifactsPath = Path.Combine(csvFolderPath, "HeroQuest_Artifacts.csv");
        var abilitiesPath = Path.Combine(csvFolderPath, "HeroQuest_MonsterAbilities.csv");
        var spellsOfTerrorPath = Path.Combine(csvFolderPath, "HeroQuest_SpellsOfTerror.csv");

        // No ocultamos una ruta incorrecta: así la app informa por qué no pudo llenar una tabla.
        foreach (var path in new[] { monsterCategoriesAbilitiesPath, miscellaneaPath, equipmentPath, artifactsPath, abilitiesPath, spellsOfTerrorPath })
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"No se encontró el CSV requerido: {Path.GetFileName(path)}", path);
        }

        await ImportSpellsOfTerrorAsync(spellsOfTerrorPath);
        await ImportMonstersAsync(monsterCategoriesAbilitiesPath);
        await ImportMiscellaneaAsync(miscellaneaPath); // Importa el CSV de objetos a la tabla Miscelánea.
        await ImportEquipmentsAndArtifactsAsync(
            equipmentPath, // Equipamiento
            artifactsPath); // Artefactos
        await ImportMonsterAbilitiesAsync(abilitiesPath); // Habilidades

        // Rehacemos los vínculos tras reordenar IDs para que las claves externas apunten a los IDs finales.
        await _context.MonsterSpellOfTerrors.ExecuteDeleteAsync();
        await new AlphabeticalIdService(_context).EnsureAlphabeticalIdsAsync();
        await ImportMonsterSpellLinksAsync(monsterCategoriesAbilitiesPath);
    }

    private async Task ImportMonstersAsync(string path)
    {
        var existingMonsters = await _context.Monsters.ToListAsync();
        var byName = existingMonsters
            .GroupBy(monster => MonsterCsvParsing.NormalizeName(monster.Name))
            .ToDictionary(group => group.Key, group => group.First());

        // Heroes y Monsters comparten la clave de Character en TPC; los nuevos IDs parten del máximo global.
        var maxHeroId = await _context.Heroes.Select(h => (int?)h.Id).MaxAsync() ?? 0;
        var maxMonsterId = await _context.Monsters.Select(m => (int?)m.Id).MaxAsync() ?? 0;
        var nextId = Math.Max(maxHeroId, maxMonsterId) + 1;

        var records = CsvRecordReader.ReadRecords(path);

        foreach (var fields in records)
        {
            var name = CsvRecordReader.GetText(fields, "Nombre");
            var hp = CsvRecordReader.GetInteger(fields, "Cuerpo");
            var dicesMind = CsvRecordReader.GetInteger(fields, "Mente");
            var attack = CsvRecordReader.GetInteger(fields, "Ataque");
            var defense = CsvRecordReader.GetInteger(fields, "Defensa");
            var maxMovement = CsvRecordReader.GetInteger(fields, "Movimiento");
            var faction = CsvRecordReader.GetText(fields, "Categoria");
            var ability = MonsterCsvParsing.CleanAbilityNames(CsvRecordReader.GetText(fields, "Habilidad"));
            var appearance = TypeAppearsParser.Parse(CsvRecordReader.GetText(fields, "Apariciones"));

            if (!byName.TryGetValue(MonsterCsvParsing.NormalizeName(name), out var monster))
            {
                monster = new Monster(nextId++, name, hp, dicesMind, attack, defense, maxMovement, ability, faction,
                    CreateLoreDescription(name, faction))
                {
                    Appears = appearance
                };
                _context.Monsters.Add(monster);
                byName[MonsterCsvParsing.NormalizeName(name)] = monster;
                continue;
            }

            // Actualizamos la ficha existente desde el CSV, pero mantenemos la vida actual de una campaña.
            monster.CharacterTypeClass = name;
            monster.Name = name;
            monster.Hp = hp;
            monster.DicesMind = dicesMind;
            monster.DicesAttack = attack;
            monster.DicesDefend = defense;
            monster.MaxMovement = maxMovement;
            monster.Attack = attack;
            monster.Defense = defense;
            monster.Faction = faction;
            monster.Ability = ability;
            monster.Appears = appearance;
            // Sustituimos las descripciones estadísticas creadas por la versión anterior,
            // pero conservamos cualquier texto personalizado que no use esa plantilla.
            if (string.IsNullOrWhiteSpace(monster.Description) ||
                monster.Description.Contains("pertenece a la facción", StringComparison.OrdinalIgnoreCase) ||
                monster.Description.Contains("ronda los dominios de", StringComparison.OrdinalIgnoreCase))
                monster.Description = CreateLoreDescription(name, faction);
        }

        await _context.SaveChangesAsync();
    }

    private static string CreateLoreDescription(string name, string faction)
    {
        // Los nombres se normalizan para que las tildes o diferencias de mayúsculas
        // no impidan encontrar el texto de ambientación correspondiente.
        var lore = MonsterCsvParsing.NormalizeName(name) switch
        {
            "abominacion" => "En las cavernas más profundas, la magia corrupta funde carne y piedra hasta dar forma a una criatura que ni siquiera los orcos se atreven a nombrar.",
            "aparicion del terror" => "Su silueta atraviesa los muros de las fortalezas olvidadas. Allí donde aparece, los recuerdos de antiguas derrotas despiertan y el valor de los héroes flaquea.",
            "arana de la maldicion" => "Teje su guarida en ruinas marcadas por viejos conjuros. Los viajeros que caen en sus redes desaparecen antes de que alguien pueda seguir su rastro.",
            "arquero elfo" => "Procedente de los bosques élficos, conoce cada sendero y cada sombra. Su arco defiende los antiguos dominios de su pueblo frente a quienes los profanan.",
            "arquero esqueleto" => "Una voluntad nigromántica guía sus huesos y su arco. Desde las almenas de una tumba, dispara sin temor y sin descanso.",
            "arquero goblin" => "Pequeño, huidizo y siempre dispuesto a atacar desde lejos, merodea por túneles donde una emboscada puede surgir de cualquier grieta.",
            "arquero orco" => "Veterano de incursiones salvajes, acecha tras barricadas y ruinas. Su presencia anuncia que una banda orca prepara el asalto.",
            "asesina" => "Se mueve en silencio por los corredores, al servicio de señores que prefieren eliminar a sus rivales sin dejar testigos.",
            "calavera de la maldicion" => "Un cráneo suspendido por hechicería recorre las criptas como presagio de una condena antigua. Su risa hueca resuena antes de que aparezca.",
            "campeon ogro" => "Ha sobrevivido a incontables desafíos tribales y luce sus cicatrices como trofeos. Los clanes lo siguen por su fuerza y su ferocidad.",
            "comandante ogro" => "Su voz impone orden entre los clanes ogros. Conduce a sus guerreros hacia la batalla para reclamar tesoros y territorios.",
            "dragon" => "Desde una guarida colmada de oro, contempla el mundo como si ya le perteneciera. Su despertar convierte cualquier expedición en una leyenda o una tragedia.",
            "engendro" => "Nacido de experimentos arcanos y de una corrupción que nadie logró contener, vaga por pasadizos donde la realidad parece deformarse.",
            "espectro" => "El espíritu de un condenado permanece atado a las cámaras donde murió. Su lamento anuncia que los vivos han cruzado un umbral prohibido.",
            "esqueleto" => "Los huesos de antiguos guerreros vuelven a levantarse cuando una fuerza oscura reclama sus tumbas. Marchan en silencio bajo el mandato de su invocador.",
            "fellmarak" => "Su nombre aparece en crónicas selladas junto a relatos de portales y pactos prohibidos. Allí donde deja su marca, la oscuridad parece cobrar vida.",
            "fimir" => "Antiguos habitantes de marismas y ruinas, los fimir sirven a poderes sombríos y conocen senderos que ningún mapa se atreve a mostrar.",
            "gargola" => "Tallada para custodiar templos y torres, esta criatura espera inmóvil entre las piedras hasta que un intruso profana el lugar que protege.",
            "goblin" => "Se desliza por pasadizos estrechos en busca de botín fácil. Los goblins prosperan en la confusión y rara vez se enfrentan a un enemigo en condiciones justas.",
            "gremlin de hielo" => "Habita en cavernas glaciares y disfruta sembrando el caos entre quienes se aventuran en el frío. Sus travesuras pueden resultar tan peligrosas como su magia.",
            "guardia magico" => "Un antiguo encantamiento lo mantiene vigilante junto a reliquias y cámaras selladas. No conoce el cansancio ni olvida la orden que recibió.",
            "guerrero del terror" => "Los ejércitos oscuros lo envían allí donde la resistencia debe quebrarse. Su armadura lleva emblemas de campañas que muchos reinos prefieren olvidar.",
            "guerrero elfo" => "Leal a los pactos de su pueblo, protege los bosques y los caminos antiguos. Su destreza nace de generaciones dedicadas a custodiar esas tierras.",
            "guerrero ogro" => "Criado entre clanes que veneran la fuerza, busca gloria en el combate y botín para su tribu. Su llegada suele preceder a una incursión mayor.",
            "horror congelado" => "Una presencia nacida del invierno eterno recorre las galerías heladas. El aire se vuelve cortante y las antorchas parecen débiles a su paso.",
            "lobo gigante" => "Cazador de los parajes salvajes, sigue el rastro de los viajeros hasta que la noche los deja sin refugio. Su aullido convoca a la manada.",
            "momia" => "Envuelta en vendas de una dinastía desaparecida, protege la tumba de su soberano. La profanación de su descanso ha despertado una maldición ancestral.",
            "ogro" => "Deambula por caminos y fortalezas en busca de comida, oro y desafíos. Aunque algunos aceptan servir como mercenarios, su lealtad suele durar lo que dura el botín.",
            "orco" => "Forjado en guerras y saqueos, combate por la fuerza de su clan y por el favor de sus caudillos. Sus incursiones dejan tras de sí aldeas en ruinas.",
            "oso de guerra polar" => "Entrenado en las tierras del norte, acompaña a sus guardianes a través de ventiscas y pasos helados. Su furia defiende el territorio que considera suyo.",
            "raptor" => "Acecha entre la vegetación de tierras remotas, siguiendo a su presa con paciencia animal. En la espesura, apenas se distingue de las sombras.",
            "reina de los espiritus" => "Señora de un séquito de almas errantes, gobierna desde un lugar apartado del mundo de los vivos. Sus órdenes resuenan incluso en las criptas vacías.",
            "secretario de la maldicion" => "Escriba de pactos oscuros, registra nombres y destinos en pergaminos que nadie debería leer. Sus archivos guardan secretos de antiguas expediciones.",
            "secretario del terror" => "Al servicio de fuerzas temibles, conserva crónicas de campañas y rituales. Se dice que sus escritos pueden convertir el miedo en un arma.",
            "senor de los ogros" => "El más temido de los caudillos ogros reúne clanes rivales bajo un mismo estandarte. Sus ambiciones amenazan las tierras fronterizas.",
            "serpiente" => "Se oculta entre piedras y raíces, aguardando a quien atraviese su territorio. En templos olvidados, su presencia suele estar ligada a antiguos guardianes.",
            "simio gigante" => "Una fuerza colosal de las selvas remotas, protege su dominio con un furor que hace temblar árboles y ruinas. Pocos exploradores regresan tras encontrar sus huellas.",
            "sir ragnar" => "Caballero de noble linaje, desapareció durante una misión en tierras peligrosas. Su destino se convirtió en una historia de cautiverio, valor y esperanza.",
            "venim" => "Envuelto en rumores y venenos, este adversario aparece en relatos de intrigas y expediciones malditas. Su rastro se pierde entre las sombras de los pasadizos.",
            "yeti" => "Señor de las cumbres nevadas, acecha en ventiscas donde el viajero pierde el rumbo. Las leyendas del norte aconsejan no seguir sus huellas.",
            "zombi" => "Un cadáver reanimado por la magia negra abandona su sepultura para servir a quien lo invocó. Avanza sin voluntad, obedeciendo una orden que no termina.",
            _ => CreateFactionLore(name, faction)
        };

        return lore;
    }

    private static string CreateFactionLore(string name, string faction)
    {
        // Cada facción aporta un contexto distinto; una frase variable evita asignar la misma plantilla a monstruos nuevos.
        var factionLore = faction.Trim() switch
        {
            "Bestias" => "En las espesuras donde los caminos se desvanecen, las bestias obedecen a la ley de la caza y el territorio.",
            "Criaturas Congeladas" => "Más allá de los pasos nevados, las criaturas del hielo despiertan con las tormentas y guardan secretos enterrados bajo glaciares.",
            "Criaturas del Terror" => "La influencia de Zargon impregna a estas criaturas, cuyo paso anuncia que una fuerza antigua vuelve a extender su dominio.",
            "Elfos" => "Los relatos élficos hablan de guardianes y errantes ligados a bosques antiguos, donde cada sendero conserva la memoria de su pueblo.",
            "Muertos Vivientes" => "Una magia impía quebró el descanso de los muertos vivientes y los ató a criptas, tumbas y voluntades oscuras.",
            "Ogros" => "En los territorios fronterizos, los clanes ogros disputan fortalezas, botín y el respeto que sólo concede la fuerza.",
            "Pieles Verdes" => "Las tribus de pieles verdes recorren túneles y tierras salvajes, siempre en busca de nuevas presas y botín.",
            "Singulares" => "Las crónicas apenas coinciden sobre estas figuras: cada una parece seguir un destino propio, ligado a un misterio mayor.",
            _ => "En una región olvidada del reino, los rumores sobre esta criatura se transmiten entre viajeros y cronistas."
        };

        var variants = new[]
        {
            $"Los exploradores describen a {name} como una presencia que surge cuando las antorchas empiezan a extinguirse.",
            $"El nombre de {name} aparece en mapas tachados y diarios de expedición que nadie se atreve a completar.",
            $"Quienes regresan de sus dominios aseguran que {name} deja señales antes de mostrarse, aunque pocos saben interpretarlas.",
            $"En las aldeas cercanas se cuenta que {name} custodia un secreto por el que muchos aventureros no han vuelto.",
            $"Una vieja balada menciona a {name} entre los peligros que aguardan al otro lado de las puertas selladas."
        };
        var hash = MonsterCsvParsing.NormalizeName(name).Aggregate(17, (value, character) => unchecked(value * 31 + character));
        var variant = variants[(int)((uint)hash % (uint)variants.Length)];
        return $"{factionLore} {variant}";
    }

    private async Task ImportSpellsOfTerrorAsync(string path)
    {
        var records = CsvRecordReader.ReadRecords(path);
        var existing = (await _context.SpellsOfTerror.ToListAsync())
            .GroupBy(spell => MonsterCsvParsing.NormalizeName(spell.Name))
            .ToDictionary(group => group.Key, group => group.First());

        foreach (var fields in records)
        {
            var name = CsvRecordReader.GetText(fields, "Nombre");
            if (!existing.TryGetValue(MonsterCsvParsing.NormalizeName(name), out var spell))
            {
                spell = new SpellOfTerror(0, name, CsvRecordReader.GetText(fields, "Descripción"), CsvRecordReader.GetText(fields, "Tipo"), TypeAppearsParser.Parse(CsvRecordReader.GetText(fields, "Apariciones")));
                _context.SpellsOfTerror.Add(spell);
                existing[MonsterCsvParsing.NormalizeName(name)] = spell;
                continue;
            }

            spell.Name = name;
            spell.Description = CsvRecordReader.GetText(fields, "Descripción");
            spell.Type = CsvRecordReader.GetText(fields, "Tipo");
            spell.Appears = TypeAppearsParser.Parse(CsvRecordReader.GetText(fields, "Apariciones"));
        }

        await _context.SaveChangesAsync();
    }

    private async Task ImportMonsterSpellLinksAsync(string monsterCsvPath)
    {
        var monstersByName = (await _context.Monsters.AsNoTracking().ToListAsync())
            .ToDictionary(monster => MonsterCsvParsing.NormalizeName(monster.Name));
        var spellsByName = (await _context.SpellsOfTerror.AsNoTracking().ToListAsync())
            .GroupBy(spell => MonsterCsvParsing.NormalizeName(spell.Name))
            .ToDictionary(group => group.Key, group => group.First());
        var links = new HashSet<(int MonsterId, int SpellId)>();

        foreach (var fields in CsvRecordReader.ReadRecords(monsterCsvPath))
        {
            if (!monstersByName.TryGetValue(MonsterCsvParsing.NormalizeName(CsvRecordReader.GetText(fields, "Nombre")), out var monster)) continue;

            foreach (var abilityName in MonsterCsvParsing.ReadAbilityNames(CsvRecordReader.GetText(fields, "Habilidad")))
            {
                if (spellsByName.TryGetValue(MonsterCsvParsing.NormalizeName(abilityName), out var spell))
                    links.Add((monster.Id, spell.Id));
            }
        }

        // Sólo los nombres que también existen en SpellsOfTerror producen una relación con clave foránea.
        _context.MonsterSpellOfTerrors.AddRange(links.Select(link => new MonsterSpellOfTerror
        {
            MonsterId = link.MonsterId,
            SpellOfTerrorId = link.SpellId
        }));
        await _context.SaveChangesAsync();
    }

    private async Task ImportMiscellaneaAsync(string ruta)
    {
        if (await _context.Miscellanea.AnyAsync()) return;

        foreach (var datos in CsvRecordReader.ReadRecords(ruta))
        {
            var entry = new MiscellaneaItem(
                id: 0,
                name: CsvRecordReader.GetText(datos, "Nombre"),
                description: CsvRecordReader.GetText(datos, "Descripción"),
                price: CsvRecordReader.GetInteger(datos, "Precio (Monedas de Oro)"),
                appears: TypeAppearsParser.Parse(CsvRecordReader.GetText(datos, "Apariciones"))
            );
            _context.Miscellanea.Add(entry);
        }
        await _context.SaveChangesAsync();
    }

    private async Task ImportEquipmentsAndArtifactsAsync(string equipmentPath, string artifactPath)
    {
        if (await _context.EquipmentsAndArtifacts.AnyAsync()) return;

        foreach (var datos in CsvRecordReader.ReadRecords(equipmentPath))
        {
            var item = new EquipmentsAndArtifacts(
                id: 0,
                name: CsvRecordReader.GetText(datos, "Nombre"),
                description: CsvRecordReader.GetText(datos, "Descripción"),
                price: CsvRecordReader.GetInteger(datos, "Precio (Monedas de Oro)"),
                type: CsvRecordReader.GetText(datos, "Tipo"),
                appears: TypeAppearsParser.Parse(CsvRecordReader.GetText(datos, "Apariciones")),
                weight: CsvRecordReader.GetText(datos, "Peso"),
                category: "Equipment"
            );
            _context.EquipmentsAndArtifacts.Add(item);
        }

        foreach (var datos in CsvRecordReader.ReadRecords(artifactPath))
        {
            var item = new EquipmentsAndArtifacts(
                id: 0,
                name: CsvRecordReader.GetText(datos, "Nombre"),
                description: CsvRecordReader.GetText(datos, "Descripción"),
                price: 0,
                type: CsvRecordReader.GetText(datos, "Tipo"),
                appears: TypeAppearsParser.Parse(CsvRecordReader.GetText(datos, "Apariciones")),
                weight: "",
                category: "Artifact"
            );
            _context.EquipmentsAndArtifacts.Add(item);
        }
        await _context.SaveChangesAsync();
    }

    private async Task ImportMonsterAbilitiesAsync(string ruta)
    {
        if (await _context.MonsterAbilities.AnyAsync()) return;

        foreach (var datos in CsvRecordReader.ReadRecords(ruta))
        {
            var ability = new MonsterAbility(
                id: 0,
                name: CsvRecordReader.GetText(datos, "Nombre"),
                description: CsvRecordReader.GetText(datos, "Descripción"),
                appears: TypeAppearsParser.Parse(CsvRecordReader.GetText(datos, "Apariciones"))
            );
            _context.MonsterAbilities.Add(ability);
        }
        await _context.SaveChangesAsync();
    }
}
