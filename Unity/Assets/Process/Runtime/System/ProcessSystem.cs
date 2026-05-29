using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using LitJson;
using UnityEngine;

namespace Process.Runtime
{
    public class ProcessSystem
    {
        private Dictionary<ulong, ProcessConfig> Configs;
        private const string ManifestFileName = "Events.manifest.json";

        public async UniTask LoadConfigs()
        {
            var manifestPath = Path.Combine(Application.streamingAssetsPath, ManifestFileName);
            var manifest = LoadManifest(manifestPath);

            var dataPath = Path.Combine(Application.streamingAssetsPath, manifest.FileName);
            IProcessConfigSerializer serializer = manifest.Format == ProcessConfigFormat.Json
                ? new JsonProcessConfigSerializer()
                : new BinaryProcessConfigSerializer();

            Configs = await serializer.LoadAsync(dataPath);
            if (Configs == null)
                Configs = new Dictionary<ulong, ProcessConfig>();
        }

        /// <summary>
        /// 创建流程实例
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public GameProcess CreateProcess(ulong processId, Action<ProcessStatus> callback)
        {
            if (Configs == null)
            {
                Debug.LogError("CreateProcess failed: configs not loaded");
                return null;
            }

            Configs.TryGetValue(processId, out var config);
            if (config == null)
            {
                Debug.LogError($"CreateProcess failed: process config not found, processId: {processId}");
                return null;
            }

            var process = new GameProcess();
            process.Initialize(config, callback);
            return process;
        }

        private static ProcessConfigManifest LoadManifest(string manifestPath)
        {
            if (!File.Exists(manifestPath))
            {
                return new ProcessConfigManifest
                {
                    Format = ProcessConfigFormat.Binary,
                    FileName = "Events.bytes"
                };
            }

            try
            {
                var text = File.ReadAllText(manifestPath);
                var manifest = JsonMapper.ToObject<ProcessConfigManifest>(text);
                if (manifest == null || string.IsNullOrEmpty(manifest.FileName))
                {
                    return new ProcessConfigManifest
                    {
                        Format = ProcessConfigFormat.Binary,
                        FileName = "Events.bytes"
                    };
                }

                return manifest;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Load process manifest failed: {ex.Message}");
                return new ProcessConfigManifest
                {
                    Format = ProcessConfigFormat.Binary,
                    FileName = "Events.bytes"
                };
            }
        }
    }
}
