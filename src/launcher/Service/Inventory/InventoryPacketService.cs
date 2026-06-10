using System.Collections.Immutable;
using System.IO;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.Model.Primitive;
using kyxsan.Service.Metadata;

namespace kyxsan.Service.Inventory;

[Service(ServiceLifetime.Scoped)]
internal sealed class InventoryPacketService
{
    private static readonly string[] WeaponTypeNames = ["", "单手剑", "", "", "", "", "", "", "", "", "法器", "双手剑", "弓", "长柄武器"];

    private readonly IMetadataService metadataService;

    public InventoryPacketService(IMetadataService metadataService)
    {
        this.metadataService = metadataService;
    }

    public async ValueTask<List<WeaponEntry>> LoadWeaponsFromJsonAsync(string directory, CancellationToken token = default)
    {
        string jsonPath = Path.Combine(directory, "inventory.json");
        if (!File.Exists(jsonPath))
            return [];

        byte[] bytes = await File.ReadAllBytesAsync(jsonPath, token).ConfigureAwait(false);
        InventoryJson? inventory = JsonSerializer.Deserialize<InventoryJson>(bytes);
        if (inventory?.Weapons is null || inventory.Weapons.Count == 0)
            return [];

        ImmutableDictionary<WeaponId, Weapon> weaponMap = await metadataService.GetIdToWeaponMapAsync(token).ConfigureAwait(false);

        List<WeaponEntry> result = new(inventory.Weapons.Count);
        foreach (RawWeapon raw in inventory.Weapons)
        {
            WeaponId wid = (WeaponId)(uint)raw.Id;
            string name;
            string type;
            int rank;

            string description;
            string affixName;
            string affixDescription;

            if (weaponMap.TryGetValue(wid, out Weapon? weapon))
            {
                name = weapon.Name;
                int wt = (int)weapon.WeaponType;
                type = wt >= 0 && wt < WeaponTypeNames.Length ? WeaponTypeNames[wt] : "未知";
                rank = (int)weapon.RankLevel;
                description = weapon.Description;
                affixName = weapon.Affix?.Name ?? string.Empty;
                affixDescription = weapon.Affix?.Descriptions is { Length: > 0 } descs
                    ? descs[Math.Min(raw.Refinement > 0 ? raw.Refinement - 1 : 0, descs.Length - 1)].Description
                    : string.Empty;
            }
            else
            {
                name = $"未知武器({raw.Id})";
                type = "未知";
                rank = 0;
                description = string.Empty;
                affixName = string.Empty;
                affixDescription = string.Empty;
            }

            result.Add(new WeaponEntry
            {
                Id = raw.Id,
                Name = name,
                Type = type,
                Rank = rank,
                Level = raw.Level,
                Refinement = raw.Refinement,
                Promote = raw.Promote,
                Description = description,
                AffixName = affixName,
                AffixDescription = affixDescription,
            });
        }

        return result;
    }
}

internal sealed class InventoryJson
{
    [JsonPropertyName("weapons")]
    public List<RawWeapon>? Weapons { get; set; }
}

internal sealed class RawWeapon
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("promote")]
    public int Promote { get; set; }

    [JsonPropertyName("refinement")]
    public int Refinement { get; set; }
}

public sealed class WeaponEntry
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Rank { get; set; }
    public int Level { get; set; }
    public int Refinement { get; set; }
    public int Promote { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AffixName { get; set; } = string.Empty;
    public string AffixDescription { get; set; } = string.Empty;

    public string Stars => new('\u2605', Rank);
    public string LevelText => $"Lv.{Level}";
    public string PromoteText => $"\u7a81\u7834 {Promote}";
    public string RefinementText => $"\u7cbe\u70bc {Refinement}";
}
