using System;
using System.Collections.Generic;
using System.Text;

namespace TestVeeam.Logic
{
    public class DataBlockModel
    {
        private int idBlock;
        public int IdBlock { get { return idBlock; } }
        private byte[] dataBuffer;
        public byte[] DataBuffer { get { return dataBuffer; } }
        private byte[] compressedDataBuffer;
        public byte[] CompressedDataBuffer { get { return compressedDataBuffer; } }

        public DataBlockModel(int idBlock, byte[] dataBuffer, byte[] compressedDataBuffer)
        {
            this.idBlock = idBlock;
            this.dataBuffer = dataBuffer;
            this.compressedDataBuffer = compressedDataBuffer;
        }
        public DataBlockModel(int idBlock, byte[] dataBuffer) : this(idBlock, dataBuffer, new byte[0])
        {

        }
    }
}
