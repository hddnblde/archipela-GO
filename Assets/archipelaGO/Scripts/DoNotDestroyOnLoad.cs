using UnityEngine;

namespace archipelaGO
{
    public class DoNotDestroyOnLoad : MonoBehaviour
    {
        private void Awake() =>
            DontDestroyOnLoad(gameObject);
    }
}