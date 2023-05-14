using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype.Service
{
    public class SceneLoaderService
    {
        public void ReloadCurrentScene()
        {
            GameObject sc = GameObject.Find("SceneContext");
            GameObject.Destroy(sc);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
