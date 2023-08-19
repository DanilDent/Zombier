using UnityEngine;

namespace Prototype.Misc
{
    public interface ICombiner
    {
        public GameObject Combine(string newGameObjectName = "New Game Object", bool isSameTexture = true);
        public void SetObjectsToCombine(GameObject[] objectsToCombine);
    }
}
