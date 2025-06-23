using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.Managers;
using LethalBots.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Netcode;

namespace LethalBots.Patches.ObjectsPatches
{
    /// <summary>
    /// Patches for <see cref="TetraChemicalItem"/>
    /// </summary>
    [HarmonyPatch(typeof(TetraChemicalItem))]
    public class TetraChemicalItemPatch
    {
        /// <summary>
        /// A very hacky way to fix the sounds for the item when used by a bot!
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="___emittingGas"></param>
        /// <param name="___previousPlayerHeldBy"></param>
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update_Postfix(TetraChemicalItem __instance, ref bool ___emittingGas, ref PlayerControllerB ___previousPlayerHeldBy)
        {
            if (___emittingGas && ___previousPlayerHeldBy.IsOwner && ___previousPlayerHeldBy.isPlayerControlled)
            {
                if (LethalBotManager.Instance.IsPlayerLethalBot(___previousPlayerHeldBy))
                {
                    if (__instance.localHelmetSFX.isPlaying 
                        || !__instance.thisAudioSource.isPlaying)
                    {
                        __instance.localHelmetSFX.Stop();
                        __instance.thisAudioSource.clip = __instance.releaseGasSFX;
                        __instance.thisAudioSource.Play();
                        __instance.thisAudioSource.PlayOneShot(__instance.twistCanSFX);
                    }
                }
            }
        }
        // FIXME: This doesn't work since the function is a Coroutine function.
        // There has to be a better way of doing this!
        /*[HarmonyPatch("UseTZPAnimation")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> UseTZPAnimation_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);

            Plugin.LogDebug("Before patching IL instructions:");
            foreach (var instruction in codes)
            {
                Plugin.LogDebug(instruction.ToString());
            }

            // Unity Netcode
            MethodInfo isOwnerGetter = AccessTools.PropertyGetter(typeof(NetworkBehaviour), "IsOwner");

            // Lets us know that the patching has begun!
            Plugin.LogDebug("Beginning to patch TetraChemicalItem.UseTZPAnimation!");

            // ----------------------------------------------------------------------
            for (var i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldloc_1 && codes[i + 1].Calls(isOwnerGetter))
                {
                    // Replace with "LethalBotManager.Instance.IsAnLethalBotAiOwnerOfObjectMethod"
                    Plugin.LogDebug($"Patching IsOwner check at index {i}...");
                    Plugin.LogDebug($"Original Codes: 1: {codes[i]} and 2: {codes[i + 1]}");
                    startIndex = i;
                    break;
                }
            }
            if (startIndex > -1)
            {
                // The codes we want to inject into the IL
                List<CodeInstruction> codesToReplace = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, PatchesUtil.IsAnLethalBotAiOwnerOfObjectMethod),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ceq)
                };

                // Remove the original instructions Ldloc_1 and IsOwner
                codes.RemoveRange(startIndex, 2);
                codes.InsertRange(startIndex, codesToReplace);

            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.ObjectsPatches.TetraChemicalItemPatch.UseTZPAnimation_Transpiler failed to replace with correct tag if bot.");
            }

            Plugin.LogDebug("Finished patching TetraChemicalItem.UseTZPAnimation!");

            Plugin.LogDebug("After patching IL instructions:");
            foreach (var instruction in codes)
            {
                Plugin.LogDebug(instruction.ToString());
            }

            // ----------------------------------------------------------------------
            return codes.AsEnumerable();
        }*/
    }
}
