using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;

namespace Chikatto.Bancho.Serialization
{
    public class SerializationReader : BinaryReader
    {
        public SerializationReader(Stream input) : base(input, Encoding.UTF8) { }

        public override string ReadString() => ReadByte() == (byte) 0 ? string.Empty : base.ReadString();

        public byte[] ReadByteArray()
        {
            var len = ReadInt32();
            return len switch
            {
                > 0 => ReadBytes(len),
                < 0 => null,
                _ => Array.Empty<byte>()
            };
        }
        
        public IEnumerable<int> ReadInt32Array()
        {
            var len = ReadInt16();
            switch (len)
            {
                case > 0:
                {
                    var arr = new int[len];
                    for (var i = 0; i < arr.Length; i++)
                    {
                        arr[i] = ReadInt32();
                    }

                    return arr;
                }
                case < 0:
                    return null;
                default:
                    return Array.Empty<int>();
            }
        }

        public char[] ReadCharArray()
        {
            var len = ReadInt32();
            return len switch
            {
                > 0 => ReadChars(len),
                < 0 => null,
                _ => Array.Empty<char>()
            };
        }

        public DateTime ReadDateTime()
        {
            var ticks = ReadInt64();
            if (ticks < 0)
                ticks = 0;
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        public List<int> ReadListInt()
        {
            var count = ReadInt16();
            if (count < 0) return null;
            
            var list = new List<int>(count);
            
            for (var i = 0; i < count; i++) 
                list.Add(ReadInt32());
            
            return list;
        }

        public T ReadBanchoObject<T>() where T : IDeserializable, new()
        {
            var t = new T();
            t.Deserialize(this);
            return t;
        }
    }
}