using System;
using System.Text;

namespace PacketStream
{
    public class PackStream //copied from original code
    {
        public PackStream()
        {
            pktBuffer = new byte[8192];
            Length = 0;
            Seek = 0;
        }

        public PackStream(int length)
        {
            pktBuffer = new byte[length];
            Length = 0;
            Seek = 0;
        }

        public PackStream(byte[] pktBuffer)
        {
            if (pktBuffer == null || pktBuffer.Length == 0)
            {
                throw new Exception("buf 不能为 null。");
            }

            Length = 0;
            Seek = 0;
            this.pktBuffer = pktBuffer;
        }

        public override string ToString()
        {
            byte[] array = new byte[Length];
            Array.Copy(pktBuffer, array, Length);
            return Encoding.Unicode.GetString(array);
        }

        public int Seek { get; set; }

        public int Length { get; private set; }

        public void Reset()
        {
            Length = 0;
            Seek = 0;
        }

        private void Write(byte x)
        {
            pktBuffer[Seek++] = x;
            if (Seek > Length)
            {
                Length = Seek;
            }
        }

        private void Write(short x)
        {
            pktBuffer[Seek++] = (byte)(x >> 8);
            pktBuffer[Seek++] = (byte)x;
            if (Seek > Length)
            {
                Length = Seek;
            }
        }

        private void Write(int x)
        {
            pktBuffer[Seek++] = (byte)(x >> 24);
            pktBuffer[Seek++] = (byte)(x >> 16);
            pktBuffer[Seek++] = (byte)(x >> 8);
            pktBuffer[Seek++] = (byte)x;
            if (Seek > Length)
            {
                Length = Seek;
            }
        }

        private void Write(uint x)
        {
            while ((x & 18446744073709551488UL) != 0UL)
            {
                pktBuffer[Seek++] = (byte)((x & 127u) | 128u);
                x >>= 7;
            }

            pktBuffer[Seek++] = (byte)x;
            if (Seek > Length)
            {
                Length = Seek;
            }
        }

        private void Write(long x)
        {
            pktBuffer[Seek++] = (byte)(x >> 56);
            pktBuffer[Seek++] = (byte)(x >> 48);
            pktBuffer[Seek++] = (byte)(x >> 40);
            pktBuffer[Seek++] = (byte)(x >> 32);
            pktBuffer[Seek++] = (byte)(x >> 24);
            pktBuffer[Seek++] = (byte)(x >> 16);
            pktBuffer[Seek++] = (byte)(x >> 8);
            pktBuffer[Seek++] = (byte)x;
            if (Seek > Length)
            {
                Length = Seek;
            }
        }

        private void Write(ulong x)
        {
            while ((x & 18446744073709551488UL) != 0UL)
            {
                pktBuffer[Seek++] = (byte)((x & 127UL) | 128UL);
                x >>= 7;
            }

            pktBuffer[Seek++] = (byte)x;
            if (Seek > Length)
            {
                Length = Seek;
            }
        }

        private void Write(byte[] bs, int offset, int len)
        {
            Array.Copy(bs, offset, pktBuffer, Seek, len);
            Seek += len;
            if (Seek > Length)
            {
                Length = Seek;
            }
        }

        public void WriteInt8(int x)
        {
            if (x > 127 || x < -128)
            {
                throw new Exception(string.Format("Int8的有限范围为[{0:d},{1:d}]。", -127, 128));
            }

            Write((byte)x);
        }

        public void WriteUint8(uint x)
        {
            if (x > 255u)
            {
                throw new Exception(string.Format("Uint8的有限范围为[{0:d},{1:d}]。", 0, 255));
            }

            Write((byte)x);
        }

        public void WriteInt16(int x)
        {
            if (x > 32767 || x < -32768)
            {
                throw new Exception(string.Format("int16的有限范围为[{0:d},{1:d}]。", -32768, 32767));
            }

            Write((short)x);
        }

        public void WriteUint16(uint x)
        {
            if (x > 65535u)
            {
                throw new Exception(string.Format("Uint16的有限范围为[{0:d},{1:d}]。", 0, 65535L));
            }

            Write((short)x);
        }

        public void WriteInt32(int x)
        {
            Write(x);
        }

        public void WriteUint32(uint x)
        {
            Write(x);
        }

        public void WriteInt64(long x)
        {
            Write(x);
        }

        public void WriteUint64(ulong x)
        {
            Write(x);
        }

        public void WriteBool(bool x)
        {
            if (x)
            {
                WriteInt8(1);
            }
            else
            {
                WriteInt8(0);
            }
        }

        public void WriteFloat(float x)
        {
            byte[] bytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian)
            {
                Write(bytes[3]);
                Write(bytes[2]);
                Write(bytes[1]);
                Write(bytes[0]);
            }
            else
            {
                Write(bytes[0]);
                Write(bytes[1]);
                Write(bytes[2]);
                Write(bytes[3]);
            }
        }

        public void WriteDouble(double x)
        {
            byte[] bytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian)
            {
                Write(bytes[7]);
                Write(bytes[6]);
                Write(bytes[5]);
                Write(bytes[4]);
                Write(bytes[3]);
                Write(bytes[2]);
                Write(bytes[1]);
                Write(bytes[0]);
            }
            else
            {
                Write(bytes[0]);
                Write(bytes[1]);
                Write(bytes[2]);
                Write(bytes[3]);
                Write(bytes[4]);
                Write(bytes[5]);
                Write(bytes[6]);
                Write(bytes[7]);
            }
        }

        public void WriteString(string x)
        {
            if (x == null)
            {
                WriteBuffer(null);
            }
            else
            {
                WriteBuffer(Encoding.UTF8.GetBytes(x));
            }
        }

        public void WriteString2(string x)
        {
            if (x == null)
            {
                WriteBuffer2(null);
            }
            else
            {
                WriteBuffer2(Encoding.UTF8.GetBytes(x));
            }
        }

        public void WriteString4(string x)
        {
            if (x == null)
            {
                WriteBuffer4(null);
            }
            else
            {
                WriteBuffer4(Encoding.UTF8.GetBytes(x));
            }
        }

        public void WriteBuffer(byte[] bs)
        {
            if (bs == null)
            {
                WriteInt8(0);
                return;
            }

            if (bs.Length != 0)
            {
                Write(bs, 0, bs.Length);
            }
            else
            {
                WriteInt8(1);
            }
        }

        public void WriteBuffer2(byte[] bs)
        {
            if (bs == null)
            {
                WriteUint16(0u);
                WriteInt8(0);
                return;
            }

            if (bs.Length > 65535)
            {
                throw new Exception("发送字符串失败（字符串长度超过255个字节）。");
            }

            WriteUint16((uint)bs.Length);
            if (bs.Length != 0)
            {
                Write(bs, 0, bs.Length);
            }
            else
            {
                WriteInt8(1);
            }
        }

        public void WriteBuffer4(byte[] bs)
        {
            if (bs == null)
            {
                WriteUint32(0u);
                WriteInt8(0);
                return;
            }

            WriteUint32((uint)bs.Length);
            if (bs.Length != 0)
            {
                Write(bs, 0, bs.Length);
            }
            else
            {
                WriteInt8(1);
            }
        }

        public byte[] ToArray()
        {
            if (Length == 0)
            {
                return null;
            }

            byte[] array = new byte[Length];
            Array.Copy(pktBuffer, 0, array, 0, Length);
            Seek = 0;
            Length = 0;
            return array;
        }

        public void ToArrayRef(out byte[] _bs, out int _offset, out int _count)
        {
            _bs = pktBuffer;
            _offset = 0;
            _count = Length;
            Seek = 0;
            Length = 0;
        }

        // Token: 0x06004231 RID: 16945 RVA: 0x0001DA63 File Offset: 0x0001BC63
        public static int ComputeUint32Size(uint value)
        {
            if ((value & 4294967168u) == 0u)
            {
                return 1;
            }

            if ((value & 4294950912u) == 0u)
            {
                return 2;
            }

            if ((value & 4292870144u) == 0u)
            {
                return 3;
            }

            if ((value & 4026531840u) == 0u)
            {
                return 4;
            }

            return 5;
        }

        public static int computeUint64Size(ulong value)
        {
            if ((value & 18446744073709551488UL) == 0UL)
            {
                return 1;
            }

            if ((value & 18446744073709535232UL) == 0UL)
            {
                return 2;
            }

            if ((value & 18446744073707454464UL) == 0UL)
            {
                return 3;
            }

            if ((value & 18446744073441116160UL) == 0UL)
            {
                return 4;
            }

            if ((value & 18446744039349813248UL) == 0UL)
            {
                return 5;
            }

            if ((value & 18446739675663040512UL) == 0UL)
            {
                return 6;
            }

            if ((value & 18446181123756130304UL) == 0UL)
            {
                return 7;
            }

            if ((value & 18374686479671623680UL) == 0UL)
            {
                return 8;
            }

            if ((value & 9223372036854775808UL) == 0UL)
            {
                return 9;
            }

            return 10;
        }

        private readonly byte[] pktBuffer;
    }
}