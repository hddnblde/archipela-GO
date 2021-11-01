using UnityEngine;

public class BGMPlaceholder : MonoBehaviour
{
    #region Fields
    private static BGMPlaceholder m_singletonInstance = null;

    [SerializeField]
    private AudioSource m_audioSource = null;
    #endregion


    #region Methods
    private void Awake()
    {
        if (InitializeSingleton())
            DontDestroyOnLoad(gameObject);

        else
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (m_singletonInstance == this)
            m_singletonInstance = null;
    }

    private void Start()
    {
        if (m_audioSource != null)
            m_audioSource.Play();
    }

    private bool InitializeSingleton()
    {
        if (m_singletonInstance != null)
            return false;

        m_singletonInstance = this;
        return true;
    }
    #endregion
}
