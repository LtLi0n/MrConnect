using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LionLibrary.Network
{
    ///<summary>Regular packet builder</summary>
    public class PacketBuilder : IPacketBase, IReplyablePacket
    {
        public PacketType Type { get; } = PacketType.Regular;
        public long Id { get; set; } = 0;
        public long ReplyId { get; set; } = -1;
        public StatusCode Status { get; set; } = StatusCode.Success;
        public string Header { get; set; } = string.Empty;
        public byte ContentTypeValue { get; set; }
        public PacketContentType ContentType
        {
            get => (PacketContentType)ContentTypeValue;
            set => ContentTypeValue = (byte)value;
        } 
        public Dictionary<string, string> Args { get; }
        public string Content { get; set; } = string.Empty;
        public byte[] ContentBinary { get; set; }

        public PacketBuilder()
        {
            ContentType = PacketContentType.RAW;
            Args = new Dictionary<string, string>();
            ContentBinary = new byte[0];
        }

        public async Task AttachFileAsync(string path)
        {
            ContentBinary = await File.ReadAllBytesAsync(path);
        }

        private void SetValue(string arg, object value, bool add)
        {
            if(value != null)
            {
                if (!add) Args[arg] = value.ToString();
                else Args.Add(arg, value.ToString());
            }
        }

        private void SetValue(string arg, string value, bool add)
        {
            if (value != null)
            {
                if (!add) Args[arg] = value;
                else Args.Add(arg, value);
            }
        }

        private void SetValueNullable<T>(string arg, T? value, bool add) where T : struct
        {
            if (value.HasValue)
            {
                if (!add) Args[arg] = value.ToString();
                else Args.Add(arg, value.ToString());
            }
            else
            {
                if (!add) Args[arg] = "null";
                else Args.Add(arg, "null");
            }
        }

        public object this[string arg]
        {
            get => Args.ContainsKey(arg) ? Args[arg] : string.Empty;
            set => SetValue(arg, value, !Args.ContainsKey(arg));
        }

        public bool this[Arg<bool> arg]
        {
            get => Args.ContainsKey(arg.Name) ? bool.Parse(Args[arg.Name]) : false;
            set => SetValue(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        public bool? this[Arg<bool?> arg]
        {
            get => Args.ContainsKey(arg.Name) ? (Args[arg.Name] != "null" ? (bool?)bool.Parse(Args[arg.Name]) : null) : null;
            set => SetValueNullable(arg.Name, value, !Args.ContainsKey(arg.Name));
        }

        public string this[Arg<string> arg]
        {
            get => Args.ContainsKey(arg.Name) ? Args[arg.Name] : string.Empty;
            set => SetValue(arg.Name, value, !Args.ContainsKey(arg.Name));
        }

        public DateTime this[Arg<DateTime> arg]
        {
            get => Args.ContainsKey(arg.Name) ? DateTimeOffset.FromUnixTimeSeconds(long.Parse(Args[arg.Name])).LocalDateTime : default;
            set
            {
                if (value.Year < 1970) return; //ignore default or irrelevant DateTime
                SetValue(arg.Name, ((DateTimeOffset)value).ToUnixTimeSeconds(), !Args.ContainsKey(arg.Name));
            }
        }

        #region 8-bit
        public byte this[Arg<byte> arg]
        {
            get => Args.ContainsKey(arg.Name) ? byte.Parse(Args[arg.Name]) : (byte)0;
            set => SetValue(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        public byte? this[Arg<byte?> arg]
        {
            get => Args.ContainsKey(arg.Name) ? (Args[arg.Name] != "null" ? (byte?)byte.Parse(Args[arg.Name]) : null) : null;
            set => SetValueNullable(arg.Name, value, !Args.ContainsKey(arg.Name));
        }

        public sbyte this[Arg<sbyte> arg]
        {
            get => Args.ContainsKey(arg.Name) ? sbyte.Parse(Args[arg.Name]) : (sbyte)0;
            set => SetValue(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        public sbyte? this[Arg<sbyte?> arg]
        {
            get => Args.ContainsKey(arg.Name) ? (Args[arg.Name] != "null" ? (sbyte?)sbyte.Parse(Args[arg.Name]) : null) : null;
            set => SetValueNullable(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        #endregion
        #region 16-bit
        public ushort this[Arg<ushort> arg]
        {
            get => Args.ContainsKey(arg.Name) ? ushort.Parse(Args[arg.Name]) : (ushort)0;
            set => SetValue(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        public ushort? this[Arg<ushort?> arg]
        {
            get => Args.ContainsKey(arg.Name) ? (Args[arg.Name] != "null" ? (ushort?)ushort.Parse(Args[arg.Name]) : null) : null;
            set => SetValueNullable(arg.Name, value, !Args.ContainsKey(arg.Name));
        }

        public short this[Arg<short> arg]
        {
            get => Args.ContainsKey(arg.Name) ? short.Parse(Args[arg.Name]) : (short)0;
            set => SetValue(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        public short? this[Arg<short?> arg]
        {
            get => Args.ContainsKey(arg.Name) ? (Args[arg.Name] != "null" ? (short?)short.Parse(Args[arg.Name]) : null) : null;
            set => SetValueNullable(arg.Name, value, !Args.ContainsKey(arg.Name));
        }

        #endregion
        #region 32-bit
        public uint this[Arg<uint> arg]
        {
            get => Args.ContainsKey(arg.Name) ? uint.Parse(Args[arg.Name]) : 0;
            set => SetValue(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        public uint? this[Arg<uint?> arg]
        {
            get => Args.ContainsKey(arg.Name) ? (Args[arg.Name] != "null" ? (uint?)uint.Parse(Args[arg.Name]) : null) : null;
            set => SetValueNullable(arg.Name, value, !Args.ContainsKey(arg.Name));
        }

        public int this[Arg<int> arg]
        {
            get => Args.ContainsKey(arg.Name) ? int.Parse(Args[arg.Name]) : 0;
            set => SetValue(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        public int? this[Arg<int?> arg]
        {
            get => Args.ContainsKey(arg.Name) ? (Args[arg.Name] != "null" ? (int?)int.Parse(Args[arg.Name]) : null) : null;
            set => SetValueNullable(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        #endregion
        #region 64-bit
        public ulong this[Arg<ulong> arg]
        {
            get => Args.ContainsKey(arg.Name) ? ulong.Parse(Args[arg.Name]) : 0UL;
            set => SetValue(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        public ulong? this[Arg<ulong?> arg]
        {
            get => Args.ContainsKey(arg.Name) ? (Args[arg.Name] != "null" ? (ulong?)ulong.Parse(Args[arg.Name]) : null) : null;
            set => SetValueNullable(arg.Name, value, !Args.ContainsKey(arg.Name));
        }

        public long this[Arg<long> arg]
        {
            get => Args.ContainsKey(arg.Name) ? long.Parse(Args[arg.Name]) : 0L;
            set => SetValue(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        public long? this[Arg<long?> arg]
        {
            get => Args.ContainsKey(arg.Name) ? (Args[arg.Name] != "null" ? (long?)long.Parse(Args[arg.Name]) : null) : null;
            set => SetValueNullable(arg.Name, value, !Args.ContainsKey(arg.Name));
        }
        #endregion
    }
}
