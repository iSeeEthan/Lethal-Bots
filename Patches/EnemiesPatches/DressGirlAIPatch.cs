using HarmonyLib;
using LethalBots.Managers;
using LethalBots.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Unity.Netcode;

namespace LethalBots.Patches.EnemiesPatches
{
    /// <summary>
    /// Patches for the <c>DressGirlAI</c>
    /// </summary>
    /// <remarks>
    /// This is a PAIN todo, there is so much that must be changed in order to get this to be
    /// compatable with bots. For now this is not implemented!
    /// </remarks>
    [HarmonyPatch(typeof(DressGirlAI))]
    public class DressGirlAIPatch
    {
        /// <summary>
        /// Patch Update to check for player and bot
        /// </summary>
        /// <remarks>
        /// The AI changes its owner to hauntingPlayer, but the default AI only checks __instance.hauntingPlayer != GameNetworkManager.Instance.localPlayerController.
        /// As a result, this causes the default AI to contantly try to change its owner if its target is a bot as the bot doesn't have a client.
        /// So this patch overrides it to check if the local player owns the bot to allow the default AI to run!
        /// </remarks>
        /// <param name="instructions"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        [HarmonyPatch("Update")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);

            MethodInfo getGameNetworkManagerInstance = AccessTools.PropertyGetter(typeof(GameNetworkManager), "Instance");
            FieldInfo localPlayerControllerField = AccessTools.Field(typeof(GameNetworkManager), "localPlayerController");
            FieldInfo hauntingPlayer = AccessTools.Field(typeof(DressGirlAI), "hauntingPlayer");
            MethodInfo opInequalityMethod = AccessTools.Method(typeof(UnityEngine.Object), "op_Inequality");
            MethodInfo getSoundManagerInstance = AccessTools.Method(typeof(SoundManager), "Instance");
            MethodInfo setDiageticMixerSnapshotMethod = AccessTools.Method(typeof(SoundManager), "SetDiageticMixerSnapshot");

            // ------- Step 1: Fix the ghost girl spamming ownership changes when a bot is targeted -------------------------
            for (var i = 0; i < codes.Count - 4; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].Calls(getGameNetworkManagerInstance) &&
                    codes[i + 1].opcode == OpCodes.Ldfld && codes[i + 1].LoadsField(localPlayerControllerField) &&
                    codes[i + 2].opcode == OpCodes.Ldarg_0 &&
                    codes[i + 3].opcode == OpCodes.Ldfld && codes[i + 3].LoadsField(hauntingPlayer) &&
                    codes[i + 4].opcode == OpCodes.Call && codes[i + 4].Calls(opInequalityMethod)) // Branch if not equal
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex > -1)
            {
                // Save the branch instruction (brfalse.s)
                CodeInstruction branchInstruction = codes[startIndex + 5];

                // Remove old condition check (6 instructions)
                codes.RemoveRange(startIndex, 6);

                // Insert new method call for !IsPlayerLocalOrLethalBotOwnerLocal(hauntingPlayer)
                List<CodeInstruction> codesToAdd = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Ldarg_0), // Load DressGirlAI instance
                    new CodeInstruction(OpCodes.Ldfld, hauntingPlayer), // Load hauntingPlayer
                    new CodeInstruction(OpCodes.Call, PatchesUtil.IsPlayerLocalOrLethalBotOwnerLocalMethod), // Call method
                    new CodeInstruction(OpCodes.Ldc_I4_0), // Load constant 0 (false)
                    new CodeInstruction(OpCodes.Ceq), // Compare equality (logical NOT)
                    branchInstruction
                };
                codes.InsertRange(startIndex, codesToAdd);
                startIndex = -1;
            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.EnemiesPatches.DressGirlAIPatch.Update_Transpiler could not check if player local or bot local 1");
            }

            // ------- Step 2: Fix the ghost girl changing the DiageticMixer when a bot is targeted -------------------------
            for (var i = 0; i < codes.Count - 3; i++)
            {
                if (codes[i].Calls(getSoundManagerInstance) &&
                    codes[i + 1].opcode == OpCodes.Ldc_I4_1 &&
                    codes[i + 2].opcode == OpCodes.Ldc_R4 && codes[i + 2].operand is float f && f == 1f &&
                    codes[i + 3].Calls(setDiageticMixerSnapshotMethod))
                {
                    startIndex = i;
                    break;
                }
            }
            // I have been thinking about this and it may be better to insert a branch here and
            // go around the statement rather than edit the call itself.
            if (startIndex > -1)
            {
                // Insert a conditional branch (if hauntingPlayer != localPlayer skip calling SetDiageticMixerSnapshot)
                Label skipSnapshot = generator.DefineLabel();

                // Remove old SetDiageticMixerSnapshot call
                //codes.RemoveRange(startIndex, 4);

                // Insert new method call for this.hauntingPlayer == GameNetworkManager.Instance.localPlayerController ? 1 : 0
                List<CodeInstruction> codesToAdd = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Ldarg_0), // DressGirlAI instance
                    new CodeInstruction(OpCodes.Ldfld, hauntingPlayer), // DressGirlAI.hauntingPlayer
                    new CodeInstruction(OpCodes.Call, getGameNetworkManagerInstance), // GameNetworkManager.Instance
                    new CodeInstruction(OpCodes.Ldfld, localPlayerControllerField), // GameNetworkManager.Instance.localPlayerController
                    new CodeInstruction(OpCodes.Ceq), // Compare ==
                    new CodeInstruction(OpCodes.Brfalse, skipSnapshot) // Branch if not equal (skip call)
                };

                // Insert our new instructions
                codes.InsertRange(startIndex, codesToAdd);
                codes[startIndex + codesToAdd.Count].labels.Add(skipSnapshot);
            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.EnemiesPatches.DressGirlAIPatch.Update_Transpiler could not find and replace sound blur check!");
            }

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Patch Update to make ghost girl invisible if the player being haunted is a bot!
        /// This is basically the same code as the in the base AI, just modified to consider bots.
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update_Postfix(DressGirlAI __instance, ref bool ___enemyMeshEnabled)
        {
            // Don't need to run this code if we are not the owner as the base AI handles this!
            if (!__instance.IsOwner)
            {
                return;
            }

            // Only run this code if the player being haunted is not the local player!
            if (__instance.hauntingPlayer != GameNetworkManager.Instance.localPlayerController)
            {
                if (___enemyMeshEnabled == true)
                {
                    ___enemyMeshEnabled = false;
                    __instance.EnableEnemyMesh(enable: false, overrideDoNotSet: true);
                }

                // Make sure we are not playing any of our sounds
                // since we are not haunting the local player, but a bot!
                //SoundManager.Instance.SetDiageticMixerSnapshot(transitionTime: 0f); // There has to be a better way of doing this, maybe the transpiler?
                __instance.SFXVolumeLerpTo = 0f;
                __instance.creatureVoice.Stop();
                __instance.heartbeatMusic.volume = 0f;
                __instance.creatureSFX.volume = 0f;

                // HACKHACK: hauntingLocalPlayer is used to check for collisions, we need to set it to true so everything works as expected!
                if (!__instance.hauntingLocalPlayer 
                    && LethalBotManager.Instance.IsPlayerLocalOrLethalBotOwnerLocal(__instance.hauntingPlayer))
                {
                    __instance.hauntingLocalPlayer = true;
                }
                else if (__instance.hauntingLocalPlayer 
                         && !LethalBotManager.Instance.IsPlayerLocalOrLethalBotOwnerLocal(__instance.hauntingPlayer))
                {
                    __instance.hauntingLocalPlayer = false;
                }
            }
        }
    }
}
