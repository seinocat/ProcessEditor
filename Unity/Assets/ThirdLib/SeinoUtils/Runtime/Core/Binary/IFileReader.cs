using System.IO;
using System.Threading.Tasks;

namespace Seino.Utils.FastFileReader
{
    /// <summary>
    /// 读取
    /// </summary>
    public interface IFileReader
    {
        public Task ReadAsync(BinaryReader reader);

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