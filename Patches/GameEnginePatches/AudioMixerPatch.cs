using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.Managers;
using UnityEngine;
using UnityEngine.Audio;

namespace LethalBots.Patches.GameEnginePatches
{
    [HarmonyPatch(typeof(AudioMixer))]
    public class AudioMixerPatch
    {
        [HarmonyPatch("SetFloat")]
        [HarmonyPrefix]
        public static bool SetFloat_Prefix(string name, float value)
        {
            //Ignore volume for now! !name.StartsWith("PlayerVolume")
            if (!name.StartsWith("PlayerPitch"))
            {
                return true;
            }

            string onlyNumberName = name.Replace("PlayerVolume", "").Replace("PlayerPitch", "");
            int playerObjectNumber = int.Parse(onlyNumberName);
            PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerObjectNumber];
            if (playerControllerB == null 
                || !LethalBotManager.Instance.IsPlayerLethalBot(playerControllerB))
            {
                return true;
            }

            AudioSource voiceSource = playerControllerB.currentVoiceChatAudioSource;
            if (voiceSource != null)
            {
                if (name.StartsWith("PlayerVolume"))
                {
                    voiceSource.volume = value / 16f;
                }
                else if (name.StartsWith("PlayerPitch"))
                {
                    voiceSource.pitch = value;
                }
            }
            return false;
        }
    }
}
