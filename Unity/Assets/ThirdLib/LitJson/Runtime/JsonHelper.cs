namespace LitJson
{
    public class JsonHelper
    {
        public static T FromJson<T>(string jsonStr)
        {
            return JsonMapper.ToObject<T>(jsonStr);
        }

        /// <summary>
        /// 转换成Json，转成一行文本；
        /// 不利于调试，但是体积最小；
        /// </summary>
        /// <param name="data"></param>
        /// <param name="write_property">是否需要序列化属性（建议维持false）</param>
        /// <param name="json_ignore">是否启用JsonIgnore特征</param>
        /// <returns></returns>
        public static string ToJson(object data, bool write_property = false, bool json_ignore = false)
        {
            return JsonMapper.ToJson(data, write_property, json_ignore);
        }

        /// <summary>
        /// 转换成Json，转换结果会交错排列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="write_property">是否需要序列化属性（建议维持false）</param>
        /// <returns></returns>
        public static string ToJsonPretty(object data, bool write_property = false)
        {
            return JsonMapper.ToJson(data, write_property, true);
        }
    }
}