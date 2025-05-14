using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    public class FrameAnimationView : VisualElement
    {
        private List<Texture2D> frames;
        private int currentFrame = 0;
        private int fps = 10;

        public FrameAnimationView(List<Texture2D> animationFrames, int framesPerSecond = 5)
        {
            frames = animationFrames;
            fps = framesPerSecond;

            style.width = 36;
            style.height = 36;

            if (frames != null && frames.Count > 0)
            {
                style.backgroundImage = new StyleBackground(frames[0]);

                // 每帧切换
                schedule.Execute(() =>
                {
                    currentFrame = (currentFrame + 1) % frames.Count;
                    style.backgroundImage = new StyleBackground(frames[currentFrame]);
                }).Every(1000 / fps);
            }
        }
    }
}