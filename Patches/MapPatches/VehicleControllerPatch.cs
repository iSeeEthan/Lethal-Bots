using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.AI;
using LethalBots.Managers;
using UnityEngine;

namespace LethalBots.Patches.MapPatches
{
    /// <summary>
    /// Patch for <c>VehicleController</c>
    /// </summary>
    [HarmonyPatch(typeof(VehicleController))]
    public class VehicleControllerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Start_PostFix()
        {
            LethalBotManager.Instance.VehicleHasLanded();
        }

        /// <summary>
        /// Patch for damaging the bots owned by client in vehicle
        /// </summary>
        [HarmonyPatch("DamagePlayerInVehicle")]
        [HarmonyPostfix]
        static void DamagePlayerInVehicle_PostFix(VehicleController __instance,
                                                  Vector3 vel,
                                                  float magnitude)
        {
            PlayerControllerB lethalBotController;
            foreach (LethalBotAI lethalBotAI in LethalBotManager.Instance.GetLethalBotsAIOwnedByLocal())
            {
                lethalBotController = lethalBotAI.NpcController.Npc;

                if (!__instance.localPlayerInPassengerSeat && !__instance.localPlayerInControl)
                {
                    if (__instance.physicsRegion.physicsTransform == lethalBotController.physicsParent
                        && lethalBotController.overridePhysicsParent == null)
                    {
                        lethalBotController.DamagePlayer(10, hasDamageSFX: true, callRPC: true, CauseOfDeath.Inertia, 0, false, vel);
                        lethalBotController.externalForceAutoFade += vel;
                    }
                    return;
                }

                if (magnitude > 28f)
                {
                    lethalBotController.KillPlayer(vel, spawnBody: true, CauseOfDeath.Inertia, 0, __instance.transform.up * 0.77f);
                    return;
                }

                if (magnitude <= 24f)
                {
                    lethalBotController.DamagePlayer(30, hasDamageSFX: true, callRPC: true, CauseOfDeath.Inertia, 0, false, vel);
                    return;
                }

                if (lethalBotController.health < 20)
                {
                    lethalBotController.KillPlayer(vel, spawnBody: true, CauseOfDeath.Inertia, 0, __instance.transform.up * 0.77f);
                    return;
                }
                lethalBotController.DamagePlayer(40, hasDamageSFX: true, callRPC: true, CauseOfDeath.Inertia, 0, false, vel);
            }
        }

        /// <summary>
        /// Patch for killing bot when car is destroyed
        /// </summary>
        /*[HarmonyPatch("DestroyCar")]
        [HarmonyPostfix]
        static void DestroyCar_PostFix()
        {
            foreach (LethalBotAI lethalBotAI in LethalBotManager.Instance.GetLethalBotsAIOwnedByLocal())
            {
                Plugin.LogDebug($"DestroyCar Killing bot #{lethalBotAI.BotId}");
                lethalBotAI.NpcController.Npc.KillPlayer(Vector3.up * 27f + 20f * Random.insideUnitSphere, spawnBody: true, CauseOfDeath.Blast, 6, Vector3.up * 1.5f);
            }
        }*/
    }
}
