using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestVeeam.Logic
{
    public abstract class GZipBase
    {
        protected string InputPath, OutputPath;
        protected bool Sucessful;
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
        public abstract void Start();

    }
}
