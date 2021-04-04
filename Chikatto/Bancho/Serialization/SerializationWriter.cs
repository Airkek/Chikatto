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
                Write((byte) 0);
            }
            else
            {
                Write((byte) 13);
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

        public void Write(List<int> value)
        {
            if (value is null)
            {
                Write(-1);
            }
            else
            {
                Write((short) value.Count);
                value.ForEach(Write);
            }
        }

        public void WriteBanchoObject<T>(T value) where T : ISerializable
        {
            value.Serialize(this);
        }
    }
}