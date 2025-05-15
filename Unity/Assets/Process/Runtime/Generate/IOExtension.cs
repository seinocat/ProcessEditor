/*** 工具自动生成 => Tools/ProcessEditor/GenerateIOExtension ***/
using System.IO;
using System.Collections.Generic;
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

        public static void Write(this BinaryWriter writer, BranchData value)
        {
            writer.Write(value.Conditions.Count);
            foreach (var element in value.Conditions)
            {
                writer.Write(element);
            }
            writer.Write(value.NextID);
        }

        public static BranchData ReadBranchData(this BinaryReader reader)
        {
            var value = new BranchData();
            var ConditionsCount = reader.ReadInt32();
            value.Conditions = new List<int>();
            for(int i = 0; i < ConditionsCount; i++)
            {
                value.Conditions.Add(reader.ReadInt32());
            }
            value.NextID = reader.ReadInt32();
            return value;
        }
    }
}
