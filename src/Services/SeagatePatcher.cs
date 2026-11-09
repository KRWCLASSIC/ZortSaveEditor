using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Zse.Models;

namespace Zse.Services;

public sealed class ZortSerializationBinder : SerializationBinder
{
    private const string TargetAssembly = "Assembly-CSharp";

    public override Type? BindToType(string assemblyName, string typeName)
    {
        if (assemblyName.StartsWith("Assembly-CSharp", StringComparison.OrdinalIgnoreCase))
        {
            return typeName switch
            {
                "SeagateSaveData" => typeof(SeagateSaveData),
                "SeagatePlayerBoatSaveData" => typeof(SeagatePlayerBoatSaveData),
                "SeagateStoryData" => typeof(SeagateStoryData),
                "SeagateIslandSaveData" => typeof(SeagateIslandSaveData),
                "SeagateIslandSaveData[]" => typeof(SeagateIslandSaveData[]),
                "WorldSaveData" => typeof(WorldSaveData),
                "TelerosaIslandTalkFlags" => typeof(TelerosaIslandTalkFlags),
                "PlayerBoatUpgrades" => typeof(PlayerBoatUpgrades),
                "SeagateIslands" => typeof(SeagateIslands),
                "SeagateCheckpoints" => typeof(SeagateCheckpoints),
                "AllItems" => typeof(AllItems),
                _ => Type.GetType($"Zse.Models.{typeName}")
            };
        }
        return null;
    }

    public override void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
    {
        if (serializedType.Namespace == "Zse.Models")
        {
            assemblyName = TargetAssembly;
            typeName = serializedType.Name;
            return;
        }
        assemblyName = null;
        typeName = null;
    }
}

public sealed class SeagatePatcher
{
    private readonly ZortSerializationBinder _binder = new();

    public bool HasData { get; private set; }

    public SeagateSaveData Load(Stream stream)
    {
#pragma warning disable SYSLIB0011
        var formatter = new BinaryFormatter { Binder = _binder };
        var data = (SeagateSaveData)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011

        HasData = true;

        // Sync player profiles list from the dictionary for UI convenience
        data.World ??= WorldSaveData.GetEmptySaveData();
        data.World.playerProfiles ??= new();
        data.World.playerProfiles.Clear();

        if (data.World.playerInventories != null)
        {
            foreach (var (steamId, (item1, item2)) in data.World.playerInventories)
            {
                var profile = new PlayerInventoryProfile
                {
                    SteamId = steamId,
                    Slots = new InventorySlotData[5]
                };

                for (int s = 0; s < 4; s++)
                {
                    profile.Slots[s].ItemIndex = (byte)((item1 >> (s * 16)) & 0xFF);
                    profile.Slots[s].Data = (byte)((item1 >> (s * 16 + 8)) & 0xFF);
                }
                profile.Slots[4].ItemIndex = (byte)(item2 & 0xFF);
                profile.Slots[4].Data = (byte)((item2 >> 8) & 0xFF);

                data.World.playerProfiles.Add(profile);
            }
        }

        return data;
    }

    public void Save(Stream stream, SeagateSaveData save)
    {
        // Sync player profiles back into playerInventories dictionary
        if (save.World != null)
        {
            save.World.playerInventories ??= new();
            save.World.playerInventories.Clear();
            if (save.World.playerProfiles != null)
            {
                foreach (var profile in save.World.playerProfiles)
                {
                    ulong item1 = 0;
                    for (int s = 0; s < 4; s++)
                    {
                        byte item = profile.Slots[s].ItemIndex;
                        byte data = profile.Slots[s].Data;
                        item1 |= ((ulong)item) << (s * 16);
                        item1 |= ((ulong)data) << (s * 16 + 8);
                    }
                    ushort item2 = (ushort)(profile.Slots[4].ItemIndex | (profile.Slots[4].Data << 8));
                    save.World.playerInventories[profile.SteamId] = (item1, item2);
                }
            }
        }

#pragma warning disable SYSLIB0011
        var formatter = new BinaryFormatter { Binder = _binder };
        formatter.Serialize(stream, save);
#pragma warning restore SYSLIB0011
    }
}
