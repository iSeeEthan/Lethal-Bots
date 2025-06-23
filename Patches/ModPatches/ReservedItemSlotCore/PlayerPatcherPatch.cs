using GameNetcodeStuff;
using LethalBots.Managers;

namespace LethalBots.Patches.ModPatches.ReservedItemSlotCore
{
    public class PlayerPatcherPatch
    {
        public static bool InitializePlayerControllerLate_Prefix(PlayerControllerB __0)
        {
            if(LethalBotManager.Instance.IsPlayerLethalBot(__0))
            {
                return false;
            }

            return true;
        }

        public static bool CheckForChangedInventorySize_Prefix(PlayerControllerB __0)
        {
            if (LethalBotManager.Instance.IsPlayerLethalBot(__0))
            {
                return false;
            }

            return true;
        }
    }
}
