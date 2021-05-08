using System.Collections;
using UnityEngine;

public class WorldCameraController : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private CursorInput m_cursorInput = null;

    [SerializeField]
    private float m_dragDeltaScale = 1f;

    [SerializeField]
    private float m_decelerationTime = 0.75f;

    [SerializeField]
    private AnimationCurve m_decelerationCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine m_dragRoutine = null;
    #endregion


    #region MonoBehaviour Implementation
    private void OnEnable() => RegisterDragEvent();
    private void OnDisable() => DeregisterDragEvent();
    #endregion


    #region Internal Methods
    private void RegisterDragEvent()
    {
        if (m_cursorInput != null)
            m_cursorInput.OnCursorDragged += OnCursorDragged;
    }

    private void DeregisterDragEvent()
    {
        if (m_cursorInput != null)
            m_cursorInput.OnCursorDragged -= OnCursorDragged;
    }

    private void OnCursorDragged(Vector2 delta)
    {
        if (m_dragRoutine != null)
            StopCoroutine(m_dragRoutine);

        m_dragRoutine = StartCoroutine(DragRoutine(delta * m_dragDeltaScale));
    }
    #endregion


    #region Helper Methods
    private IEnumerator DragRoutine(Vector2 delta)
    {
        for (float current = 0f; current < m_decelerationTime; current += Time.deltaTime)
        {
            float t = Mathf.InverseLerp(0f, m_decelerationTime, current);
            t = m_decelerationCurve.Evaluate(t);
            Vector2 finalDelta = Vector2.Lerp(delta, Vector2.zero, t);
            MovePosition(finalDelta);
            yield return null;
        }
    }

    private void MovePosition(Vector2 delta) => transform.Translate(delta, Space.World);
    #endregion
}