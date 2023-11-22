using System.Collections;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;

namespace DynamicStoragePiles {
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.ClientMustHaveMod, VersionStrictness.Minor)]
    internal class DynamicStoragePiles : BaseUnityPlugin {
        public const string PluginName = "DynamicStoragePiles";
        public const string PluginGuid = "com.maxsch.valheim.DynamicStoragePiles";
        public const string PluginVersion = "0.4.0";

        public static DynamicStoragePiles Instance { get; private set; }
        public static Harmony harmony;

        private static AssetBundle assetBundle;
        private List<CustomPiece> pieces = new List<CustomPiece>();
        private static Dictionary<string, string> allowedItemsByStack = new Dictionary<string, string>();

        public static ConfigEntry<bool> disableVanillaRecipes;
        public static ConfigEntry<bool> azuAutoStoreCompat;
        public static ConfigEntry<bool> azuAutoStoreItemWhitelist;
        public static ConfigEntry<bool> ingotStacksDisableRecipes;
        public static ConfigEntry<bool> ingotStacksDisableAdditionalStackRecipes;
        public static ConfigEntry<bool> restrictDynamicPiles;
        public static bool ShouldRestrictItems => restrictDynamicPiles.Value;

        public static readonly Dictionary<string, string> allowedItemsByContainer = new Dictionary<string, string>();
        

        private void Awake() {
            Instance = this;
            assetBundle = AssetUtils.LoadAssetBundleFromResources("containerstacks");

            AddPiece("MS_container_wood_stack", "Wood");
            AddPiece("MS_container_finewood_stack", "FineWood");
            AddPiece("MS_container_corewood_stack", "RoundLog");
            AddPiece("MS_container_yggdrasil_wood_stack", "YggdrasilWood");
            AddPiece("MS_container_stone_pile", "Stone");
            AddPiece("MS_container_coal_pile", "Coal");
            AddPiece("MS_container_blackmarble_pile", "BlackMarble");
            AddPiece("MS_container_coin_pile", "Coins");

            disableVanillaRecipes = Config.Bind("1 - General", "Disable Vanilla Stack Recipes", false, "Prevents vanilla stack pieces from being placeable with the hammer. It uses the vanilla system to disable pieces, cheats or world modifiers can overwrite this setting. Existing pieces in the world are not affected");
            disableVanillaRecipes.SettingChanged += (sender, args) => DisablePieceRecipes(true);

            restrictDynamicPiles = Config.Bind("1 - General", "Restrict Container Item Type", true, "Prevents items other than the one the dynamic storage container represents from being placed in it");

            azuAutoStoreCompat = Config.Bind("2 - Compatibility", "AzuAutoStore Compatibility", true, "Enables compatibility with AzuAutoStore. Requires a restart to take effect");
            azuAutoStoreItemWhitelist = Config.Bind("2 - Compatibility", "AzuAutoStore Item Whitelist", true, "Only allows the respective items to be stored in stack piles");

            ingotStacksDisableRecipes = Config.Bind("2 - Compatibility", "IngotStacks Disable Stack Recipes", false, "Prevents the IngotStacks recipes from being placeable with the hammer. It uses the vanilla system to disable pieces, cheats or world modifiers can overwrite this setting. Existing pieces in the world are not affected");
            ingotStacksDisableRecipes.SettingChanged += (sender, args) => DisablePieceRecipes(true);

            ingotStacksDisableAdditionalStackRecipes = Config.Bind("2 - Compatibility", "IngotStacks Disable Dynamic Stack Recipes", false, "Prevents the additional dynamic container stack recipes from being placeable with the hammer. It uses the vanilla system to disable pieces, cheats or world modifiers can overwrite this setting. Existing pieces in the world are not affected");
            ingotStacksDisableAdditionalStackRecipes.SettingChanged += (sender, args) => DisablePieceRecipes(true);

            PieceManager.OnPiecesRegistered += OnPiecesRegistered;
            PrefabManager.OnPrefabsRegistered += () => DisablePieceRecipes(false);

            harmony = new Harmony(PluginGuid);
            harmony.PatchAll();
        }

        private void Start() {
            if (Chainloader.PluginInfos.ContainsKey("Azumatt.AzuAutoStore") && azuAutoStoreCompat.Value) {
                Compatibility.AzuAutoStore.Init();
            }

            if (Chainloader.PluginInfos.ContainsKey("Richard.IngotStacks")) {
                Compatibility.IngotStacks.Init();
            }
        }

        private static void DisablePieceRecipes(bool forceUpdate) {
            if (!ZNetScene.instance) {
                return;
            }

            if (forceUpdate || disableVanillaRecipes.Value) {
                DisableVanillaRecipes(disableVanillaRecipes.Value);
            }

            if (Chainloader.PluginInfos.ContainsKey("Richard.IngotStacks")) {
                if (forceUpdate || ingotStacksDisableRecipes.Value) {
                    Compatibility.IngotStacks.DisablePieceRecipes(ingotStacksDisableRecipes.Value);
                }

                if (forceUpdate || ingotStacksDisableAdditionalStackRecipes.Value) {
                    Compatibility.IngotStacks.DisableAdditionalPieceRecipes(ingotStacksDisableAdditionalStackRecipes.Value);
                }
            }

            if (Player.m_localPlayer) {
                Player.m_localPlayer.UpdateAvailablePiecesList();
            }
        }

        private static void DisableVanillaRecipes(bool disable) {
            TogglePieceRecipes("wood_stack", !disable);
            TogglePieceRecipes("wood_fine_stack", !disable);
            TogglePieceRecipes("wood_core_stack", !disable);
            TogglePieceRecipes("wood_yggdrasil_stack", !disable);
            TogglePieceRecipes("stone_pile", !disable);
            TogglePieceRecipes("coal_pile", !disable);
            TogglePieceRecipes("blackmarble_pile", !disable);
            TogglePieceRecipes("treasure_stack", !disable);
        }

        public static void TogglePieceRecipes(string prefabName, bool enabled) {
            GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);

            if (prefab && prefab.TryGetComponent(out Piece piece)) {
                piece.m_enabled = enabled;
            } else {
                Jotunn.Logger.LogWarning($"Could not find Piece {prefabName} to toggle recipes");
            }
        }

        private void AddPiece(string pieceName, string craftItem) {
            CustomPiece piece = new CustomPiece(assetBundle, pieceName, true, StackConfig(craftItem));
            PieceManager.Instance.AddPiece(piece);
            pieces.Add(piece);
            allowedItemsByStack.Add(pieceName, craftItem);
            allowedItemsByContainer.Add(piece.PiecePrefab.GetComponent<Container>().m_name, craftItem);
        }

        public void AddPiece(GameObject prefab, string craftItem, int amount) {
            CustomPiece piece = new CustomPiece(prefab, false, StackConfig(craftItem, amount));
            PieceManager.Instance.AddPiece(piece);
            pieces.Add(piece);
            allowedItemsByStack.Add(prefab.name, craftItem);
            allowedItemsByContainer.Add(prefab.GetComponent<Container>().m_name, craftItem);
        }

        private void OnPiecesRegistered() {
            PieceManager.OnPiecesRegistered -= OnPiecesRegistered;

            foreach (CustomPiece piece in pieces) {
                StartCoroutine(RenderSprite(piece));
            }
        }

        private IEnumerator RenderSprite(CustomPiece piece) {
            yield return null;

            var visualStacks = piece.Piece.GetComponentsInChildren<VisualStack>();

            foreach (VisualStack visualStack in visualStacks) {
                visualStack.SetVisualsActive(55f);
            }

            if (!piece.PiecePrefab.name.StartsWith("MS_IngotStacks_")) {
                piece.Piece.m_icon = RenderManager.Instance.Render(new RenderManager.RenderRequest(piece.PiecePrefab) {
                    Width = 64,
                    Height = 64,
                    Rotation = RenderManager.IsometricRotation * Quaternion.Euler(0, -90f, 0),
                    UseCache = true,
                    TargetPlugin = Info.Metadata,
                });
            }

            foreach (VisualStack visualStack in visualStacks) {
                visualStack.SetVisualsActive(100f);
            }
        }

        private PieceConfig StackConfig(string item, int amount = 10) {
            PieceConfig stackConfig = new PieceConfig();
            stackConfig.PieceTable = PieceTables.Hammer;
            stackConfig.AddRequirement(new RequirementConfig(item, amount, 0, true));
            return stackConfig;
        }

        public static bool IsStackPiece(string pieceName, out string allowedItem) {
            return allowedItemsByStack.TryGetValue(pieceName, out allowedItem);
        }
    }
}
