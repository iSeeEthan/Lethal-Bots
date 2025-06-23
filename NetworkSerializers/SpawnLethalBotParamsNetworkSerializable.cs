using LethalBots.Enums;
using System;
using Unity.Netcode;
using UnityEngine;

namespace LethalBots.NetworkSerializers
{
    [Serializable]
    public struct SpawnLethalBotParamsNetworkSerializable : INetworkSerializable
    {
        public int IndexNextLethalBot;
        public int IndexNextPlayerObject;
        public int LethalBotIdentityID;
        public int Hp;
        public int SuitID;
        public EnumSpawnAnimation enumSpawnAnimation;
        public Vector3 SpawnPosition;
        public float YRot;
        public bool IsOutside;
        public bool ShouldDestroyDeadBody;

        // INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref IndexNextLethalBot);
            serializer.SerializeValue(ref IndexNextPlayerObject);
            serializer.SerializeValue(ref LethalBotIdentityID);
            serializer.SerializeValue(ref Hp);
            serializer.SerializeValue(ref SuitID);
            serializer.SerializeValue(ref enumSpawnAnimation);
            serializer.SerializeValue(ref SpawnPosition);
            serializer.SerializeValue(ref YRot);
            serializer.SerializeValue(ref IsOutside);
            serializer.SerializeValue(ref ShouldDestroyDeadBody);
        }
    }
}
