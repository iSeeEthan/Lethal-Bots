using HarmonyLib;
using LethalBots.Constants;
using LethalBots.Managers;
using Unity.Netcode;

namespace LethalBots.Patches.GameEnginePatches
{
    /// <summary>
    /// Patch for <c>NetworkSceneManager</c>
    /// </summary>
    [HarmonyPatch(typeof(NetworkSceneManager))]
    [HarmonyAfter(Const.MORECOMPANY_GUID)]
    [HarmonyBefore(Const.LETHALINTERNS_GUID)]
    public class NetworkSceneManagerPatch
    {
        /// <summary>
        /// Patch for populate the pool of bots at the start of the load scene
        /// </summary>
        [HarmonyPatch("PopulateScenePlacedObjects")]
        [HarmonyPostfix]
        public static void PopulateScenePlacedObjects_Postfix()
        {
            if (Plugin.IsModMoreCompanyLoaded)
            {
                UpdateIrlPlayerAfterMoreCompany();
            }

            LethalBotManager.Instance?.ManagePoolOfBots();
        }

        private static void UpdateIrlPlayerAfterMoreCompany()
        {
            Plugin.PluginIrlPlayersCount = MoreCompany.MainClass.newPlayerCount;
            Plugin.LogDebug($"PluginIrlPlayersCount after morecompany = {Plugin.PluginIrlPlayersCount}");
        }
    }
}
