using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private UIDocument UIDoc;

    private VisualElement root;
    private Slider masterVolSlider;
    private Slider musicVolSlider;
    private Slider sfxVolSlider;

    private Button backButton;


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

        masterVolSlider = root.Q<Slider>("MasterVol");
        musicVolSlider = root.Q<Slider>("MusicVol");
        sfxVolSlider = root.Q<Slider>("SFXVol");

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

    }
    public void OnMusicSliderValueChange(ChangeEvent<float> evt)
    {

    }
    public void OnSfxSliderValueChange(ChangeEvent<float> evt)
    {

    }
}
