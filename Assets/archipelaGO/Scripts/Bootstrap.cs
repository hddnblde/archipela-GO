using UnityEngine;
using archipelaGO.SceneHandling;

namespace archipelaGO
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField]
        private SceneLoader m_sceneLoader = null;

        [SerializeField]
        private string m_firstSceneToLoad = string.Empty;

        private void Start()
        {
            if (m_sceneLoader != null)
                m_sceneLoader.LoadScene(m_firstSceneToLoad);
        }
    }
}