using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LionLibrary.Extensions;

namespace LionLibrary.Network
{
    internal struct PacketReader : IPacketBase, IReplyablePacket
    {
        public PacketType Type { get; }
        public long Id { get; }
        public long ReplyId { get; }
        public StatusCode Status { get; }
        public string Header { get; }
        public IDictionary<string, string> Args { get; }
        public byte ContentTypeValue { get; }
        public string Content { get; }
        internal byte[] _contentBinary;

        internal PacketReader(Stream stream)
        {
            Args = new Dictionary<string, string>();

            Type = (PacketType)stream.ReadByte();
            Id = stream.ReadInt64();
            ReplyId = stream.ReadInt64();
            Status = (StatusCode)stream.ReadInt32();
            //header
            ushort header_size = stream.ReadUInt16();
            Header = stream.ReadStringUTF8(header_size);
            //args
            byte args_count = stream._ReadByte();
            for (byte arg_i = 0; arg_i < args_count; arg_i++)
            {
                byte arg_name_length = stream._ReadByte();
                byte[] arg_name_data = stream.ReadBytes(arg_name_length);
                string arg_name = Encoding.UTF8.GetString(arg_name_data);

                ushort arg_value_length = stream.ReadUInt16();
                byte[] arg_value_data = stream.ReadBytes(arg_value_length);
                string arg_value = Encoding.UTF8.GetString(arg_value_data);

                Args.Add(arg_name, arg_value);
            }
            //content
            ContentTypeValue = (byte)stream.ReadByte();

            //content_regular
            int content_size = stream.ReadInt32();
            byte[] content_data = new byte[content_size];
            stream.Read(content_data, 0, content_size);
            Content = Encoding.UTF8.GetString(content_data);
            
            //content_binary
            int contentb_size = stream.ReadInt32();
            _contentBinary = new byte[contentb_size];
            stream.Read(_contentBinary, 0, contentb_size);
        }
    }
}
