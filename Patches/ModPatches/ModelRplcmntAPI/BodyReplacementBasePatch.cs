using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.AI;
using LethalBots.Managers;
using ModelReplacement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LethalBots.Patches.ModPatches.ModelRplcmntAPI
{
    [HarmonyPatch(typeof(BodyReplacementBase))]
    public class BodyReplacementBasePatch
    {
        public static List<BodyReplacementBase> ListBodyReplacementOnDeadBodies = new List<BodyReplacementBase>();

        [HarmonyPatch("LateUpdate")]
        [HarmonyPrefix]
        static bool LateUpdate_Prefix(BodyReplacementBase __instance, ref GameObject ___replacementDeadBody)
        {
            LethalBotAI? lethalBotAI = LethalBotManager.Instance.GetLethalBotAI((int)__instance.controller.playerClientId);
            if (lethalBotAI == null)
            {
                return true;
            }

            if (__instance.controller.deadBody != null
                && !ListBodyReplacementOnDeadBodies.Contains(__instance))
            {
                ListBodyReplacementOnDeadBodies.Add(__instance);
                __instance.viewState.ReportBodyReplacementRemoval();
                __instance.cosmeticAvatar = __instance.ragdollAvatar;
                CreateAndParentRagdoll_ReversePatch(__instance, __instance.controller.deadBody);
                lethalBotAI.LethalBotIdentity.BodyReplacementBase = __instance;
            }

            if (ListBodyReplacementOnDeadBodies.Contains(__instance))
            {
                //Plugin.LogDebug($"{internAI.NpcController.Npc.playerUsername} {__instance.GetInstanceID()} only ragdoll update, {__instance.controller.deadBody}");
                __instance.ragdollAvatar.Update();
                return false;
            }

            //Plugin.LogDebug($"{internAI.NpcController.Npc.playerUsername} {__instance.GetInstanceID()} all update");
            return true;
        }


        [HarmonyPatch("CreateAndParentRagdoll")]
        [HarmonyReversePatch(type: HarmonyReversePatchType.Snapshot)]
        [HarmonyPriority(Priority.Last)]
        public static void CreateAndParentRagdoll_ReversePatch(object instance, DeadBodyInfo bodyinfo) => throw new NotImplementedException("Stub LethalBot.Patches.ModPatches.ModelRplcmntAPI.BodyReplacementBasePatch.CreateAndParentRagdoll_ReversePatch");

        [HarmonyPatch("GetBounds")]
        [HarmonyPrefix]
        public static bool GetBounds_Prefix(BodyReplacementBase __instance, GameObject model, ref Bounds __result)
        {
            LethalBotAI? lethalBot = LethalBotManager.Instance.GetLethalBotAI(__instance.controller);
            if (lethalBot == null)
            {
                return true;
            }

            __result = lethalBot.NpcController.GetBoundsTimedCheck.GetBoundsModel(model);
            return false;
        }
    }
}
