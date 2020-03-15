using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace TestVeeam.Logic
{
    public class Compressor : GZipBase
    {
        public Compressor(string inputPath, string outputPath) : base(inputPath, outputPath)
        { 

        }
        protected override string GetNameClass() => "Compressing.....";
        protected override string GetPath() => OutputPath + ".gz";
        protected override void GetBytes(DataBlockModel buffer)
        {
            BitConverter.GetBytes(buffer.DataBuffer.Length).CopyTo(buffer.DataBuffer, 4);
        }
        protected override void ReadCompressOrDecompress(FileStream fileInput, ProgressBar pb) 
        {
            byte[] buffer;
            while (fileInput.Position < fileInput.Length && !Cancel)
            {
                int byteSize = fileInput.Length - fileInput.Position <= ByteSize ?
                    (int)(fileInput.Length - fileInput.Position) : ByteSize;
                buffer = new byte[byteSize];
                fileInput.Read(buffer, 0, byteSize);
                queueFromReader.addDataToBuffer(buffer);
                pb.drawTextProgressBar((int)fileInput.Position);
            }
        }
        protected override void CompressOrDecompressLogic(object i, DataBlockModel buffer)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer.DataBuffer, 0, buffer.DataBuffer.Length);
                }
                byte[] compressedData = memoryStream.ToArray();
                var compressBuffer = new DataBlockModel(buffer.IdBlock, compressedData);
                queueFromWriter.AddDataToWriting(compressBuffer);
            }
            var doneManualResetEvents = manualResetEvents[(int)i];
            doneManualResetEvents.Set();
        }
    }
}
