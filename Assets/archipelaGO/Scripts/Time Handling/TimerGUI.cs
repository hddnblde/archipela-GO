using UnityEngine;
using UnityEngine.UI;
using archipelaGO.UI;

namespace archipelaGO.TimeHandling
{
    public class TimerGUI : UIWindow
    {
        #region Fields
        [SerializeField]
        private Timer m_timer = null;

        [SerializeField]
        private Text m_text = null;
        #endregion


        #region MonoBehaviour Implementation
        protected override void Awake()
        {
            base.Awake();
            RegisterEvents();
        }

        private void OnDestroy() => DeregisterEvents();
        #endregion


        #region Internal Methods
        private void RegisterEvents()
        {
            if (m_timer == null)
                return;

            m_timer.OnBegin += Show;
            m_timer.OnEnd += Hide;
            m_timer.OnTick += OnTick;
        }

        private void DeregisterEvents()
        {
            if (m_timer == null)
                return;

            m_timer.OnBegin -= Show;
            m_timer.OnEnd -= Hide;
            m_timer.OnTick -= OnTick;
        }

        private void OnTick(int seconds)
        {
            string timeLabel = ConvertToNiceTime(seconds);

            if (m_text != null)
                m_text.text = timeLabel;
        }

        private string ConvertToNiceTime(int seconds)
        {
            const int SecondsInMinute = 60;
            int minutes = 0;

            while (seconds > SecondsInMinute)
            {
                seconds -= SecondsInMinute;
                minutes++;
            }

            return $"{ minutes.ToString("D2") }:{ seconds.ToString("D2") }";
        }
        #endregion
    }
}