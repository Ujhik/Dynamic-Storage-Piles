using Jotunn;

namespace ContainerStacks {
    public static class InventoryHelper {
        public static string PrefabName(this ItemDrop.ItemData item) {
            if (item.m_dropPrefab) {
                return item.m_dropPrefab.name;
            }

            Logger.LogWarning("Item has missing prefab " + item.m_shared.m_name);
            return item.m_shared.m_name;
        }
    }
}
