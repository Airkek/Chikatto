using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho.Objects
{
    public class Match : ISerializable
    {
        public short Id;
        public bool InProgress;
        
        //may be later...
        //TODO
        
        public void Serialize(SerializationWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(SerializationReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}