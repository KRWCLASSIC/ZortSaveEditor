using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zse.Bridge;
using Zse.Models;
using Zse.Services;

namespace Zse.UI;

public sealed class MainWindow
{
    private readonly IntPtr _appHandle;
    private readonly SaveManager _saveManager;

    private readonly QMainWindow _window;
    private readonly QLabel _lblPath;

    // --- Tab 1: Seagate Controls ---
    private readonly QCheckBox _chkRepaired = new("Boat Initial Beach Quest Repaired");
    private readonly QDoubleSpinBox _dsbFuel = new(0.0, 9999.0, 100.0, 5.0);
    private readonly QSpinBox _sbPlanks = new(0, 99, 0);
    private readonly QSpinBox _sbGas = new(0, 99, 0);
    private readonly QSpinBox _sbZorti = new(0, 99, 0);
    private readonly QSpinBox _sbBounti = new(0, 99, 0);
    private readonly QDoubleSpinBox _dsbCoordX = new(-10000.0, 10000.0, 0.0, 1.0);
    private readonly QDoubleSpinBox _dsbCoordZ = new(-10000.0, 10000.0, 0.0, 1.0);
    private readonly QDoubleSpinBox _dsbCoordYaw = new(-360.0, 360.0, 0.0, 5.0);

    // Boat Upgrades Checkboxes (PlayerBoatUpgrades bitmask)
    private readonly QCheckBox _chkUpZorti = new("Plushie Zorti Rack (0x01)");
    private readonly QCheckBox _chkUpGas = new("Gasoline Stack Rack (0x02)");
    private readonly QCheckBox _chkUpPlanks = new("Plank Stack Rack (0x04)");
    private readonly QCheckBox _chkUpBounti = new("Bounti Fridge Rack (0x08)");
    private readonly QCheckBox _chkUpJetEngine = new("Jet Engine (0x10)");
    private readonly QCheckBox _chkUpSwords = new("Swords Rack (0x20)");
    private readonly QCheckBox _chkUpGrapplingHooks = new("Grappling Hooks (0x40)");

    // Hull Damage Holes Checkboxes (damageMask bits 0-7)
    private readonly QCheckBox[] _chkDamageHoles = new QCheckBox[8]
    {
        new("Hole #1 (Active Leak)"),
        new("Hole #2 (Active Leak)"),
        new("Hole #3 (Active Leak)"),
        new("Hole #4 (Active Leak)"),
        new("Hole #5 (Active Leak)"),
        new("Hole #6 (Active Leak)"),
        new("Hole #7 (Active Leak)"),
        new("Hole #8 (Active Leak)")
    };

    private readonly QSpinBox _sbMapCursor = new(0, 255, 0);
    private readonly QSpinBox _sbMapTrack = new(0, 255, 0);

    private readonly QSpinBox _sbMoney = new(0, 99999999, 0);
    private readonly QSpinBox _sbCroach = new(0, 999999, 0);
    private readonly QDoubleSpinBox _dsbTimeOfDay = new(-1.0, 1.0, 0.5, 0.05);
    private readonly QComboBox _cmbCheckpoint = new();
    private readonly QCheckBox[] _chkCollectibles = new QCheckBox[5]
    {
        new("Collectible #1"),
        new("Collectible #2"),
        new("Collectible #3"),
        new("Collectible #4"),
        new("Collectible #5")
    };
    private readonly QCheckBox _chkMassacre = new("Aminaku Massacre Triggered");
    private readonly QComboBox _cmbNextSpawn = new();
    private readonly QDoubleSpinBox _dsbSpawnCooldown = new(-1.0, 999999.0, 0.0, 10.0);

    // Story Dialogs
    private readonly QCheckBox _chkEnteredTavern = new("Entered Tavern & Talked with Telerosa");
    private readonly QCheckBox _chkCompletedPirateTalk = new("Completed Pirate Island & Talked with Telerosa");
    private readonly QCheckBox _chkTalkStarter = new("Telerosa: Starter Beach Talk (0x10)");
    private readonly QCheckBox _chkTalkTavern = new("Telerosa: Tavern Talk (0x08)");
    private readonly QCheckBox _chkTalkPirate1 = new("Telerosa: Pirate Island Talk 1 (0x01)");
    private readonly QCheckBox _chkTalkPirate2 = new("Telerosa: Pirate Island Talk 2 (0x02)");
    private readonly QCheckBox _chkTalkPirate3 = new("Telerosa: Pirate Island Talk 3 (0x04)");

    // All 16 Seagate Islands (Official in-game names)
    private readonly Dictionary<SeagateIslands, QCheckBox> _islandCheckboxes = new()
    {
        [SeagateIslands.Starter] = new("Starting Island (Crash Site)"),
        [SeagateIslands.Aminakoy] = new("Aminakoy (Village)"),
        [SeagateIslands.PirateIsland] = new("Pirate Island (Fortress & Cells)"),
        [SeagateIslands.Tavern] = new("Old Tavern"),
        [SeagateIslands.SwordIsland] = new("Abandoned Barracks (Sword Island)"),
        [SeagateIslands.SumoIsland] = new("Boox Arena (Sumo Island)"),
        [SeagateIslands.HookIsland] = new("Abandoned Inventor (Hook Island)"),
        [SeagateIslands.BigContent1] = new("Abandoned Boiler Room (Gas Depot)"),
        [SeagateIslands.BigContent2] = new("Abandoned Hardware Store (Flare Gun)"),
        [SeagateIslands.SideContentIslandTop1] = new("Mamoka Island (Top 1)"),
        [SeagateIslands.SideContentIslandTop2] = new("Sype Island (Top 2)"),
        [SeagateIslands.SideContentIslandTop3] = new("Lemon Island (Top 3)"),
        [SeagateIslands.SideContentIslandBottom1] = new("Cago Island (Bottom 1)"),
        [SeagateIslands.SideContentIslandBottom2] = new("Kitten Island (Bottom 2)"),
        [SeagateIslands.SideContentIslandBottom3] = new("Down Island (Bottom 3)"),
        [SeagateIslands.SideContentIslandBottom4] = new("Sparkling Island (Bottom 4)")
    };

    // Item Catalog & Mappings (All 36 items + Empty Slot)
    private static readonly (byte Id, string Name)[] CatalogItems =
    [
        (255, "(Empty Slot)"),
        (35,  "AK-47 Rifle (35)"),
        (33,  "Sword (33)"),
        (30,  "Flare Gun (30)"),
        (31,  "Flare Shell (31)"),
        (22,  "Grappling Hook (22)"),
        (21,  "Boxing Gloves (21)"),
        (0,   "Flashlight (0)"),
        (3,   "Gasoline Can (3)"),
        (32,  "Plank (32)"),
        (6,   "Rubik's Cube (6)"),
        (27,  "Aminaku Crown (27)"),
        (34,  "Cell Keys (34)"),
        (2,   "Door Key (2)"),
        (4,   "Cabinet Key (4)"),
        (19,  "Jimmy's Key (19)"),
        (20,  "Mudur Heart (20)"),
        (1,   "Saw (1)"),
        (5,   "Van Tire (5)"),
        (7,   "Aminaking (7)"),
        (8,   "Bowling Ball (8)"),
        (9,   "ID Card (9)"),
        (10,  "Throwable Bottle (10)"),
        (11,  "Airhorn (11)"),
        (12,  "Bounty (12)"),
        (13,  "Zorran (13)"),
        (14,  "Recorder / Flute (14)"),
        (15,  "Guitar (15)"),
        (16,  "Violin (16)"),
        (17,  "Accordion (17)"),
        (18,  "Plushie Zorti (18)"),
        (23,  "Snowball (23)"),
        (24,  "Boomerang (24)"),
        (25,  "Zubbidi Raki (25)"),
        (26,  "Water Gun (26)"),
        (28,  "Throwable Cheese (28)"),
        (29,  "Throwable Cupcake (29)")
    ];

    private static int ItemIdToComboIndex(byte id)
    {
        for (int i = 0; i < CatalogItems.Length; i++)
        {
            if (CatalogItems[i].Id == id) return i;
        }
        return 0;
    }

    private static byte ComboIndexToItemId(int idx)
    {
        if (idx >= 0 && idx < CatalogItems.Length)
            return CatalogItems[idx].Id;
        return 255;
    }

    // Equipped Inventory Controls (Dynamic multi-player support)
    private readonly QComboBox _cmbActivePlayer = new();
    private readonly QLineEdit _txtSteamId = new();
    private readonly QLabel _lblPlayerCount = new("No player profiles loaded");
    private readonly QComboBox[] _cmbSlotItem = [new(), new(), new(), new(), new()];
    private int _activePlayerIndex = -1;
    private bool _isPopulating = false;

    // --- Tab 2: Settings Controls ---
    private readonly QSpinBox _sbWidth = new(640, 7680, 1920);
    private readonly QSpinBox _sbHeight = new(480, 4320, 1080);
    private readonly QComboBox _cmbTargetFps = new();
    private readonly QComboBox _cmbFullscreen = new();
    private readonly QSpinBox _sbDisplayIndex = new(0, 10, 0);
    private readonly QCheckBox _chkVSync = new("VSync Enabled");
    private readonly QCheckBox _chkAntiAliasing = new("Anti-Aliasing");
    private readonly QCheckBox _chkSubtitles = new("Subtitles");
    private readonly QCheckBox _chkCameraEffects = new("Camera Effects (Insanity / Distortion)");
    private readonly QCheckBox _chkArachnophobia = new("Arachnophobia Mode (Spider Mimics)");
    private readonly QCheckBox _chkHeadBob = new("Head Bob");
    private readonly QCheckBox _chkCameraLean = new("Camera Lean");
    private readonly QCheckBox _chkMotionBlur = new("Motion Blur");
    private readonly QCheckBox _chkInvertY = new("Invert Mouse Y Axis");
    private readonly QCheckBox _chkInvertScroll = new("Invert Scroll Wheel");
    private readonly QDoubleSpinBox _dsbLensDistortion = new(0.0, 1.0, 0.3, 0.05);
    private readonly QDoubleSpinBox _dsbSensitivity = new(0.01, 100.0, 3.0, 0.1);
    private readonly QCheckBox _chkEnableController = new("Enable Gamepad Controller");
    private readonly QDoubleSpinBox _dsbControllerSensitivity = new(0.1, 50.0, 10.0, 0.5);
    private readonly QDoubleSpinBox _dsbVolume = new(0.0, 2.0, 0.8, 0.05);
    private readonly QDoubleSpinBox _dsbVoiceOut = new(0.0, 3.0, 1.5, 0.1);
    private readonly QDoubleSpinBox _dsbVoiceIn = new(0.0, 3.0, 1.65, 0.1);

    // --- Tab 3: Finished Maps Controls ---
    private readonly QCheckBox _chkFinZortagane = new("Episode 1 - Zortagane Factory (ZortaganeMapOlan)");
    private readonly QCheckBox _chkFinYozgat = new("Episode 2 - Yozgat (YozgatMapOlan)");
    private readonly QCheckBox _chkFinSeagate = new("Episode 3 - Seagate (SeagateMapOlan)");
    private readonly QCheckBox _chkFinErzurum = new("Episode 4 - Erzurum (ErzurumMapOlan)");

    // --- Tab 4: Discovered Entities (Checklist) ---
    private readonly Dictionary<EntitiesLog, QCheckBox> _entityCheckboxes = new()
    {
        [EntitiesLog.ManSlender] = new("Slender Man"),
        [EntitiesLog.FriendlySkinwalker] = new("Friendly Skinwalker"),
        [EntitiesLog.Dexter] = new("Dexter"),
        [EntitiesLog.Osmis] = new("Osmis"),
        [EntitiesLog.Giant] = new("Giant"),
        [EntitiesLog.NaberMudur] = new("Naber Mudur"),
        [EntitiesLog.Zoomie] = new("Zoomie"),
        [EntitiesLog.Shark] = new("Shark"),
        [EntitiesLog.ScaryFaces] = new("Scary Faces"),
        [EntitiesLog.Pirates] = new("Pirates"),
        [EntitiesLog.LittleRaft] = new("Little Raft"),
        [EntitiesLog.Gobit] = new("Gobit"),
        [EntitiesLog.CRoaches] = new("CRoach Bugs"),
        [EntitiesLog.Bubbie] = new("Bubbie (Whale)"),
        [EntitiesLog.BlindJohn] = new("Blind John"),
        [EntitiesLog.BigGuy] = new("Big Guy"),
        [EntitiesLog.Ness] = new("Ness"),
        [EntitiesLog.AminakuBoss] = new("Aminaku Boss"),
        [EntitiesLog.AminakuHito] = new("Aminaku Hito")
    };

    public MainWindow(IntPtr appHandle, SaveManager saveManager)
    {
        _appHandle = appHandle;
        _saveManager = saveManager;

        _window = new QMainWindow();
        _window.SetTitle("Zort Save Editor (zse)");
        _window.Resize(860, 680);

        // Native Menu Bar
        SetupMenuBar();

        var central = new QWidget();
        var rootLayout = new QVBoxLayout();

        // 1. Top Control Bar (Clean native styling)
        var topBox = new QGroupBox("Active Save Location");
        var topLayout = new QHBoxLayout();

        _lblPath = new QLabel($"Folder: {_saveManager.CurrentDirectory}");
        topLayout.AddWidget(_lblPath, 1);

        var btnBrowse = new QPushButton("Browse...");
        btnBrowse.Clicked += OnBrowseClicked;
        topLayout.AddWidget(btnBrowse);

        var btnReload = new QPushButton("Reload");
        btnReload.Clicked += OnReloadClicked;
        topLayout.AddWidget(btnReload);

        var btnSave = new QPushButton("Save All Changes");
        btnSave.Clicked += OnSaveClicked;
        topLayout.AddWidget(btnSave);

        topBox.SetLayout(topLayout);
        rootLayout.AddWidget(topBox);

        // Initialize inventory slot item comboboxes
        for (int s = 0; s < 5; s++)
        {
            foreach (var item in CatalogItems)
            {
                _cmbSlotItem[s].AddItem(item.Name);
            }
        }
        _cmbActivePlayer.SelectedIndexChanged += OnActivePlayerIndexChanged;

        // 2. Tab Widget (Only the 4 content categories)
        var tabs = new QTabWidget();
        tabs.AddTab(BuildSeagateTab(), "Episode 3: Seagate");
        tabs.AddTab(BuildSettingsTab(), "Game Settings");
        tabs.AddTab(BuildFinishedTab(), "Episodes Beaten");
        tabs.AddTab(BuildEntityLogTab(), "Discovered Entities");
        rootLayout.AddWidget(tabs, 1);

        central.SetLayout(rootLayout);
        _window.SetCentralWidget(central);

        Populate();
        _window.SetStatusMessage("Ready. Save files loaded successfully.");
    }

    public void Show() => _window.Show();

    private void SetupMenuBar()
    {
        var menuBar = _window.MenuBar;

        // File Menu
        var fileMenu = menuBar.AddMenu("&File");
        fileMenu.AddAction("&Open Save Folder...", OnBrowseClicked, "Ctrl+O");
        fileMenu.AddAction("&Save All Changes", OnSaveClicked, "Ctrl+S");
        fileMenu.AddAction("&Reload from Disk", OnReloadClicked, "F5");
        fileMenu.AddSeparator();
        fileMenu.AddAction("E&xit", () => QtNative.AppQuit(_appHandle), "Alt+F4");

        // View Menu
        var viewMenu = menuBar.AddMenu("&View");
        viewMenu.AddAction("Toggle &Dark Mode", OnThemeToggleClicked);

        // Help Menu
        var helpMenu = menuBar.AddMenu("&Help");
        helpMenu.AddAction("&About Zort Save Editor...", OnAboutClicked);
    }

    private QWidget BuildSeagateTab()
    {
        var scrollContent = new QWidget();
        var layout = new QVBoxLayout();

        // 1. Boat Status & Supplies Group
        var grpBoat = new QGroupBox("Boat Status & Supplies");
        var boatGrid = new QGridLayout();
        boatGrid.AddWidget(_chkRepaired, 0, 0, 1, 2);
        boatGrid.AddWidget(new QLabel("Fuel Reserve:"), 1, 0);
        boatGrid.AddWidget(_dsbFuel, 1, 1);
        boatGrid.AddWidget(new QLabel("Planks Count:"), 2, 0);
        boatGrid.AddWidget(_sbPlanks, 2, 1);
        boatGrid.AddWidget(new QLabel("Gasoline Cans:"), 3, 0);
        boatGrid.AddWidget(_sbGas, 3, 1);
        boatGrid.AddWidget(new QLabel("Plushie Zorti (Respawn Dolls):"), 4, 0);
        boatGrid.AddWidget(_sbZorti, 4, 1);
        boatGrid.AddWidget(new QLabel("Bounti (Fridge Food Stack):"), 5, 0);
        boatGrid.AddWidget(_sbBounti, 5, 1);
        boatGrid.AddWidget(new QLabel("Map Cursor Index:"), 6, 0);
        boatGrid.AddWidget(_sbMapCursor, 6, 1);
        boatGrid.AddWidget(new QLabel("Map Track Index:"), 7, 0);
        boatGrid.AddWidget(_sbMapTrack, 7, 1);

        var coordRow = new QWidget();
        var coordLayout = new QHBoxLayout();
        coordLayout.AddWidget(_dsbCoordX);
        coordLayout.AddWidget(_dsbCoordZ);
        coordLayout.AddWidget(_dsbCoordYaw);
        coordRow.SetLayout(coordLayout);
        boatGrid.AddWidget(new QLabel("Position (X, Z, Yaw):"), 8, 0);
        boatGrid.AddWidget(coordRow, 8, 1);
        grpBoat.SetLayout(boatGrid);
        layout.AddWidget(grpBoat);

        // 2. Boat Upgrades Group (Unlocked Racks & Mounted Equipment)
        var grpUpgrades = new QGroupBox("Boat Upgrades (Unlocked Racks & Equipment)");
        var upLayout = new QVBoxLayout();
        var upGrid = new QGridLayout();
        upGrid.AddWidget(_chkUpPlanks, 0, 0);
        upGrid.AddWidget(_chkUpGas, 0, 1);
        upGrid.AddWidget(_chkUpZorti, 1, 0);
        upGrid.AddWidget(_chkUpBounti, 1, 1);
        upGrid.AddWidget(_chkUpJetEngine, 2, 0);
        upGrid.AddWidget(_chkUpSwords, 2, 1);
        upGrid.AddWidget(_chkUpGrapplingHooks, 3, 0, 1, 2);
        upLayout.AddLayout(upGrid);

        var upBtnRow = new QWidget();
        var upBtnLayout = new QHBoxLayout();
        var btnUnlockAllUpgrades = new QPushButton("Unlock All Upgrades");
        btnUnlockAllUpgrades.Clicked += () =>
        {
            _chkUpPlanks.IsChecked = true;
            _chkUpGas.IsChecked = true;
            _chkUpZorti.IsChecked = true;
            _chkUpBounti.IsChecked = true;
            _chkUpJetEngine.IsChecked = true;
            _chkUpSwords.IsChecked = true;
            _chkUpGrapplingHooks.IsChecked = true;
        };
        var btnClearAllUpgrades = new QPushButton("Lock All Upgrades");
        btnClearAllUpgrades.Clicked += () =>
        {
            _chkUpPlanks.IsChecked = false;
            _chkUpGas.IsChecked = false;
            _chkUpZorti.IsChecked = false;
            _chkUpBounti.IsChecked = false;
            _chkUpJetEngine.IsChecked = false;
            _chkUpSwords.IsChecked = false;
            _chkUpGrapplingHooks.IsChecked = false;
        };
        upBtnLayout.AddWidget(btnUnlockAllUpgrades);
        upBtnLayout.AddWidget(btnClearAllUpgrades);
        upBtnRow.SetLayout(upBtnLayout);
        upLayout.AddWidget(upBtnRow);
        grpUpgrades.SetLayout(upLayout);
        layout.AddWidget(grpUpgrades);

        // 3. Hull Damage Holes Group (damageMask)
        var grpDamage = new QGroupBox("Boat Hull Damage (Active Leak Spots)");
        var dmgLayout = new QVBoxLayout();
        var dmgGrid = new QGridLayout();
        dmgGrid.AddWidget(_chkDamageHoles[0], 0, 0);
        dmgGrid.AddWidget(_chkDamageHoles[1], 0, 1);
        dmgGrid.AddWidget(_chkDamageHoles[2], 1, 0);
        dmgGrid.AddWidget(_chkDamageHoles[3], 1, 1);
        dmgGrid.AddWidget(_chkDamageHoles[4], 2, 0);
        dmgGrid.AddWidget(_chkDamageHoles[5], 2, 1);
        dmgGrid.AddWidget(_chkDamageHoles[6], 3, 0);
        dmgGrid.AddWidget(_chkDamageHoles[7], 3, 1);
        dmgLayout.AddLayout(dmgGrid);

        var dmgBtnRow = new QWidget();
        var dmgBtnLayout = new QHBoxLayout();
        var btnRepairAll = new QPushButton("Repair All Holes (Clean Hull)");
        btnRepairAll.Clicked += () =>
        {
            for (int i = 0; i < 8; i++) _chkDamageHoles[i].IsChecked = false;
        };
        var btnDamageAll = new QPushButton("Puncture All Spots");
        btnDamageAll.Clicked += () =>
        {
            for (int i = 0; i < 8; i++) _chkDamageHoles[i].IsChecked = true;
        };
        dmgBtnLayout.AddWidget(btnRepairAll);
        dmgBtnLayout.AddWidget(btnDamageAll);
        dmgBtnRow.SetLayout(dmgBtnLayout);
        dmgLayout.AddWidget(dmgBtnRow);
        grpDamage.SetLayout(dmgLayout);
        layout.AddWidget(grpDamage);

        // 4. World, Time & Progression Group
        var grpStory = new QGroupBox("World, Time & Progression");
        var storyGrid = new QGridLayout();
        storyGrid.AddWidget(new QLabel("Wallet Money:"), 0, 0);
        storyGrid.AddWidget(_sbMoney, 0, 1);
        storyGrid.AddWidget(new QLabel("CRoach Bugs Killed:"), 1, 0);
        storyGrid.AddWidget(_sbCroach, 1, 1);

        // Time of Day Row with Quick Presets
        var todRow = new QWidget();
        var todLayout = new QHBoxLayout();
        todLayout.AddWidget(_dsbTimeOfDay, 1);
        var btnNoon = new QPushButton("Noon (0.5)");
        btnNoon.Clicked += () => _dsbTimeOfDay.Value = 0.5;
        var btnMidnight = new QPushButton("Midnight (-0.5)");
        btnMidnight.Clicked += () => _dsbTimeOfDay.Value = -0.5;
        var btnSunset = new QPushButton("Sunset (0.0)");
        btnSunset.Clicked += () => _dsbTimeOfDay.Value = 0.0;
        todLayout.AddWidget(btnNoon);
        todLayout.AddWidget(btnMidnight);
        todLayout.AddWidget(btnSunset);
        todRow.SetLayout(todLayout);
        storyGrid.AddWidget(new QLabel("Time of Day (-1.0 to 1.0):"), 2, 0);
        storyGrid.AddWidget(todRow, 2, 1);

        // Respawn Checkpoint Dropdown
        string[] checkpoints =
        [
            "0: Start Beach",
            "1: Player Boat",
            "2: Aminakoy Village",
            "3: Aminakoy Donkeys",
            "4: Aminakoy Arena Awake",
            "5: Aminakoy Arena Inside",
            "6: Pirate Island (Entrance)",
            "7: Pirate Island After Jump",
            "8: Pirate Island Sword Fight",
            "9: Pirate Island Before Platform",
            "10: Pirate Island Platform",
            "11: Pirate Island Inside Elevator",
            "12: Ness Ship 1",
            "13: Ness Ship 2",
            "14: Ness Ship 3",
            "15: Type B After Lock",
            "16: Type C After Lock",
            "17: Pirate Island Office",
            "18: Pirate Island Cells"
        ];
        foreach (var cp in checkpoints) _cmbCheckpoint.AddItem(cp);
        storyGrid.AddWidget(new QLabel("Respawn Checkpoint:"), 3, 0);
        storyGrid.AddWidget(_cmbCheckpoint, 3, 1);

        // Seagate Collectibles (5 total in Episode 3)
        var colBox = new QWidget();
        var colLayout = new QVBoxLayout();
        var colCheckRow = new QHBoxLayout();
        for (int i = 0; i < 5; i++)
        {
            colCheckRow.AddWidget(_chkCollectibles[i]);
        }
        colLayout.AddLayout(colCheckRow);

        var colBtnRow = new QHBoxLayout();
        var btnAllCol = new QPushButton("Collect All 5 (Unlock Achievement)");
        btnAllCol.Clicked += () =>
        {
            for (int i = 0; i < 5; i++) _chkCollectibles[i].IsChecked = true;
        };
        var btnClearCol = new QPushButton("Clear All");
        btnClearCol.Clicked += () =>
        {
            for (int i = 0; i < 5; i++) _chkCollectibles[i].IsChecked = false;
        };
        colBtnRow.AddWidget(btnAllCol);
        colBtnRow.AddWidget(btnClearCol);
        colLayout.AddLayout(colBtnRow);
        colBox.SetLayout(colLayout);
        storyGrid.AddWidget(new QLabel("Seagate Collectibles (5 items):"), 4, 0);
        storyGrid.AddWidget(colBox, 4, 1);

        storyGrid.AddWidget(_chkMassacre, 5, 0, 1, 2);

        _cmbNextSpawn.AddItem("Default (Alternating Random)");
        _cmbNextSpawn.AddItem("Force Bubbie (Whale)");
        _cmbNextSpawn.AddItem("Force Pirate Ship");
        storyGrid.AddWidget(new QLabel("Next Ocean Spawn:"), 6, 0);
        storyGrid.AddWidget(_cmbNextSpawn, 6, 1);

        storyGrid.AddWidget(new QLabel("Encounter Cooldown (seconds):"), 7, 0);
        storyGrid.AddWidget(_dsbSpawnCooldown, 7, 1);
        grpStory.SetLayout(storyGrid);
        layout.AddWidget(grpStory);

        // 5. Story Dialogs & Telerosa Talks
        var grpDialogs = new QGroupBox("Story Events & Telerosa Dialogs");
        var dlgLayout = new QVBoxLayout();
        var dlgGrid = new QGridLayout();
        dlgGrid.AddWidget(_chkEnteredTavern, 0, 0);
        dlgGrid.AddWidget(_chkCompletedPirateTalk, 0, 1);
        dlgGrid.AddWidget(_chkTalkStarter, 1, 0);
        dlgGrid.AddWidget(_chkTalkTavern, 1, 1);
        dlgGrid.AddWidget(_chkTalkPirate1, 2, 0);
        dlgGrid.AddWidget(_chkTalkPirate2, 2, 1);
        dlgGrid.AddWidget(_chkTalkPirate3, 3, 0, 1, 2);
        dlgLayout.AddLayout(dlgGrid);

        var dlgBtnRow = new QWidget();
        var dlgBtnLayout = new QHBoxLayout();
        var btnUnlockAllDialogs = new QPushButton("Unlock All Dialogs");
        btnUnlockAllDialogs.Clicked += () =>
        {
            _chkEnteredTavern.IsChecked = true;
            _chkCompletedPirateTalk.IsChecked = true;
            _chkTalkStarter.IsChecked = true;
            _chkTalkTavern.IsChecked = true;
            _chkTalkPirate1.IsChecked = true;
            _chkTalkPirate2.IsChecked = true;
            _chkTalkPirate3.IsChecked = true;
        };
        var btnClearAllDialogs = new QPushButton("Clear All Dialogs");
        btnClearAllDialogs.Clicked += () =>
        {
            _chkEnteredTavern.IsChecked = false;
            _chkCompletedPirateTalk.IsChecked = false;
            _chkTalkStarter.IsChecked = false;
            _chkTalkTavern.IsChecked = false;
            _chkTalkPirate1.IsChecked = false;
            _chkTalkPirate2.IsChecked = false;
            _chkTalkPirate3.IsChecked = false;
        };
        dlgBtnLayout.AddWidget(btnUnlockAllDialogs);
        dlgBtnLayout.AddWidget(btnClearAllDialogs);
        dlgBtnRow.SetLayout(dlgBtnLayout);
        dlgLayout.AddWidget(dlgBtnRow);
        grpDialogs.SetLayout(dlgLayout);
        layout.AddWidget(grpDialogs);

        // 6. Islands Group (All 16 Archipelago Islands)
        var grpIslands = new QGroupBox("Completed Islands (All 16 Archipelago Islands)");
        var islLayout = new QVBoxLayout();
        var islandGrid = new QGridLayout();
        int islRow = 0;
        int islCol = 0;
        foreach (var chk in _islandCheckboxes.Values)
        {
            islandGrid.AddWidget(chk, islRow, islCol);
            islCol++;
            if (islCol >= 2)
            {
                islCol = 0;
                islRow++;
            }
        }
        islLayout.AddLayout(islandGrid);

        var islBtnRow = new QWidget();
        var islBtnLayout = new QHBoxLayout();
        var btnCompleteAllIslands = new QPushButton("Complete All 16 Islands");
        btnCompleteAllIslands.Clicked += () =>
        {
            foreach (var chk in _islandCheckboxes.Values) chk.IsChecked = true;
        };
        var btnResetAllIslands = new QPushButton("Reset All Islands");
        btnResetAllIslands.Clicked += () =>
        {
            foreach (var chk in _islandCheckboxes.Values) chk.IsChecked = false;
        };
        islBtnLayout.AddWidget(btnCompleteAllIslands);
        islBtnLayout.AddWidget(btnResetAllIslands);
        islBtnRow.SetLayout(islBtnLayout);
        islLayout.AddWidget(islBtnRow);
        grpIslands.SetLayout(islLayout);
        layout.AddWidget(grpIslands);

        // 7. Equipped Player Inventory (Dynamic Multi-Player Support)
        var grpInv = new QGroupBox("Equipped Player Inventory (5 Active Slots)");
        var invLayout = new QVBoxLayout();

        // Player Selection Header Row
        var playerSelRow = new QWidget();
        var playerSelLayout = new QHBoxLayout();
        playerSelLayout.AddWidget(new QLabel("Active Player Profile:"));
        playerSelLayout.AddWidget(_cmbActivePlayer, 1);

        var btnAddPlayer = new QPushButton("+ Add Player");
        btnAddPlayer.Clicked += OnAddPlayerClicked;
        playerSelLayout.AddWidget(btnAddPlayer);

        var btnRemovePlayer = new QPushButton("- Remove");
        btnRemovePlayer.Clicked += OnRemovePlayerClicked;
        playerSelLayout.AddWidget(btnRemovePlayer);

        playerSelLayout.AddWidget(_lblPlayerCount);
        playerSelRow.SetLayout(playerSelLayout);
        invLayout.AddWidget(playerSelRow);

        // Player Steam ID Row
        var steamIdRow = new QWidget();
        var steamIdLayout = new QHBoxLayout();
        steamIdLayout.AddWidget(new QLabel("Steam ID (64-bit uint):"));
        steamIdLayout.AddWidget(_txtSteamId, 1);
        steamIdRow.SetLayout(steamIdLayout);
        invLayout.AddWidget(steamIdRow);

        // 5 Inventory Slots Grid (Clean item dropdowns)
        var slotGrid = new QGridLayout();
        for (int s = 0; s < 5; s++)
        {
            slotGrid.AddWidget(new QLabel($"Slot #{s + 1}:"), s, 0);
            slotGrid.AddWidget(_cmbSlotItem[s], s, 1);
        }
        invLayout.AddLayout(slotGrid);

        grpInv.SetLayout(invLayout);
        layout.AddWidget(grpInv);

        scrollContent.SetLayout(layout);
        return new QScrollArea(scrollContent);
    }

    private QWidget BuildSettingsTab()
    {
        var panel = new QWidget();
        var layout = new QVBoxLayout();

        // 1. Display & Graphics
        var grpDisp = new QGroupBox("Display & Graphics");
        var dispGrid = new QGridLayout();
        dispGrid.AddWidget(new QLabel("Resolution Width:"), 0, 0);
        dispGrid.AddWidget(_sbWidth, 0, 1);
        dispGrid.AddWidget(new QLabel("Resolution Height:"), 1, 0);
        dispGrid.AddWidget(_sbHeight, 1, 1);
        _cmbTargetFps.AddItem("Unlimited (0)");
        _cmbTargetFps.AddItem("30 FPS (1)");
        _cmbTargetFps.AddItem("60 FPS (2)");
        _cmbTargetFps.AddItem("120 FPS (3)");
        _cmbTargetFps.AddItem("144 FPS (4)");
        _cmbTargetFps.AddItem("165 FPS (5)");
        _cmbTargetFps.AddItem("180 FPS (6)");
        _cmbTargetFps.AddItem("240 FPS (7)");
        _cmbTargetFps.AddItem("500 FPS (8)");
        dispGrid.AddWidget(new QLabel("Target FPS:"), 2, 0);
        dispGrid.AddWidget(_cmbTargetFps, 2, 1);

        _cmbFullscreen.AddItem("Exclusive Fullscreen (0)");
        _cmbFullscreen.AddItem("Borderless Window (1)");
        _cmbFullscreen.AddItem("Maximized Window (2)");
        _cmbFullscreen.AddItem("Windowed (3)");
        dispGrid.AddWidget(new QLabel("Fullscreen Mode:"), 3, 0);
        dispGrid.AddWidget(_cmbFullscreen, 3, 1);

        dispGrid.AddWidget(new QLabel("Monitor Display Index:"), 4, 0);
        dispGrid.AddWidget(_sbDisplayIndex, 4, 1);

        dispGrid.AddWidget(_chkVSync, 5, 0);
        dispGrid.AddWidget(_chkAntiAliasing, 5, 1);
        dispGrid.AddWidget(_chkMotionBlur, 6, 0);
        dispGrid.AddWidget(_chkCameraEffects, 6, 1);
        dispGrid.AddWidget(new QLabel("Lens Distortion:"), 7, 0);
        dispGrid.AddWidget(_dsbLensDistortion, 7, 1);
        grpDisp.SetLayout(dispGrid);
        layout.AddWidget(grpDisp);

        // 2. Gameplay, Controls & Audio
        var grpGame = new QGroupBox("Gameplay, Controls & Audio");
        var gameGrid = new QGridLayout();
        gameGrid.AddWidget(_chkSubtitles, 0, 0);
        gameGrid.AddWidget(_chkArachnophobia, 0, 1);
        gameGrid.AddWidget(_chkHeadBob, 1, 0);
        gameGrid.AddWidget(_chkCameraLean, 1, 1);
        gameGrid.AddWidget(_chkInvertY, 2, 0);
        gameGrid.AddWidget(_chkInvertScroll, 2, 1);
        gameGrid.AddWidget(_chkEnableController, 3, 0);
        gameGrid.AddWidget(new QLabel("Mouse Sensitivity:"), 4, 0);
        gameGrid.AddWidget(_dsbSensitivity, 4, 1);
        gameGrid.AddWidget(new QLabel("Controller Sensitivity:"), 5, 0);
        gameGrid.AddWidget(_dsbControllerSensitivity, 5, 1);
        gameGrid.AddWidget(new QLabel("Master Volume:"), 6, 0);
        gameGrid.AddWidget(_dsbVolume, 6, 1);
        gameGrid.AddWidget(new QLabel("Voicechat Output Volume:"), 7, 0);
        gameGrid.AddWidget(_dsbVoiceOut, 7, 1);
        gameGrid.AddWidget(new QLabel("Voicechat Input Volume:"), 8, 0);
        gameGrid.AddWidget(_dsbVoiceIn, 8, 1);
        grpGame.SetLayout(gameGrid);
        layout.AddWidget(grpGame);

        layout.AddStretch();
        panel.SetLayout(layout);
        return new QScrollArea(panel);
    }

    private QWidget BuildFinishedTab()
    {
        var panel = new QWidget();
        var layout = new QVBoxLayout();
        var grp = new QGroupBox("Episodes Beaten (finished.json)");
        var inner = new QVBoxLayout();
        inner.AddWidget(_chkFinZortagane);
        inner.AddWidget(_chkFinYozgat);
        inner.AddWidget(_chkFinSeagate);
        inner.AddWidget(_chkFinErzurum);

        var btnRow = new QWidget();
        var btnLayout = new QHBoxLayout();
        var btnAll = new QPushButton("Mark All Episodes Finished");
        btnAll.Clicked += () =>
        {
            _chkFinZortagane.IsChecked = true;
            _chkFinYozgat.IsChecked = true;
            _chkFinSeagate.IsChecked = true;
            _chkFinErzurum.IsChecked = true;
        };
        var btnNone = new QPushButton("Clear All");
        btnNone.Clicked += () =>
        {
            _chkFinZortagane.IsChecked = false;
            _chkFinYozgat.IsChecked = false;
            _chkFinSeagate.IsChecked = false;
            _chkFinErzurum.IsChecked = false;
        };
        btnLayout.AddWidget(btnAll);
        btnLayout.AddWidget(btnNone);
        btnRow.SetLayout(btnLayout);
        inner.AddWidget(btnRow);

        inner.AddStretch();
        grp.SetLayout(inner);
        layout.AddWidget(grp);
        panel.SetLayout(layout);
        return panel;
    }

    private QWidget BuildEntityLogTab()
    {
        var panel = new QWidget();
        var layout = new QVBoxLayout();

        var topRow = new QHBoxLayout();
        var lblDesc = new QLabel("Select discovered entities and mimics (entitylog.txt):");
        topRow.AddWidget(lblDesc, 1);

        var btnSelectAll = new QPushButton("Select All");
        btnSelectAll.Clicked += () =>
        {
            foreach (var chk in _entityCheckboxes.Values) chk.IsChecked = true;
        };
        topRow.AddWidget(btnSelectAll);

        var btnClearAll = new QPushButton("Clear All");
        btnClearAll.Clicked += () =>
        {
            foreach (var chk in _entityCheckboxes.Values) chk.IsChecked = false;
        };
        topRow.AddWidget(btnClearAll);
        layout.AddLayout(topRow);

        var grp = new QGroupBox("Entities Log");
        var grid = new QGridLayout();
        int row = 0;
        int col = 0;
        foreach (var chk in _entityCheckboxes.Values)
        {
            grid.AddWidget(chk, row, col);
            col++;
            if (col >= 2)
            {
                col = 0;
                row++;
            }
        }
        grp.SetLayout(grid);
        layout.AddWidget(grp);

        layout.AddStretch();
        panel.SetLayout(layout);
        return panel;
    }

    private void Populate()
    {
        _lblPath.Text = $"Folder: {_saveManager.CurrentDirectory}";

        // 1. Seagate
        var sg = _saveManager.SeagateSave ?? SeagateSaveData.GetEmptySaveData();
        var boat = sg.PlayerBoat ?? new SeagatePlayerBoatSaveData();
        var story = sg.Story ?? new SeagateStoryData();

        _chkRepaired.IsChecked = boat.isRepaired;
        _dsbFuel.Value = boat.fuelReserve;
        _sbPlanks.Value = boat.plankStack;
        _sbGas.Value = boat.gasStack;
        _sbZorti.Value = boat.zortiStack;
        _sbBounti.Value = boat.bountiStack;
        _sbMapCursor.Value = boat.mapCursorIndex;
        _sbMapTrack.Value = boat.mapTrackIndex;
        _dsbCoordX.Value = boat.positionX;
        _dsbCoordZ.Value = boat.positionZ;
        _dsbCoordYaw.Value = boat.yaw;

        _chkUpPlanks.IsChecked = (boat.upgrades & (byte)PlayerBoatUpgrades.PlankStack) != 0;
        _chkUpGas.IsChecked = (boat.upgrades & (byte)PlayerBoatUpgrades.GasStack) != 0;
        _chkUpZorti.IsChecked = (boat.upgrades & (byte)PlayerBoatUpgrades.ZortiStack) != 0;
        _chkUpBounti.IsChecked = (boat.upgrades & (byte)PlayerBoatUpgrades.BountiStack) != 0;
        _chkUpJetEngine.IsChecked = (boat.upgrades & (byte)PlayerBoatUpgrades.JetEngine) != 0;
        _chkUpSwords.IsChecked = (boat.upgrades & (byte)PlayerBoatUpgrades.Swords) != 0;
        _chkUpGrapplingHooks.IsChecked = (boat.upgrades & (byte)PlayerBoatUpgrades.GrapplingHooks) != 0;

        for (int i = 0; i < 8; i++)
        {
            _chkDamageHoles[i].IsChecked = (boat.damageMask & (1 << i)) != 0;
        }

        _sbMoney.Value = sg.World?.money ?? 0;
        _sbCroach.Value = (int)sg.croachKills;
        _dsbTimeOfDay.Value = sg.timeOfDay;
        _cmbCheckpoint.SelectedIndex = Math.Clamp((int)(sg.World?.checkpointIndex ?? 0), 0, 18);
        ushort colMask = sg.World?.pickedCollectiblesMask ?? 0;
        for (int i = 0; i < 5; i++)
        {
            _chkCollectibles[i].IsChecked = (colMask & (1 << i)) != 0;
        }
        _chkMassacre.IsChecked = story.massacreHappened;

        // Story Dialogs
        _chkEnteredTavern.IsChecked = story.enteredTavernAndTelerosaTalk ?? false;
        _chkCompletedPirateTalk.IsChecked = story.completedPirateIslandAndTelerosaTalk ?? false;
        var talk = story.telerosaIslandTalkFlags;
        _chkTalkStarter.IsChecked = (talk & TelerosaIslandTalkFlags.Starter) != 0;
        _chkTalkTavern.IsChecked = (talk & TelerosaIslandTalkFlags.Tavern) != 0;
        _chkTalkPirate1.IsChecked = (talk & TelerosaIslandTalkFlags.PirateIsland1) != 0;
        _chkTalkPirate2.IsChecked = (talk & TelerosaIslandTalkFlags.PirateIsland2) != 0;
        _chkTalkPirate3.IsChecked = (talk & TelerosaIslandTalkFlags.PirateIsland3) != 0;

        int spawnIdx = 0;
        if (story.forceNextSpawnAsShipOrBubbie.HasValue)
        {
            spawnIdx = story.forceNextSpawnAsShipOrBubbie.Value ? 2 : 1;
        }
        _cmbNextSpawn.SelectedIndex = spawnIdx;
        _dsbSpawnCooldown.Value = story.bigEnemySpawnCooldownTime;

        if (sg.Islands != null)
        {
            var islMap = new Dictionary<SeagateIslands, bool>();
            foreach (var isl in sg.Islands)
            {
                if (isl != null) islMap[isl.id] = isl.isCompleted;
            }
            foreach (var (islId, chk) in _islandCheckboxes)
            {
                chk.IsChecked = islMap.GetValueOrDefault(islId);
            }
        }

        // Inventory Profiles (Dynamic Multi-Player)
        RebuildPlayerCombo(sg.World?.playerProfiles, selectIndex: 0);

        // 2. Settings
        var set = _saveManager.GameSettings ?? new SettingsSave();
        _sbWidth.Value = set.ScreenWidth;
        _sbHeight.Value = set.ScreenHeight;
        _cmbTargetFps.SelectedIndex = Math.Clamp(set.TargetFpsIndex, 0, 8);
        _cmbFullscreen.SelectedIndex = Math.Clamp(set.FullscreenIndex, 0, 3);
        _sbDisplayIndex.Value = set.DisplayIndex;
        _chkVSync.IsChecked = set.Vsync != 0;
        _chkAntiAliasing.IsChecked = set.AntiAliasing;
        _chkSubtitles.IsChecked = set.Subtitles;
        _chkCameraEffects.IsChecked = set.CameraEffects;
        _chkArachnophobia.IsChecked = set.ArachnophobiaMode;
        _chkHeadBob.IsChecked = set.HeadBob;
        _chkCameraLean.IsChecked = set.CameraLean;
        _chkMotionBlur.IsChecked = set.MotionBlur;
        _chkInvertY.IsChecked = set.InvertY;
        _chkInvertScroll.IsChecked = set.InvertScroll;
        _dsbLensDistortion.Value = set.LensDistortion;
        _dsbSensitivity.Value = set.SensitivityMouse;
        _chkEnableController.IsChecked = set.EnableController;
        _dsbControllerSensitivity.Value = set.SensitivityController;
        _dsbVolume.Value = set.MasterVolume;
        _dsbVoiceOut.Value = set.VoicechatOutputVolume;
        _dsbVoiceIn.Value = set.VoicechatInputVolume;

        // 3. Finished Maps
        _chkFinZortagane.IsChecked = _saveManager.FinishedMaps.Contains("ZortaganeMapOlan");
        _chkFinYozgat.IsChecked = _saveManager.FinishedMaps.Contains("YozgatMapOlan");
        _chkFinSeagate.IsChecked = _saveManager.FinishedMaps.Contains("SeagateMapOlan");
        _chkFinErzurum.IsChecked = _saveManager.FinishedMaps.Contains("ErzurumMapOlan");

        // 4. Entity Log
        foreach (var (flag, chk) in _entityCheckboxes)
        {
            chk.IsChecked = (_saveManager.EntityLog & flag) != 0;
        }
    }

    private void ApplyToModels()
    {
        // 1. Seagate
        _saveManager.SeagateSave ??= SeagateSaveData.GetEmptySaveData();
        _saveManager.SeagateSave.PlayerBoat ??= new SeagatePlayerBoatSaveData();
        _saveManager.SeagateSave.Story ??= new SeagateStoryData();
        _saveManager.SeagateSave.World ??= WorldSaveData.GetEmptySaveData();

        var boat = _saveManager.SeagateSave.PlayerBoat;
        boat.isRepaired = _chkRepaired.IsChecked;
        if (boat.isRepaired) boat.afterGreeting = true;
        boat.fuelReserve = (float)_dsbFuel.Value;
        boat.plankStack = (byte)_sbPlanks.Value;
        boat.gasStack = (byte)_sbGas.Value;
        boat.zortiStack = (byte)_sbZorti.Value;
        boat.bountiStack = (byte)_sbBounti.Value;
        boat.mapCursorIndex = (byte)_sbMapCursor.Value;
        boat.mapTrackIndex = (byte)_sbMapTrack.Value;
        boat.positionX = (float)_dsbCoordX.Value;
        boat.positionZ = (float)_dsbCoordZ.Value;
        boat.yaw = (float)_dsbCoordYaw.Value;

        byte up = 0;
        if (_chkUpZorti.IsChecked) up |= (byte)PlayerBoatUpgrades.ZortiStack;
        if (_chkUpGas.IsChecked) up |= (byte)PlayerBoatUpgrades.GasStack;
        if (_chkUpPlanks.IsChecked) up |= (byte)PlayerBoatUpgrades.PlankStack;
        if (_chkUpBounti.IsChecked) up |= (byte)PlayerBoatUpgrades.BountiStack;
        if (_chkUpJetEngine.IsChecked) up |= (byte)PlayerBoatUpgrades.JetEngine;
        if (_chkUpSwords.IsChecked) up |= (byte)PlayerBoatUpgrades.Swords;
        if (_chkUpGrapplingHooks.IsChecked) up |= (byte)PlayerBoatUpgrades.GrapplingHooks;
        boat.upgrades = up;

        byte dmg = 0;
        for (int i = 0; i < 8; i++)
        {
            if (_chkDamageHoles[i].IsChecked)
                dmg |= (byte)(1 << i);
        }
        boat.damageMask = dmg;

        _saveManager.SeagateSave.timeOfDay = (float)_dsbTimeOfDay.Value;
        _saveManager.SeagateSave.croachKills = (uint)_sbCroach.Value;
        _saveManager.SeagateSave.World.money = _sbMoney.Value;
        _saveManager.SeagateSave.World.checkpointIndex = (byte)_cmbCheckpoint.SelectedIndex;
        ushort colMask = 0;
        for (int i = 0; i < 5; i++)
        {
            if (_chkCollectibles[i].IsChecked)
                colMask |= (ushort)(1 << i);
        }
        _saveManager.SeagateSave.World.pickedCollectiblesMask = colMask;

        var story = _saveManager.SeagateSave.Story;
        story.massacreHappened = _chkMassacre.IsChecked;
        story.forceNextSpawnAsShipOrBubbie = _cmbNextSpawn.SelectedIndex switch
        {
            1 => false, // Bubbie
            2 => true,  // Pirate ship
            _ => null
        };
        story.bigEnemySpawnCooldownTime = (float)_dsbSpawnCooldown.Value;

        // Story Dialogs
        story.enteredTavernAndTelerosaTalk = _chkEnteredTavern.IsChecked;
        story.completedPirateIslandAndTelerosaTalk = _chkCompletedPirateTalk.IsChecked;
        TelerosaIslandTalkFlags talkFlags = TelerosaIslandTalkFlags.None;
        if (_chkTalkStarter.IsChecked) talkFlags |= TelerosaIslandTalkFlags.Starter;
        if (_chkTalkTavern.IsChecked) talkFlags |= TelerosaIslandTalkFlags.Tavern;
        if (_chkTalkPirate1.IsChecked) talkFlags |= TelerosaIslandTalkFlags.PirateIsland1;
        if (_chkTalkPirate2.IsChecked) talkFlags |= TelerosaIslandTalkFlags.PirateIsland2;
        if (_chkTalkPirate3.IsChecked) talkFlags |= TelerosaIslandTalkFlags.PirateIsland3;
        story.telerosaIslandTalkFlags = talkFlags;

        // All 16 Islands
        var islList = _saveManager.SeagateSave.Islands?.ToList() ?? new List<SeagateIslandSaveData>();
        foreach (var (islId, chk) in _islandCheckboxes)
        {
            var item = islList.FirstOrDefault(x => x.id == islId);
            if (item != null) item.isCompleted = chk.IsChecked;
            else islList.Add(new SeagateIslandSaveData { id = islId, isCompleted = chk.IsChecked });
        }
        _saveManager.SeagateSave.Islands = islList.ToArray();

        // Inventory
        SaveCurrentPlayerFromUi();

        // 2. Settings
        _saveManager.GameSettings ??= new SettingsSave();
        var set = _saveManager.GameSettings;
        set.ScreenWidth = _sbWidth.Value;
        set.ScreenHeight = _sbHeight.Value;
        set.TargetFpsIndex = _cmbTargetFps.SelectedIndex;
        set.FullscreenIndex = _cmbFullscreen.SelectedIndex;
        set.DisplayIndex = _sbDisplayIndex.Value;
        set.Vsync = _chkVSync.IsChecked ? 1 : 0;
        set.AntiAliasing = _chkAntiAliasing.IsChecked;
        set.Subtitles = _chkSubtitles.IsChecked;
        set.CameraEffects = _chkCameraEffects.IsChecked;
        set.ArachnophobiaMode = _chkArachnophobia.IsChecked;
        set.HeadBob = _chkHeadBob.IsChecked;
        set.CameraLean = _chkCameraLean.IsChecked;
        set.MotionBlur = _chkMotionBlur.IsChecked;
        set.InvertY = _chkInvertY.IsChecked;
        set.InvertScroll = _chkInvertScroll.IsChecked;
        set.LensDistortion = (float)_dsbLensDistortion.Value;
        set.SensitivityMouse = (float)_dsbSensitivity.Value;
        set.EnableController = _chkEnableController.IsChecked;
        set.SensitivityController = (float)_dsbControllerSensitivity.Value;
        set.MasterVolume = (float)_dsbVolume.Value;
        set.VoicechatOutputVolume = (float)_dsbVoiceOut.Value;
        set.VoicechatInputVolume = (float)_dsbVoiceIn.Value;

        // 3. Finished Maps
        void UpdateMap(string name, bool checkedVal)
        {
            if (checkedVal) _saveManager.FinishedMaps.Add(name);
            else _saveManager.FinishedMaps.Remove(name);
        }
        UpdateMap("ZortaganeMapOlan", _chkFinZortagane.IsChecked);
        UpdateMap("YozgatMapOlan", _chkFinYozgat.IsChecked);
        UpdateMap("SeagateMapOlan", _chkFinSeagate.IsChecked);
        UpdateMap("ErzurumMapOlan", _chkFinErzurum.IsChecked);

        // 4. Entity Log
        EntitiesLog mask = EntitiesLog.None;
        foreach (var (flag, chk) in _entityCheckboxes)
        {
            if (chk.IsChecked) mask |= flag;
        }
        _saveManager.EntityLog = mask;
    }

    private void RebuildPlayerCombo(List<PlayerInventoryProfile>? profiles, int selectIndex)
    {
        _isPopulating = true;
        _cmbActivePlayer.Clear();
        if (profiles != null && profiles.Count > 0)
        {
            for (int i = 0; i < profiles.Count; i++)
            {
                string role = i == 0 ? "Host" : $"Co-op Buddy #{i}";
                _cmbActivePlayer.AddItem($"Player {i + 1} ({role} - SteamID: {profiles[i].SteamId})");
            }
            _lblPlayerCount.Text = $"{profiles.Count} player{(profiles.Count > 1 ? "s" : "")} in save";
            int idx = Math.Clamp(selectIndex, 0, profiles.Count - 1);
            _activePlayerIndex = idx;
            _cmbActivePlayer.SelectedIndex = idx;
            LoadPlayerSlotsIntoUi(profiles[idx]);
        }
        else
        {
            _cmbActivePlayer.AddItem("(No player profiles found in save)");
            _lblPlayerCount.Text = "0 players in save";
            _activePlayerIndex = -1;
            _txtSteamId.Text = string.Empty;
            for (int s = 0; s < 5; s++)
            {
                _cmbSlotItem[s].SelectedIndex = 0;
            }
        }
        _isPopulating = false;
    }

    private void OnActivePlayerIndexChanged()
    {
        if (_isPopulating) return;
        SaveCurrentPlayerFromUi();

        int newIdx = _cmbActivePlayer.SelectedIndex;
        var profiles = _saveManager.SeagateSave?.World?.playerProfiles;
        if (profiles == null || newIdx < 0 || newIdx >= profiles.Count) return;

        _activePlayerIndex = newIdx;
        LoadPlayerSlotsIntoUi(profiles[newIdx]);
    }

    private void SaveCurrentPlayerFromUi()
    {
        if (_isPopulating) return;
        var profiles = _saveManager.SeagateSave?.World?.playerProfiles;
        if (profiles == null || _activePlayerIndex < 0 || _activePlayerIndex >= profiles.Count) return;

        var profile = profiles[_activePlayerIndex];
        if (ulong.TryParse(_txtSteamId.Text.Trim(), out ulong steamId))
        {
            profile.SteamId = steamId;
        }

        for (int s = 0; s < 5; s++)
        {
            byte newItem = ComboIndexToItemId(_cmbSlotItem[s].SelectedIndex);
            byte oldItem = profile.Slots[s].ItemIndex;
            byte oldData = profile.Slots[s].Data;

            profile.Slots[s].ItemIndex = newItem;
            if (newItem == 255) // Empty
            {
                profile.Slots[s].Data = 255;
            }
            else if (newItem == 30) // Flare Gun
            {
                profile.Slots[s].Data = 1; // loaded
            }
            else if (newItem == oldItem)
            {
                profile.Slots[s].Data = oldData;
            }
            else
            {
                profile.Slots[s].Data = 0;
            }
        }
    }

    private void LoadPlayerSlotsIntoUi(PlayerInventoryProfile profile)
    {
        _txtSteamId.Text = profile.SteamId.ToString();
        for (int s = 0; s < 5; s++)
        {
            _cmbSlotItem[s].SelectedIndex = ItemIdToComboIndex(profile.Slots[s].ItemIndex);
        }
    }

    private void OnAddPlayerClicked()
    {
        SaveCurrentPlayerFromUi();
        _saveManager.SeagateSave ??= SeagateSaveData.GetEmptySaveData();
        _saveManager.SeagateSave.World ??= WorldSaveData.GetEmptySaveData();
        _saveManager.SeagateSave.World.playerProfiles ??= new();
        var profiles = _saveManager.SeagateSave.World.playerProfiles;

        ulong newSteamId = 76561198000000000UL + (ulong)(profiles.Count + 1);
        var newProfile = new PlayerInventoryProfile
        {
            SteamId = newSteamId,
            Slots = new InventorySlotData[5]
        };
        for (int s = 0; s < 5; s++)
        {
            newProfile.Slots[s].ItemIndex = 255; // None
            newProfile.Slots[s].Data = 255;
        }
        profiles.Add(newProfile);

        RebuildPlayerCombo(profiles, selectIndex: profiles.Count - 1);
        _window.SetStatusMessage($"Added new player profile (SteamID: {newSteamId}). Enter your friend's SteamID and pick their items.");
    }

    private void OnRemovePlayerClicked()
    {
        var profiles = _saveManager.SeagateSave?.World?.playerProfiles;
        if (profiles == null || profiles.Count <= 1)
        {
            _window.SetStatusMessage("Cannot remove host player profile (at least one player profile must exist).");
            return;
        }

        if (_activePlayerIndex < 0 || _activePlayerIndex >= profiles.Count) return;

        profiles.RemoveAt(_activePlayerIndex);
        int nextIdx = Math.Min(_activePlayerIndex, profiles.Count - 1);
        RebuildPlayerCombo(profiles, selectIndex: nextIdx);
        _window.SetStatusMessage("Player profile removed.");
    }

    private void OnBrowseClicked()
    {
        string? selected = QFileDialog.GetExistingDirectory(_window, "Select Zort Save Folder", _saveManager.CurrentDirectory);
        if (!string.IsNullOrWhiteSpace(selected) && Directory.Exists(selected))
        {
            _saveManager.SetSaveDirectory(selected);
            _saveManager.LoadAll();
            Populate();
            _window.SetStatusMessage($"Switched save folder: {selected}");
        }
    }

    private void OnReloadClicked()
    {
        _saveManager.LoadAll();
        Populate();
        _window.SetStatusMessage("Reloaded save files from disk.");
    }

    private void OnSaveClicked()
    {
        try
        {
            ApplyToModels();
            _saveManager.SaveAll();
            _window.SetStatusMessage("All changes and backups saved successfully!");
            QMessageBox.ShowInfo(_window, "Zort Save Editor", "Successfully saved all changes!\nAutomatic backups (*.bak) were created.");
        }
        catch (Exception ex)
        {
            _window.SetStatusMessage($"Error saving: {ex.Message}");
            QMessageBox.ShowError(_window, "Save Error", $"Failed to save: {ex.Message}");
        }
    }

    private void OnThemeToggleClicked()
    {
        _saveManager.Config.IsDarkMode = !_saveManager.Config.IsDarkMode;
        _saveManager.SaveAppConfig();
        QtNative.AppSetDarkTheme(_appHandle, _saveManager.Config.IsDarkMode ? 1 : 0);
        _window.SetStatusMessage(_saveManager.Config.IsDarkMode ? "Theme: Dark Mode" : "Theme: Light Mode");
    }

    private void OnAboutClicked()
    {
        QtNative.MessageBox(_window.Handle, 0, "About Zort Save Editor",
            "Zort Save Editor (zse) v1.0.0\n" +
            "by KRWCLASSIC\n\n" +
            "Built for Zort v0.4.9\n\n" +
            "Offline save game editor and settings manager.");
    }
}
