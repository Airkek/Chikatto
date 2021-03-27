using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Microsoft.AspNetCore.Components.Web;

namespace Chikatto.Bancho.Serialization
{
    public class SerializationReader : BinaryReader
    {
        public SerializationReader(Stream input) : base(input, Encoding.UTF8) { }

        public override string ReadString() => ReadByte() == (byte) TypeBytes.Null ? string.Empty : base.ReadString();

        public byte[] ReadByteArray()
        {
            var len = ReadInt32();
            if (len > 0) return ReadBytes(len);
            if (len < 0) return null;
            return Array.Empty<byte>();
        }
        
        public IEnumerable<int> ReadInt32Array()
        {
            var len = ReadInt16();
            if (len > 0)
            {
                var arr = new int[len];
                for (var i = 0; i < arr.Length; i++)
                {
                    arr[i] = ReadInt32();
                }

                return arr;
            }
                
            if (len < 0) return null;
            return Array.Empty<int>();
        }

        public char[] ReadCharArray()
        {
            var len = ReadInt32();
            if (len > 0) return ReadChars(len);
            if (len < 0) return null;
            return Array.Empty<char>();
        }

        public DateTime ReadDateTime()
        {
            var ticks = ReadInt64();
            if (ticks < 0)
                ticks = 0;
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        public List<T> ReadList<T>()
        {
            var count = ReadInt32();
            if (count < 0) return null;
            
            var list = new List<T>(count);
            
            for (var i = 0; i < count; i++) 
                list.Add((T)ReadObject());
            
            return list;
        }

        public T ReadBanchoObject<T>() where T : IDeserializable, new()
        {
            var t = new T();
            t.Deserialize(this);
            return t;
        } 

        public object ReadObject()
        {
            var type = (TypeBytes) ReadByte();

            return type switch
            {
                TypeBytes.Bool => ReadBoolean(),
                TypeBytes.Byte => ReadByte(),
                TypeBytes.UShort => ReadUInt16(),
                TypeBytes.UInt => ReadUInt32(),
                TypeBytes.ULong => ReadUInt64(),
                TypeBytes.SByte => ReadSByte(),
                TypeBytes.Short => ReadInt16(),
                TypeBytes.Int => ReadInt32(),
                TypeBytes.Long => ReadInt64(),
                TypeBytes.Char => ReadChar(),
                TypeBytes.String => base.ReadString(),
                TypeBytes.Float => ReadSingle(),
                TypeBytes.Double => ReadDouble(),
                TypeBytes.Decimal => ReadDecimal(),
                TypeBytes.DateTime => ReadDateTime(),
                TypeBytes.ByteArray => ReadByteArray(),
                TypeBytes.CharArray => ReadCharArray(),
                _ => null
            };
        }
    }
}