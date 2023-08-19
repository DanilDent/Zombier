using Prototype.Misc;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototype.StaticBatch
{
    public class StaticBatcher : ICombiner
    {
        public StaticBatcher()
        {
            _objectsToCombine = new List<GameObject>();
        }

        public GameObject Combine(string newGameObjectName = "New Game Object", bool isSameTexture = true)
        {
            GameObject result = new GameObject(newGameObjectName);
            foreach (var go in _objectsToCombine)
            {
                go.transform.SetParent(result.transform);
            }
            StaticBatchingUtility.Combine(result);
            return result;
        }

        public void SetObjectsToCombine(GameObject[] objectsToCombine)
        {
            _objectsToCombine.Clear();
            foreach (GameObject objectToCombine in objectsToCombine)
            {
                // Get all objects with mesh renderer
                GameObject[] goWithMeshes = objectToCombine.GetComponentsInChildren<MeshRenderer>().Select(x => x.gameObject).ToArray();
                _objectsToCombine.AddRange(goWithMeshes);
            }
        }

        private List<GameObject> _objectsToCombine;
    }
}
