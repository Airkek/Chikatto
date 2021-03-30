using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;

namespace Chikatto.Bancho.Serialization
{
    public class SerializationWriter : BinaryWriter
    {
        public SerializationWriter(Stream output) : base(output, Encoding.UTF8) { }

        public override void Write(string value)
        {
            if (value is null)
            {
                Write((byte) TypeBytes.Null);
            }
            else
            {
                Write((byte) TypeBytes.String);
                base.Write(value);
            }
        }

        public override void Write(byte[] value)
        {
            if (value is null)
            {
                Write(-1);
            }
            else
            {
                Write(value.Length);
                if(value.Length > 0)
                    base.Write(value);
            }
        }

        public override void Write(char[] value)
        {
            if (value is null)
            {
                Write(-1);
            }
            else
            {   
                Write(value.Length);
                if (value.Length > 0)
                    base.Write(value);
            }
        }

        public void Write(DateTime value)
        {
            Write(value.ToUniversalTime().Ticks);
        }

        public void Write<T>(List<T> value)
        {
            if (value is null)
            {
                Write(-1);
            }
            else
            {
                Write(value.Count);
                value.ForEach(t => WriteObject(t));
            }
        }

        public void WriteBanchoObject<T>(T value) where T : ISerializable
        {
            value.Serialize(this);
        } 

        public void WriteObject(object obj)
        {
            switch (obj)
            {
                case null:
                    Write((byte) TypeBytes.Null);
                    break;
                case bool v:
                    Write((byte) TypeBytes.Bool);
                    base.Write(v);
                    break;
                case byte v:
                    Write((byte) TypeBytes.Byte);
                    base.Write(v);
                    break;
                case ushort v:
                    Write((byte) TypeBytes.UShort);
                    base.Write(v);
                    break;
                case uint v:
                    Write((byte) TypeBytes.UInt);
                    base.Write(v);
                    break;
                case ulong v:
                    Write((byte) TypeBytes.ULong);
                    base.Write(v);
                    break;
                case sbyte v:
                    Write((byte) TypeBytes.SByte);
                    base.Write(v);
                    break;
                case short v:
                    Write((byte) TypeBytes.Short);
                    base.Write(v);
                    break;
                case int v:
                    Write((byte) TypeBytes.Int);
                    base.Write(v);
                    break;
                case long v:
                    Write((byte) TypeBytes.Long);
                    base.Write(v);
                    break;
                case char v:
                    Write((byte) TypeBytes.Char);
                    base.Write(v);
                    break;
                case string v:
                    Write((byte) TypeBytes.String);
                    base.Write(v);
                    break;
                case float v:
                    Write((byte) TypeBytes.Float);
                    base.Write(v);
                    break;
                case double v:
                    Write((byte) TypeBytes.Double);
                    base.Write(v);
                    break;
                case decimal v:
                    Write((byte) TypeBytes.Decimal);
                    base.Write(v);
                    break;
                case DateTime v:
                    Write((byte) TypeBytes.DateTime);
                    Write(v);
                    break;
                case byte[] v:
                    Write((byte) TypeBytes.ByteArray);
                    base.Write(v);
                    break;
                case char[] v:
                    Write((byte) TypeBytes.CharArray);
                    base.Write(v);
                    break;
            }
        }
    }
}