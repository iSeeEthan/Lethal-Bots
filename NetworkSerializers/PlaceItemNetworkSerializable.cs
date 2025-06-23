using Unity.Netcode;
using UnityEngine;

namespace LethalBots.NetworkSerializers
{
    public struct PlaceItemNetworkSerializable : INetworkSerializable
    {
        public NetworkObjectReference GrabbedObject;
        public NetworkObjectReference ParentObject;
        public Vector3 PlacePositionOffset;
        public bool MatchRotationOfParent;
        public bool SkipOwner;

        // INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref GrabbedObject);
            serializer.SerializeValue(ref ParentObject);
            serializer.SerializeValue(ref PlacePositionOffset);
            serializer.SerializeValue(ref MatchRotationOfParent);
            serializer.SerializeValue(ref SkipOwner);
        }
    }
}
