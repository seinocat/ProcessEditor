using System.IO;
using System.Threading.Tasks;

namespace Seino.Utils.FastFileReader
{
    /// <summary>
    /// 写入
    /// </summary>
    public interface IFileWriter
    {
        public Task WriteAsync(BinaryWriter writer);

        /// <summary>
        /// 进度，0~100
        /// </summary>
        public float Progress { get; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete { get; }
    }
}
