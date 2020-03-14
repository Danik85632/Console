using System;
using System.Collections.Generic;
using System.Text;

namespace TestVeeam.Logic
{
    public class DataBlockModel
    {
        public int IdBlock { get; }
        public byte[] DataBuffer { get; }
        public byte[] CompressedDataBuffer { get; }

        public DataBlockModel(int idBlock, byte[] dataBuffer, byte[] compressedDataBuffer)
        {
            this.IdBlock = idBlock;
            this.DataBuffer = dataBuffer;
            this.CompressedDataBuffer = compressedDataBuffer;
        }
        public DataBlockModel(int idBlock, byte[] dataBuffer) : this(idBlock, dataBuffer, new byte[0])
        {

        }
    }
}
