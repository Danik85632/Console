using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestVeeam.Logic
{
    public abstract class GZipBase
    {
        protected string InputPath, OutputPath;
        protected bool Sucessful, Cancel;
        protected static readonly int ProcessCount = Environment.ProcessorCount;
        protected ManualResetEvent[] manualResetEvents = new ManualResetEvent[ProcessCount];
        protected static readonly int ByteSize = 1000000; //1mb
        protected ProducerConsumer queueFromReader = new ProducerConsumer();
        protected ProducerConsumer queueFromWriter = new ProducerConsumer();
        public GZipBase() { }
        public GZipBase(string input, string output) 
        {
            InputPath = input;
            OutputPath = output;
        }
        public int GetResult()
        {            
            return Sucessful ? 0 : 1;
        }
        public abstract void Start();

    }
}
