using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SendBuffer
    {
        byte[] _buffer;
        int _usedSize = 0;

        public int FreeSize { get => _buffer.Length - _usedSize; }
        public ArraySegment<byte> Open(int reserveSize)  // reserveSize : 예상하는 최대크기
        {
            if (reserveSize > FreeSize)
                return null;
            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
        }
        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return segment;
        }
    }
}
