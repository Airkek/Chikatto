using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho.Objects
{
    public interface IDeserializable
    {
        public void Deserialize(SerializationReader reader);
    }
}