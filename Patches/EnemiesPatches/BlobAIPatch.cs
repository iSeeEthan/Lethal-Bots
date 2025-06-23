using HarmonyLib;
using LethalBots.Constants;
using LethalBots.Managers;
using LethalBots.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalBots.Patches.EnemiesPatches
{
    /// <summary>
    /// Patch for the <c>BlobAI</c>
    /// </summary>
    [HarmonyPatch(typeof(BlobAI))]
    [HarmonyAfter(Const.MORECOMPANY_GUID)]
    public class BlobAIPatch
    {
        [HarmonyPatch("OnCollideWithPlayer")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> OnCollideWithPlayer_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);

            // ----------------------------------------------------------------------
            for (var i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].ToString().StartsWith("ldloc.0 NULL") // 30
                    && codes[i + 1].ToString().StartsWith("ldc.i4.s 35"))
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex > -1)
            {
                codes[startIndex + 1].opcode = OpCodes.Nop;
                codes[startIndex + 1].operand = null;

                List<CodeInstruction> codesToAdd = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Callvirt, PatchesUtil.GetDamageFromSlimeIfLethalBotMethod),
                };
                codes.InsertRange(startIndex + 1, codesToAdd);
                startIndex = -1;
            }
            else
            {
                Plugin.LogError($"LethalBot.Patches.EnemiesPatches.BlobAIPatch.OnCollideWithPlayer_Transpiler could not change default damage to bot.");
            }

            return codes.AsEnumerable();
        }
    }
}
