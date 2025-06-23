using System;
using LethalBots.Enums;
using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;

namespace LethalBots.NetworkSerializers
{
    /// <summary>
    /// Struct for serializing the look at target of the bot
    /// </summary>
    /// <remarks>
    /// This really should be rewritten to use a much better system!
    /// The current is a bit outdated and not very efficient!
    /// This makes it hard to add new look at targets!
    /// </remarks>
    [Serializable]
    public class LookAtTarget : INetworkSerializable, IEquatable<LookAtTarget>
    {
        public EnumObjectsLookingAt enumObjectsLookingAt;
        public Vector3 directionToUpdateTurnBodyTowardsTo;
        public Vector3 positionPlayerEyeToLookAt;
        public Vector3 positionToLookAt;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref enumObjectsLookingAt);
            serializer.SerializeValue(ref directionToUpdateTurnBodyTowardsTo);
            serializer.SerializeValue(ref positionPlayerEyeToLookAt);
            serializer.SerializeValue(ref positionToLookAt);
        }

        /// <summary>
        /// Checks if the bot is looking foward
        /// </summary>
        /// <returns>true: is the bot looking forward, false: is not looking forward</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsLookingForward()
        {
            return enumObjectsLookingAt == EnumObjectsLookingAt.Forward;
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="LookAtTarget"/> instance
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LookAtTarget Clone()
        {
            return new LookAtTarget(){
                enumObjectsLookingAt = this.enumObjectsLookingAt,
                directionToUpdateTurnBodyTowardsTo = this.directionToUpdateTurnBodyTowardsTo,
                positionPlayerEyeToLookAt = this.positionPlayerEyeToLookAt,
                positionToLookAt = this.positionToLookAt
            };
        }

        public bool Equals(LookAtTarget? other)
        {
            if (other is null)
            {
                return false;
            }
            return enumObjectsLookingAt == other.enumObjectsLookingAt 
                && directionToUpdateTurnBodyTowardsTo == other.directionToUpdateTurnBodyTowardsTo 
                && positionPlayerEyeToLookAt == other.positionPlayerEyeToLookAt 
                && positionToLookAt == other.positionToLookAt;
        }

        public override bool Equals(object? obj)
        {
            return obj is LookAtTarget other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(enumObjectsLookingAt, directionToUpdateTurnBodyTowardsTo, positionPlayerEyeToLookAt, positionToLookAt);
        }

        public static bool operator ==(LookAtTarget? left, LookAtTarget? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(LookAtTarget? left, LookAtTarget? right)
        {
            return !(left == right);
        }
    }
}
