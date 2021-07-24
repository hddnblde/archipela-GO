using UnityEngine;

namespace archipelaGO.VisualNovel.DialogueSystem.Elements
{
    [System.Serializable]
    public struct DialogueLine
    {
        #region Fields
        [SerializeField]
        private AudioClip m_voiceOver;

        [SerializeField, TextArea(3, 50)]
        private string m_text;
        #endregion


        #region Properties
        public AudioClip voiceOver => m_voiceOver;
        public string text => m_text;
        #endregion
    }
}