using System.Collections;
using UnityEngine;

namespace archipelaGO.WorldMap
{
    public class WorldCameraController : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Camera m_camera = null;

        [SerializeField]
        private CursorInput m_cursorInput = null;

        [Space]

        [SerializeField]
        private float m_dragDeltaScale = 1f;

        [SerializeField]
        private float m_decelerationTime = 0.75f;

        [SerializeField]
        private AnimationCurve m_decelerationCurve =
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        [Space]

        [SerializeField]
        private Vector2 m_bounds = Vector2.one * 10f;

        [SerializeField]
        private Vector2 m_boundsPadding = Vector2.zero;

        [SerializeField]
        private Vector2 m_boundsPivot = Vector2.zero;

        private Coroutine m_dragRoutine = null;
        #endregion


        #region MonoBehaviour Implementation
        private void Awake()
        {
            if (m_cursorInput != null)
                m_cursorInput.InitializeCamera(m_camera);
        }

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

        private void MovePosition(Vector2 delta)
        {
            transform.Translate(delta, Space.World);
            ClampToBounds();
        }

        private void ClampToBounds()
        {
            float clampedX = GetClampedHorizontalPoint();
            float clampedY = GetClampedVerticalPoint();
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
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

        private float GetClampedHorizontalPoint()
        {
            float extent = (m_bounds.x / 2f);
            float pivot = m_boundsPivot.x;
            float point = transform.position.x;
            float padding = (CalculateHorizontalOrthoSize() + m_boundsPadding.x);

            return ClampPoint(point, pivot, extent, padding);
        }

        private float GetClampedVerticalPoint()
        {
            float extent = (m_bounds.y / 2f);
            float pivot = m_boundsPivot.y;
            float point = transform.position.y;
            float padding = (GetVerticalOrthoSize() + m_boundsPadding.y);

            return ClampPoint(point, pivot, extent, padding);
        }

        private float GetVerticalOrthoSize()
        {
            if (m_camera == null)
                return 0f;
            else
                return m_camera.orthographicSize;
        }

        private float CalculateHorizontalOrthoSize()
        {
            if (m_camera == null)
                return 0f;
            else
                return (m_camera.orthographicSize * m_camera.aspect);
        }

        private float ClampPoint(float point, float pivot, float extent, float padding)
        {
            float offset = (extent - padding);

            return Mathf.Clamp(point, pivot - offset, pivot + offset);
        }
        #endregion
    }
}