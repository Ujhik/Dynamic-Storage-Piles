using BepInEx;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;

namespace ContainerStacks {
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class ContainerStacks : BaseUnityPlugin {
        public const string PluginName = "ContainerStacks";
        public const string PluginGuid = "com.maxsch.valheim.ContainerStacks";
        public const string PluginVersion = "0.0.1";

        private static AssetBundle assetBundle;
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake() {
            assetBundle = AssetUtils.LoadAssetBundleFromResources("containerstacks");

            AddPiece("MS_container_wood_stack", "Wood");

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();
        }

        private void AddPiece(string pieceName, string craftItem) {
            PieceManager.Instance.AddPiece(new CustomPiece(assetBundle, pieceName, true, StackConfig(craftItem)));
        }

        private PieceConfig StackConfig(string item) {
            PieceConfig stackConfig = new PieceConfig();
            stackConfig.PieceTable = PieceTables.Hammer;
            stackConfig.AddRequirement(new RequirementConfig(item, 10, 0, true));
            return stackConfig;
        }
    }
}
