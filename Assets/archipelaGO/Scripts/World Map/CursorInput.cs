using UnityEngine;

namespace archipelaGO.WorldMap
{

    public class CursorInput : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Camera m_camera = null;

        [SerializeField]
        private bool m_horizontal = true;

        [SerializeField]
        private bool m_vertical = true;

        private int m_previousTouchCount = 0;
        private Vector2 m_previousCursorPosition = Vector2.zero;
        private Vector2 m_cursorDelta = Vector2.zero;

        public delegate void CursorDragged(Vector2 delta);
        public event CursorDragged OnCursorDragged;
        #endregion


        #region MonoBehaviour Implementation
        private void Update() => DetectCursorDrag();
        #endregion


        #region Internal Methods
        private void DetectCursorDrag()
        {
            (Vector3 position, bool began) cursor = GetCursorPosition();
            CaptureDrag(cursor.position, cursor.began);

            if (cursor.began)
                InvokeCursorDragged(Vector2.zero);

            else if (DragDetected())
                InvokeCursorDragged(m_cursorDelta);
        }

        private void InvokeCursorDragged(Vector2 delta) =>
            OnCursorDragged?.Invoke(delta);

        private void CaptureDrag(Vector3 cursorPosition, bool began)
        {
            m_cursorDelta = (began ? Vector2.zero :
                GetDelta(m_previousCursorPosition, cursorPosition));

            m_previousCursorPosition = cursorPosition;
        }
        #endregion


        #region Helper Methods
        private (Vector3 position, bool began) GetCursorPosition()
        {
            if (Application.isEditor)
                return GetCursorPositionFromMouse();

            else
                return GetCursorPositionFromTouch();
        }

        private (Vector3 position, bool began) GetCursorPositionFromMouse()
        {
            const int LeftMouseButtonIndex = 0;

            if (Input.GetMouseButtonDown(LeftMouseButtonIndex))
                return (Input.mousePosition, true);

            else if (Input.GetMouseButton(LeftMouseButtonIndex))
                return (Input.mousePosition, false);

            else
                return (m_previousCursorPosition, false);
        }

        private (Vector3 position, bool began) GetCursorPositionFromTouch()
        {
            Vector2 averageTouchPosition = Vector2.zero;
            int currentTouchCount = Input.touchCount;
            int recordedTouchCount = 0;
            bool began = false;

            for (int i = 0; i < currentTouchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (TouchBegan(touch) || m_previousTouchCount != currentTouchCount)
                {
                    began = true;
                    break;
                }

                if (!TouchMoved(touch))
                    continue;
                
                recordedTouchCount++;
                averageTouchPosition += touch.position;
            }

            m_previousTouchCount = currentTouchCount;

            if (recordedTouchCount <= 0)
                return (m_previousCursorPosition, began);

            else
                return ((averageTouchPosition / recordedTouchCount), began);
        }

        private bool DragDetected() => (m_cursorDelta.sqrMagnitude > 0f);
        private bool TouchMoved(Touch touch) => (touch.phase == TouchPhase.Moved);
        private bool TouchBegan(Touch touch) => (touch.phase == TouchPhase.Began);

        private Vector2 GetDelta(Vector2 currentPosition, Vector2 previousPosition)
        {
            Vector3 currentWorldPosition = GetWorldPositionFromCameraRay(currentPosition);
            Vector3 previousWorldPosition = GetWorldPositionFromCameraRay(previousPosition);

            return ConvertDeltaTo2D(currentWorldPosition - previousWorldPosition);
        }

        private Vector2 ConvertDeltaTo2D(Vector3 delta)
        {
            delta = transform.InverseTransformDirection(delta);

            float horizontalMultiplier = (m_horizontal ? 1f : 0f);
            float verticalMultiplier = (m_vertical ? 1f : 0f);

            Vector2 deltaMultiplier =
                new Vector2(horizontalMultiplier, verticalMultiplier);

            return Vector2.Scale(delta, deltaMultiplier);
        }

        private Vector3 GetWorldPositionFromCameraRay(Vector2 cursorPosition)
        {
            const float RayDistance = 1f;
            Ray ray = GetCameraRay(cursorPosition);

            return ray.GetPoint(RayDistance);
        }

        private Ray GetCameraRay(Vector2 cursorPosition)
        {
            if (m_camera == null)
                return new Ray(Vector3.zero, Vector3.forward);

            else
                return m_camera.ScreenPointToRay(cursorPosition);
        }
        #endregion
    }
}