using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private UIDocument UIDoc;

    private VisualElement root;
    private Slider masterVolSlider;
    private Slider musicVolSlider;
    private Slider sfxVolSlider;

    private Button backButton;

    [SerializeField] AudioMixerGroup masterMixerGroup;
    [SerializeField] AudioMixerGroup musicMixerGroup;
    [SerializeField] AudioMixerGroup sfxMixerGroup;

    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    // Start is called before the first frame update
    void Start()
    {
        root = UIDoc.rootVisualElement;
        root.style.visibility = Visibility.Hidden;

        backButton = root.Q<Button>("Quit");

        backButton.clicked += backClicked;

        masterVolSlider = root.Q<Slider>("MasterVolSlider");
        musicVolSlider = root.Q<Slider>("MusicVolSlider");
        sfxVolSlider = root.Q<Slider>("SoundVolSlider");

        masterVolSlider.RegisterValueChangedCallback(OnMasterSliderValueChange);
        musicVolSlider.RegisterValueChangedCallback(OnMusicSliderValueChange);
        sfxVolSlider.RegisterValueChangedCallback(OnSfxSliderValueChange);

    }

    public void showMenu()
    {
        root.style.visibility = Visibility.Visible;
    }

    public void backClicked()
    {
        root.style.visibility = Visibility.Hidden;
    }

    public void OnMasterSliderValueChange(ChangeEvent<float> evt)
    {
        masterVolume = evt.newValue;
        masterMixerGroup.audioMixer.SetFloat("MasterVolParam", evt.newValue);
        Debug.Log("new master volume is " + evt.newValue);
    }
    public void OnMusicSliderValueChange(ChangeEvent<float> evt)
    {
        musicVolume = evt.newValue;
        musicMixerGroup.audioMixer.SetFloat("MusicVolParam", evt.newValue);
        Debug.Log("new music volume is " + evt.newValue);
    }
    public void OnSfxSliderValueChange(ChangeEvent<float> evt)
    {
        sfxVolume = evt.newValue;
        sfxMixerGroup.audioMixer.SetFloat("SfxVolParam", evt.newValue);
        Debug.Log("new sfx volume is " + evt.newValue);
    }
}
