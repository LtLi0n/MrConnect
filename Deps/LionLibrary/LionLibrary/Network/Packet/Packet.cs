using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Linq;

namespace LionLibrary.Network
{
    public class Packet<T> : IPacketBase, IReplyablePacket
    {
        private readonly Packet _packet;

        public PacketType Type => _packet.Type;
        public long Id => _packet.Id;
        public StatusCode Status => _packet.Status;
        public string Header => _packet.Header;
        public byte ContentTypeValue => _packet.ContentTypeValue;
        public string Content => _packet.Content;

        public long ReplyId => _packet.ReplyId;

        public T ToEntity() => _packet.As<T>();
        public InheritedT ToEntity<InheritedT>() where InheritedT : T => _packet.As<InheritedT>();

        public IEnumerable<T> AsEnumerable() => _packet.AsEnumerable<T>();
        public IEnumerable<InheritedT> AsEnumerable<InheritedT>() where InheritedT : T => _packet.AsEnumerable<InheritedT>();

        public string this[string arg] => _packet[arg];

        public Packet(Packet packet)
        {
            _packet = packet;
        }
    }

    public class Packet : IPacketBase, IReplyablePacket
    {
        public PacketType Type { get; private set; }
        ///<summary>Ids should be unique and increase incrementally for each packet.</summary>
        public long Id { get; private set; }
        public long ReplyId { get; private set; }
        public bool IsReply => ReplyId != -1;

        public StatusCode Status { get; private set; }

        ///<summary>[ModuleName].[CommandName]</summary>
        public string Header { get; private set; }
        public string Module => Header.Split('.')[0];
        public string Command => Header.Split('.')[1];

        ///<summary>Args passed to the command. E.g. ping 127.0.0.1</summary>
        public IReadOnlyDictionary<string, string> Args { get; private set; }

        ///<summary>Hints what kind of content to expect. E.g. JSON, XML, EDI.</summary>
        public PacketContentType ContentType { get => (PacketContentType)ContentTypeValue; set => ContentTypeValue = (byte)value; }
        public byte ContentTypeValue { get; set; }
        ///<summary>Content encoded in UTF8 to be sent. Feel free to make a version of your own inherited class that can decode content into JSON or XML.</summary>
        public string Content { get; private set; }

        public ReadOnlyCollection<byte> ContentBinary { get; private set; }
        protected byte[] _contentBinary;

        ///<summary>Packet reading</summary>
        public Packet(Stream stream)
        {
            PacketReader pr = new PacketReader(stream);

            InitAsBase(pr);
            Args = new ReadOnlyDictionary<string, string>(pr.Args);
            _contentBinary = pr._contentBinary;
            ContentBinary = new ReadOnlyCollection<byte>(_contentBinary);
        }

        public string this[string arg] => Args.ContainsKey(arg)? Args[arg] : string.Empty;
        public T As<T>()
        {
            if(ContentType == PacketContentType.JSON)
            {
                var token = JToken.Parse(Content);

                return token switch
                {
                    JArray _ => token.ToObject<IEnumerable<T>>().FirstOrDefault(),
                    JObject _ => token.ToObject<T>(),
                    _ => throw new ArgumentException("Provided string was not recognized as parsable json."),
                };
            }
            else
            {
                return default;
            }
        }

        public IEnumerable<T> AsEnumerable<T>()
        {
            if (ContentType == PacketContentType.JSON)
            {
                var array = JArray.Parse(Content);
                return array.ToObject<IEnumerable<T>>();
            }
            else
            {
                return Enumerable.Empty<T>();
            }
        }

        public Packet(PacketBuilder pb) => Init(pb);

        protected Packet() { }

        protected void Init(PacketBuilder pb)
        {
            InitAsBase(pb);
            Args = pb.Args;
            _contentBinary = pb.ContentBinary;
            ContentBinary = new ReadOnlyCollection<byte>(_contentBinary);
        }

        private void InitAsBase<T>(T ipb)
            where T : IPacketBase, IReplyablePacket
        {
            Id = ipb.Id;
            ReplyId = ipb.ReplyId;
            Status = ipb.Status;
            Header = ipb.Header;
            ContentTypeValue = ipb.ContentTypeValue;
            Content = ipb.Content;
        }

        ///<summary>Builds bytes ready to sent across the world!</summary>
        public byte[] Build()
        {
            using (MemoryStream ms = new MemoryStream())
            {

                using (BinaryWriter bw = new BinaryWriter(ms))
                {

                    bw.Write(0); //reserve for size value to prevent overwriting
                    bw.Write((byte)Type);
                    bw.Write(Id);
                    bw.Write(ReplyId);
                    bw.Write((int)Status);

                    //header
                    bw.Write((ushort)Encoding.UTF8.GetByteCount(Header));
                    bw.Write(Encoding.UTF8.GetBytes(Header));

                    //args
                    bw.Write((byte)Args.Count);
                    foreach (var arg in Args)
                    {
                        //arg:name
                        bw.Write((byte)Encoding.UTF8.GetByteCount(arg.Key));
                        bw.Write(Encoding.UTF8.GetBytes(arg.Key));

                        //arg:value
                        bw.Write((ushort)Encoding.UTF8.GetByteCount(arg.Value));
                        bw.Write(Encoding.UTF8.GetBytes(arg.Value));
                    }

                    bw.Write(ContentTypeValue);

                    //content
                    bw.Write(Encoding.UTF8.GetByteCount(Content));
                    bw.Write(Encoding.UTF8.GetBytes(Content));

                    //content_binary
                    bw.Write(_contentBinary.Length);
                    bw.Write(_contentBinary);

                    //packet size
                    bw.Seek(0, SeekOrigin.Begin);
                    bw.Write((int)bw.BaseStream.Length);

                    return ms.ToArray();
                }
            }
        }

        public override string ToString() => Content;
    }
}
