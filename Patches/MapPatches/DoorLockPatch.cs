using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.AI;
using LethalBots.Managers;

namespace LethalBots.Patches.MapPatches
{
    /// <summary>
    /// Patch for <c>DoorLock</c>
    /// </summary>
    [HarmonyPatch(typeof(DoorLock))]
    public class DoorLockPatch
    {
        /// <summary>
        /// Patch for making the bot only open door if not locked or already opened
        /// </summary>
        /// <remarks>
        /// Needed a prefix patch for accessing the private field <c>isLocked</c> and <c>isDoorOpened</c>
        /// </remarks>
        /// <param name="__instance"></param>
        /// <param name="___isLocked"></param>
        /// <param name="___isDoorOpened"></param>
        /// <param name="playerWhoTriggered"></param>
        /// <returns></returns>
        [HarmonyPatch("OpenOrCloseDoor")]
        [HarmonyPrefix]
        static bool OpenOrCloseDoor_PreFix(DoorLock __instance,
                                           bool ___isLocked,
                                           bool ___isDoorOpened,
                                           PlayerControllerB playerWhoTriggered)
        {
            LethalBotAI? lethalBotAI = LethalBotManager.Instance.GetLethalBotAIIfLocalIsOwner(playerWhoTriggered);
            if (lethalBotAI?.NpcController.Npc.playerClientId != playerWhoTriggered.playerClientId)
            {
                return true;
            }

            if (___isLocked || ___isDoorOpened)
            {
                return false;
            }

            __instance.OpenDoorAsEnemy();
            return true;
        }
    }
}
