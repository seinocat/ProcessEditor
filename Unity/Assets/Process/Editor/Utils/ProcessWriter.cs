using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Process.Editor
{
    public static class ProcessWriter
    {
        public static void WriteFile(StringBuilder builder, string path, bool refresh = true)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(fileStream, Encoding.UTF8);
            try
            {
                fileWriter.Write(builder.ToString());
                fileWriter.Flush();
            }
            catch (Exception ex)
            {
                Debug.LogError($"{path}生成失败 :{ex}");
            }
            finally
            {
                fileWriter.Close();
                fileStream.Close();
            }

            if (refresh)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}