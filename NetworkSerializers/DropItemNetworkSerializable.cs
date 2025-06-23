using Unity.Netcode;
using UnityEngine;

namespace LethalBots.NetworkSerializers
{
    public struct DropItemNetworkSerializable : INetworkSerializable
    {
        public NetworkObjectReference GrabbedObject;
        public bool DroppedInElevator;
        public bool DroppedInShipRoom;
        public Vector3 TargetFloorPosition;
        public int FloorYRot;
        public bool SkipOwner;

        // INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref GrabbedObject);
            serializer.SerializeValue(ref DroppedInElevator);
            serializer.SerializeValue(ref DroppedInShipRoom);
            serializer.SerializeValue(ref TargetFloorPosition);
            serializer.SerializeValue(ref FloorYRot);
            serializer.SerializeValue(ref SkipOwner);
        }
    }
}
