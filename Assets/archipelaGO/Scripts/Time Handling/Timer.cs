using System.Collections;
using UnityEngine;

namespace archipelaGO.TimeHandling
{
    public class Timer : MonoBehaviour
    {
        #region Fields
        public event Tick OnTick;
        public Notification OnBegin;
        public Notification OnEnd;

        public delegate void Notification();
        public delegate void Tick(int seconds);

        private Coroutine m_countdownRoutine = null;
        #endregion


        #region Methods
        public void StopCountdown()
        {
            if (m_countdownRoutine != null)
                StopCoroutine(m_countdownRoutine);

            InvokeEnd();
        }

        public void StartCountdown(float duration, Notification onFinishedCallback)
        {
            if (m_countdownRoutine != null)
                StopCoroutine(m_countdownRoutine);

            m_countdownRoutine = StartCoroutine(CountdownRoutine(duration, onFinishedCallback));
        }

        private IEnumerator CountdownRoutine(float duration, Notification onFinishedCallback)
        {
            int tick = -1;
            InvokeBegin();
            InvokeTick(Mathf.FloorToInt(duration));

            for (; duration > 0; duration -= Time.deltaTime)
            {
                int currentTick = Mathf.FloorToInt(duration);

                if (currentTick != tick)
                {
                    tick = currentTick;
                    InvokeTick(tick);
                }

                yield return null;
            }

            InvokeTick(0);
            InvokeEnd();
            onFinishedCallback?.Invoke();
        }

        private void InvokeTick(int seconds) =>
            OnTick?.Invoke(seconds);

        private void InvokeBegin() =>
            OnBegin?.Invoke();

        private void InvokeEnd() =>
            OnEnd?.Invoke();
        #endregion
    }
}