using System;
using System.IO;
using System.Text;

namespace LionLibrary.Extensions
{
    public static class StreamExtensions
    {
        public static byte _ReadByte(this Stream stream) => (byte)stream.ReadByte();

        public static byte[] ReadBytes(this Stream stream, int n)
        {
            byte[] buffer = new byte[n];
            if (stream.Read(buffer, 0, n) == n) return buffer;
            else throw new NullReferenceException("Read less bytes than were expected.");
        }

        public static ushort ReadUInt16(this Stream stream) => BitConverter.ToUInt16(ReadBytes(stream, 2), 0);
        public static short ReadInt16(this Stream stream) => BitConverter.ToInt16(ReadBytes(stream, 2), 0);

        public static uint ReadUInt32(this Stream stream) => BitConverter.ToUInt32(ReadBytes(stream, 4), 0);
        public static int ReadInt32(this Stream stream) => BitConverter.ToInt32(ReadBytes(stream, 4), 0);

        public static ulong ReadUInt64(this Stream stream) => BitConverter.ToUInt64(ReadBytes(stream, 8), 0);
        public static long ReadInt64(this Stream stream) => BitConverter.ToInt64(ReadBytes(stream, 8), 0);

        public static string ReadStringUTF8(this Stream stream, int n) => Encoding.UTF8.GetString(ReadBytes(stream, n));
    }
}
