using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Seino.Utils.FastFileReader;
using UnityEngine;

namespace Process.Runtime
{
    public class ProcessSystem
    {
        private Dictionary<ulong, ProcessConfig> Configs;

        public async UniTask LoadConfigs()
        {
            ProcessConfigLoader configLoader = new ProcessConfigLoader();
            await FastFileUtils.ReadFileByBinaryAsync($"{Application.streamingAssetsPath}/Events.bytes", configLoader);
            Configs = configLoader.Configs;
        }
        
        /// <summary>
        /// 创建流程实例
        /// </summary>
        /// <param name="res"></param>
        /// <param name="processId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public GameProcess CreateProcess(ulong processId, Action<ProcessStatus> callback)
        {
            //先找配置
            Configs.TryGetValue(processId, out var config);
            if (config == null)
                return null;

            //创建流程实例
            var process = new GameProcess();
            process.Initialize(config, callback);
            
        
            return process;
        }
        
        
    }
}