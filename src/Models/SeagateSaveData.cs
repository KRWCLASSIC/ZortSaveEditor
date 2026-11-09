using System;
using System.Collections.Generic;

namespace Zse.Models;

public enum SeagateIslands
{
    Starter,
    Aminakoy,
    PirateIsland,
    Tavern,
    SwordIsland,
    SumoIsland,
    HookIsland,
    BigContent1,
    BigContent2,
    SideContentIslandTop1,
    SideContentIslandTop2,
    SideContentIslandTop3,
    SideContentIslandBottom1,
    SideContentIslandBottom2,
    SideContentIslandBottom3,
    SideContentIslandBottom4
}

[Flags]
public enum TelerosaIslandTalkFlags
{
    None = 0,
    PirateIsland1 = 1,
    PirateIsland2 = 2,
    PirateIsland3 = 4,
    Tavern = 8,
    Starter = 0x10
}

[Flags]
public enum PlayerBoatUpgrades : byte
{
    None = 0,
    ZortiStack = 1,
    GasStack = 2,
    PlankStack = 4,
    BountiStack = 8,
    JetEngine = 16,
    Swords = 32,
    GrapplingHooks = 64
}

public enum SeagateCheckpoints : byte
{
    Start = 0,
    PlayerBoat = 1,
    Aminakoy = 2,
    AminakoyDonkeys = 3,
    AminakoyArenaAwake = 4,
    AminakoyArenaInside = 5,
    PirateIsland = 6,
    PirateIslandAfterJump = 7,
    PirateIslandSwordFight = 8,
    PirateIslandBeforePlatform = 9,
    PirateIslandPlatform = 10,
    PirateIslandInsideElevator = 11,
    NessShip1 = 12,
    NessShip2 = 13,
    NessShip3 = 14,
    TypeBAfterLock = 15,
    TypeCAfterLock = 16,
    PirateIslandOffice = 17,
    PirateIslandCells = 18
}

public enum AllItems : byte
{
    Flashlight = 0,
    Saw = 1,
    DoorKey = 2,
    GasCan = 3,
    CabinetKey = 4,
    VanTire = 5,
    RubiksCube = 6,
    Aminaking = 7,
    BowlingBall = 8,
    IDCard = 9,
    ThrowableBottle = 10,
    Airhorn = 11,
    Bounty = 12,
    Zorran = 13,
    Recorder = 14,
    Guitar = 15,
    Violin = 16,
    Accordion = 17,
    PlushieZorti = 18,
    JimmysKey = 19,
    MudurHeart = 20,
    BoxingGloves = 21,
    GrapplingHook = 22,
    Snowball = 23,
    Boomerang = 24,
    ZubbidiRaki = 25,
    WaterGun = 26,
    AminakuCrown = 27,
    ThrowableCheese = 28,
    ThrowableCupcake = 29,
    FlareGun = 30,
    FlareShell = 31,
    Plank = 32,
    Sword = 33,
    CellKeys = 34,
    Ak47 = 35,
    None = 255
}

public struct InventorySlotData
{
    public byte ItemIndex;
    public byte Data;
}

public sealed class PlayerInventoryProfile
{
    public ulong SteamId { get; set; }
    public int SteamIdOffset { get; set; } = -1;
    public int ValueTupleOffset { get; set; } = -1;
    public InventorySlotData[] Slots { get; set; } = new InventorySlotData[5];
}

[Serializable]
public sealed class SeagateSaveData
{
    public const byte SAVE_CURRENT_VERSION = 0;

    public byte version;
    public SeagateStoryData? Story;
    public SeagatePlayerBoatSaveData? PlayerBoat;
    public SeagateIslandSaveData[]? Islands;
    public WorldSaveData? World;
    public uint croachKills;
    public float timeOfDay;

    public static SeagateSaveData GetEmptySaveData()
    {
        return new SeagateSaveData
        {
            version = 0,
            PlayerBoat = new SeagatePlayerBoatSaveData(),
            Story = new SeagateStoryData(),
            Islands = Array.Empty<SeagateIslandSaveData>(),
            World = WorldSaveData.GetEmptySaveData(),
            croachKills = 0u,
            timeOfDay = 0f
        };
    }
}

[Serializable]
public sealed class SeagatePlayerBoatSaveData
{
    public bool afterGreeting;
    public bool isRepaired;
    public float positionX;
    public float positionZ;
    public float yaw;
    public float fuelReserve = 100f;
    public byte plankStack;
    public byte gasStack;
    public byte zortiStack;
    public byte bountiStack;
    public byte upgrades;
    public byte damageMask;
    public byte mapCursorIndex;
    public byte mapTrackIndex;
    public ushort mapNotificationMask;
}

[Serializable]
public sealed class SeagateStoryData
{
    public bool? enteredTavernAndTelerosaTalk;
    public bool? completedPirateIslandAndTelerosaTalk;
    public bool massacreHappened;
    public bool? forceNextSpawnAsShipOrBubbie;
    public bool lastSpawnedShip;
    public float bigEnemySpawnCooldownTime;
    public TelerosaIslandTalkFlags telerosaIslandTalkFlags;
}

[Serializable]
public sealed class SeagateIslandSaveData
{
    public SeagateIslands id;
    public bool isCompleted;
    public bool didTelerosaCommentAboutGettingClose;
    public ulong[]? dungeonFinishers;
}

[Serializable]
public sealed class WorldSaveData
{
    public int money;
    public byte checkpointIndex;
    public ushort pickedCollectiblesMask;
    public Dictionary<string, string> saveRecords = new();
    public Dictionary<ulong, (ulong, ushort)> playerInventories = new();
    [NonSerialized]
    public List<PlayerInventoryProfile> playerProfiles = new();
    public Dictionary<string, byte> objectives = new();

    public static WorldSaveData GetEmptySaveData()
    {
        return new WorldSaveData
        {
            saveRecords = new Dictionary<string, string>(),
            playerInventories = new Dictionary<ulong, (ulong, ushort)>(),
            playerProfiles = new List<PlayerInventoryProfile>(),
            objectives = new Dictionary<string, byte>()
        };
    }
}

