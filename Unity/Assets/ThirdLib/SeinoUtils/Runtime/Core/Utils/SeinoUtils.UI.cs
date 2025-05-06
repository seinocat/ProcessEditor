using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Seino.Utils
{
    public static partial class SeinoUtils
    {
        /// <summary>
        /// 世界坐标转换UGUI坐标
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="position"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2 WorldToUguiPoint(this Camera camera, Vector3 position, Rect rect)
        {
            return WorldToUguiPoint(camera, position, rect, Vector2.zero);
        }
        
        /// <summary>
        /// 世界坐标转换UGUI坐标
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="position"></param>
        /// <param name="rect"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Vector2 WorldToUguiPoint(this Camera camera, Vector3 position, Rect rect, Vector2 offset)
        {
            Vector3 viewPos = camera.WorldToViewportPoint(position);
            Vector2 uiPos   = new Vector2(viewPos.x * rect.width, viewPos.y * rect.height) + offset;
            return uiPos;
        }

        /// <summary>
        /// 屏幕坐标转UI坐标
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="rect"></param>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        public static Vector2 ScreenToUIPos(this Camera camera, RectTransform rect, Vector2 screenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos,
                camera, out Vector2 uiPos);
            return uiPos;
        }
        
        /// <summary>
        /// 十六进制转Color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
        
        /// <summary>
        /// 解析富文本(不支持多重标签)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<RichTextData> ParseRichText(ref string text)
        {
            List<RichTextData> richTextDatas = new List<RichTextData>();
            Regex regex  = new Regex("<(color|size|b|i|u)(=[^>]*)?>(.*?)</\\1>");

            var matchs = regex.Matches(text);
            foreach (Match match in matchs)
            {
                if (match.Success)
                {
                    RichTextData data   = new RichTextData();
                    data.Index          = text.IndexOf(match.Groups[0].Value, StringComparison.Ordinal);
                    data.TagName        = match.Groups[1].Value;
                    data.Value          = match.Groups[2].Value;
                    data.Length         = match.Groups[3].Length;
                    text                = text.Replace(match.Groups[0].Value, match.Groups[3].Value);
                    
                    richTextDatas.Add(data);
                }
            }
            
            return richTextDatas;
        }

        /// <summary>
        /// 查找富文本标签索引
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static RichTextData FindRichText(this List<RichTextData> datas, int index)
        {
            if (datas == null)
                return null;
            
            foreach (var data in datas)
            {
                if (index >= data.Index && index < data.Index + data.Length)
                    return data;
            }

            return null;
        }
        
    }
}