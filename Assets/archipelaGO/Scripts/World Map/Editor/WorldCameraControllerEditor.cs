using UnityEngine;
using UnityEditor;

namespace archipelaGO.WorldMap
{
    [CustomEditor(typeof(WorldCameraController))]
    public class WorldCameraControllerEditor : Editor
    {
        #region Fields
        private SerializedProperty m_bounds = null,
            m_boundsPadding = null,
            m_boundsPivot = null;
        #endregion


        #region Editor Implementation
        private void OnEnable()
        {
            m_bounds = serializedObject.FindProperty("m_bounds");
            m_boundsPadding = serializedObject.FindProperty("m_boundsPadding");
            m_boundsPivot = serializedObject.FindProperty("m_boundsPivot");
        }

        private void OnSceneGUI() => DrawBoundsGizmo();
        #endregion


        #region Bounds Drawing Implementation
        private void DrawBoundsGizmo()
        {
            Vector2 bounds = m_bounds.vector2Value;
            Vector2 pivot = m_boundsPivot.vector2Value;
            Vector2 padding = m_boundsPadding.vector2Value;

            DrawBoundingPlane(Color.cyan, bounds, pivot, padding, false);
            DrawBoundingPlane(Color.blue, bounds, pivot, Vector2.zero, true);
        }

        private void DrawBoundingPlane(Color color, Vector2 bounds, Vector2 pivot, Vector2 padding, bool showCorners = true)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(pivot, Quaternion.identity, Vector3.one);
            float horizontalExtents = (bounds.x / 2f);
            float verticalExtents = (bounds.y / 2f);

            using (new Handles.DrawingScope(color, matrix))
            {
                Vector3 right = Vector3.right * (horizontalExtents - padding.x);
                Vector3 up = Vector3.up * (verticalExtents - padding.y);

                Vector3 topLeft = up - right;
                Vector3 topRight = up + right;
                Vector3 bottomLeft = -up - right;
                Vector3 bottomRight = -up + right;

                Vector3[] lineSegments = new Vector3[]
                {
                    topLeft, topRight,
                    topRight, bottomRight,
                    bottomRight, bottomLeft,
                    bottomLeft, topLeft
                };

                Handles.DrawLines(lineSegments);

                if (!showCorners)
                    return;
                
                DrawDot(topLeft);
                DrawDot(topRight);
                DrawDot(bottomLeft);
                DrawDot(bottomRight);
            }
        }

        private void DrawDot(Vector3 position)
        {
            float dotSize = (HandleUtility.GetHandleSize(position) * 0.03f);
            Handles.DotHandleCap(0, position, Quaternion.identity, dotSize, Event.current.type);
        }
        #endregion
    }
}