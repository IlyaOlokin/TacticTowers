using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private List<GameObject> towers;
    [SerializeField] private Text creditsCount;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private AudioMixer audioMixer;
    private bool isForRestart;
    public Slider soundSlider;
    public Slider musicSlider;

    public void OnButtonRestart()
    {
        isForRestart = true;
        ActivateConfirmPanel(isForRestart);
        AudioManager.Instance.Play("ButtonClick1");

    }
    
    public void OnButtonMenu()
    {
        isForRestart = false;
        ActivateConfirmPanel(isForRestart);
        AudioManager.Instance.Play("ButtonClick2");

    }

    private void ActivateConfirmPanel(bool isForRestart)
    {
        pausePanel.SetActive(false);
        confirmPanel.transform.Find("CreditsCount").transform.Find("Count").GetComponent<Text>().text = creditsCount.text;
        //confirmButton.transform.Find("Header").GetComponent<TextLocaliser>().SetKey(isForRestart ? "menuRestartButton" : "menuMenuButton"); 
        confirmButton.transform.Find("Text").GetComponent<TextLocaliser>().SetKey(isForRestart ? "menuRestartButton" : "menuMenuButton"); 
        confirmPanel.SetActive(true);
    }

    public void OnButtonClose()
    {
        Resume();
        AudioManager.Instance.Play("ButtonClick1");
    }

    public void OnButtonCancel()
    {
        confirmPanel.SetActive(false);
        pausePanel.SetActive(true);
        AudioManager.Instance.Play("ButtonClick2");

    }

    public void OnButtonPause()
    {
        Pause();
    }

    public void OnButtonContinue()
    {
        Resume();
        AudioManager.Instance.Play("ButtonClick1");
        Credits.LoseSessionCredits();
        SceneManager.LoadScene(isForRestart ? SceneManager.GetActiveScene().name : "MainMenu");
    }
    
    private void Pause()
    {
        TimeManager.Pause(audioMixer);
        pausePanel.SetActive(true);
        AudioManager.Instance.Play("ButtonClick2");
    }

    private void Resume()
    {
        TimeManager.Resume(audioMixer);
        pausePanel.SetActive(false);
        foreach (var tower in towers)
            tower.GetComponent<CircleCollider2D>().enabled = true;
    }

    private void Start()
    {
        Resume();
    }

    private void OnEnable()
    {
        soundSlider.value = float.Parse(DataLoader.LoadString("SoundVolume", "1"));
        musicSlider.value = float.Parse(DataLoader.LoadString("MusicVolume", "1"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeInHierarchy)
            {
                Resume();
            }
            else if (!pausePanel.activeInHierarchy)
            {
                if (towers.Any(tower =>  tower.GetComponent<TowerDrag>().needToDrop) || Time.timeScale == 0) return;
                Pause();
            }
        }
    }

    public void ChangeMusicVolume()
    {
        if (musicSlider.value == 0) audioMixer.SetFloat("MusicVol", -80.0f);
        else audioMixer.SetFloat("MusicVol", -20.0f + (20.0f * musicSlider.value));
        DataLoader.SaveString("MusicVolume", musicSlider.value.ToString());
    }

    public void ChangeSoundVolume()
    {
        if (soundSlider.value == 0) audioMixer.SetFloat("SoundVol", -80.0f);
        else audioMixer.SetFloat("SoundVol", -20.0f + (20.0f * soundSlider.value));
        DataLoader.SaveString("SoundVolume", soundSlider.value.ToString());
    }
}
