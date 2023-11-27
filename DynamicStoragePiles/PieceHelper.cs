using DynamicStoragePiles.Compatibility;
using Jotunn;
using Jotunn.Managers;
using UnityEngine;

namespace DynamicStoragePiles {
    public static class PieceHelper {
        public static GameObject PreparePiecePrefab(GameObject prefab, GameObject basePrefab, string nameToken) {
            Object.Destroy(prefab.GetComponent<Gibber>());
            Object.Destroy(prefab.GetComponent<Collider>());

            if (prefab.transform.Find("collider")) {
                Object.Destroy(prefab.transform.Find("collider").gameObject);
            }

            ZNetView nview = prefab.GetComponent<ZNetView>();
            nview.m_persistent = true;

            if (!prefab.GetComponent<Piece>()) {
                prefab.AddComponentCopy(basePrefab.GetComponent<Piece>());
            }

            if (!prefab.GetComponent<WearNTear>()) {
                prefab.AddComponentCopy(basePrefab.GetComponent<WearNTear>());
            }

            Container container = prefab.AddComponent<Container>();
            container.m_name = nameToken;
            container.m_width = 5;
            container.m_height = 2;

            VisualStack visualStack = prefab.AddComponent<VisualStack>();

            foreach (MeshRenderer meshRenderer in prefab.GetComponentsInChildren<MeshRenderer>()) {
                if (!meshRenderer.gameObject.GetComponent<Collider>()) {
                    meshRenderer.gameObject.AddComponent<BoxCollider>();
                }

                visualStack.stackMeshes.Add(meshRenderer.transform);
            }

            return prefab;
        }

        public static GameObject FindDestroyVFX(GameObject prefab, string vfxName) {
            if (prefab.TryGetComponent(out WearNTear wearNTear)) {
                foreach (EffectList.EffectData effect in wearNTear.m_destroyedEffect.m_effectPrefabs) {
                    if (effect.m_prefab && effect.m_prefab.name == vfxName) {
                        return effect.m_prefab;
                    }
                }
            }

            return null;
        }

        public static AssetBundle GetAssetBundle(string bundleName) {
            foreach (AssetBundle loadedAssetBundle in AssetBundle.GetAllLoadedAssetBundles()) {
                if (loadedAssetBundle.name == bundleName) {
                    return loadedAssetBundle;
                }
            }

            return null;
        }
    }
}
