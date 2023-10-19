using HarmonyLib;

namespace DynamicStoragePiles.Compatibility {
    public static class AzuAutoStore {
        public static void Init() {
            DynamicStoragePiles.harmony.PatchAll(typeof(AzuAutoStore));
        }

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
