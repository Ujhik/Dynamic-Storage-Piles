using Jotunn.Managers;
using UnityEngine;

namespace DynamicStoragePiles.Compatibility {
    public static class StackedBars {
        private static AssetBundle assetBundle;

        public static void Init() {
            assetBundle = PieceHelper.GetAssetBundle("stackedbars");
            ConvertToPiece("stack_tinbars", "MS_StackedBars_Tin", "Tin");
            ConvertToPiece("stack_copperbars", "MS_StackedBars_Copper", "Copper");
            ConvertToPiece("stack_bronzebars", "MS_StackedBars_Bronze", "Bronze");
            ConvertToPiece("stack_ironbars", "MS_StackedBars_Iron", "Iron");
            ConvertToPiece("stack_silverbars", "MS_StackedBars_Silver", "Silver");
            ConvertToPiece("stack_blackmetalbars", "MS_StackedBars_BlackMetal", "BlackMetal");
            ConvertToPiece("stack_flametalbars", "MS_StackedBars_FlameMetal", "Flametal");
        }

        public static void ConvertToPiece(string baseAssetName, string newPrefabName, string resource) {
            GameObject basePrefab = assetBundle.LoadAsset<GameObject>(baseAssetName);
            GameObject stack = PrefabManager.Instance.CreateClonedPrefab(newPrefabName, basePrefab);
            stack = PieceHelper.PreparePiecePrefab(stack, basePrefab, $"$piece_{baseAssetName}");
            stack.name = newPrefabName;
            DynamicStoragePiles.Instance.AddPiece(stack, resource, 3);
        }

        public static void DisablePieceRecipes(bool disable) {
            DynamicStoragePiles.TogglePieceRecipes("stack_tinbars", !disable);
            DynamicStoragePiles.TogglePieceRecipes("stack_copperbars", !disable);
            DynamicStoragePiles.TogglePieceRecipes("stack_bronzebars", !disable);
            DynamicStoragePiles.TogglePieceRecipes("stack_ironbars", !disable);
            DynamicStoragePiles.TogglePieceRecipes("stack_silverbars", !disable);
            DynamicStoragePiles.TogglePieceRecipes("stack_blackmetalbars", !disable);
            DynamicStoragePiles.TogglePieceRecipes("stack_flametalbars", !disable);
        }

        public static void DisableAdditionalPieceRecipes(bool disable) {
            DynamicStoragePiles.TogglePieceRecipes("MS_StackedBars_Tin", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_StackedBars_Copper", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_StackedBars_Bronze", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_StackedBars_Iron", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_StackedBars_Silver", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_StackedBars_BlackMetal", !disable);
            DynamicStoragePiles.TogglePieceRecipes("MS_StackedBars_FlameMetal", !disable);
        }
    }
}
