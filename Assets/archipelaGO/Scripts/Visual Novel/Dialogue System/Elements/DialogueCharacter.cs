using UnityEngine;

namespace archipelaGO.VisualNovel.DialogueSystem.Elements
{
    [System.Serializable]
    public enum DialogueCharacterBlocking
    {
        StageLeft = 0,
        StageRight = 1
    }

    [CreateAssetMenu(fileName = "Character", menuName = "archipelaGO/Visual Novel/Character")]
    public class DialogueCharacter : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private string m_name = string.Empty;

        [SerializeField]
        private Sprite m_sprite = null;

        [SerializeField]
        private Vector2 m_pivotOffset = Vector2.zero;

        [SerializeField, Range(0.01f, 3f)]
        private float m_scale = 1f;
        #endregion


        #region Properties
        public Sprite sprite => m_sprite;
        public string displayedName => m_name;
        public Vector2 pivotOffset => m_pivotOffset;
        public float scale => m_scale;
        #endregion
    }
}