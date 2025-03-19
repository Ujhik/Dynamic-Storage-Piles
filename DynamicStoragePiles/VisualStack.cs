using System;
using System.Collections.Generic;
using UnityEngine;
using static DynamicStoragePiles.ConfigSettings;

namespace DynamicStoragePiles {
    public class VisualStack : MonoBehaviour {
        public List<Transform> stackMeshes = new List<Transform>();
        private Container container;
        private Inventory inventory;

        private void Start() {
            container = GetComponentInParent<Container>();

            if (container) {
                inventory = container.GetInventory();
            }

            if (inventory != null) {
                inventory.m_onChanged += UpdateVisuals;
                UpdateVisuals();
            }
        }
        private void OnEnable() {
            ConfigSettings.OnVisualConfigurationChanges += UpdateVisuals;
        }

        private void OnDisable() {
            ConfigSettings.OnVisualConfigurationChanges -= UpdateVisuals;
        }

        private void UpdateVisuals() {
            float containerFillPercentage;
            
            if (ConfigSettings.calculateVisualStackSettings.Value.Equals(ConfigSettings.CalculateVisualStackEnum.ByNumberOfItems)) {
                containerFillPercentage = GetNumberOfItemsFillPercentage();
            }
            else {
                containerFillPercentage = inventory.SlotsUsedPercentage();
            }

            SetVisualsActive(containerFillPercentage);
        }

        public void SetVisualsActive(float fillPercentage) {
            float fillCount = Mathf.Ceil(fillPercentage / 100f * stackMeshes.Count);

            for (int i = 0; i < stackMeshes.Count; i++) {
                bool active = i == 0 || i < fillCount;
                stackMeshes[i].gameObject.SetActive(active);
            }
        }

        private float GetNumberOfItemsFillPercentage() {
            List<ItemDrop.ItemData> inventoryItems = inventory.GetAllItems();

            if(inventoryItems.Count == 0)
                return 0;

            int maxItemStackSize = inventoryItems[0].m_shared.m_maxStackSize;

            return (float)inventory.NrOfItemsIncludingStacks() / (float)(inventory.GetHeight() * inventory.GetWidth() * maxItemStackSize) * 100f;
        }
    }
}
