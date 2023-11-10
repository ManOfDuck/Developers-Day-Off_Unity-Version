using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    //Hearts Set Up
    [SerializeField] private Sprite heartImage;
    private List<Image> images = new List<Image>();

    [Header("UI Docs")]
    [Tooltip("Reference to UIDocument object")]
    [SerializeField] private UIDocument UIDoc;
    [SerializeField] private UIDocument WinUIDoc;
    [SerializeField] private UIDocument LoseUIDoc;

    [Header("Sound Sources")]
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource loseSound;
    [SerializeField] private AudioSource BGMusic;

    //Timer Veriables    
    private bool timerStarted;
    private float timerDuration = 100f;

    //UI Variables
    private VisualElement heartsContainer;
    private Label timeLabel;

    public bool TimerStarted { get { return timerStarted; } }
    public float Duration { get { return timerDuration; } set { timerDuration = value; } }

    void Start()
    {
        //set up UI variables
        VisualElement root = UIDoc.rootVisualElement;
        timeLabel = root.Q<Label>("time-left");
        heartsContainer = root.Q<VisualElement>("lives");

        //hide win/lose screens
        WinUIDoc.rootVisualElement.style.visibility = Visibility.Hidden;
        LoseUIDoc.rootVisualElement.style.visibility = Visibility.Hidden;

        //display timer @ start
        timeLabel.text = "" + timerDuration;
        StartCoroutine(StartCountdown());

        //Add 3 lives
        AddLife(heartsContainer);
        AddLife(heartsContainer);
        AddLife(heartsContainer);
    }


    public void AddLife(VisualElement container)
    {
        Image heart = new Image();
        heart.sprite = heartImage;
        images.Add(heart);

        #region padding
        //make sure no padding is inherited from parent
        heart.style.paddingTop = 5;
        heart.style.paddingLeft = 0;
        heart.style.paddingRight = 0;
        //set hxw
        heart.style.width = 64;
        heart.style.height = 64;
        //no weird shrink/grow
        heart.style.flexGrow = 0;
        heart.style.flexShrink = 0;
        #endregion

        container.Add(heart);
    }

    public void LoseLife()
    {
        Image heart = new Image();
        heart.sprite = heartImage;
        Debug.Log("HI");
        heartsContainer.Remove(images[images.Count-1]);
        images.RemoveAt(images.Count-1); //remover from list as well
        Debug.Log("images List: " + images.Count);

        if (images.Count == 0)
        {
            Lose();
        }
    }

    public void Win()
    {
        WinUIDoc.rootVisualElement.style.visibility = Visibility.Visible;
        BGMusic.Stop();
        loseSound.Stop();
        winSound.Play();
        GameManager.Instance.GameWon();
    }
    public void Lose()
    {
        LoseUIDoc.rootVisualElement.style.visibility = Visibility.Visible;
        BGMusic.Stop();
        winSound.Stop();
        loseSound.Play();
        GameManager.Instance.GameOver();
    }

    protected IEnumerator StartCountdown()
    { 
        timerDuration = 0;
        float totalTime = 100f;

        while (totalTime >= timerDuration)
        {
            totalTime -= Time.deltaTime;
            //print timer to UI w/o decimals
            timeLabel.text = "" + totalTime.ToString("f0");
            yield return null;
        }
    }       
}
