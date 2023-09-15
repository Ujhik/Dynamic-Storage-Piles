using System;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicStoragePiles {
    public class VisualStack : MonoBehaviour {
        public List<Transform> stackMeshes;
        private Container container;
        private Inventory inventory;

        private void Start() {
            container = GetComponentInParent<Container>();

            if (container) {
                inventory = container.GetInventory();
            }

            if (inventory != null) {
                inventory.m_onChanged += UpdateVisuals;
            }

            UpdateVisuals();
        }

        private void UpdateVisuals() {
            List<ItemDrop.ItemData> items = inventory?.GetAllItems();

            if (items == null) {
                return;
            }

            float fillCount = Mathf.Ceil(inventory.SlotsUsedPercentage() / 100f * stackMeshes.Count);

            for (int i = 0; i < stackMeshes.Count; i++) {
                bool active = i == 0 || i < fillCount;
                stackMeshes[i].gameObject.SetActive(active);
            }
        }
    }
}
