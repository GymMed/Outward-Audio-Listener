using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardAudioListener.Helpers
{
    public static class AudioHelper
    {
        public static HashSet<GlobalAudioManager.Sounds> GetAllGameSounds()
        {
            HashSet<GlobalAudioManager.Sounds> sounds = new HashSet<GlobalAudioManager.Sounds>();

            foreach(GlobalAudioManager.Sounds sound in Enum.GetValues(typeof(GlobalAudioManager.Sounds)))
            {
                sounds.Add(sound);
            }

            return sounds;
        }

        public static HashSet<GlobalAudioManager.Sounds> GetAudioByFilter(string filter, HashSet<GlobalAudioManager.Sounds> availableAudio)
        {
            HashSet<GlobalAudioManager.Sounds> filteredSounds = new HashSet<GlobalAudioManager.Sounds>();

            foreach(GlobalAudioManager.Sounds sound in availableAudio)
            {
                if (ContainsIgnoreCase(sound.ToString(), filter))
                    filteredSounds.Add(sound);
            }

            return filteredSounds;
        }

        public static  void CleanUpAllMusic()
        {
            for (int i = 0; i < GlobalAudioManager.s_musicSources.Values.Count; i++)
            {
                UnityEngine.Object.Destroy(GlobalAudioManager.s_musicSources.Values[i].Source.gameObject);
                GlobalAudioManager.s_musicSources.Remove(GlobalAudioManager.s_musicSources.Keys[i]);
                i--;
            }
        }

        public static bool ContainsIgnoreCase(string source, string toCheck)
        {
            return source?.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
