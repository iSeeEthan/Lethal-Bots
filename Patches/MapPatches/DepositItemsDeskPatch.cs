using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.AI;
using LethalBots.Managers;
using LethalBots.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalBots.Patches.MapPatches
{
    /// <summary>
    /// Patch for <c>DepositItemsDesk</c>
    /// </summary>
    [HarmonyPatch(typeof(DepositItemsDesk))]
    public class DepositItemsDeskPatch
    {
        /// <summary>
        /// Patch for making the company only count the number of REAL players for sound checks!
        /// Fixes the bug where it takes a ridiculous amount of noise to get the company to collect the loot!
        /// </summary>
        /// <remarks>
        /// The bug only really happens with more company, I can't seem to find the main reason why!
        /// </remarks>
        /// <param name="instructions"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        [HarmonyPatch("INoiseListener.DetectNoise")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> DetectNoise_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);

            // Target property: StartOfRound.Instance.connectedPlayersAmount
            MethodInfo getStartOfRoundInstance = AccessTools.PropertyGetter(typeof(StartOfRound), "Instance");
            FieldInfo connectedPlayersAmountField = AccessTools.Field(typeof(StartOfRound), "connectedPlayersAmount");

            //Plugin.LogInfo("DetectNoise_Transpiler");
            // ----------------------------------------------------------------------
            for (var i = 0; i < codes.Count - 2; i++)
            {
                //Plugin.LogWarning(codes[i].ToString());
                if (codes[i].Calls(getStartOfRoundInstance)
                    && codes[i + 1].LoadsField(connectedPlayersAmountField))
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex > -1)
            {
                // Replace the original call to StartOfRound::get_instance()
                codes[startIndex].opcode = OpCodes.Call;
                codes[startIndex].operand = PatchesUtil.AllRealPlayersCountMethod;

                // Replace the original reference to connectedPlayersAmount with a direct value from the custom method
                codes[startIndex + 1].opcode = OpCodes.Nop;
                codes[startIndex + 1].operand = null;

                startIndex = -1;
            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.MapPatches.DepositItemDeskPatch.DetectNoise_Transpiler use other method for detecting player sounds!");
            }

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Patch for alloing bots to call PlaceItemOnCounter without being the local player!
        /// This makes its so other mods that use this method will work with bots!
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        [HarmonyPatch("PlaceItemOnCounter")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> PlaceItemOnCounter_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);

            // Target property: GameNetworkManager.Instance.localPlayerController
            MethodInfo getGameNetworkManagerInstance = AccessTools.PropertyGetter(typeof(GameNetworkManager), "Instance");
            FieldInfo localPlayerControllerField = AccessTools.Field(typeof(GameNetworkManager), "localPlayerController");

            // Unity Object Equality Method
            MethodInfo opEqualityMethod = AccessTools.Method(typeof(UnityEngine.Object), "op_Equality");

            //Plugin.LogInfo("PlaceItemOnCounter_Transpiler");
            // ----------------------------------------------------------------------
            for (var i = 0; i < codes.Count - 2; i++)
            {
                //Plugin.LogWarning(codes[i].ToString());
                // Look for occurrences of "GameNetworkManager.Instance.localPlayerController"
                if (codes[i].Calls(getGameNetworkManagerInstance) 
                    && codes[i + 1].LoadsField(localPlayerControllerField) 
                    && codes[i + 2].Calls(opEqualityMethod))
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex > -1)
            {
                // Remove the three instructions: GameNetworkManager.Instance, localPlayerController, and op_Equality
                codes.RemoveRange(startIndex, 3);

                // Insert the new instruction to call the replacement method.
                codes.Insert(startIndex, new CodeInstruction(OpCodes.Call, PatchesUtil.IsPlayerLocalOrLethalBotOwnerLocalMethod)); // Call our replacement method
                startIndex = -1;
            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.MapPatches.DepositItemDeskPatch.PlaceItemOnCounter_Transpiler failed to allow bots to use PlaceItemOnCounter!");
            }

            return codes.AsEnumerable();
        }

        /*[HarmonyPatch("SetTimesHeardNoiseServerRpc")]
        [HarmonyPostfix]
        public static void SetTimesHeardNoiseServerRpc_Postfix(DepositItemsDesk __instance, ref float valueChange)
        {
            // Debugging: log the original noise value before modification
            Plugin.LogInfo($"Old Noise value {valueChange}");
            
            // Calculate the number of bots by subtracting the number of connected players
            // from the number of players that don't count the host (StartOfRound.Instance.connectedPlayersAmount)
            int numberOfBots = (StartOfRound.Instance.connectedPlayersAmount + 1) - GameNetworkManager.Instance.connectedPlayers;  // Custom logic
                           
            // If there are any bots, adjust the noise value
            if (numberOfBots > 0)
            {
                valueChange *= numberOfBots;  // Adjust valueChange based on the number of bots
            }

            // Debugging: log the new adjusted noise value
            Plugin.LogInfo($"New Noise value {valueChange}");
        }*/
    }
}
