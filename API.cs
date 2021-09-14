using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using Jotunn.Utils;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System.Reflection;
using System.Linq;

namespace ValEx
{
    public class Admin
    {
        public class AntiCheat
        {
            public class System
            {
                public static List<Player> localPlayers { get; } = Player.GetAllPlayers();
                public static Process[] localProcesses { get; } = Process.GetProcesses();
                public static Process[] localValheims { get; } = Process.GetProcessesByName("valheim.exe");
                public static List<Process> GetLocalProcesses()
                {
                    List<Process> tempList = new List<Process> { };
                    foreach (Process process in localProcesses)
                    {
                        tempList.Add(process);
                    }
                    return tempList;
                }
                public void InitializeOn(string ProcessName, bool DisplayWindow = false)
                {
                    foreach (Process localProcess in localProcesses)
                    {
                        if (localProcess.ProcessName == ProcessName)
                        {
                            CreateCMDBridge(ProcessName, DisplayWindow);
                        }
                    }
                }
                public void CreateCMDBridge(string ProcessName, bool DisplayWindow = false)
                {
                    Process cmdBridge = new Process();
                    {
                        cmdBridge.StartInfo.FileName = ProcessName;
                        if (DisplayWindow == true) { DisplayWindow = false; }
                        else { DisplayWindow = true; }
                        cmdBridge.StartInfo.CreateNoWindow = DisplayWindow;
                        cmdBridge.Start();
                    }
                }
                public void DisplayPlayerOnScreen(string PlayerName)
                {

                }
                public void KillProcess(string ProcessName)
                {
                    foreach (Process localProcess in localProcesses)
                    {
                        if (localProcess.ProcessName == ProcessName)
                        {
                            localProcess.Kill();
                        }
                    }
                }
                public void KillProcesses(string[] ProcessNames)
                {
                    foreach (string ProcessName in ProcessNames)
                    {
                        foreach (Process localProcess in localProcesses)
                        {
                            if (localProcess.ProcessName == ProcessName)
                            {
                                localProcess.Kill();
                            }
                        }
                    }
                }
            }
            public class Interface
            {

            }
        }
    }
    public static class Registry
    {
        public static List<string> DebugModules { get; set; } = new List<string> { };
        public static List<string> DebugConfigs { get; set; } = new List<string> { };
        public static List<string> EnabledContent_name { get; set; } = new List<string> { };
        public static List<bool> EnabledContent_bool { get; set; } = new List<bool> { };
        public static List<string> EnabledFeature_name { get; set; } = new List<string> { };
        public static List<bool> EnabledFeature_bool { get; set; } = new List<bool> { };
        public static List<string> Types { get; } = new List<string>() { "Atgeir", "Axe", "Battleaxe", "Club", "Bow", "Mace", "Knife", "Pickaxe", "Spear", "Sword", "Sledge", "Shield", "TowerShield" };
        public static List<string> Textures { get; } = new List<string>() { "Wood", "Stone", "Flint", "Bone", "Primal", "Bronze", "Abyssal", "Iron", "Ancient", "Silver", "Fang", "Blackmetal", "Needle", "Titanium", "Steel", "Fromyr", "Flamyr" };
        public static List<string> Materials { get; } = new List<string>() { "Wood", "Stone", "Flint", "BoneFragments", "TrollHide", "Bronze", "Chitin", "Iron", "ElderBark", "Silver", "WolfFang", "BlackMetal", "Needle", "Titanium_VE", "Steel_VE", "Fromyr_VE", "Flamyr_VE" };
    }
    public static class API
    {
        public static class Cache
        {
            public static AssetBundle Bundle { get; set; } = AssetUtils.LoadAssetBundleFromResources("zmaterials", typeof(Core).Assembly);
            public static ConfigFile Settings { get; set; } = new ConfigFile(Path.Combine(BepInEx.Paths.ConfigPath, "ValheimExpanded - (0) Settings.cfg"), true);
            public static ConfigFile Weapons { get; set; } = new ConfigFile(Path.Combine(BepInEx.Paths.ConfigPath, "ValheimExpanded - (2) Weapons.cfg"), true);
            public static ConfigFile Armors { get; set; } = new ConfigFile(Path.Combine(BepInEx.Paths.ConfigPath, "ValheimExpanded - (1) Armors.cfg"), true);
        }
        public static class Bundle
        {
            public static Mesh GetMesh(string MeshName)
            {
                string input = MeshName;
                Mesh output = Cache.Bundle.LoadAsset<Mesh>(input);
                return output;
            }
            public static Sprite GetIcon(string IconName)
            {
                string input = IconName;
                Sprite output = Cache.Bundle.LoadAsset<Sprite>(input);
                return output;
            }
            public static Material GetMaterial(string MaterialName)
            {
                string input = MaterialName;
                Material output = Cache.Bundle.LoadAsset<Material>(input);
                return output;
            }
            public static GameObject GetObject(string GameObjectName)
            {
                string input = GameObjectName;
                GameObject output = Cache.Bundle.LoadAsset<GameObject>(input);
                return output;
            }
            public static AssetBundle Load(string AssetBundle, bool PrintToConsole = false)
            {
                Cache.Bundle = AssetUtils.LoadAssetBundleFromResources(AssetBundle, typeof(Core).Assembly);
                UnityEngine.Object[] Assets = Cache.Bundle.LoadAllAssets();
                if (PrintToConsole == true)
                {
                    foreach (var Object in Assets)
                    {
                        Jotunn.Logger.LogMessage($"Found: Bundle ({Cache.Bundle.name}): Type[{Object.GetType()}] : Name '{Object.name}' ");
                        Jotunn.Logger.LogInfo($"Embedded resources: {string.Join(",", typeof(Core).Assembly.GetManifestResourceNames())}");
                        Jotunn.Logger.LogInfo($"Bundle On Load: {Cache.Bundle}");
                    }
                }
                return Cache.Bundle;
            }
            public static void Unload(bool UnloadAllObjects = false) { Cache.Bundle.Unload(UnloadAllObjects); }
        }
        public static class New
        {
            public class Armor
            {
                #region Item_Properties
                public ConfigEntry<string> CE_MeshName { get; set; }
                public ConfigEntry<string> CE_PrefabName { get; set; }
                public ConfigEntry<string> CE_CustomName { get; set; }
                public ConfigEntry<string> CE_TokenName { get; set; }
                public ConfigEntry<string> CE_Description { get; set; }
                public ConfigEntry<string> CE_Type { get; set; }
                public ConfigEntry<string> CE_Material { get; set; }
                public ConfigEntry<string> CE_Texture { get; set; }
                public ConfigEntry<string> CE_AmmoType { get; set; }
                public ConfigEntry<string> CE_Signature { get; set; }
                public ConfigEntry<int> CE_Armor { get; set; }
                public ConfigEntry<int> CE_ArmorPerLevel { get; set; }
                public ConfigEntry<int> CE_Durability { get; set; }
                public ConfigEntry<int> CE_DurabilityPerLevel { get; set; }
                public ConfigEntry<int> CE_MaxQuality { get; set; }
                public ConfigEntry<int> CE_MaxStackSize { get; set; }
                public ConfigEntry<int> CE_Value { get; set; }
                public ConfigEntry<int> CE_Weight { get; set; }
                public ConfigEntry<int> CE_MovementModifier { get; set; }
                public ConfigEntry<bool> CE_Teleportable { get; set; }
                public ConfigEntry<bool> CE_Repairable { get; set; }
                public ConfigEntry<bool> CE_Destroyable { get; set; }
                public ConfigEntry<string> CE_RecipeName { get; set; }
                public ConfigEntry<string> CE_CraftingStation { get; set; }
                public ConfigEntry<string> CE_RepairStation { get; set; }
                public ConfigEntry<string> CE_Station { get; set; }
                public ConfigEntry<int> CE_MinStationLevel { get; set; }
                public ConfigEntry<string> CE_Slot1Item { get; set; }
                public ConfigEntry<int> CE_Slot1Amount { get; set; }
                public ConfigEntry<int> CE_Slot1AmountPerLevel { get; set; }
                public ConfigEntry<string> CE_Slot2Item { get; set; }
                public ConfigEntry<int> CE_Slot2Amount { get; set; }
                public ConfigEntry<int> CE_Slot2AmountPerLevel { get; set; }
                public ConfigEntry<string> CE_Slot3Item { get; set; }
                public ConfigEntry<int> CE_Slot3Amount { get; set; }
                public ConfigEntry<int> CE_Slot3AmountPerLevel { get; set; }
                public ConfigEntry<string> CE_Slot4Item { get; set; }
                public ConfigEntry<int> CE_Slot4Amount { get; set; }
                public ConfigEntry<int> CE_Slot4AmountPerLevel { get; set; }
                public ConfigEntry<bool> CE_Enabled { get; set; }
                #endregion
                public Armor WithConfigs(string Material, string Type, string BasePrefab, string Signature, string SetNumber = "", bool FixVanilla = true)
                {
                    if (SetNumber != "") { SetNumber += "_"; }

                    #region Item_ConfigBinds
                    CE_Repairable = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Repairable", true, ConfigType: "Armor");
                    CE_Destroyable = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Breakable", false, ConfigType: "Armor");
                    CE_TokenName = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Name", $"{Material} {Type}", ConfigType: "Armor");
                    CE_Armor = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Armor", 0, ConfigType: "Armor");
                    CE_ArmorPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Armor Per Level", 50, ConfigType: "Armor");
                    CE_Durability = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Durability", 100, ConfigType: "Armor");
                    CE_DurabilityPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Durability Per Level", 50, ConfigType: "Armor");
                    CE_MaxQuality = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Max Quality", 1, ConfigType: "Armor");
                    CE_Teleportable = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Teleportable", true, ConfigType: "Armor");
                    CE_Station = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Station", "piece_workbench", ConfigType: "Armor");
                    CE_MinStationLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Minimum Station Level", 1, ConfigType: "Armor");
                    CE_Slot1Item = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 1: Item", "Stone", ConfigType: "Armor");
                    CE_Slot1Amount = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 1: Amount", 1, ConfigType: "Armor");
                    CE_Slot1AmountPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 1: Amount Per Level", 2, ConfigType: "Armor");
                    CE_Slot2Item = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 2: Item", "LeatherScraps", ConfigType: "Armor");
                    CE_Slot2Amount = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 2: Amount", 1, ConfigType: "Armor");
                    CE_Slot2AmountPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 2: Amount Per Level", 1, ConfigType: "Armor");
                    CE_Slot3Item = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 3: Item", "Resin", ConfigType: "Armor");
                    CE_Slot3Amount = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 3: Amount", 1, ConfigType: "Armor");
                    CE_Slot3AmountPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 3: Amount Per Level", 0, ConfigType: "Armor");
                    CE_Slot4Item = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 4: Item", "Wood", ConfigType: "Armor");
                    CE_Slot4Amount = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 4: Amount", 1, ConfigType: "Armor");
                    CE_Slot4AmountPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 4: Amount Per Level", 0, ConfigType: "Armor");
                    #endregion

                    string PrefabName; string CustomName; string Description;
                    if (Type == "Belt")
                    {
                        PrefabName = Type + Material; CustomName = Material + " " + Type;
                        Description = "Type: " + Type + Environment.NewLine + "Tier: " + Material + ".";
                    } else if (Type == "Cape")
                    {
                        PrefabName = Type + Material; CustomName = Material + " " + Type;
                        Description = "Type: " + Type + Environment.NewLine + "Tier: " + Material + ".";
                    } else if (Type == "Helmet")
                    {
                        PrefabName = Type + Material; CustomName = Material + " " + Type;
                        Description = "Type: " + Type + Environment.NewLine + "Tier: " + Material + ".";
                    } else
                    {
                        PrefabName = "Armor" + Material + Type; CustomName = Material + " " + Type;
                        Description = "Type: " + Type + Environment.NewLine + "Tier: " + Material + ".";
                    }
                    if (FixVanilla == true) { try { HelpMe.Fix.Vanilla(PrefabName); } finally { } }
                    CustomItem Item = new CustomItem(PrefabName + Signature, BasePrefab);
                    ItemManager.Instance.AddItem(Item);
                    GameObject Prefab                   = Item.ItemPrefab;
                    ItemDrop Drop                       = Item.ItemDrop;
                    ItemDrop.ItemData Data              = Drop.m_itemData;
                    ItemDrop.ItemData.SharedData Shared = Data.m_shared;
                    Shared.m_name                       = CustomName;
                    Shared.m_description                = Description;
                    Shared.m_armor                      = CE_Armor.Value;
                    Shared.m_armorPerLevel              = CE_ArmorPerLevel.Value;
                    Shared.m_canBeReparied              = CE_Repairable.Value;
                    Shared.m_destroyBroken              = CE_Destroyable.Value;
                    Shared.m_maxDurability              = CE_Durability.Value;
                    Shared.m_durabilityPerLevel         = CE_DurabilityPerLevel.Value;
                    Shared.m_maxQuality                 = CE_MaxQuality.Value;
                    Shared.m_questItem                  = false;
                    Shared.m_teleportable               = CE_Teleportable.Value;
                    Shared.m_equipStatusEffect          = null;
                    Renderer MeshRenderer               = Prefab.GetComponentInChildren<MeshRenderer>();
                    MeshRenderer.material               = Bundle.GetMaterial(Material);
                    Shared.m_armorMaterial              = ArmorMaterial( MeshRenderer.material, Type );
                    if (Type != "Helmet")
                    {   Transform chestWearPrefab = Prefab.transform.Find("attach_skin");
                        Renderer[] skinnedMeshRenderers = chestWearPrefab?.GetComponentsInChildren<Renderer>();
                        if (skinnedMeshRenderers != null)
                        {
                            foreach (Renderer renderer in skinnedMeshRenderers)
                            {
                                renderer.material = MeshRenderer.material;
                            }
                        }
                    }   else { }
                    ItemManager.Instance.AddRecipe(Recipe(
                        Type: Type,
                        Material: Material,
                        Signature: "_VE",
                        Amount: 1,
                        Station: CE_Station.Value,
                        MinStationLevel: CE_MinStationLevel.Value,
                        Enabled: true,
                        Requirements: Requirements(
                                        Slot(Item: CE_Slot1Item.Value, Amount: CE_Slot1Amount.Value, AmountPerLevel: CE_Slot1AmountPerLevel.Value),
                                        Slot(Item: CE_Slot2Item.Value, Amount: CE_Slot2Amount.Value, AmountPerLevel: CE_Slot2AmountPerLevel.Value),
                                        Slot(Item: CE_Slot3Item.Value, Amount: CE_Slot3Amount.Value, AmountPerLevel: CE_Slot3AmountPerLevel.Value),
                                        Slot(Item: CE_Slot4Item.Value, Amount: CE_Slot4Amount.Value, AmountPerLevel: CE_Slot4AmountPerLevel.Value))
                    ));

                    string log = CustomName; for (int i = CustomName.Length + 2; i < 57; i++) { log += " "; }
                    Registry.EnabledContent_name.Add($"| {log} |");

                    return this;
                }
            }
            public class BasicItem
            {
                public BasicItem WithConfigs(string Type, string Material, string BasePrefab, int Amount, string Station, int MinStationLevel, string Signature = "_VE", bool Teleportable = true, int Value = 0, bool Enabled = true, params RequirementConfig[] Requirements)
                {
                    try
                    {
                        string PrefabName = Type + Material; string Name = Material + " " + Type;
                        string Description = "Type: " + Type + Environment.NewLine + "Tier: " + Material + ".";
                        CustomItem Item = new CustomItem(name: PrefabName + Signature, basePrefabName: BasePrefab);
                        ItemManager.Instance.AddItem(Item);
                        GameObject Prefab = Item.ItemPrefab; MeshRenderer MeshRenderer = Prefab.GetComponentInChildren<MeshRenderer>();
                        ItemDrop Drop = Item.ItemDrop; ItemDrop.ItemData Data = Drop.m_itemData; ItemDrop.ItemData.SharedData Shared = Data.m_shared;
                        MeshRenderer.material = Bundle.GetMaterial(Material);
                        Shared.m_name = Name;
                        Shared.m_description = Description;
                        Shared.m_maxQuality = 1;
                        Shared.m_questItem = false;
                        Shared.m_teleportable = Teleportable;
                        Shared.m_value = Value;
                        CustomRecipe Recipe = new CustomRecipe(new RecipeConfig()
                        {
                            Item = PrefabName + Signature,
                            Amount = Amount,
                            CraftingStation = Station,
                            RepairStation = Station,
                            MinStationLevel = MinStationLevel,
                            Requirements = Requirements,
                            Enabled = Enabled
                        });
                        ItemManager.Instance.AddRecipe(Recipe);
                    }
                    catch (Exception ex)
                    {
                        Jotunn.Logger.LogError($"PrefabBuilder: Skipping {Type} of {Material}, due to {ex}");
                    }
                    finally
                    {

                    }
                    return this;
                }
            }
            public class BuildPiece
            {
                public BuildPiece WithConfigs(string Name, string BaseName, string Material, string PieceTableItem, string Description, string Icon, string ReqStation, string ReqItem, int ReqAmount, int ReqAmountPerLevel, bool ReqRecover)
                {

                    CustomPiece CP = new CustomPiece(Name, BaseName, new PieceConfig
                    {
                        Name = Name,
                        Category = Material,
                        Icon = Bundle.GetIcon(Icon),
                        Description = Description,
                        PieceTable = PieceTableItem,
                        AllowedInDungeons = false,
                        CraftingStation = ReqStation,
                        Requirements = Requirements(Slot(ReqItem, ReqAmount, ReqAmountPerLevel, ReqRecover)),
                        Enabled = true
                    })
                    {
                        FixReference = false,
                        PieceTable = PieceTableItem
                    };
                    if (CP != null)
                    {
                        if (BaseName == "CargoCrate")
                        {
                            Container thisContainer = CP.Piece.GetComponent<Container>();
                            thisContainer.m_width = 8; thisContainer.m_height = 8;
                            thisContainer.m_autoDestroyEmpty = false;
                            thisContainer.m_checkGuardStone = true;
                            thisContainer.m_name = Name;
                        }
                        GameObject prefab = CP.PiecePrefab;
                        Renderer[] Renderers = prefab.GetComponentsInChildren<Renderer>();
                        foreach (Renderer Renderer in Renderers)
                        {
                            Renderer.material = Bundle.GetMaterial(Material);
                            Renderer.sharedMaterial = Bundle.GetMaterial(Material);
                        }
                        PieceManager.Instance.AddPiece(CP);
                    }
                    return this;
                }
            }
            public class Weapon
            {
                public ConfigEntry<string> CE_MeshName { get; set; }
                public ConfigEntry<string> CE_PrefabName { get; set; }
                public ConfigEntry<string> CE_CustomName { get; set; }
                public ConfigEntry<string> CE_TokenName { get; set; }
                public ConfigEntry<string> CE_Description { get; set; }
                public ConfigEntry<string> CE_Type { get; set; }
                public ConfigEntry<string> CE_Material { get; set; }
                public ConfigEntry<string> CE_Texture { get; set; }
                public ConfigEntry<string> CE_AmmoType { get; set; }
                public ConfigEntry<string> CE_Signature { get; set; }
                public ConfigEntry<int> CE_TimedBlockBonus { get; set; }
                public ConfigEntry<int> CE_BlockPower { get; set; }
                public ConfigEntry<int> CE_BlockPowerPerLevel { get; set; }
                public ConfigEntry<int> CE_Blunt { get; set; }
                public ConfigEntry<int> CE_Pierce { get; set; }
                public ConfigEntry<int> CE_Slash { get; set; }
                public ConfigEntry<int> CE_Fire { get; set; }
                public ConfigEntry<int> CE_Frost { get; set; }
                public ConfigEntry<int> CE_Lightning { get; set; }
                public ConfigEntry<int> CE_Poison { get; set; }
                public ConfigEntry<int> CE_Spirit { get; set; }
                public ConfigEntry<int> CE_Chop { get; set; }
                public ConfigEntry<int> CE_Pickaxe { get; set; }
                public ConfigEntry<int> CE_BluntPerLevel { get; set; }
                public ConfigEntry<int> CE_PiercePerLevel { get; set; }
                public ConfigEntry<int> CE_SlashPerLevel { get; set; }
                public ConfigEntry<int> CE_FirePerLevel { get; set; }
                public ConfigEntry<int> CE_FrostPerLevel { get; set; }
                public ConfigEntry<int> CE_LightningPerLevel { get; set; }
                public ConfigEntry<int> CE_PoisonPerLevel { get; set; }
                public ConfigEntry<int> CE_SpiritPerLevel { get; set; }
                public ConfigEntry<int> CE_ChopPerLevel { get; set; }
                public ConfigEntry<int> CE_PickaxePerLevel { get; set; }
                public ConfigEntry<int> CE_Durability { get; set; }
                public ConfigEntry<int> CE_DurabilityPerLevel { get; set; }
                public ConfigEntry<int> CE_DeflectionForce { get; set; }
                public ConfigEntry<int> CE_DeflectionForcePerLevel { get; set; }
                public ConfigEntry<int> CE_MaxQuality { get; set; }
                public ConfigEntry<int> CE_MaxStackSize { get; set; }
                public ConfigEntry<int> CE_Value { get; set; }
                public ConfigEntry<int> CE_Weight { get; set; }
                public ConfigEntry<int> CE_MovementModifier { get; set; }
                public ConfigEntry<bool> CE_Teleportable { get; set; }
                public ConfigEntry<bool> CE_Repairable { get; set; }
                public ConfigEntry<bool> CE_Destroyable { get; set; }
                public ConfigEntry<string> CE_RecipeName { get; set; }
                public ConfigEntry<string> CE_CraftingStation { get; set; }
                public ConfigEntry<string> CE_RepairStation { get; set; }
                public ConfigEntry<string> CE_Station { get; set; }
                public ConfigEntry<int> CE_MinStationLevel { get; set; }
                public ConfigEntry<string> CE_Slot1Item { get; set; }
                public ConfigEntry<int> CE_Slot1Amount { get; set; }
                public ConfigEntry<int> CE_Slot1AmountPerLevel { get; set; }
                public ConfigEntry<string> CE_Slot2Item { get; set; }
                public ConfigEntry<int> CE_Slot2Amount { get; set; }
                public ConfigEntry<int> CE_Slot2AmountPerLevel { get; set; }
                public ConfigEntry<string> CE_Slot3Item { get; set; }
                public ConfigEntry<int> CE_Slot3Amount { get; set; }
                public ConfigEntry<int> CE_Slot3AmountPerLevel { get; set; }
                public ConfigEntry<string> CE_Slot4Item { get; set; }
                public ConfigEntry<int> CE_Slot4Amount { get; set; }
                public ConfigEntry<int> CE_Slot4AmountPerLevel { get; set; }
                public ConfigEntry<bool> CE_Enabled { get; set; }
                public Weapon WithConfigs(string Material, string Type, string BasePrefab, string Signature, string SetNumber = "", bool FixVanilla = true)
                {
                    if (SetNumber != "") { SetNumber += "_"; }
                    CE_Repairable = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Repairable", true, ConfigType: "Weapon");
                    CE_Destroyable = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Breakable", false, ConfigType: "Weapon");
                    CE_TokenName = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Name", $"{Material} {Type}", ConfigType: "Weapon");
                    CE_Durability = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Durability", 100, ConfigType: "Weapon");
                    CE_DurabilityPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Durability Per Level", 50, ConfigType: "Weapon");
                    CE_MaxQuality = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Max Quality", 1, ConfigType: "Weapon");
                    CE_MaxStackSize = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Max Stack Size", 1, ConfigType: "Weapon");
                    CE_Teleportable = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Teleportable", true, ConfigType: "Weapon");
                    CE_Weight = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Weight", 0, ConfigType: "Weapon");
                    CE_Value = ConfigBind($"{SetNumber}{Material}.{Type}.Data", "Value", 0, ConfigType: "Weapon");
                    CE_Blunt = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Blunt", 0, ConfigType: "Weapon");
                    CE_Pierce = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Pierce", 0, ConfigType: "Weapon");
                    CE_Slash = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Slash", 0, ConfigType: "Weapon");
                    CE_Fire = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Fire", 0, ConfigType: "Weapon");
                    CE_Frost = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Frost", 0, ConfigType: "Weapon");
                    CE_Lightning = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Lightning", 0, ConfigType: "Weapon");
                    CE_Poison = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Poison", 0, ConfigType: "Weapon");
                    CE_Spirit = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Spirit", 0, ConfigType: "Weapon");
                    CE_Chop = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Chop", 0, ConfigType: "Weapon");
                    CE_Pickaxe = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Pickaxe", 0, ConfigType: "Weapon");
                    CE_BluntPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Blunt Per Level", 0, ConfigType: "Weapon");
                    CE_PiercePerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Pierce Per Level", 0, ConfigType: "Weapon");
                    CE_SlashPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Slash Per Level", 0, ConfigType: "Weapon");
                    CE_FirePerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Fire Per Level", 0, ConfigType: "Weapon");
                    CE_FrostPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages", "Frost Per Level", 0, ConfigType: "Weapon");
                    CE_LightningPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages.PerLevel", "Lightning Per Level", 0, ConfigType: "Weapon");
                    CE_PoisonPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages.PerLevel", "Poison Per Level", 0, ConfigType: "Weapon");
                    CE_SpiritPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages.PerLevel", "Spirit Per Level", 0, ConfigType: "Weapon");
                    CE_ChopPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages.PerLevel", "Chop Per Level", 0, ConfigType: "Weapon");
                    CE_PickaxePerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages.PerLevel", "Pickaxe Per Level", 0, ConfigType: "Weapon");
                    CE_TimedBlockBonus = ConfigBind($"{SetNumber}{Material}.{Type}.Damages.Blocking", "Timed Block Bonus", 0, ConfigType: "Weapon");
                    CE_BlockPower = ConfigBind($"{SetNumber}{Material}.{Type}.Damages.Blocking", "Block Power", 0, ConfigType: "Weapon");
                    CE_BlockPowerPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages.Blocking", "Block Power Per Level", 0, ConfigType: "Weapon");
                    CE_DeflectionForce = ConfigBind($"{SetNumber}{Material}.{Type}.Damages.Blocking", "Deflection Force", 0, ConfigType: "Weapon");
                    CE_DeflectionForcePerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Damages.Blocking", "Deflection Force Per Level", 0, ConfigType: "Weapon");
                    CE_Station = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Station", "piece_workbench", ConfigType: "Weapon");
                    CE_MinStationLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Minimum Station Level", 1, ConfigType: "Weapon");
                    CE_Slot1Item = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 1: Item", "Stone", ConfigType: "Weapon");
                    CE_Slot1Amount = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 1: Amount", 1, ConfigType: "Weapon");
                    CE_Slot1AmountPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 1: Amount Per Level", 2, ConfigType: "Weapon");
                    CE_Slot2Item = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 2: Item", "LeatherScraps", ConfigType: "Weapon");
                    CE_Slot2Amount = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 2: Amount", 1, ConfigType: "Weapon");
                    CE_Slot2AmountPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 2: Amount Per Level", 1, ConfigType: "Weapon");
                    CE_Slot3Item = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 3: Item", "Resin", ConfigType: "Weapon");
                    CE_Slot3Amount = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 3: Amount", 1, ConfigType: "Weapon");
                    CE_Slot3AmountPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 3: Amount Per Level", 0, ConfigType: "Weapon");
                    CE_Slot4Item = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 4: Item", "Wood", ConfigType: "Weapon");
                    CE_Slot4Amount = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 4: Amount", 1, ConfigType: "Weapon");
                    CE_Slot4AmountPerLevel = ConfigBind($"{SetNumber}{Material}.{Type}.Recipe", "Slot 4: Amount Per Level", 0, ConfigType: "Weapon");

                    string PrefabName; string CustomName; string Description; string AmmoType;
                    if (Type == "Shield")
                    {
                        PrefabName = Type + Material; CustomName = Material + " " + Type;
                        Description = "Type: " + Type + Environment.NewLine + "Tier: " + Material + ".";
                    }
                    else if (Type == "ShieldTower")
                    {
                        PrefabName = "Shield" + Material + "Tower";
                        CustomName = Material + " Tower Shield";
                        Description = "Type: Tower Shield" + Environment.NewLine + "Tier: " + Material + ".";
                    }
                    else
                    {
                        PrefabName = Type + Material; CustomName = Material + " " + Type;
                        Description = "Type: " + Type + Environment.NewLine + "Tier: " + Material + ".";
                    }
                    if (Type == "Bow") { AmmoType = "arrow"; } else { AmmoType = ""; }
                    if (FixVanilla == true) { try { HelpMe.Fix.Vanilla(PrefabName); } finally { } }
                    CustomItem Item = new CustomItem(PrefabName + Signature, BasePrefab);
                    ItemManager.Instance.AddItem(Item);
                    GameObject Prefab = Item.ItemPrefab;
                    MeshRenderer MeshRenderer = Prefab.GetComponentInChildren<MeshRenderer>();
                    ItemDrop Drop = Item.ItemDrop;
                    ItemDrop.ItemData Data = Drop.m_itemData;
                    ItemDrop.ItemData.SharedData Shared = Data.m_shared;
                    MeshRenderer.material = Bundle.GetMaterial(Material);
                    Shared.m_name = CustomName;
                    Shared.m_description = Description;
                    Shared.m_ammoType = AmmoType;
                    Shared.m_canBeReparied = CE_Repairable.Value;
                    Shared.m_destroyBroken = CE_Destroyable.Value;
                    Shared.m_maxDurability = CE_Durability.Value;
                    Shared.m_durabilityPerLevel = CE_DurabilityPerLevel.Value;
                    Shared.m_maxQuality = CE_MaxQuality.Value;
                    Shared.m_maxStackSize = CE_MaxStackSize.Value;
                    Shared.m_questItem = false;
                    Shared.m_teleportable = CE_Teleportable.Value;
                    Shared.m_damages.m_blunt = CE_Blunt.Value;
                    Shared.m_damages.m_pierce = CE_Pierce.Value;
                    Shared.m_damages.m_slash = CE_Slash.Value;
                    Shared.m_damages.m_fire = CE_Fire.Value;
                    Shared.m_damages.m_frost = CE_Frost.Value;
                    Shared.m_damages.m_lightning = CE_Lightning.Value;
                    Shared.m_damages.m_poison = CE_Poison.Value;
                    Shared.m_damages.m_spirit = CE_Spirit.Value;
                    Shared.m_damages.m_chop = CE_Chop.Value;
                    Shared.m_damages.m_pickaxe = CE_Pickaxe.Value;
                    Shared.m_damagesPerLevel.m_blunt = CE_BluntPerLevel.Value;
                    Shared.m_damagesPerLevel.m_pierce = CE_PiercePerLevel.Value;
                    Shared.m_damagesPerLevel.m_slash = CE_SlashPerLevel.Value;
                    Shared.m_damagesPerLevel.m_fire = CE_FirePerLevel.Value;
                    Shared.m_damagesPerLevel.m_frost = CE_FrostPerLevel.Value;
                    Shared.m_damagesPerLevel.m_lightning = CE_LightningPerLevel.Value;
                    Shared.m_damagesPerLevel.m_poison = CE_PoisonPerLevel.Value;
                    Shared.m_damagesPerLevel.m_spirit = CE_SpiritPerLevel.Value;
                    Shared.m_damagesPerLevel.m_chop = CE_ChopPerLevel.Value;
                    Shared.m_damagesPerLevel.m_pickaxe = CE_PickaxePerLevel.Value;
                    Shared.m_timedBlockBonus = CE_TimedBlockBonus.Value;
                    Shared.m_blockPower = CE_BlockPower.Value;
                    Shared.m_blockPowerPerLevel = CE_BlockPowerPerLevel.Value;
                    Shared.m_deflectionForce = CE_DeflectionForce.Value;
                    Shared.m_deflectionForcePerLevel = CE_DeflectionForcePerLevel.Value;
                    ItemManager.Instance.AddRecipe(Recipe(
                        Type: Type,
                        Material: Material,
                        Signature: "_VE",
                        Amount: 1,
                        Station: CE_Station.Value,
                        MinStationLevel: CE_MinStationLevel.Value,
                        Enabled: true,
                        Requirements: Requirements(
                                        Slot(Item: CE_Slot1Item.Value, Amount: CE_Slot1Amount.Value, AmountPerLevel: CE_Slot1AmountPerLevel.Value),
                                        Slot(Item: CE_Slot2Item.Value, Amount: CE_Slot2Amount.Value, AmountPerLevel: CE_Slot2AmountPerLevel.Value),
                                        Slot(Item: CE_Slot3Item.Value, Amount: CE_Slot3Amount.Value, AmountPerLevel: CE_Slot3AmountPerLevel.Value),
                                        Slot(Item: CE_Slot4Item.Value, Amount: CE_Slot4Amount.Value, AmountPerLevel: CE_Slot4AmountPerLevel.Value))
                    ));

                    string log = CustomName; for (int i = CustomName.Length + 2; i < 57; i++) { log += " "; }
                    Registry.EnabledContent_name.Add($"| {log} |");
                    
                    return this;
                }
            }

            public static Material ArmorMaterial(Material Material, string Type)
            {
                Texture thisTexture = Material.GetTexture("_MainTex"); 
                Material thisMaterial = new Material((Material) PrefabManager.Cache.GetPrefab(typeof(Material), "PlayerMaterial"));
                if (Type == "Chest")
                {
                    thisMaterial.SetTexture("_ChestTex", thisTexture);
                }
                if (Type == "Legs")
                {
                    thisMaterial.SetTexture("_LegsTex", thisTexture);
                }

                return thisMaterial;
            }
            public static CustomRecipe Recipe(string Type, string Material, string Signature = "", int Amount = 1, string Station = "piece_workbench", int MinStationLevel = 1, bool Enabled = true, params RequirementConfig[] Requirements)
            {
                string Item = Type + Material;
                if (Type == "ShieldTower") { Item = "Shield" + Material + "Tower"; }
                if (Type == "Helmet") { Item = Type + Material; }
                if (Type == "Cape") { Item = Type + Material; }
                if (Type == "Belt") { Item = Type + Material; }
                if (Type == "Legs") { Item = "Armor" + Material + Type; }
                if (Type == "Chest") { Item = "Armor" + Material + Type; }
                CustomRecipe Recipe = new CustomRecipe(new RecipeConfig()
                {
                    Item = Item + Signature,
                    Amount = Amount,
                    CraftingStation = Station,
                    RepairStation = Station,
                    MinStationLevel = MinStationLevel,
                    Requirements = Requirements,
                    Enabled = Enabled
                });
                return Recipe;
            }
            public static RequirementConfig[] Requirements(RequirementConfig Slot1, RequirementConfig Slot2 = null, RequirementConfig Slot3 = null, RequirementConfig Slot4 = null)
            {
                if (Slot2 == null) { return new RequirementConfig[] { Slot1 }; }
                if (Slot2 == null && Slot3 != null) { return new RequirementConfig[] { Slot1 }; }
                if (Slot2 == null && Slot4 != null) { return new RequirementConfig[] { Slot1 }; }
                if (Slot3 == null) { return new RequirementConfig[] { Slot1, Slot2 }; }
                if (Slot3 == null && Slot4 != null) { return new RequirementConfig[] { Slot1, Slot2 }; }
                if (Slot4 == null) { return new RequirementConfig[] { Slot1, Slot2, Slot3 }; }
                else return new RequirementConfig[] { Slot1, Slot2, Slot3, Slot4 };
            }
            public static RequirementConfig Slot(string Item, int Amount = 1, int AmountPerLevel = 0, bool Recover = false)
            {
                return new RequirementConfig() { Item = Item, Amount = Amount, AmountPerLevel = AmountPerLevel, Recover = Recover };
            }
            public static CustomItemConversion Conversion(string station, string fromitem, string toitem)
            {
                switch (station)
                {
                    case "piece_cookingstation":
                        return new CustomItemConversion(new CookingConversionConfig { Station = station, FromItem = fromitem, ToItem = toitem });
                    case "fermenter":
                        return new CustomItemConversion(new FermenterConversionConfig { Station = station, FromItem = fromitem, ToItem = toitem });
                    case "smelter":
                        return new CustomItemConversion(new SmelterConversionConfig { Station = station, FromItem = fromitem, ToItem = toitem });
                    case "charcoal_kiln":
                        return new CustomItemConversion(new SmelterConversionConfig { Station = station, FromItem = fromitem, ToItem = toitem });
                    case "blastfurnace":
                        return new CustomItemConversion(new SmelterConversionConfig { Station = station, FromItem = fromitem, ToItem = toitem });
                    case "windmill":
                        return new CustomItemConversion(new SmelterConversionConfig { Station = station, FromItem = fromitem, ToItem = toitem });
                    case "piece_spinningwheel":
                        return new CustomItemConversion(new SmelterConversionConfig { Station = station, FromItem = fromitem, ToItem = toitem });
                }
                return new CustomItemConversion(new SmelterConversionConfig { Station = station, FromItem = fromitem, ToItem = toitem });
            }
            public static ConfigEntry<T> ConfigBind<T>(string Section, string Key, T DefaultValue, string Description = "", bool IsAdminOnly = true, bool IsHidden = false, string ConfigType = "")
            {
                if (ConfigType == "Weapon")
                {
                    ConfigEntry<T> thisEntry = Cache.Weapons.Bind(Section, Key, DefaultValue, new ConfigDescription(Description, null, new ConfigurationManagerAttributes { IsAdminOnly = IsAdminOnly }));
                    return thisEntry;
                }
                else if (ConfigType == "Armor")
                {
                    ConfigEntry<T> thisEntry = Cache.Armors.Bind(Section, Key, DefaultValue, new ConfigDescription(Description, null, new ConfigurationManagerAttributes { IsAdminOnly = IsAdminOnly }));
                    return thisEntry;
                }
                else
                {
                    ConfigEntry<T> thisEntry = Cache.Settings.Bind(Section, Key, DefaultValue, new ConfigDescription(Description, null, new ConfigurationManagerAttributes { IsAdminOnly = IsAdminOnly }));
                    return thisEntry;
                }
            }
        }
        public static class Settings
        {
            public static class Admin
            {
                public static ConfigEntry<bool> FixVanilla { get; set; }
            }
        }
        public static class HelpMe
        {
            public static class Fix
            {
                public static void Vanilla(string Item)
                {
                    ItemManager.Instance.RemoveRecipe("Recipe_" + Item);
                }
                public static Piece LODGroupFor(Piece Piece, string Name, string Tag, float Size, bool AnimateCrossFading, bool Enabled, Vector3 LocalReferencePoint, LODFadeMode LODFadeMode = LODFadeMode.None)
                {
                    GameObject PieceObject = Piece.gameObject;
                    if (Piece != null) 
                    {
                        LODGroup[] LODGroups = Piece.GetComponents<LODGroup>();
                        if (LODGroups != null) { foreach (LODGroup LODGroup in LODGroups) { UnityEngine.Object.Destroy(LODGroup); } }
                        LODGroup PieceLOD = PieceObject.AddComponent<LODGroup>();
                        // None - Indicates the LOD fading is turned off.
                        // CrossFade - Perform cross-fade style blending between the current LOD and the next LOD if the distance to camera falls in the range specified by the LOD.fadeTransitionWidth of each LOD.
                        // SpeedTree - By specifying this mode, your LODGroup will perform a SpeedTree-style LOD fading scheme:For all the mesh LODs other than the last (most crude) mesh LOD, the fade factor is calculated as the percentage of the object's current screen height, compared to the whole range of the LOD. It is 1, if the camera is right at the position where the previous LOD switches out and 0, if the next LOD is just about to switch in.For the last mesh LOD and the billboard LOD, the cross-fade mode is used.
                        PieceLOD.animateCrossFading = AnimateCrossFading;
                        // The rest are self-explanatory...
                        PieceLOD.enabled = Enabled;
                        PieceLOD.fadeMode = LODFadeMode;
                        PieceLOD.hideFlags = HideFlags.None;
                        PieceLOD.localReferencePoint = LocalReferencePoint;
                        PieceLOD.name = Name;
                        PieceLOD.size = Size;
                        PieceLOD.tag = Tag;
                    } return Piece;
                }
                public static CustomPiece LODGroupFor(CustomPiece Piece, string Name, string Tag, float Size, bool AnimateCrossFading, bool Enabled, Vector3 LocalReferencePoint, LODFadeMode LODFadeMode = LODFadeMode.None)
                {
                    GameObject PieceObject = Piece.PiecePrefab;
                    Piece PieceActual = PieceObject.GetComponent<Piece>();
                    if (Piece != null)
                    {
                        LODGroup[] LODGroups = PieceActual.GetComponents<LODGroup>();
                        if (LODGroups != null) { foreach (LODGroup LODGroup in LODGroups) { UnityEngine.Object.Destroy(LODGroup); } }
                        LODGroup PieceLOD = PieceObject.AddComponent<LODGroup>();
                        // None - Indicates the LOD fading is turned off.
                        // CrossFade - Perform cross-fade style blending between the current LOD and the next LOD if the distance to camera falls in the range specified by the LOD.fadeTransitionWidth of each LOD.
                        // SpeedTree - By specifying this mode, your LODGroup will perform a SpeedTree-style LOD fading scheme:For all the mesh LODs other than the last (most crude) mesh LOD, the fade factor is calculated as the percentage of the object's current screen height, compared to the whole range of the LOD. It is 1, if the camera is right at the position where the previous LOD switches out and 0, if the next LOD is just about to switch in.For the last mesh LOD and the billboard LOD, the cross-fade mode is used.
                        PieceLOD.animateCrossFading = AnimateCrossFading;
                        // The rest are self-explanatory...
                        PieceLOD.enabled = Enabled;
                        PieceLOD.fadeMode = LODFadeMode;
                        PieceLOD.hideFlags = HideFlags.None;
                        PieceLOD.localReferencePoint = LocalReferencePoint;
                        PieceLOD.name = Name;
                        PieceLOD.size = Size;
                        PieceLOD.tag = Tag;
                    } return Piece;
                }
            }
            public static class Get
            {
                public static Material CameraMaterial(string playerName)
                {
                    Camera playerCamera; Material newMaterial = null;
                    foreach (Player player in Player.GetAllPlayers())
                    {
                        try
                        {
                            if (player.m_name != playerName) { }
                            else
                            {
                                playerCamera = GameCamera.instance.m_camera;
                                newMaterial = new Material(Shader.Find("Standard"))
                                { mainTexture = CameraTexture(playerCamera) };
                            }
                        }
                        finally { }
                    }
                    return newMaterial;
                }
                public static Texture CameraTexture(Camera playerCamera)
                {
                    if (playerCamera.enabled != true) { return null; }
                    else
                    {
                        RenderTexture renderTexture = new RenderTexture(Screen.height, Screen.width, 0);
                        playerCamera.targetTexture = renderTexture;
                        return renderTexture;
                    }
                }
                public static Transform ClosestTransformFor(Transform ObjectA, Transform[] ObjectBs)
                {
                    Transform bestTarget = null;
                    float closestDistanceSqr = Mathf.Infinity;
                    Vector3 currentPosition = ObjectA.position;
                    foreach (Transform potentialTarget in ObjectBs)
                    {
                        Vector3 directionToTarget = potentialTarget.position - currentPosition;
                        float dSqrToTarget = directionToTarget.sqrMagnitude;
                        if (dSqrToTarget < closestDistanceSqr)
                        {
                            closestDistanceSqr = dSqrToTarget;
                            bestTarget = potentialTarget;
                        }
                    }

                    return bestTarget;
                }
            }
            public static class Create
            {
                public static LODGroup LODGroup(string Name, string Tag, float Size, bool AnimateCrossFading, bool Enabled, Vector3 LocalReferencePoint, LODFadeMode LODFadeMode = LODFadeMode.None, HideFlags HideFlags = HideFlags.None)
                {
                    LODGroup LODGroup = new LODGroup
                    {
                        tag = Tag,
                        name = Name,
                        size = Size,
                        enabled = Enabled,
                        hideFlags = HideFlags,
                        fadeMode = LODFadeMode,
                        animateCrossFading = AnimateCrossFading,
                        localReferencePoint = LocalReferencePoint,
                    };
                    return LODGroup;
                }
            }
            public class Serialize
            {
                public Serialize()
                {

                }
            }
        }
        public static class Info
        {
            public static readonly List<string> Types = new List<string> { "Atgeir", "Axe", "Battleaxe", "Club", "Bow", "Mace", "Knife", "Pickaxe", "Spear", "Sword", "Sledge", "Shield", "TowerShield" };
            public static readonly List<string> Atgeirs = new List<string> { "AtgeirBronze", "AtgeirBronze", "AtgeirIron", "AtgeirBlackMetal" };
            public static readonly List<string> Axes = new List<string> { "AxeStone", "AxeBronze", "AxeIron", "AxeBlackMetal" };
            public static readonly List<string> Battleaxes = new List<string> { "Battleaxe", "Battleaxe", "Battleaxe", "Battleaxe" };
            public static readonly List<string> Clubs = new List<string> { "Club", "Club", "Club", "Club" };
            public static readonly List<string> Bows = new List<string> { "Bow", "BowFineWood", "BowHuntsmen", "BowDraugrFang" };
            public static readonly List<string> Maces = new List<string> { "MaceBronze", "MaceIron", "MaceSilver", "MaceNeedle" };
            public static readonly List<string> Knives = new List<string> { "KnifeFlint", "KnifeCopper", "KnifeChitin", "KnifeBlackMetal" };
            public static readonly List<string> Pickaxes = new List<string> { "AntlerPickaxe", "PickaxeBronze", "PickaxeIron", "PickaxeIron" };
            public static readonly List<string> Spears = new List<string> { "SpearFlint", "SpearBronze", "SpearElderBark", "SpearWolfFang" };
            public static readonly List<string> Swords = new List<string> { "SwordBronze", "SwordIron", "SwordSilver", "SwordBlackMetal" };
            public static readonly List<string> Sledges = new List<string> { "SledgeStagbreaker", "SledgeStagbreaker", "SledgesIron", "SledgesIron" };
            public static readonly List<string> Shields = new List<string> { "ShieldWood", "ShieldBronze", "ShieldIron", "ShieldBlackMetal" };
            public static readonly List<string> TowerShields = new List<string> { "ShieldWoodTower", "ShieldIronTower", "ShieldSerpentScale", "ShieldBlackMetalTower" };
            public static readonly List<string> Tools = new List<string> { "Cultivator", "FishingRod", "Hammer", "Hoe" };
            public static readonly List<string> OtherTypes = new List<string> { "Bomb", "Tankard" };
            public static readonly List<List<string>> WeaponTypes = new List<List<string>> { Atgeirs, Axes, Battleaxes, Clubs, Bows, Maces, Knives, Pickaxes, Spears, Swords, Sledges, Shields, TowerShields, Tools, OtherTypes };
        }
    }
    class AssetBundleHelper
    {
        public static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var execAssembly = Assembly.GetExecutingAssembly();

            var resourceName = execAssembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(fileName));

            AssetBundle assetBundle;
            using (var stream = execAssembly.GetManifestResourceStream(resourceName))
            {
                assetBundle = AssetBundle.LoadFromStream(stream);
            }

            return assetBundle;
        }
    }
}