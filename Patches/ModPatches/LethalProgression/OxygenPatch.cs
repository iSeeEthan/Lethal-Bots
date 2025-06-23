using GameNetcodeStuff;
using LethalBots.Managers;

namespace LethalBots.Patches.ModPatches.LethalProgression
{
    public class OxygenPatch
    {
        public static bool EnteredWater_Prefix(PlayerControllerB __0)
        {
            if (LethalBotManager.Instance.IsPlayerLethalBot(__0))
            {
                return false;
            }

            return true;
        }

        public static bool LeftWater_Prefix(PlayerControllerB __0)
        {
            if (LethalBotManager.Instance.IsPlayerLethalBot(__0))
            {
                return false;
            }

            return true;
        }

        public static bool ShouldDrown_Prefix(PlayerControllerB __0)
        {
            if (LethalBotManager.Instance.IsPlayerLethalBot(__0))
            {
                return false;
            }

            return true;
        }

        public static bool OxygenUpdate_Prefix(PlayerControllerB __0)
        {
            if (LethalBotManager.Instance.IsPlayerLethalBot(__0))
            {
                return false;
            }

            return true;
        }
    }
}
