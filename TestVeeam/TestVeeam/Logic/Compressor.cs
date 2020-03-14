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
        public override void Start()
        {
            Console.WriteLine("Compress");

            var threads = new[] { new Thread(Read), new Thread(Write) };
            foreach (var t in threads)
                t.Start();
            for (int i = 0; i < ProcessCount; i++)
            {
                manualResetEvents[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(Compress, i);
            }
            WaitHandle.WaitAll(manualResetEvents);
            
            Sucessful = true;
        }
        private void Read()
        {
            try
            {
                using (FileStream fileInput = new FileStream(InputPath, FileMode.Open))
                {
                    byte[] buffer;
                    while (fileInput.Position < fileInput.Length) 
                    {
                        int byteSize = fileInput.Length - fileInput.Position <= ByteSize ? 
                            (int)(fileInput.Length - fileInput.Position) : ByteSize;
                        buffer = new byte[byteSize];
                        fileInput.Read(buffer, 0, byteSize);
                        queueFromReader.addDataToBuffer(buffer);
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

        private void Compress(object i)
        {
            try
            {
                while (true)
                {
                    DataBlockModel buffer = queueFromReader.Dequeue();

                    if (buffer == null)
                        return;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (GZipStream gZipStream = new GZipStream(memoryStream,
                            CompressionMode.Compress, true))
                        {
                            gZipStream.Write(buffer.DataBuffer, 0, buffer.DataBuffer.Length);
                        }
                        byte[] compressedData = memoryStream.ToArray();
                        DataBlockModel compressBuffer = new DataBlockModel(buffer.IdBlock, compressedData);
                        queueFromWriter.AddDataToWriting(compressBuffer);
                    }
                    ManualResetEvent doneManualResetEvents = manualResetEvents[(int)i];
                    doneManualResetEvents.Set();
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
                using (FileStream fileOutput = new FileStream(OutputPath + ".gz", FileMode.Append))
                {
                    while (true)
                    {
                        DataBlockModel compressBuffer = queueFromWriter.Dequeue();

                        if (compressBuffer == null)
                            return;

                        BitConverter.GetBytes(compressBuffer.DataBuffer.Length).CopyTo(compressBuffer.DataBuffer, 4);
                        fileOutput.Write(compressBuffer.DataBuffer, 0, compressBuffer.DataBuffer.Length);
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
