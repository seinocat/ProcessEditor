using System;
using UnityEngine;

namespace Process.Runtime
{
    /// <summary>
    /// 流程状态
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>
        /// 准备中
        /// </summary>
        Preparing,
        /// <summary>
        /// 准备完成
        /// </summary>
        Ready,
        /// <summary>
        /// 进行中
        /// </summary>
        Running,
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 失败继续
        /// </summary>
        FailedSkip,
        /// <summary>
        /// 失败中止
        /// </summary>
        FailedBreak,
    }
    
    [Serializable]
    public enum OpenUIType
    {
        [InspectorName("卡牌展示")]
        CardShow,
        [InspectorName("核弹危机功能开启")]
        NuclearCrisisOpenPanel,
    }

    [Serializable]
    public enum HudType
    {
        [InspectorName("Emoji表情")]
        Emoji,
        [InspectorName("气泡")]
        Bubble,
    }

    [Serializable]
    public enum eFadeType : byte
    {
        [InspectorName("渐入")]
        FadeIn,
        [InspectorName("渐出")]
        FadeOut
    }

    [Serializable]
    public enum eTimelineWrapType
    {
        Hold,
        Loop,
        None,
        Auto
    }
    
    [Serializable]
    public enum eTimelineRecycleType
    {
        [InspectorName("播放完成时回收")]
        PlayFinish,
        [InspectorName("流程结束时回收")]
        ProcessEnd,
    }

    [Serializable]
    public enum eActiveType
    {
        [InspectorName("显示")]
        Show,
        [InspectorName("隐藏")]
        Hide
    }

    [Serializable]
    public enum eRecordeType
    {
        [InspectorName("记录")]
        Write,
        [InspectorName("读取")]
        Read
    }
    
    [Serializable]
    public enum eEnableType
    {
        [InspectorName("启用")]
        Enable,
        [InspectorName("禁用")]
        Disable
    }
    
    [Serializable]
    public enum eBehaviorType
    {
        [InspectorName("动态")]
        Dynamic,
        [InspectorName("静态")]
        Static
    }
}