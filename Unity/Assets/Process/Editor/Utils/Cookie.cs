using UnityEngine;

namespace Process.Editor
{
    public class Cookie
    {
        public static void SetPublic(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
        public static void SetPublic(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        public static void SetPublic(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static void SetPublic(string key,bool value)
        {
            if (value)
                PlayerPrefs.SetInt(key, 1);
            else
                PlayerPrefs.SetInt(key, 0);
        }

        public static string GetPublic(string key, string defaultValue = null)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        public static int GetPublic(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        public static float GetPublic(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public static bool GetPublic(string key,bool defaultValue=false)
        {
            int newValue = 0;
            if (defaultValue)
                newValue = 1;
            return PlayerPrefs.GetInt(key, newValue) ==1;
        }

        public static void DeletePublicKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        public static bool ContainsPublicKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}