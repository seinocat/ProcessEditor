/*** 工具自动生成 => Tools/ProcessEditor/GenerateIOExtension ***/
using System.IO;
using Seino.Utils.FastFileReader;

namespace Process.Runtime
{
    public static class GenerateIOExtension
    {
        public static void Write(this BinaryWriter writer, TestClass value)
        {
            writer.Write(value.Value1);
            writer.Write(value.Value2);
        }

        public static TestClass ReadTestClass(this BinaryReader reader)
        {
            var value = new TestClass();
            value.Value1 = reader.ReadInt32();
            value.Value2 = reader.ReadString();
            return value;
        }

    }
}
