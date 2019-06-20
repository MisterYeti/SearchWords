using UnityEditor;
using UnityEngine;

namespace CtrlF
{
    public class GUILayoutExtension : MonoBehaviour
    {
        public static void DrawHorizontalLine(string header)
        {
            GUIStyle box = GUI.skin.box;
            var textWidth = box.CalcSize(new GUIContent(header));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            var horizontalRect = EditorGUILayout.GetControlRect(false, 20f);

            Rect headerRect, line2Rect, lineRect;
            headerRect = line2Rect = lineRect = horizontalRect;

            lineRect.size = new Vector2((horizontalRect.size.x - textWidth.x - 20f) / 2f, 1f);
            line2Rect.position = new Vector2(horizontalRect.position.x + lineRect.size.x + textWidth.x + 20f, line2Rect.position.y);
            line2Rect.size = lineRect.size;

            headerRect.position = new Vector2(horizontalRect.position.x + lineRect.size.x + 10f, headerRect.position.y - (headerRect.size.y / 2f));
            headerRect.size = new Vector2(headerRect.size.x / 3f, headerRect.size.y);

            EditorGUI.DrawRect(lineRect, Color.grey);
            EditorGUI.LabelField(headerRect, header);
            EditorGUI.DrawRect(line2Rect, Color.grey);
        }


        public static void DrawHorizontalLine()
        {
            EditorGUILayout.Space();
            var controlRect = EditorGUILayout.GetControlRect(GUILayout.MaxHeight(1f));
            EditorGUI.DrawRect(controlRect, Color.grey);
            EditorGUILayout.Space();
        }
    }
}
