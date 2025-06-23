using HarmonyLib;
using LethalBots.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace LethalBots.Patches.EnemiesPatches
{
    [HarmonyPatch(typeof(RadMechMissile))]
    public class RadMechMissilePatch
    {
        [HarmonyPatch("CheckCollision")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> CheckCollision_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var startIndex = -1;
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            // ----------------------------------------------------------------------
            for (var i = 0; i < codes.Count - 2; i++)
            {
                if (codes[i].ToString() == "call static GameNetworkManager GameNetworkManager::get_Instance()" // 69
                    && codes[i + 1].ToString() == "ldfld GameNetcodeStuff.PlayerControllerB GameNetworkManager::localPlayerController"
                    && codes[i + 2].ToString() == "call static bool UnityEngine.Object::op_Equality(UnityEngine.Object x, UnityEngine.Object y)")
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex > -1)
            {
                codes[startIndex].opcode = OpCodes.Call;
                codes[startIndex].operand = PatchesUtil.IsPlayerLocalOrLethalBotOwnerLocalMethod;
                codes[startIndex + 1].opcode = OpCodes.Nop;
                codes[startIndex + 1].operand = null;
                codes[startIndex + 2].opcode = OpCodes.Nop;
                codes[startIndex + 2].operand = null;
                startIndex = -1;
            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.EnemiesPatches.RadMechMissile.CheckCollision_Transpiler could not check if bots collide with missile.");
            }

            return codes.AsEnumerable();
        }
    }
}
