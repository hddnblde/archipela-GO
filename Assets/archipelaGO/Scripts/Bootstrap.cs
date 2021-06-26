using UnityEngine;
using archipelaGO.SceneHandling;

namespace archipelaGO
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField]
        private Scene m_firstSceneToLoad = Scene.WorldMap;

        private void Start() => SceneLoader.LoadScene(m_firstSceneToLoad);
    }
}