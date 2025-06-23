using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.AI;
using LethalBots.Constants;
using LethalBots.Managers;
using System;
using Random = System.Random;

namespace LethalBots.Patches.MapPatches
{
    [HarmonyPatch(typeof(ShipTeleporter))]
    public class ShipTeleporterPatch
    {
        [HarmonyPatch("SetPlayerTeleporterId")]
        [HarmonyPrefix]
        static void SetPlayerTeleporterId_PreFix(PlayerControllerB playerScript,
                                                 int teleporterId)
        {
            LethalBotAI? lethalBotAI = LethalBotManager.Instance.GetLethalBotAI((int)playerScript.playerClientId);
            if (lethalBotAI == null)
            {
                return;
            }

            if (playerScript.shipTeleporterId == 1
                && teleporterId == -1)
            {
                // The bot is being teleported to the ship
                playerScript.ResetFallGravity(); // Found out the hard way gravity wasn't reset on the bots......poor Felix....
                lethalBotAI.InitStateToSearchingNoTarget();
                AudioReverbPresets audioReverbPresets = UnityEngine.Object.FindObjectOfType<AudioReverbPresets>();
                if ((bool)audioReverbPresets)
                {
                    audioReverbPresets.audioPresets[3].ChangeAudioReverbForPlayer(playerScript);
                }
            }
        }

        [HarmonyPatch("beamOutPlayer")]
        [HarmonyPostfix]
        static void beamOutPlayer_PostFix(ShipTeleporter __instance,
                                          Random ___shipTeleporterSeed)
        {
            LethalBotManager.Instance.TeleportOutLethalBots(__instance, ___shipTeleporterSeed);
        }

        [HarmonyPatch("SetPlayerTeleporterId")]
        [HarmonyReversePatch(type: HarmonyReversePatchType.Snapshot)]
        [HarmonyPriority(Priority.Last)]
        public static void SetPlayerTeleporterId_ReversePatch(object instance, PlayerControllerB playerScript, int teleporterId) => throw new NotImplementedException("Stub LethalBot.Patches.MapPatches.ShipTeleporterPatch.SetPlayerTeleporterId_ReversePatch");

        [HarmonyPatch("SpikeTrapsReactToInverseTeleport")]
        [HarmonyReversePatch(type: HarmonyReversePatchType.Snapshot)]
        [HarmonyPriority(Priority.Last)]
        public static void SpikeTrapsReactToInverseTeleport_ReversePatch(object instance) => throw new NotImplementedException("Stub LethalBot.Patches.MapPatches.ShipTeleporterPatch.SpikeTrapsReactToInverseTeleport_ReversePatch");

        [HarmonyPatch("SetCaveReverb")]
        [HarmonyReversePatch(type: HarmonyReversePatchType.Snapshot)]
        [HarmonyPriority(Priority.Last)]
        public static void SetCaveReverb_ReversePatch(object instance, PlayerControllerB playerScript) => throw new NotImplementedException("Stub LethalBot.Patches.MapPatches.ShipTeleporterPatch.SetCaveReverb_ReversePatch");
    }
}
