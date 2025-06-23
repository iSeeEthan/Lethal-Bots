using GameNetcodeStuff;
using LethalBots.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LethalBots.Patches.ModPatches.ButteryFixes
{
    public class BodyPatchesPatch
    {
        public static bool DeadBodyInfoPostStart_Prefix(DeadBodyInfo __0)
        {
            if (__0.playerScript != null 
                && LethalBotManager.Instance.IsPlayerLethalBot(__0.playerScript))
            {
                return false;
            }
            return true;
        }
    }
}
