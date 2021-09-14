using BepInEx;
using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

namespace ValEx
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]

    public class Core : BaseUnityPlugin
    {
        public const string PluginGUID = "MrRageous.ValEx";
        public const string PluginName = "ValheimExpanded: Core Module";
        public const string PluginVersion = "0.5.0";
        public static ConfigEntry<bool> DebugVE { get; set; }
        public static ConfigEntry<bool> DeNotes { get; set; }
        public static ConfigEntry<bool> DisplayEnabled { get; set; }
        public static ConfigEntry<bool> FirstTimeSetup { get; set; }
        public static int ModuleCount { get; set; } = 1;
        private void Awake()
        {
            DebugVE = API.New.ConfigBind("00_DebugConsole", "DebugVE", true, "", false, false, "Settings");
            if (DebugVE.Value == true) { ItemManager.OnItemsRegisteredFejd += Concept.ConstructLogs; }

        }
        private void Update()
        {
           // if (Input.GetKeyDown(KeyCode.Numlock)) { Content.AdminGUI MainForm = new Content.AdminGUI(); MainForm.TogglePanel(); }
        }
        public static class Concept
        {
            public static void ConstructLogs()
            {
                Jotunn.Logger.LogMessage(".============={ Welcome To ValheimExpanded. }=============.");
                Jotunn.Logger.LogMessage($"|                  Detected ({ModuleCount}) Modules:                  |");
                Jotunn.Logger.LogMessage("|---------------------------------------------------------|");
                Jotunn.Logger.LogMessage("|  Type: Library     Name: Core         Version: [0.5.0]  |"); Registry.DebugModules.Sort(); foreach (string log in Registry.DebugModules) { Jotunn.Logger.LogMessage($"{log}"); }
                Jotunn.Logger.LogMessage("|                                                         |"); FirstTimeSetup = API.New.ConfigBind("00_DebugConsole", "First Time Setup", true, "", false, false, "Settings"); DeNotes = API.New.ConfigBind("00_DebugConsole", "Developers Notes", true, "", false, false, "Settings"); if (FirstTimeSetup.Value == true)
                {
                    Jotunn.Logger.LogMessage(":============{ FIRST TIME SETUP INSTRUCTIONS }============:");
                    Jotunn.Logger.LogMessage("|                                                         |");
                    Jotunn.Logger.LogMessage("|  This module utilizes BepInEx Configs, so in order for  |");
                    Jotunn.Logger.LogMessage("|  you to generate your Weapons, you will be required to  |");
                    Jotunn.Logger.LogMessage("|  first generate your ValEx Settings Configs:            |");
                    Jotunn.Logger.LogMessage("|   - Enable Sets in 'configs/ValheimExpanded - Settings' |");
                    Jotunn.Logger.LogMessage("|     or use a local Config Editor to configure it.       |");
                    Jotunn.Logger.LogMessage("|   - Relaunch Valheim to generate Item Configs.          |");
                    Jotunn.Logger.LogMessage("|   - Relaunch once more to generate your Items.          |");
                    Jotunn.Logger.LogMessage("|  Once your Weapon Sets have been recognized by the      |");
                    Jotunn.Logger.LogMessage("|  ValEx Core, they will generate specific configs for    |");
                    Jotunn.Logger.LogMessage("|  each one of your Enabled Set's items. This panel will  |");
                    Jotunn.Logger.LogMessage("|  become your new configuration manifest, so that you    |");
                    Jotunn.Logger.LogMessage("|  can easily tell which configurations have been enabled |");
                    Jotunn.Logger.LogMessage("|  for debugging purposes, or for sanity. That is all!    |");
                    Jotunn.Logger.LogMessage("|                                                         |");
                    Jotunn.Logger.LogMessage("|---------------------------------------------------------|");
                    Jotunn.Logger.LogMessage("| [!]                        ^                        [!] |");
                    Jotunn.Logger.LogMessage("|     This message should not display more than once.     |");
                    Jotunn.Logger.LogMessage("|       To see this again, enable First Time Setup.       |");
                    Jotunn.Logger.LogMessage("| [!]                                                 [!] |"); FirstTimeSetup.Value = false;
                    Jotunn.Logger.LogMessage("'========================================================='");
                }
                else
                {
                    if (Registry.EnabledContent_name.Count != 0)
                    {
                        foreach (string log in Registry.EnabledContent_name)
                        {
                            Jotunn.Logger.LogMessage($"{log}");
                        }
                        Jotunn.Logger.LogMessage("|                                                         |");
                    }
                    if (DeNotes.Value)
                    {
                        Jotunn.Logger.LogMessage(".=================={ DEVELOPER'S NOTES }==================.");
                        Jotunn.Logger.LogMessage("|                                                         |");
                        Jotunn.Logger.LogMessage("| To the Player,                                          |");
                        Jotunn.Logger.LogMessage("| I would like to personally thank you for your support.  |");
                        Jotunn.Logger.LogMessage("| It can be difficult providing quality mods for games    |");
                        Jotunn.Logger.LogMessage("| which are not yet released, for those with no support,  |");
                        Jotunn.Logger.LogMessage("| therefore, I appreciate your time and patience as we    |");
                        Jotunn.Logger.LogMessage("| come to work out all the issues in this mod.            |");
                        Jotunn.Logger.LogMessage("|                                                         |");
                        Jotunn.Logger.LogMessage("| I would also like to thank all the people of the Jotunn |");
                        Jotunn.Logger.LogMessage("| Modding Discord for their attention and many efforts.   |");
                        Jotunn.Logger.LogMessage("|                                                         |");
                        Jotunn.Logger.LogMessage("| If you consider yourself 'well-off', perhaps you might  |");
                        Jotunn.Logger.LogMessage("| help me out by donating through my Nexus Modpage.       |");
                        Jotunn.Logger.LogMessage("| By donating, you allow me more time for this hobby.     |");
                        Jotunn.Logger.LogMessage("|                                          - Mr-Rageous   |");
                        Jotunn.Logger.LogMessage("|                                                         |");
                        Jotunn.Logger.LogMessage("'========================================================='");
                    }
                    ItemManager.OnItemsRegisteredFejd -= ConstructLogs;
                }
            }
        }
        public static class Content
        {
            public static ConfigEntry<List<string>> Register { get; set; }
            public static List<string> Libraries { get; set; } = new List<string>() { "Core" };
            public static List<List<string>> Modules { get; set; } = new List<List<string>>() {  };
            public static List<string> Features { get; set; } = new List<string>() {  };
            public static List<bool> IsEnabled { get; set; } // Reload ConfigFile then check for enabled packs
            public class AdminGUI
            {
                public GameObject Form { get; private set; }
                public GameObject ScrollView { get; private set; }
                public GameObject buttonObject { get; private set; }

                public void TogglePanel()
                {
                    // Create the panel if it does not exist
                    if (!Form)
                    {
                        #region Background Panel
                        Form = GUIManager.Instance.CreateWoodpanel(
                            parent: GUIManager.CustomGUIFront.transform,
                            anchorMin: new Vector2(0.5f, 0.5f),
                            anchorMax: new Vector2(0.5f, 0.5f),
                            position: new Vector2(0, 0),
                            width: 850,
                            height: 600,
                            draggable: false);
                        Form.SetActive(false);
                        #endregion
                        #region Title: ValheimExpanded: AdminTools
                        GameObject Title = GUIManager.Instance.CreateText(
                            text: "ValheimExpanded: AdminTools Menu",
                            parent: Form.transform,
                            anchorMin: new Vector2(0.5f, 1f),
                            anchorMax: new Vector2(0.5f, 1f),
                            position: new Vector2(0f, -100f),
                            font: GUIManager.Instance.AveriaSerifBold,
                            fontSize: 30,
                            color: GUIManager.Instance.ValheimOrange,
                            outline: true,
                            outlineColor: Color.black,
                            width: 350f,
                            height: 40f,
                            addContentSizeFitter: false);
                        #endregion
                        #region Button: Test
                        buttonObject = GUIManager.Instance.CreateButton(
                            text: "Test",
                            parent: Form.transform,
                            anchorMin: new Vector2(0.5f, 0.5f),
                            anchorMax: new Vector2(0.5f, 0.5f),
                            position: new Vector2(0, 0),
                            width: 250,
                            height: 100);
                        buttonObject.SetActive(true);

                        Button button = buttonObject.GetComponent<Button>();
                        button.onClick.AddListener(TogglePanel);
                        #endregion
                    }

                    // Switch the current state
                    bool state = !Form.activeSelf;

                    // Set the active state of the panel
                    Form.SetActive(state);

                    // Toggle input for the player and camera while displaying the GUI
                    GUIManager.BlockInput(state);
                }
            }
        }
    }
}
