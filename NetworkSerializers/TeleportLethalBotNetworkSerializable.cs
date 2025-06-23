using Unity.Netcode;
using UnityEngine;

namespace LethalBots.NetworkSerializers
{
    public struct TeleportLethalBotNetworkSerializable : INetworkSerializable
    {
        public Vector3 Pos;
        public bool? SetOutside;
        public bool AllowInteractTrigger;
        public NetworkObjectReference? TargetEntrance;
        public bool SkipNavMeshCheck;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Pos);
            LethalBotNetworkSerializer.SerializeNullable(serializer, ref SetOutside);
            serializer.SerializeValue(ref AllowInteractTrigger);
            LethalBotNetworkSerializer.SerializeNullable(serializer, ref TargetEntrance);
            serializer.SerializeValue(ref SkipNavMeshCheck);
        }
    }
}
