using System.Runtime.CompilerServices;
using UnityEngine;

namespace ContainerStacks {
    public class ContainerItemLimit : MonoBehaviour {
        public string allowedItemName;

        private static readonly ConditionalWeakTable<Inventory, ContainerItemLimit> ContainerLimits = new ConditionalWeakTable<Inventory, ContainerItemLimit>();

        private void Awake() {
            Container container = GetComponentInParent<Container>();

            if (!container) {
                return;
            }

            Inventory inventory = container.GetInventory();

            if (inventory != null) {
                ContainerLimits.Add(inventory, this);
            }
        }

        public static bool Get(Inventory inventory, out ContainerItemLimit containerItemLimit) {
            return ContainerLimits.TryGetValue(inventory, out containerItemLimit) ? containerItemLimit : null;
        }
    }
}
