using HarmonyLib;
using LethalBots.AI;
using UnityEngine;

namespace LethalBots.Patches.MapHazardsPatches
{
    /// <summary>
    /// Patch for <c>SpikeRoofTrap</c>
    /// </summary>
    [HarmonyPatch(typeof(SpikeRoofTrap))]
    public class SpikeRoofTrapPatch
    {
        // TODO: This needs to be changed to use a transpiler since we use the player collider
        // NEEDTOVALIDATE: Should I use a transpiler or do my own logic here.......
        [HarmonyPatch("OnTriggerStay")]
        [HarmonyPostfix]
        static void OnTriggerStay_PostFix(Collider other)
        {
            EnemyAICollisionDetect enemyAICollisionDetect = other.gameObject.GetComponent<EnemyAICollisionDetect>();
            if (enemyAICollisionDetect != null 
                && enemyAICollisionDetect.mainScript != null 
                && enemyAICollisionDetect.mainScript.IsOwner 
                && enemyAICollisionDetect.mainScript.enemyType.canDie 
                && !enemyAICollisionDetect.mainScript.isEnemyDead)
            {
                LethalBotAI? lethalBotAI = enemyAICollisionDetect.mainScript as LethalBotAI;
                if (lethalBotAI != null)
                {
                    lethalBotAI.NpcController.Npc.KillPlayer(Vector3.down * 17f, spawnBody: true, CauseOfDeath.Crushing, 0, default(Vector3));
                }
            }
        }
    }
}
