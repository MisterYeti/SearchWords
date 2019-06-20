using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace CtrlF
{
    [System.Serializable]
    public class TextObject
    {
        public GameObject textObject;
        public string text;

        public TextObject(GameObject textObject, string text)
        {
            this.textObject = textObject;
            this.text = text;
        }
    }
}

namespace CtrlF
{
    public class CtrlFWindow : EditorWindow
    {

        private static string _wordToSearch = string.Empty;
        private static string _tempWord;
        private static bool _searching = false;
        private static bool _ignoreCase = true;
        private static bool _tempIgnoreCase = true;

        private static List<TextObject> _texts = new List<TextObject>();
        private static List<TextObject> _textsWithWord = new List<TextObject>();

        private static StringComparison _comparison = StringComparison.OrdinalIgnoreCase;

        //Styles
        static GUIStyle guiStyle_RedBold16 = new GUIStyle();
        static Vector2 scrollPosition = Vector2.zero;


        [MenuItem("CrtlF/SearchWords")]
        public static void CreateSearch()
        {
            EditorWindow searchWindow = GetWindow<CtrlFWindow>();
            searchWindow.minSize = new Vector2(350, 500);
            SearchingTexts();
            _searching = true;
        }




        void OnGUI()
        {
            //Some styles
            guiStyle_RedBold16.fontSize = 16;
            guiStyle_RedBold16.fontStyle = FontStyle.Bold;
            guiStyle_RedBold16.normal.textColor = Color.white;
            guiStyle_RedBold16.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Find words in scene", guiStyle_RedBold16);
            EditorGUILayout.Space();

            _tempWord = _wordToSearch;

            GUI.backgroundColor = Color.cyan;
            _wordToSearch = EditorGUILayout.TextField("Word(s) to search : ", _wordToSearch);
            GUI.backgroundColor = Color.white;

            _tempIgnoreCase = _ignoreCase;
            _ignoreCase = EditorGUILayout.Toggle("Ignore uppercase : ", _ignoreCase);

            _comparison = (_ignoreCase == true) ? _comparison = StringComparison.OrdinalIgnoreCase : _comparison = StringComparison.Ordinal;

            //Automatic Refresh (if the word change or the case change)
            if ((!_wordToSearch.Equals(_tempWord)) || (!_ignoreCase.Equals(_tempIgnoreCase)))
            {
                _searching = true;
            }

            //Manual Refresh
            GUI.backgroundColor = Color.cyan;
            GUILayout.BeginHorizontal();
            GUI.skin.button.fontStyle = FontStyle.Bold;
            GUI.skin.button.fontSize = 11;
            if (GUILayout.Button("REFRESH", new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(25) }))
            {
                _searching = true;
                SearchingTexts();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            //Refresh
            if (_searching)
            {
                _textsWithWord = LookingForWord(_texts, _wordToSearch);
                _searching = false;
            }

            GUILayoutExtension.DrawHorizontalLine("Research");

            DisplayObjects();

            GUILayout.EndScrollView();

        }

        static void SearchingTexts()
        {
            _texts.Clear();
            List<Text> normalTexts;
            List<TextMeshProUGUI> proTexts;

            normalTexts = Resources.FindObjectsOfTypeAll<Text>().ToList();
            proTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().ToList();

            foreach (Text text in normalTexts)
            {
                _texts.Add(new TextObject(text.gameObject, text.text));
            }

            foreach (TextMeshProUGUI text in proTexts)
            {
                _texts.Add(new TextObject(text.gameObject, text.text));
            }

            _texts = _texts.OrderBy(
                x => x.textObject.transform.GetSiblingIndex())
                .ToList();
        }

        static List<TextObject> LookingForWord(List<TextObject> textObjects, string word)
        {
            List<TextObject> toReturn = new List<TextObject>();
            foreach (TextObject textObj in textObjects)
            {

                if (textObj.text.Contains(word, _comparison))
                {
                    toReturn.Add(textObj);
                }

            }

            return toReturn;
        }

        static void DisplayObjects()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);

            GUI.backgroundColor = Color.black;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Texts containing \"" + _wordToSearch + "\"", EditorStyles.boldLabel);

            for (int i = 0; i < _textsWithWord.Count; i++)
            {
                if (_textsWithWord[i] != null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(_textsWithWord[i].textObject, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Space(50);

                    var areaStyle = new GUIStyle(GUI.skin.label);
                    areaStyle.wordWrap = true;
                    areaStyle.fontSize = 14;
                    areaStyle.margin = new RectOffset(2, 0, 10, 10);
                    areaStyle.CalcSize(new GUIContent(_textsWithWord[i].text));
                    areaStyle.richText = true;

                    if (_tempIgnoreCase)
                    {
                        if (!String.IsNullOrEmpty(_wordToSearch))
                        {
                            string newText = Regex.Replace(_textsWithWord[i].text, _wordToSearch, "<color=yellow>" + _wordToSearch + "</color>",RegexOptions.IgnoreCase);
                            EditorGUILayout.TextArea(newText, areaStyle);
                        }                         
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(_wordToSearch))
                            EditorGUILayout.TextArea(_textsWithWord[i].text.Replace(_wordToSearch, "<color=yellow>" + _wordToSearch + "</color>"), areaStyle);
                    }

                    GUI.color = Color.white;

                    EditorGUILayout.EndHorizontal();

                    if (i + 1 < _textsWithWord.Count)
                        GUILayoutExtension.DrawHorizontalLine();
                }
            }

            EditorGUILayout.EndVertical();
        }
    }

}

namespace CtrlF
{
    public static class StringExtension
    {
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source?.IndexOf(value, comparisonType) >= 0;
        }
    }

}