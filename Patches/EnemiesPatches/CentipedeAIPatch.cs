using HarmonyLib;
using LethalBots.Managers;
using System;

namespace LethalBots.Patches.EnemiesPatches
{
    /// <summary>
    /// Patches for <c>CentipedeAI</c>
    /// </summary>
    [HarmonyPatch(typeof(CentipedeAI))]
    public class CentipedeAIPatch
    {
        /// <summary>
        /// Patch for making the centipede hurt the bot
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update_PostFix(ref CentipedeAI __instance)
        {
            if (__instance.isEnemyDead)
            {
                return;
            }

            switch (__instance.currentBehaviourStateIndex)
            {
                case 3:
                    if (__instance.clingingToPlayer == null)
                    {
                        break;
                    }

                    if (LethalBotManager.Instance.IsPlayerLethalBotOwnerLocal(__instance.clingingToPlayer))
                    {
                        DamagePlayerOnIntervals_ReversePatch(__instance);
                    }
                    break;
            }
        }

        /// <summary>
        /// Reverse patch used for damaging bot
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="NotImplementedException"></exception>
        [HarmonyPatch("DamagePlayerOnIntervals")]
        [HarmonyReversePatch(type: HarmonyReversePatchType.Snapshot)]
        [HarmonyPriority(Priority.Last)]
        public static void DamagePlayerOnIntervals_ReversePatch(object instance) => throw new NotImplementedException("Stub LethalBot.Patches.EnemiesPatches.DamagePlayerOnIntervals");

    }
}
