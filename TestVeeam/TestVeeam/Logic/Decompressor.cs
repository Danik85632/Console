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
        public override void Start()
        {
            Console.WriteLine("Decompress");

            var threads = new[] { new Thread(Read), new Thread(Write) };
            foreach (var t in threads)
                t.Start();
            for (int i = 0; i < ProcessCount; i++)
            {
                manualResetEvents[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(Decompress, i);
            }
            WaitHandle.WaitAll(manualResetEvents);

            Sucessful = true;
        }
        private void Read()
        {
            try
            {
                int inkrement = 0;
                using (FileStream fileInput = new FileStream(InputPath, FileMode.Open))
                {
                    while (fileInput.Position < fileInput.Length)
                    {
                        byte[] lengthBuffer = new byte[8];
                        fileInput.Read(lengthBuffer, 0, lengthBuffer.Length);
                        int blockLength = BitConverter.ToInt32(lengthBuffer, 4);
                        byte[] compressedData = new byte[blockLength];
                        fileInput.Read(compressedData, 8, blockLength - 8);
                        lengthBuffer.CopyTo(compressedData, 0);

                        DataBlockModel compressBuffer = new DataBlockModel(inkrement, new byte[ByteSize], compressedData);
                        queueFromReader.AddDataToWriting(compressBuffer);
                        inkrement++;
                        ProgressBar.drawTextProgressBar((int)fileInput.Position, (int)fileInput.Length);
                    }
                }
                queueFromReader.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Decompress(object i)
        {
            try
            {
                while (true)
                {
                    DataBlockModel compressBuffer = queueFromReader.Dequeue();

                    if (compressBuffer == null)
                        return;

                    using (MemoryStream memoryStream = new MemoryStream(compressBuffer.CompressedDataBuffer))
                    {
                        using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                        {
                            gZipStream.Read(compressBuffer.DataBuffer, 0, compressBuffer.DataBuffer.Length);
                            DataBlockModel block = new DataBlockModel(compressBuffer.IdBlock, compressBuffer.DataBuffer.ToArray());
                            queueFromWriter.AddDataToWriting(block);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Write()
        {
            try
            {
                using (FileStream fileOutput = new FileStream(InputPath.Remove(InputPath.Length-3), FileMode.Append))
                {
                    while (true)
                    {
                        DataBlockModel decompressedBuffer = queueFromWriter.Dequeue();

                        if (decompressedBuffer == null)
                            return;

                        fileOutput.Write(decompressedBuffer.DataBuffer, 0, decompressedBuffer.DataBuffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
