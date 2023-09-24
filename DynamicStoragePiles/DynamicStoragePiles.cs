using System.Collections;
using System.Collections.Generic;
using BepInEx;
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
        public const string PluginVersion = "0.1.0";

        private static AssetBundle assetBundle;
        private List<CustomPiece> pieces = new List<CustomPiece>();

        // public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake() {
            assetBundle = AssetUtils.LoadAssetBundleFromResources("containerstacks");

            AddPiece("MS_container_wood_stack", "Wood");
            AddPiece("MS_container_finewood_stack", "FineWood");
            AddPiece("MS_container_corewood_stack", "RoundLog");
            AddPiece("MS_container_stone_pile", "Stone");
            AddPiece("MS_container_coal_pile", "Coal");
            AddPiece("MS_container_yggdrasil_wood_stack", "YggdrasilWood");
            AddPiece("MS_container_blackmarble_pile", "BlackMarble");
            AddPiece("MS_container_coin_pile", "Coins");

            PieceManager.OnPiecesRegistered += OnPiecesRegistered;

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();
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
