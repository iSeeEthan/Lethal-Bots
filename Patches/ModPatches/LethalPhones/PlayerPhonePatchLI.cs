using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.AI;
using LethalBots.Managers;
using Scoops.misc;
using UnityEngine;

namespace LethalBots.Patches.ModPatches.LethalPhones
{
    [HarmonyPatch(typeof(PlayerPhone))]
    public class PlayerPhonePatchLI
    {
        [HarmonyPatch("UpdatePhoneSanity")]
        [HarmonyPrefix]
        static bool UpdatePhoneSanity_PreFix(PlayerControllerB playerController)
        {
            LethalBotAI? lethalBotAI = LethalBotManager.Instance.GetLethalBotAI(playerController);
            if (lethalBotAI != null)
            {
                return false;
            }

            Transform? phoneTransform = playerController.transform.Find("PhonePrefab(Clone)");
            if (phoneTransform == null)
            {
                return false;
            }

            PlayerPhone? playerPhone = phoneTransform.GetComponent<PlayerPhone>();
            if (playerPhone == null)
            {
                return false;
            }

            return true;
        }
    }
}
