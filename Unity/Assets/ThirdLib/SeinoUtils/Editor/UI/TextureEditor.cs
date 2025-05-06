using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Seino.Utils.Editor
{
    public enum TargetPlatform
    {
        Standalone,
        IOS,
        Android
    }

    public class TextureEditorWindow :EditorWindow
    {
        [MenuItem("Tools/SeinoUtils/TextureEditor")]
        public static void OpenWindow()
        {
            TextureEditorWindow window = EditorWindow.CreateInstance<TextureEditorWindow>();
            window.Show();
        }

        string TexPath;
        string TexSuffix = "*.bmp|*.jpg|*.gif|*.png|*.tif|*.psd";
        TargetPlatform SelectPlatform = TargetPlatform.Android;
        TextureImporterFormat WithAlpha = TextureImporterFormat.ASTC_4x4;
        TextureImporterFormat WithoutAlpha = TextureImporterFormat.PVRTC_RGB4;
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            TexPath = EditorGUILayout.TextField("图片资源路径", TexPath);
            if (GUILayout.Button(EditorGUIUtility.IconContent("Folder Icon"), GUILayout.Width(18), GUILayout.Height(18)))
            {
                TexPath = EditorUtility.OpenFolderPanel("图片路径选择", "", "");
            }
            GUILayout.EndHorizontal();
            
            GUI.enabled = false;
            TexSuffix = EditorGUILayout.TextField("图片格式", TexSuffix);
            GUI.enabled = true;
            
            SelectPlatform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), EditorGUILayout.EnumPopup("选择目标平台", SelectPlatform).ToString());
            WithAlpha = (TextureImporterFormat)Enum.Parse(typeof(TextureImporterFormat), EditorGUILayout.EnumPopup("有Alpha通道", WithAlpha).ToString());
            WithoutAlpha = (TextureImporterFormat)Enum.Parse(typeof(TextureImporterFormat), EditorGUILayout.EnumPopup("没有Alpha通道", WithoutAlpha).ToString());
            

            if (GUILayout.Button("转换"))
            {
                if (string.IsNullOrEmpty(TexPath) || !Directory.Exists(TexPath))
                {
                    EditorUtility.DisplayDialog("错误", "路径不能为空或路径不存在", "确定");
                    return;
                }
                if (string.IsNullOrEmpty(TexSuffix))
                {
                    EditorUtility.DisplayDialog("错误", "路径不能为空或路径不存在", "确定");
                    return;
                }
                List<string> lst = GetAllTexPaths(TexPath);
                TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
                settings.name = SelectPlatform.ToString();
                settings.crunchedCompression = true;
                settings.overridden = true;
                
                int i = 0;
                EditorUtility.DisplayProgressBar("修改", "修改图片格式", 0);
                for (; i < lst.Count; i++)
                {
                    Modifty(lst[i], settings);
                    EditorUtility.DisplayProgressBar("转换", $"修改图片格式    {i}/{lst.Count}", i / (float)lst.Count);
                }
                AssetDatabase.SaveAssets();
                EditorUtility.ClearProgressBar();
            }

        }
        private void Modifty(string path, TextureImporterPlatformSettings platformSettings)
        {
            path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
            try
            {
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                platformSettings.format = textureImporter.DoesSourceTextureHaveAlpha() ? WithAlpha : WithoutAlpha;
                textureImporter.SetPlatformTextureSettings(platformSettings);
                textureImporter.SaveAndReimport();
                AssetDatabase.ImportAsset(path);
            }
            catch
            {
                AssetDatabase.SaveAssets();
                EditorUtility.ClearProgressBar();
            }
        }


        private List<string> GetAllTexPaths(string rootPath)
        {
            List<string> lst = new List<string>();
            string[] types = TexSuffix.Split('|');
            for (int i = 0; i < types.Length; i++)
            {
                lst.AddRange(Directory.GetFiles(rootPath, types[i], SearchOption.AllDirectories));
            }
            return lst;
        }
    }
}
