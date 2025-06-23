using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.Managers;
using TooManyEmotes.Patches;

namespace LethalBots.Patches.ModPatches.TooManyEmotes
{
    [HarmonyPatch(typeof(ThirdPersonEmoteController))]
    public class ThirdPersonEmoteControllerPatch
    {
        [HarmonyPatch("UseFreeCamWhileEmoting")]
        [HarmonyPrefix]
        public static bool UseFreeCamWhileEmoting_Prefix(PlayerControllerB __0)
        {
            if (LethalBotManager.Instance.IsPlayerLethalBot(__0))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch("InitLocalPlayerController")]
        [HarmonyPrefix]
        public static bool InitLocalPlayerController_Prefix(PlayerControllerB __0)
        {
            if (LethalBotManager.Instance.IsPlayerLethalBot(__0))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch("OnPlayerSpawn")]
        [HarmonyPrefix]
        public static bool OnPlayerSpawn_Prefix(PlayerControllerB __0)
        {
            if (LethalBotManager.Instance.IsPlayerLethalBot(__0))
            {
                return false;
            }
            return true;
        }
    }
}
