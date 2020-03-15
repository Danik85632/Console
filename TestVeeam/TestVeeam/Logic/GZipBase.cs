using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace TestVeeam.Logic
{
    public abstract class GZipBase
    {
        protected string InputPath, OutputPath;
        protected bool Sucessful,Cancel;
        protected static readonly int ProcessCount = Environment.ProcessorCount;
        protected readonly int ByteSize = 1000000; //1mb
        protected ManualResetEvent[] manualResetEvents = new ManualResetEvent[ProcessCount];
        protected ProducerConsumer queueFromReader = new ProducerConsumer();
        protected ProducerConsumer queueFromWriter = new ProducerConsumer();


        public GZipBase(string input, string output)
        {
            InputPath = input;
            OutputPath = output;
        }
        public int GetResult()
        {
            return Sucessful ? 0 : 1;
        }
        public void Start()
        {
            Console.WriteLine(GetNameClass());
            var threads = new[] { new Thread(Read), new Thread(Write) };
            foreach (var t in threads)
                t.Start();
            for (int i = 0; i < ProcessCount; i++)
            {
                manualResetEvents[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(CompressDecompress, i);
            }
            WaitHandle.WaitAll(manualResetEvents);
            threads[1].Join();

            Sucessful = !Cancel;
        }
        protected abstract string GetNameClass();
        private void Read()
        {
            using (var fileInput = new FileStream(InputPath, FileMode.Open))
            {
                ReadCompressOrDecompress(fileInput);
            }
            queueFromReader.Stop();
        }
        protected abstract void ReadCompressOrDecompress(FileStream fileInput);
        private void Write()
        {
            try
            {
                using (var fileOutput = new FileStream(GetPath(), FileMode.Append))
                {
                    while (true && !Cancel)
                    {
                        var buffer = queueFromWriter.Dequeue();

                        if (buffer == null)
                            return;

                        GetBytes(buffer);
                        fileOutput.Write(buffer.DataBuffer, 0, buffer.DataBuffer.Length);
                    }
                }
            }
            catch (IOException)
            {
                Cancel = true;
                ProgressBar.StopProgessBarAndWriteConsole(ConsoleColor.Black, "Not enough disk space to complete the operation.");
            }
        }
        protected abstract string GetPath();
        protected virtual void GetBytes(DataBlockModel buffer) { }
        private void CompressDecompress(object i) 
        {
            while (true && !Cancel)
            {
                var buffer = queueFromReader.Dequeue();

                if (buffer == null)
                    return;
                CompressOrDecompressLogic(i,buffer);
            }
        }
        protected abstract void CompressOrDecompressLogic(object i, DataBlockModel buffer);
    }
}
