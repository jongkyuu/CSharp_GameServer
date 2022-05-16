using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public  class RecvBuffer
    {
        ArraySegment<byte> _buffer; // 큰 바이트 배열에서 부분적으로 잘라서 쓸 수 있으므로 ArraySegment 사용
        int _readPos;
        int _writePos;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return _writePos - _readPos; } }
        public int FreeSize { get { return _buffer.Count - _writePos; } } // 버퍼에 남은 공간 

        public ArraySegment<byte> ReadSegment
        {
            get => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize);
        }
        public ArraySegment<byte> WriteSegment
        {
            get => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize);
        }
        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                // 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                // 남은 데이터가 있으면 시작 위치로 복사
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        // 성공적으로 처리했으면 OnRead 호출해서 read 커서를 이동
        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;
        }

        // client에서 데이터를 보내서 Recv를 했을때 write 커서를 이동 
        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;
            return true;
        }
    }
}
