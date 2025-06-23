using HarmonyLib;
using LethalBots.AI;
using LethalBots.Managers;
using LethalBots.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace LethalBots.Patches.EnemiesPatches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    public class MaskedPlayerEnemyPatch
    {
        [HarmonyPatch("FinishKillAnimation")]
        [HarmonyPrefix]
        static bool FinishKillAnimation_PreFix(MaskedPlayerEnemy __instance)
        {
            if (__instance.inSpecialAnimationWithPlayer == null)
            {
                return true;
            }

            LethalBotAI? lethalBotAI = LethalBotManager.Instance.GetLethalBotAI((int)__instance.inSpecialAnimationWithPlayer.playerClientId);
            if (lethalBotAI == null)
            {
                return true;
            }

            if (lethalBotAI.NpcController.EnemyInAnimationWith == __instance)
            {
                lethalBotAI.NpcController.EnemyInAnimationWith = null;
            }
            return true;
        }

        [HarmonyPatch("KillPlayerAnimationClientRpc")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> KillPlayerAnimationClientRpc_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);

            // ----------------------------------------------------------------------
            for (var i = 0; i < codes.Count - 2; i++)
            {
                if (codes[i].ToString() == "call static GameNetworkManager GameNetworkManager::get_Instance()" //
                    && codes[i + 1].ToString() == "ldfld GameNetcodeStuff.PlayerControllerB GameNetworkManager::localPlayerController"
                    && codes[i + 2].ToString() == "call static bool UnityEngine.Object::op_Equality(UnityEngine.Object x, UnityEngine.Object y)") //
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex > -1)
            {
                codes[startIndex].opcode = OpCodes.Nop;
                codes[startIndex].operand = null;
                codes[startIndex + 1].opcode = OpCodes.Nop;
                codes[startIndex + 1].operand = null;
                codes[startIndex + 2].opcode = OpCodes.Call;
                codes[startIndex + 2].operand = PatchesUtil.IsPlayerLocalOrLethalBotOwnerLocalMethod;
                startIndex = -1;
            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.EnemiesPatches.MaskedPlayerEnemyPatch.KillPlayerAnimationClientRpc_Transpiler could not check if player local or bot local 1");
            }

            // ----------------------------------------------------------------------
            for (var i = 0; i < codes.Count - 2; i++)
            {
                if (codes[i].ToString() == "call static GameNetworkManager GameNetworkManager::get_Instance()" //
                    && codes[i + 1].ToString() == "ldfld GameNetcodeStuff.PlayerControllerB GameNetworkManager::localPlayerController"
                    && codes[i + 2].ToString() == "call static bool UnityEngine.Object::op_Equality(UnityEngine.Object x, UnityEngine.Object y)") //
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex > -1)
            {
                codes[startIndex].opcode = OpCodes.Nop;
                codes[startIndex].operand = null;
                codes[startIndex + 1].opcode = OpCodes.Nop;
                codes[startIndex + 1].operand = null;
                codes[startIndex + 2].opcode = OpCodes.Call;
                codes[startIndex + 2].operand = PatchesUtil.IsPlayerLocalOrLethalBotOwnerLocalMethod;
                startIndex = -1;
            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.EnemiesPatches.MaskedPlayerEnemyPatch.KillPlayerAnimationClientRpc_Transpiler could not check if player local or bot local 2");
            }

            return codes.AsEnumerable();
        }

        [HarmonyPatch("KillPlayerAnimationClientRpc")]
        [HarmonyPostfix]
        static void KillPlayerAnimationClientRpc_PostFix(MaskedPlayerEnemy __instance)
        {
            if (__instance.inSpecialAnimationWithPlayer == null)
            {
                return;
            }

            LethalBotAI? lethalBotAI = LethalBotManager.Instance.GetLethalBotAI((int)__instance.inSpecialAnimationWithPlayer.playerClientId);
            if (lethalBotAI == null)
            {
                return;
            }

            lethalBotAI.NpcController.EnemyInAnimationWith = __instance;
        }
    }
}
