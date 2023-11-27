using Jotunn;
using Jotunn.Managers;
using UnityEngine;

namespace DynamicStoragePiles.Compatibility {
    public static class IngotStacks {
        private static AssetBundle assetBundle;

        public static void Init() {
            assetBundle = PieceHelper.GetAssetBundle("stackedingots");
            ConvertToPiece("ingot_copper_stack", "vfx_copper_stack_destroyed", "MS_IngotStacks_Copper", "Copper");
            ConvertToPiece("ingot_tin_stack", "vfx_tin_stack_destroyed", "MS_IngotStacks_Tin", "Tin");
            ConvertToPiece("ingot_bronze_stack", "vfx_bronze_stack_destroyed", "MS_IngotStacks_Bronze", "Bronze");
            ConvertToPiece("ingot_iron_stack", "vfx_iron_stack_destroyed", "MS_IngotStacks_Iron", "Iron");
            ConvertToPiece("ingot_silver_stack", "vfx_silver_stack_destroyed", "MS_IngotStacks_Silver", "Silver");
            ConvertToPiece("ingot_blackmetal_stack", "vfx_blackmetal_stack_destroyed", "MS_IngotStacks_Blackmetal", "BlackMetal");
            ConvertToPiece("ingot_flametal_stack", "vfx_flametal_stack_destroyed", "MS_IngotStacks_Flametal", "Flametal");
        }

        public static void ConvertToPiece(string baseAssetName, string vfxName, string newPrefabName, string resource) {
            GameObject basePrefab = assetBundle.LoadAsset<GameObject>(baseAssetName);
            GameObject vfx = PieceHelper.FindDestroyVFX(basePrefab, vfxName);
            GameObject stack = PrefabManager.Instance.CreateClonedPrefab(newPrefabName, vfx);
            stack = PieceHelper.PreparePiecePrefab(stack, basePrefab, $"${baseAssetName}");
            stack.name = newPrefabName;
            DynamicStoragePiles.Instance.AddPiece(stack, resource, 3);
        }

        public static void DisablePieceRecipes(bool disable) {
            DynamicStoragePiles.TogglePieceRecipes("ingot_copper_stack", !disable);
            DynamicStoragePiles.TogglePieceRecipes("ingot_tin_stack", !disable);
            DynamicStoragePiles.TogglePieceRecipes("ingot_bronze_stack", !disable);
            DynamicStoragePiles.TogglePieceRecipes("ingot_iron_stack", !disable);
            DynamicStoragePiles.TogglePieceRecipes("ingot_silver_stack", !disable);
            DynamicStoragePiles.TogglePieceRecipes("ingot_blackmetal_stack", !disable);
            DynamicStoragePiles.TogglePieceRecipes("ingot_flametal_stack", !disable);
        }

        public static void DisableAdditionalPieceRecipes(bool disable) {
            DynamicStoragePiles.TogglePieceRecipes("MS_IngotStacks_Copper", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_IngotStacks_Tin", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_IngotStacks_Bronze", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_IngotStacks_Iron", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_IngotStacks_Silver", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_IngotStacks_Blackmetal", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_IngotStacks_Flametal", !disable);
        }
    }
}
