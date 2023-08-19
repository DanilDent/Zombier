using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototype.MeshCombine
{
    // TODO: Rewrite so it's non MonoBehaviour
    public class MeshCombiner
    {
        public MeshCombiner()
        {
            _objectsToCombine = new List<GameObject>();
        }

        public bool UseMipMaps = true;
        public TextureFormat TextureFormat = TextureFormat.RGB24;

        /*
         * Combines all object textures into a single texture then creates a material used by all objects.
         * The materials properties are based on those of the material of the object at position[0].
         *
         * Also combines any meshes marked as static into a single mesh.
         */
        public GameObject Combine(string newGameObjectName = "New Game Object", bool isSameTexture = true)
        {
            int size;
            int originalSize;
            int pow2;
            Texture2D combinedTexture;
            Material material;
            Texture2D texture;
            Mesh mesh;
            Hashtable textureAtlas = new Hashtable();
            GameObject resultGO = null;

            if (_objectsToCombine.Count > 1)
            {
                originalSize = _objectsToCombine[0].GetComponent<Renderer>().material.mainTexture.width;
                pow2 = GetTextureSize(_objectsToCombine, isSameTexture);
                size = pow2 * originalSize;
                combinedTexture = new Texture2D(size, size, TextureFormat, UseMipMaps);

                // Create the combined texture (remember to ensure the total size of the texture isn't
                // larger than the platform supports)

                if (_objectsToCombine.Count > 0)
                {
                    texture = (Texture2D)_objectsToCombine[0].GetComponent<Renderer>().material.mainTexture;
                }
                else
                {
                    texture = default;
                }
                for (int i = 0; i < _objectsToCombine.Count; i++)
                {
                    if (!isSameTexture)
                    {
                        texture = (Texture2D)_objectsToCombine[i].GetComponent<Renderer>().material.mainTexture;
                    }
                    if (!textureAtlas.ContainsKey(texture))
                    {
                        combinedTexture.SetPixels((i % pow2) * originalSize, (i / pow2) * originalSize, originalSize, originalSize, texture.GetPixels());
                        textureAtlas.Add(texture, new Vector2(i % pow2, i / pow2));
                    }
                }
                combinedTexture.Apply();
                material = new Material(_objectsToCombine[0].GetComponent<Renderer>().material);
                material.mainTexture = combinedTexture;

                // Update texture co-ords for each mesh (this will only work for meshes with coords betwen 0 and 1).
                if (!isSameTexture)
                {
                    for (int i = 0; i < _objectsToCombine.Count; i++)
                    {
                        mesh = _objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                        Vector2[] uv = new Vector2[mesh.uv.Length];
                        Vector2 offset;
                        if (textureAtlas.ContainsKey(_objectsToCombine[i].GetComponent<Renderer>().material.mainTexture))
                        {
                            offset = (Vector2)textureAtlas[_objectsToCombine[i].GetComponent<Renderer>().material.mainTexture];
                            for (int u = 0; u < mesh.uv.Length; u++)
                            {
                                uv[u] = mesh.uv[u] / (float)pow2;
                                uv[u].x += ((float)offset.x) / (float)pow2;
                                uv[u].y += ((float)offset.y) / (float)pow2;
                            }
                        }
                        else
                        {
                            // This happens if you use the same object more than once, don't do it :)
                        }

                        mesh.uv = uv;
                        _objectsToCombine[i].GetComponent<Renderer>().material = material;
                    }
                }
                else
                {

                    if (_objectsToCombine.Count > 0)
                    {
                        mesh = _objectsToCombine[0].GetComponent<MeshFilter>().mesh;
                        Vector2 offset;
                        Vector2[] uv = new Vector2[mesh.uv.Length];

                        if (textureAtlas.ContainsKey(_objectsToCombine[0].GetComponent<Renderer>().material.mainTexture))
                        {
                            offset = (Vector2)textureAtlas[_objectsToCombine[0].GetComponent<Renderer>().material.mainTexture];
                            for (int u = 0; u < mesh.uv.Length; u++)
                            {
                                uv[u] = mesh.uv[u] / (float)pow2;
                                uv[u].x += ((float)offset.x) / (float)pow2;
                                uv[u].y += ((float)offset.y) / (float)pow2;
                            }
                        }

                        for (int i = 0; i < _objectsToCombine.Count; i++)
                        {
                            mesh = _objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                            mesh.uv = uv;
                            _objectsToCombine[i].GetComponent<Renderer>().material = material;
                        }
                    }
                }

                // Combine each mesh 
                int count = 0;
                CombineInstance[] combine = new CombineInstance[_objectsToCombine.Count];
                for (int i = 0; i < _objectsToCombine.Count; i++)
                {
                    count++;
                    combine[i].mesh = _objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                    combine[i].transform = _objectsToCombine[i].transform.localToWorldMatrix;
                }

                // Create a mesh filter and renderer
                if (count > 0)
                {
                    resultGO = new GameObject();
                    resultGO.name = newGameObjectName;
                    MeshFilter filter = resultGO.AddComponent<MeshFilter>();
                    MeshRenderer renderer = resultGO.AddComponent<MeshRenderer>();
                    filter.mesh = new Mesh();
                    filter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    filter.mesh.CombineMeshes(combine);
                    renderer.material = material;
                }

                Resources.UnloadUnusedAssets();
                _objectsToCombine.Clear();
                return resultGO;
            }

            _objectsToCombine.Clear();
            return resultGO == null ? new GameObject(newGameObjectName) : resultGO;
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

        // The objects to combine, each should have a mesh filter and renderer with a single material.
        private List<GameObject> _objectsToCombine;

        private int GetTextureSize(List<GameObject> o, bool isSameTexture = true)
        {
            ArrayList textures = new ArrayList();
            if (!isSameTexture)
            {
                // Find unique textures
                for (int i = 0; i < o.Count; i++)
                {
                    if (!textures.Contains(o[i].GetComponent<Renderer>().material.mainTexture))
                    {
                        textures.Add(o[i].GetComponent<Renderer>().material.mainTexture);
                    }
                }
            }
            else if (o.Count > 0)
            {
                textures.Add(o[0].GetComponent<Renderer>().material.mainTexture);
            }

            if (textures.Count == 1) return 1;
            if (textures.Count < 5) return 2;
            if (textures.Count < 17) return 4;
            if (textures.Count < 65) return 8;
            // Doesn't handle more than 64 different textures but I think you can see how to extend
            return 0;
        }
    }
}
