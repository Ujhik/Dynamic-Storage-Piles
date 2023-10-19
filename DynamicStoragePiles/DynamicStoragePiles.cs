using System.Collections;
using System.Collections.Generic;
using BepInEx;
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
    // [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class DynamicStoragePiles : BaseUnityPlugin {
        public const string PluginName = "DynamicStoragePiles";
        public const string PluginGuid = "com.maxsch.valheim.DynamicStoragePiles";
        public const string PluginVersion = "0.2.0";

        private static AssetBundle assetBundle;
        private List<CustomPiece> pieces = new List<CustomPiece>();

        private static ConfigEntry<bool> disableVanillaRecipes;

        // public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake() {
            assetBundle = AssetUtils.LoadAssetBundleFromResources("containerstacks");

            AddPiece("MS_container_wood_stack", "Wood");
            AddPiece("MS_container_finewood_stack", "FineWood");
            AddPiece("MS_container_corewood_stack", "RoundLog");
            AddPiece("MS_container_yggdrasil_wood_stack", "YggdrasilWood");
            AddPiece("MS_container_stone_pile", "Stone");
            AddPiece("MS_container_coal_pile", "Coal");
            AddPiece("MS_container_blackmarble_pile", "BlackMarble");
            AddPiece("MS_container_coin_pile", "Coins");

            disableVanillaRecipes = Config.Bind("General", "Disable Vanilla Stack Recipes", false, "Prevents vanilla stack pieces from being placeable with the hammer. It uses the vanilla system to disable pieces, cheats or world modifiers can overwrite this setting. Existing pieces in the world are not affected");
            disableVanillaRecipes.SettingChanged += (sender, args) => DisablePieceRecipes(true);

            PieceManager.OnPiecesRegistered += OnPiecesRegistered;
            PrefabManager.OnPrefabsRegistered += () => DisablePieceRecipes(false);

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();
        }

        private static void DisablePieceRecipes(bool forceUpdate) {
            if (!ZNetScene.instance) {
                return;
            }

            bool disableRecipes = disableVanillaRecipes.Value;

            if (forceUpdate || disableRecipes) {
                TogglePieceRecipes("wood_stack", !disableRecipes);
                TogglePieceRecipes("wood_fine_stack", !disableRecipes);
                TogglePieceRecipes("wood_core_stack", !disableRecipes);
                TogglePieceRecipes("wood_yggdrasil_stack", !disableRecipes);
                TogglePieceRecipes("stone_pile", !disableRecipes);
                TogglePieceRecipes("coal_pile", !disableRecipes);
                TogglePieceRecipes("blackmarble_pile", !disableRecipes);
                TogglePieceRecipes("treasure_stack", !disableRecipes);
            }

            if (Player.m_localPlayer) {
                Player.m_localPlayer.UpdateAvailablePiecesList();
            }
        }

        private static void TogglePieceRecipes(string prefabName, bool enabled) {
            GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);

            if (prefab && prefab.TryGetComponent(out Piece piece)) {
                piece.m_enabled = enabled;
            } else {
                Jotunn.Logger.LogWarning($"Could not find Piece {prefabName} to toggle recipes");
            }
        }

        private void AddPiece(string pieceName, string craftItem) {
            CustomPiece piece = new CustomPiece(assetBundle, pieceName, true, StackConfig(craftItem));
            pieces.Add(piece);
            PieceManager.Instance.AddPiece(piece);
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

            piece.Piece.m_icon = RenderManager.Instance.Render(new RenderManager.RenderRequest(piece.PiecePrefab) {
                Width = 64,
                Height = 64,
                Rotation = RenderManager.IsometricRotation * Quaternion.Euler(0, -90f, 0),
                UseCache = true,
                TargetPlugin = Info.Metadata,
            });

            foreach (VisualStack visualStack in visualStacks) {
                visualStack.SetVisualsActive(100f);
            }
        }

        private PieceConfig StackConfig(string item) {
            PieceConfig stackConfig = new PieceConfig();
            stackConfig.PieceTable = PieceTables.Hammer;
            stackConfig.AddRequirement(new RequirementConfig(item, 10, 0, true));
            return stackConfig;
        }
    }
}
