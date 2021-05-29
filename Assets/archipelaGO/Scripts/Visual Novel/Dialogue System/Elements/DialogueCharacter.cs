using UnityEngine;

namespace archipelaGO.VisualNovel.DialogueSystem.Elements
{
    [CreateAssetMenu(fileName = "Dialogue Character", menuName = "archipelaGO/Visual Novel/Character")]
    public class DialogueCharacter : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private Sprite m_sprite;

        [SerializeField]
        private string m_name;
        #endregion


        #region Properties
        public Sprite sprite => m_sprite;
        public string characterName => m_name;
        #endregion
    }
}