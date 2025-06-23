using GameNetcodeStuff;
using LethalBots.Managers;

namespace LethalBots.Patches.ModPatches.LethalProgression
{
    public class HPRegenPatch
    {
        public static bool HPRegenUpdate_Prefix(PlayerControllerB __0)
        {
            if (LethalBotManager.Instance.IsPlayerLethalBot(__0))
            {
                return false;
            }

            return true;
        }
    }
}
