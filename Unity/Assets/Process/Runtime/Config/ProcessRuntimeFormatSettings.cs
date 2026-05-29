using UnityEngine;

namespace Process.Runtime
{
    public static class ProcessRuntimeFormatSettings
    {
        private const string FormatKey = "Process_RuntimeFormat";

        public static ProcessConfigFormat GetFormat()
        {
            var value = PlayerPrefs.GetInt(FormatKey, (int)ProcessConfigFormat.Binary);
            return (ProcessConfigFormat)value;
        }

        public static void SetFormat(ProcessConfigFormat format)
        {
            PlayerPrefs.SetInt(FormatKey, (int)format);
            PlayerPrefs.Save();
        }
    }
}
