using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace TestVeeam.Logic
{
    class Decompressor : GZipBase
    {
        public Decompressor(string inputPath, string outputPath) : base(inputPath, outputPath)
        {
        }
        protected override string GetNameClass() => "Decompressing.....";
        protected override string GetPath() => InputPath.Remove(InputPath.Length - 3);
        protected override void ReadCompressOrDecompress(FileStream fileInput, ProgressBar pb)
        {
            int inkrement = 0;
            while (fileInput.Position < fileInput.Length && !Cancel)
            {
                byte[] lengthBuffer = new byte[8];
                fileInput.Read(lengthBuffer, 0, lengthBuffer.Length);
                int blockLength = BitConverter.ToInt32(lengthBuffer, 4);
                byte[] compressedData = new byte[blockLength];
                fileInput.Read(compressedData, 8, blockLength - 8);
                lengthBuffer.CopyTo(compressedData, 0);

                var compressBuffer = new DataBlockModel(inkrement, new byte[ByteSize], compressedData);
                QueueFromReader.AddDataToWriting(compressBuffer);
                inkrement++;
                pb.drawTextProgressBar((int)fileInput.Position);
            }
        }
        protected override void CompressOrDecompressLogic(object i,DataBlockModel buffer)
        {
            using (var memoryStream = new MemoryStream(buffer.CompressedDataBuffer))
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer.DataBuffer, 0, buffer.DataBuffer.Length);
                    var block = new DataBlockModel(buffer.IdBlock, buffer.DataBuffer.ToArray());
                    queueFromWriter.AddDataToWriting(block);
                }
            }
        }
    }
}
