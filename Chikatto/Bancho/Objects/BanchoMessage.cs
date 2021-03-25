using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho.Objects
{
    public class BanchoMessage : ISerializable
    {
        public string From;
        public string Body;
        public string To;
        public int ClientId;

        public void Serialize(SerializationWriter writer)
        {
            writer.Write(From);
            writer.Write(Body);
            writer.Write(To);
            writer.Write(ClientId);
        }

        public void Deserialize(SerializationReader reader)
        {
            From = reader.ReadString();
            Body = reader.ReadString();
            To = reader.ReadString();
            ClientId = reader.ReadInt32();
        }
    }
}