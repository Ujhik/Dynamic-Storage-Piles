using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;

namespace DynamicStoragePiles.Compatibility {
    public static class AzuAutoStore {
        public static void Init() {
            PluginInfo info = Chainloader.PluginInfos["Azumatt.AzuAutoStore"];

            if (info.Metadata.Version >= new System.Version(2, 0, 0)) {
                DynamicStoragePiles.harmony.PatchAll(typeof(AzuAutoStore_After_2_0_0));
            } else {
                DynamicStoragePiles.harmony.PatchAll(typeof(AzuAutoStore_Before_2_0_0));
            }
        }

        private static class AzuAutoStore_After_2_0_0 {
            [HarmonyPatch("AzuAutoStore.Util.Boxes, AzuAutoStore", "CanItemBeStored"), HarmonyPostfix]
            private static void CanItemBeStoredPatch(string container, string prefab, ref bool __result) {
                if (!__result) {
                    return;
                }

                if (!DynamicStoragePiles.azuAutoStoreItemWhitelist.Value) {
                    return;
                }

                if (DynamicStoragePiles.IsStackPiece(container, out string allowedItem) && prefab != allowedItem) {
                    __result = false;
                }
            }
        }

        private static class AzuAutoStore_Before_2_0_0 {
            [HarmonyPatch("AzuAutoStore.Util.Functions, AzuAutoStore", "CheckDisallowedItems"), HarmonyPostfix]
            private static void CheckDisallowedItemsPatch(Container container, ItemDrop.ItemData item, ref bool __result) {
                if (__result) {
                    return;
                }

                if (!DynamicStoragePiles.azuAutoStoreItemWhitelist.Value) {
                    return;
                }

                if (!container || item == null || !item.m_dropPrefab) {
                    return;
                }

                string prefabName = Utils.GetPrefabName(container.name);

                if (DynamicStoragePiles.IsStackPiece(prefabName, out string allowedItem) && item.m_dropPrefab.name != allowedItem) {
                    __result = true;
                }
            }
        }
    }
}
