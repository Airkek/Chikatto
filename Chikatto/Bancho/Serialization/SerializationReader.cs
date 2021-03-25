using System.IO;
using System.Text;

namespace Chikatto.Bancho.Serialization
{
    public class SerializationReader : BinaryReader
    {
        public SerializationReader(Stream input) : base(input) { }
    }
}