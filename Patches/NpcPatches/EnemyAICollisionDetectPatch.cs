using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.AI;
using LethalBots.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalBots.Patches.NpcPatches
{
    [HarmonyPatch(typeof(EnemyAICollisionDetect))]
    public class EnemyAICollisionDetectPatch
    {
        [HarmonyPatch("OnTriggerStay")]
        [HarmonyPrefix]
        static void Prefix(EnemyAICollisionDetect __instance, Collider other)
        {
            if (__instance.mainScript is LethalBotAI)
            {
                PlayerControllerB playerControllerB = other.gameObject.GetComponentInParent<PlayerControllerB>();
                if (playerControllerB != null) // Disable collisions for all players, since we use the character controller for collison testing! LethalBotManager.Instance.IsPlayerLethalBot(lethalBotController)
                {
                    Physics.IgnoreCollision(__instance.GetComponent<Collider>(), other);
                }
            }
        }
    }
}
