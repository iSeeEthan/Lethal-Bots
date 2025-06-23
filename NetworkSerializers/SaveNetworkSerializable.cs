using Unity.Netcode;

namespace LethalBots.NetworkSerializers
{
    internal struct SaveNetworkSerializable : INetworkSerializable
    {
        public IdentitySaveFileNetworkSerializable[] Identities;

        // INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Identities);
        }
    }

    internal struct IdentitySaveFileNetworkSerializable : INetworkSerializable
    {
        public int IdIdentity;
        public int SuitID;
        public int Hp;
        public int Status;
        public int XP;
        public int Level;

        // INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref IdIdentity);
            serializer.SerializeValue(ref SuitID);
            serializer.SerializeValue(ref Hp);
            serializer.SerializeValue(ref Status);
            serializer.SerializeValue(ref XP);
            serializer.SerializeValue(ref Level);
        }
    }
}
