using HarmonyLib;
using LethalBots.Managers;
using LethalBots.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalBots.Patches.EnemiesPatches
{
    /// <summary>
    /// Patches for <c>ButlerEnemyAI</c>
    /// </summary>
    [HarmonyPatch(typeof(ButlerEnemyAI))]
    public class ButlerEnemyAIPatch
    {
        /// <summary>
        /// <inheritdoc cref="ButlerBeesEnemyAIPatch.OnCollideWithPlayer_Transpiler"/>
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        [HarmonyPatch("OnCollideWithPlayer")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> OnCollideWithPlayer_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);

            // ----------------------------------------------------------------------
            for (var i = 0; i < codes.Count - 2; i++)
            {
                if (codes[i].ToString() == "ldloc.0 NULL" //44
                    && codes[i + 2].ToString() == "ldfld GameNetcodeStuff.PlayerControllerB GameNetworkManager::localPlayerController") //46
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex > -1)
            {
                codes[startIndex + 1].opcode = OpCodes.Nop;
                codes[startIndex + 1].operand = null;
                codes[startIndex + 2].opcode = OpCodes.Call;
                codes[startIndex + 2].operand = PatchesUtil.IsPlayerLocalOrLethalBotOwnerLocalMethod;
                codes[startIndex + 3].opcode = OpCodes.Nop;
                codes[startIndex + 3].operand = null;
                startIndex = -1;
            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.EnemiesPatches.ButlerEnemyAIPatch.OnCollideWithPlayer_Transpiler could not check if local player or bot owner local player");
            }

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Fixes the bug where the butler enemy AI does not break chase when the player
        /// being chased is not the local player, but a bot.
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        [HarmonyPatch("DoAIInterval")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> DoAIInterval_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);

            /*Plugin.LogDebug("Before patching IL instructions:");
            foreach (var instruction in codes)
            {
                Plugin.LogDebug(instruction.ToString());
            }*/

            // Target property: GameNetworkManager.Instance.localPlayerController
            MethodInfo getGameNetworkManagerInstance = AccessTools.PropertyGetter(typeof(GameNetworkManager), "Instance");
            FieldInfo localPlayerControllerField = AccessTools.Field(typeof(GameNetworkManager), "localPlayerController");

            // Unity Object Inequality Method
            MethodInfo opInequalityMethod = AccessTools.Method(typeof(UnityEngine.Object), "op_Inequality");

            // Replacement field: this.targetPlayer
            FieldInfo targetPlayerField = AccessTools.Field(typeof(EnemyAI), "targetPlayer");

            // Lets us know that the patching has begun!
            Plugin.LogDebug("Beginning to patch ButlerEnemyAI.DoAIInterval!");

            // ---------- Step 1: Replace 'GameNetworkManager.Instance.localPlayerController != targetPlayerField' ----------
            for (var i = 0; i < codes.Count - 4; i++)
            {
                // Look for occurrences of "GameNetworkManager.Instance.localPlayerController"
                if (codes[i].Calls(getGameNetworkManagerInstance)
                    && codes[i + 1].LoadsField(localPlayerControllerField)
                    && codes[i + 2].IsLdarg(0)
                    && codes[i + 3].LoadsField(targetPlayerField)
                    && codes[i + 4].Calls(opInequalityMethod))
                {
                    // Replace with "IsPlayerLocalOrLethalBotOwnerLocalMethod"
                    Plugin.LogDebug($"Patching localPlayerController comparison at index {i}...");
                    Plugin.LogDebug($"Original Codes: 1: {codes[i]}, 2: {codes[i + 1]}, 3: {codes[i + 2]}, 4: {codes[i + 3]}, 5: {codes[i + 4]}");
                    startIndex = i;
                    break;
                }
            }

            if (startIndex != -1)
            {
                // Save any labels that were on the original instruction
                var originalLabel = codes[startIndex].labels; // The GameNetworkManager.Instance instruction, for some reason the label is here

                // Remove the five instructions: GameNetworkManager.Instance, localPlayerController, ldarg.0, targetPlayer, and op_Inequality
                codes.RemoveRange(startIndex, 5);

                // Insert the new instruction to call the replacement method.
                List<CodeInstruction> codesToReplace = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(originalLabel), // Load `this`
                    new CodeInstruction(OpCodes.Ldfld, targetPlayerField), // Load `targetPlayer`
                    new CodeInstruction(OpCodes.Call, PatchesUtil.IsPlayerLocalOrLethalBotOwnerLocalMethod), // Call our replacement method
                    new CodeInstruction(OpCodes.Ldc_I4_0), // Push 0 (false)
                    new CodeInstruction(OpCodes.Ceq) // Compare equal: (result == false) --> equivalent to !
                };
                codes.InsertRange(startIndex, codesToReplace); // Call our replacement methods
                startIndex = -1;
            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.EnemyPatches.ButlerEnemyAI.DoAIInterval_Transpiler could not find the localPlayerController reference!");
            }

            Plugin.LogDebug("Finished patching ButlerEnemyAI.DoAIInterval!");

            /*Plugin.LogDebug("After patching IL instructions:");
            foreach (var instruction in codes)
            {
                Plugin.LogDebug(instruction.ToString());
            }*/

            return codes.AsEnumerable();
        }
    }
}
