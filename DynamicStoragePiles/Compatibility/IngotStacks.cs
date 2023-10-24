using Jotunn;
using Jotunn.Managers;
using UnityEngine;

namespace DynamicStoragePiles.Compatibility {
    public static class IngotStacks {
        private static AssetBundle assetBundle;

        public static void Init() {
            assetBundle = GetAssetBundle();
            ConvertToPiece("ingot_copper_stack", "vfx_copper_stack_destroyed", "MS_IngotStacks_Copper", "Copper");
        }

        private static void ConvertToPiece(string baseAssetName, string vfxName, string newPrefabName, string resource) {
            GameObject basePrefab = assetBundle.LoadAsset<GameObject>(baseAssetName);
            GameObject vfx = FindDestroyVFX(basePrefab, vfxName);
            GameObject stack = PrefabManager.Instance.CreateClonedPrefab(newPrefabName, vfx);
            stack = PreparePiecePrefab(stack, basePrefab, $"${baseAssetName}");
            DynamicStoragePiles.Instance.AddPiece(stack, resource, 3);
        }

        private static GameObject PreparePiecePrefab(GameObject prefab, GameObject basePrefab, string nameToken) {
            Object.Destroy(prefab.GetComponent<Gibber>());

            ZNetView nview = prefab.GetComponent<ZNetView>();
            nview.m_persistent = true;

            prefab.AddComponentCopy(basePrefab.GetComponent<Piece>());
            prefab.AddComponentCopy(basePrefab.GetComponent<WearNTear>());

            Container container = prefab.AddComponent<Container>();
            container.m_name = nameToken;
            container.m_width = 5;
            container.m_height = 2;

            VisualStack visualStack = prefab.AddComponent<VisualStack>();

            foreach (Transform child in prefab.transform) {
                child.gameObject.AddComponent<BoxCollider>();
                visualStack.stackMeshes.Add(child);
            }

            return prefab;
        }

        private static GameObject FindDestroyVFX(GameObject prefab, string vfxName) {
            if (prefab.TryGetComponent(out WearNTear wearNTear)) {
                foreach (EffectList.EffectData effect in wearNTear.m_destroyedEffect.m_effectPrefabs) {
                    if (effect.m_prefab && effect.m_prefab.name == vfxName) {
                        return effect.m_prefab;
                    }
                }
            }

            return null;
        }

        private static AssetBundle GetAssetBundle() {
            foreach (AssetBundle loadedAssetBundle in AssetBundle.GetAllLoadedAssetBundles()) {
                if (loadedAssetBundle.name == "stackedingots") {
                    return loadedAssetBundle;
                }
            }

            return null;
        }
    }
}
