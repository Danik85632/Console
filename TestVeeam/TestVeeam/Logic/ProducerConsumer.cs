using System;
using System.Collections.Generic;
using System.Threading;

namespace TestVeeam.Logic
{
    public class ProducerConsumer
    {
        private object locker = new object();
        private int idBlock = 0;
        private bool isDead = false;
        private Queue<DataBlockModel> queue = new Queue<DataBlockModel>();

        public void addDataToBuffer(byte[] buffer) 
        {
            if (buffer == null)
                throw new InvalidOperationException("buffer is null");
            lock (locker)
            {
                if (isDead)
                    throw new InvalidOperationException("Queue already stopped");

                DataBlockModel newBlock = new DataBlockModel(idBlock, buffer);
                queue.Enqueue(newBlock);
                idBlock++;
                Monitor.PulseAll(locker);
            }
        }
        public void AddDataToWriting(DataBlockModel dataBlock) 
        {
            lock (locker)
            {              
                while (dataBlock.IdBlock != idBlock)
                    Monitor.Wait(locker);

                queue.Enqueue(dataBlock);
                idBlock++;
                Monitor.PulseAll(locker);
            }
        }
        public DataBlockModel Dequeue()
        {
            lock (locker)
            {
                while (queue.Count == 0 && !isDead)
                    Monitor.Wait(locker);

                if (queue.Count == 0)
                    return null;

                return queue.Dequeue();

            }
        }
        public void Stop()
        {
            lock (locker)
            {
                isDead = true;
                Monitor.PulseAll(locker);
            }
        }
    }
}
