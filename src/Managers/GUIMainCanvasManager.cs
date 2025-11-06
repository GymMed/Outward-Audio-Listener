using Mono.Cecil;
using OutwardAudioListener.Helpers;
using SideLoader.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OutwardAudioListener.Managers
{
    public class GUIMainCanvasManager : MonoBehaviour
    {
        private HashSet<GlobalAudioManager.Sounds> _availableSounds = new HashSet<GlobalAudioManager.Sounds>();

        private Dictionary<string, GlobalAudioManager.Sounds> _audioDictionary = new Dictionary<string, GlobalAudioManager.Sounds>();
        private SplitableSoundSource _playingSound;

        private Text _resultText;

        private Button _playSoundButton;
        private Button _stopSoundButton;
        private Button _stopAllSoundsButton;
        private Button _closeButton;

        private Dropdown _chooseAudioDropdown;

        private InputField _audioFilterInput;

        private int availableAudioCount;

        public Text ResultText { get => _resultText; set => _resultText = value; }

        public HashSet<GlobalAudioManager.Sounds> AvailableSounds { get => _availableSounds; set => _availableSounds = value; }
        public Dictionary<string, GlobalAudioManager.Sounds> AudioDictionary { get => _audioDictionary; set => _audioDictionary = value; }
        public Button PlaySoundButton { get => _playSoundButton; set => _playSoundButton = value; }
        public Button StopSoundButton { get => _stopSoundButton; set => _stopSoundButton = value; }
        public Button CloseButton { get => _closeButton; set => _closeButton = value; }
        public Dropdown ChooseAudioDropdown { get => _chooseAudioDropdown; set => _chooseAudioDropdown = value; }
        public InputField AudioFilterInput { get => _audioFilterInput; set => _audioFilterInput = value; }
        public int AvailableAudioCount { get => availableAudioCount; set => availableAudioCount = value; }
        public SplitableSoundSource PlayingSound { get => _playingSound; set => _playingSound = value; }
        public Button StopAllSoundsButton { get => _stopAllSoundsButton; set => _stopAllSoundsButton = value; }

        private void Awake()
        {
            try 
            {
                Init();
            }
            catch(Exception ex)
            {
                AudioListener.LogMessage("GUIMainCanvasManager error: " + ex.Message);
            }
        }

        public void Init()
        {
            string mainPanelPath = "Background-Panel/Main-Panel/";

            ResultText = this.transform.Find(mainPanelPath + "Lower-Panel/Result-Text")?.GetComponent<Text>();

            if(ResultText == null)
            {
                AudioListener.LogMessage("Couldn't find Result Text component");
            }

            Transform headerPanelTransform = this.transform.Find(mainPanelPath + "Header-Panel/");

            if (headerPanelTransform == null)
            {
                ResultAndLogMessage("Couldn't find GUI header");
            }
            else
            {
                headerPanelTransform.gameObject.AddComponent<DragParent>();

                CloseButton = headerPanelTransform.Find("Close-Button")?.GetComponent<Button>();

                if (CloseButton == null)
                {
                    ResultAndLogMessage("Couldn't find Close Button component");
                }
                else
                {
                    CloseButton.onClick.AddListener(() =>
                    {
                        this.HideCanvas();
                    });
                }
            }

            PlaySoundButton = this.transform.Find(mainPanelPath + "Middle-Panel/Buttons-Panel/Play-Button")?.GetComponent<Button>();

            if(PlaySoundButton == null)
            {
                ResultAndLogMessage("Couldn't find play audio button component");
            }
            else
            {
                PlaySoundButton.onClick.AddListener(() => HandleOnPlayButtonClick());
            }

            StopSoundButton = this.transform.Find(mainPanelPath + "Middle-Panel/Buttons-Panel/Stop-Button")?.GetComponent<Button>();

            if(StopSoundButton == null)
            {
                ResultAndLogMessage("Couldn't find stop audio button component");
            }
            else
            {
                StopSoundButton.onClick.AddListener(() => HandleOnStopButtonClick());
            }

            StopAllSoundsButton = this.transform.Find(mainPanelPath + "Middle-Panel/StopAll-Button")?.GetComponent<Button>();

            if(StopAllSoundsButton == null)
            {
                ResultAndLogMessage("Couldn't find stop all audio button component");
            }
            else
            {
                StopAllSoundsButton.onClick.AddListener(() => HandleOnStopAllButtonClick());
            }

            ChooseAudioDropdown = this.transform.Find(mainPanelPath + "Middle-Panel/AudioPicker-Panel/AudioPick-Dropdown")?.GetComponent<Dropdown>();
            
            if(ChooseAudioDropdown == null)
            {
                ResultAndLogMessage("Couldn't find Audio Dropdown component");
            }

            AudioFilterInput = this.transform.Find(mainPanelPath + "Middle-Panel/AudioFilter-Panel/AudioFilter-Input")?.GetComponent<InputField>();

            if(AudioFilterInput == null)
            {
                ResultAndLogMessage("Couldn't find Item Filter Input component");
            }
            else
            {
                AudioFilterInput.onEndEdit.AddListener(HandleOnAudioFilterEnd);
            }
        }

        public void HandleOnAudioFilterEnd(string text)
        {
            FilterAudioData(text);           
        }

        public void HandleOnStopAllButtonClick()
        {
            try
            {
                //Global.AudioManager.CleanUpMusic();
                AudioHelper.CleanUpAllMusic();

                this.ResultMessage($"Successfully cleaned all music!");
            }
            catch(Exception ex) 
            {
                this.ResultAndLogMessage("Audio listener error: " + ex.Message);
            }
        }

        public void HandleOnStopButtonClick()
        {
            try
            {
                GlobalAudioManager.Sounds sound = GlobalAudioManager.Sounds.NONE;
                if (PlayingSound != null)
                {
                    sound = PlayingSound.Sound;
                    PlayingSound.StopSound();
                }

                this.ResultMessage($"Successfully stopped audio! \nAudio: {ChooseAudioDropdown.options[ChooseAudioDropdown.value].text} \n" + 
                    $"Sound Enum: GlobalAudioManager.Sounds.{sound}");
            }
            catch(Exception ex) 
            {
                this.ResultAndLogMessage("Audio listener error: " + ex.Message);
            }
        }

        public void HandleOnPlayButtonClick()
        {
            if(ChooseAudioDropdown == null)
            {
                this.ResultAndLogMessage("Couldn't find audio selection dropdown!");
                return;
            }

            if(ChooseAudioDropdown.options == null || ChooseAudioDropdown.options.Count < 1)
            {
                this.ResultAndLogMessage($"Couldn't generate audio options in dropdown! Available audio count: {AvailableAudioCount}");
                return;
            }

            string selectedItemValue = ChooseAudioDropdown.options[ChooseAudioDropdown.value].text;

            if(!AudioDictionary.TryGetValue(selectedItemValue, out GlobalAudioManager.Sounds sound))
            {
                this.ResultAndLogMessage("Tried to play audio without providing sound!");
                return;
            }

            try
            {
                if (PlayingSound != null)
                {
                    PlayingSound.StopSound();
                }

                PlayingSound = Global.AudioManager.PlaySound(sound);

                this.ResultMessage($"Successfully started audio! \nAudio: {ChooseAudioDropdown.options[ChooseAudioDropdown.value].text} \n" + 
                    $"Sound Enum: GlobalAudioManager.Sounds.{sound}");
            }
            catch(Exception ex) 
            {
                this.ResultAndLogMessage("Audio listener error: " + ex.Message);
            }
        }

        public void FilterAudioData(string filter)
        {
            string previousValue = "";
            int selectionValue = 0;

            if (ChooseAudioDropdown.options.Count > 0)
                previousValue = ChooseAudioDropdown.options[ChooseAudioDropdown.value].text;

            this.ResultMessage("Loading Sounds . . .");
            ChooseAudioDropdown.interactable = false;

            // Start coroutine for filtering and populating dropdown incrementally
            StartCoroutine(FilterAudioCoroutine(filter, previousValue, selectionValue));
        }

        private IEnumerator FilterAudioCoroutine(string filter, string previousValue, int selectionValue, int chunkSize = 50)
        {
            List<string> dropdownOptions = new List<string>();
            Dictionary<string, GlobalAudioManager.Sounds> localAudioDictionary = new Dictionary<string, GlobalAudioManager.Sounds>();
            HashSet<GlobalAudioManager.Sounds> filteredAudio = AvailableSounds;

            if (!string.IsNullOrEmpty(AudioFilterInput.text))
                filteredAudio = AudioHelper.GetAudioByFilter(AudioFilterInput.text, AvailableSounds);

            int index = 0;
            int availableCount = filteredAudio.Count;

            foreach (var sound in filteredAudio)
            {
                string keyName = sound.ToString();

                if (AudioHelper.ContainsIgnoreCase(keyName, filter))
                {
                    if (!string.IsNullOrEmpty(previousValue) && string.Equals(keyName, previousValue, StringComparison.OrdinalIgnoreCase))
                    {
                        selectionValue = dropdownOptions.Count;
                    }

                    dropdownOptions.Add(keyName);
                    localAudioDictionary[keyName] = sound;
                }

                index++;

                // Yield after processing a chunk to avoid freezing
                if (index % chunkSize == 0)
                    yield return null;
            }

            AudioDictionary = localAudioDictionary;
            AvailableAudioCount = availableCount;

            // Populate dropdown incrementally
            yield return StartCoroutine(FillDropdownCoroutine(ChooseAudioDropdown, dropdownOptions));

            ChooseAudioDropdown.value = selectionValue;
            ChooseAudioDropdown.RefreshShownValue();
            ChooseAudioDropdown.onValueChanged.Invoke(ChooseAudioDropdown.value);

            this.ResultMessage("Finished Loading Sounds!");
            ChooseAudioDropdown.interactable = true;
        }

        private IEnumerator FillDropdownCoroutine(Dropdown dropdown, List<string> options, int chunkSize = 50)
        {
            dropdown.ClearOptions();
            int index = 0;

            while (index < options.Count)
            {
                int count = Mathf.Min(chunkSize, options.Count - index);
                dropdown.AddOptions(options.GetRange(index, count));
                index += count;

                // Wait one frame to prevent freezing
                yield return null;
            }

            dropdown.value = 0;
            dropdown.RefreshShownValue();
            dropdown.onValueChanged.Invoke(dropdown.value);
            this.ResultMessage("Finished Loading Audio!");
            dropdown.interactable = true;
        }

        public void FillAudioData()
        {
            try
            {
                this.ResultMessage("Loading Audio . . .");
                ChooseAudioDropdown.interactable = false;
                AudioListener.LogMessage($"Available Sounds: {AvailableSounds.Count}.");

                // Prepare data
                List<string> dropdownOptions = new List<string>();
                Dictionary<string, GlobalAudioManager.Sounds> localAudioDictionary = new Dictionary<string, GlobalAudioManager.Sounds>();

                foreach (GlobalAudioManager.Sounds sound in AvailableSounds)
                {
                    string keyName = sound.ToString();
                    dropdownOptions.Add(keyName);
                    localAudioDictionary[keyName] = sound;
                }

                AudioDictionary = localAudioDictionary;
                AvailableAudioCount = localAudioDictionary.Count;

                // Start coroutine to populate dropdown incrementally
                StartCoroutine(FillDropdownCoroutine(ChooseAudioDropdown, dropdownOptions));
            }
            catch (Exception ex) 
            {
                AudioListener.LogMessage("GUIMainCanvasManager@FillAudioData error: " + ex.Message);
            }
        }

        public void FillDropdownChoices(Dropdown dropdown, List<string> options)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
        }

        public void ResultAndLogMessage(string message)
        {
            AudioListener.LogMessage(message);
            ResultMessage(message);
        }

        public void ResultMessage(string message)
        {
            if(ResultText == null)
            {
                AudioListener.LogMessage($"Tried to display result message: \"{message}\" on null UI Result reference.");
                return;
            }

            ResultText.text = message;
        }

        public void HideCanvas()
        {
            this.gameObject.SetActive(false);
            ForceUnlockCursor.RemoveUnlockSource();
        }
        
        public void ShowCanvas() 
        {
            this.gameObject.SetActive(true);
            ForceUnlockCursor.AddUnlockSource();
        }
    }
}