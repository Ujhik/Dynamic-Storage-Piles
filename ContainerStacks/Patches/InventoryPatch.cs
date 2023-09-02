using HarmonyLib;

namespace ContainerStacks.Patches {
    [HarmonyPatch]
    public static class InventoryPatch {
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.AddItem), typeof(ItemDrop.ItemData), typeof(int), typeof(int), typeof(int))]
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.AddItem), typeof(ItemDrop.ItemData))]
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.CanAddItem), typeof(ItemDrop.ItemData), typeof(int))]
        [HarmonyPrefix]
        public static void DisallowAddItem(Inventory __instance, ref bool __runOriginal, ItemDrop.ItemData item, ref bool __result) {
            if (!__runOriginal) {
                return;
            }

            if (ContainerItemLimit.Get(__instance, out ContainerItemLimit containerLimit)) {
                if (item.PrefabName() != containerLimit.allowedItemName) {
                    __result = false;
                    __runOriginal = false;
                }
            }
        }
    }
}
