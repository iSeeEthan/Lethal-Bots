using HarmonyLib;
using LethalBots.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace LethalBots.Patches.ModPatches.AdditionalNetworking
{
    public class AdditionalNetworkingPatch
    {
        public static IEnumerable<CodeInstruction> Start_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var startIndex = 0;
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            // ----------------------------------------------------------------------
            Label labelToJumpTo = generator.DefineLabel();
            codes[startIndex].labels.Add(labelToJumpTo);

            List<CodeInstruction> codesToAdd = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PatchesUtil.IsPlayerLethalBotMethod),
                new CodeInstruction(OpCodes.Brfalse, labelToJumpTo),
                new CodeInstruction(OpCodes.Ret, null)
            };
            codes.InsertRange(startIndex, codesToAdd);

            return codes.AsEnumerable();
        }
    }
}
