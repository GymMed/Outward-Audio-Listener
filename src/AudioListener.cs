using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using OutwardAudioListener.Helpers;
using OutwardAudioListener.Managers;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// RENAME 'OutwardAudioListener' TO SOMETHING ELSE
namespace OutwardAudioListener
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class AudioListener : BaseUnityPlugin
    {
        // Choose a GUID for your project. Change "myname" and "mymod".
        public const string GUID = "gymmed.outward_audio_listener";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "Audio Listener";
        // Increment the VERSION when you release a new version of your mod.
        public const string VERSION = "0.0.1";

        public static string prefix = "[GymMed-Audio-Listener]";

        public const string GUI_SHOW = "Audio Listener GUI Show/Hide";

        internal static ManualLogSource Log;

        // If you need settings, define them like so:
        //public static ConfigEntry<bool> ExampleConfig;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"Hello world from {NAME} {VERSION}!");

            // Any config settings you define should be set up like this:
            //ExampleConfig = Config.Bind("ExampleCategory", "ExampleSetting", false, "This is an example setting.");
            CustomKeybindings.AddAction(GUI_SHOW, KeybindingsCategory.CustomKeybindings, ControlType.Both);

            SL.OnPacksLoaded += ()=>
            {
                try
                {
                    GUIManager.Instance.CreateCanvas();
                    FillAudiosToGUI();
                }
                catch(Exception ex)
                {
                    LogMessage($"We encountered a problem: {ex.Message}");
                }
            };

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            new Harmony(GUID).PatchAll();
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {
            if (!CustomKeybindings.GetKeyDown(GUI_SHOW))
            {
                return;
            }
            GUIMainCanvasManager GUICanvasManager = GUIManager.Instance?.MainCanvasManager;

            if(GUICanvasManager == null)
            {
                LogMessage("Can't access null! Make sure GUI Canvas exist!");
                return;
            }

            if(GUICanvasManager.gameObject.activeSelf)
            {
                GUICanvasManager.HideCanvas();
            }
            else
            {
                GUICanvasManager.ShowCanvas();
                GUICanvasManager.transform.SetAsLastSibling();
            }
        }

        public static void LogMessage(string message)
        {
            Log.LogMessage(AudioListener.prefix + " " + message);
        }

        public static void FillAudiosToGUI()
        {
            try
            {
                #if DEBUG
                LogMessage("OutwardAudioListener@FillSpawnableItemToGUI called!");
                LogMessage($"OutwardAudioListener@FillSpawnableItemToGUI GUICanvasManager exist: {GUIManager.Instance?.MainCanvasManager == null}");
                #endif

                GUIMainCanvasManager GUICanvasManager = GUIManager.Instance?.MainCanvasManager;

                if (GUICanvasManager == null)
                {
                    LogMessage("Can't access null! Make sure GUI Canvas exist!");
                    return;
                }

                HashSet<GlobalAudioManager.Sounds> sounds = AudioHelper.GetAllGameSounds();

                GUICanvasManager.AvailableSounds = sounds;
                GUICanvasManager.FillAudioData();

                GUICanvasManager.transform.SetAsLastSibling();

                LogMessage("Success with GUI Canvas creation!");
            }
            catch (Exception ex)
            { 
                AudioListener.LogMessage("OutwardAudioListener@FillAudiosToGUI error: " + ex.Message);
            }
        }
    }
}
