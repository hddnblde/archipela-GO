using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.Progression
{
    public class ProgressManager : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private GameProgressCollection m_progressCollection = null;

        [SerializeField]
        private int m_progressIndex = -1;

        [SerializeField]
        private List<ProgressManager> m_unlockableKeys = new List<ProgressManager>();
        #endregion


        #region Public Methods
        public bool IsUnlocked()
        {
            if (m_progressIndex == -1)
                return true;

            if (m_progressCollection == null)
                return false;

            string key = GetProgressKey(this);

            return ProgressDataHandler.IsUnlocked(key);
        }

        public void UnlockAssignedKeys()
        {
            foreach (ProgressManager unlockableKey in m_unlockableKeys)
            {
                string key = GetProgressKey(unlockableKey);
                ProgressDataHandler.Unlock(key);
            }
        }
        #endregion


        #region Internal Method
        private static string GetProgressKey(ProgressManager progressManager)
        {
            if (progressManager == null || progressManager.m_progressCollection == null)
                return string.Empty;

            return progressManager.m_progressCollection.
                GetKey(progressManager.m_progressIndex);
        }
        #endregion
    }
}