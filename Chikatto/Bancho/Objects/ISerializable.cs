using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho.Objects
{
    public interface ISerializable
    {
        public void Serialize(SerializationWriter writer);
        public void Deserialize(SerializationReader reader);
    }
}