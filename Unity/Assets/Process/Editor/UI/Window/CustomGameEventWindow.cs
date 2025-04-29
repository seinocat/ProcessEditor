using System;
using UnityEditor;
using UnityEngine;

namespace Process.Editor
{
    public class CustomGameEventWindow : EditorWindow
    {
        private string inputText = "";
        private Action<string> m_CallBack;
        
        private const int windowWidth = 300;
        private const int windowHeight = 150;
        private static CustomGameEventWindow m_Window;
        
        public static void OpenWindow(string title, string input, Action<string> callBack)
        {
            if (m_Window == null)
            {
                m_Window = GetWindow<CustomGameEventWindow>();
            }
            m_Window.m_CallBack = callBack;
            m_Window.inputText = input;
            m_Window.titleContent = new GUIContent(title);
            m_Window.ShowUtility();
        }

        public static void CloseWindow()
        {
            m_Window?.Close();
            m_Window = null;
        }
        
        private void OnGUI()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("名称:", EditorStyles.boldLabel);
            inputText = EditorGUILayout.TextField(inputText);
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("确定"))
            {
                m_CallBack?.Invoke(inputText);
            }
            
            if (GUILayout.Button("取消"))
            {
                Close(); 
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void OnEnable()
        {
            this.position = new Rect(Screen.currentResolution.width / 2 - windowWidth / 2,
                Screen.currentResolution.height / 2 - windowHeight / 2,
                windowWidth, windowHeight);
            this.minSize = new Vector2(windowWidth, windowHeight);
            this.maxSize = new Vector2(windowWidth, windowHeight);
        }
        

    }

}