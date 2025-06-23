using GameNetcodeStuff;
using HarmonyLib;
using LCPeeper;
using LethalBots.Managers;
using UnityEngine;

namespace LethalBots.Patches.ModPatches.Peepers
{
    [HarmonyPatch(typeof(PeeperAttachHitbox))]
    public class PeeperAttachHitboxPatch
    {
        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPostfix]
        public static void OnTriggerEnter_Postfix(PeeperAttachHitbox __instance, Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB != null
                    && LethalBotManager.Instance.IsPlayerLethalBot(playerControllerB))
                {
                    __instance.mainScript.AttachToPlayerServerRpc(playerControllerB.playerClientId);
                }
            }
            else if (other is BoxCollider)
            {
                // bot character controller is inactive but box collider (spine, thighs, arms) still procs
                PlayerControllerB playerControllerB = other.gameObject.GetComponentInParent<PlayerControllerB>();
                if (playerControllerB != null
                    && LethalBotManager.Instance.IsPlayerLethalBot(playerControllerB))
                {
                    __instance.mainScript.AttachToPlayerServerRpc(playerControllerB.playerClientId);
                }
            }
        }
    }
}
