using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SendBufferHelper
    {
        // Thread 간의 경합을 없애기 위해 ThreadLocal을 사용함
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        // Thread마다 자신의 Chunk를 할당하여 계속 쪼개서 쓴다 
        public static int ChunkSize { get; set; } = 4096 * 100;

        // Wrapping
        public static ArraySegment<byte> Open(int reserveSize)  
        {
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            if (CurrentBuffer.Value.FreeSize < reserveSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }

        // Wrapping
        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }

    public class SendBuffer
    {
        // _buffer는 다수의 thread가 참조할 수 있다
        // 하지만 문제가 되지 않는 이유는 정보를 읽기만 하고 수정하지 않기 때문
        // 실제로 수정은 open, close에서 처음 한번만 한다
        byte[] _buffer;  
        int _usedSize = 0;

        public int FreeSize { get => _buffer.Length - _usedSize; }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];  
        }
        
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
