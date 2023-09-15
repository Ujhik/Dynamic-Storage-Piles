using HarmonyLib;

namespace DynamicStoragePiles.Patches {
    [HarmonyPatch]
    public class PlacementPatch {
        [HarmonyPatch(typeof(Player), nameof(Player.SetupPlacementGhost)), HarmonyPostfix]
        public static void SetupPlacementGhostPatch(Player __instance) {
            if (!__instance.m_placementGhost) {
                return;
            }

            foreach (VisualStack visualStack in __instance.m_placementGhost.GetComponentsInChildren<VisualStack>()) {
                UnityEngine.Object.Destroy(visualStack);
            }
        }
    }
}
