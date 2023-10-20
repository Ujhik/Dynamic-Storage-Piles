using System;
using System.Collections.Generic;
using UnityEngine;

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

        private void UpdateVisuals() {
            SetVisualsActive(inventory.SlotsUsedPercentage());
        }

        public void SetVisualsActive(float fillPercentage) {
            float fillCount = Mathf.Ceil(fillPercentage / 100f * stackMeshes.Count);

            for (int i = 0; i < stackMeshes.Count; i++) {
                bool active = i == 0 || i < fillCount;
                stackMeshes[i].gameObject.SetActive(active);
            }
        }
    }
}
