using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingContro : MonoBehaviour
{
    public GameObject BoxSetting;
    public GameObject BoxThua;
    public AudioSource[] audioSource;
    public GameObject btnMusic;
    public Color OnMusicColor;
    public Color OffMusicColor;
    bool isMusic = true;

    private void Start()
    {
        Time.timeScale = 1f;
    }
    public void OnBoxSetting()
    {
        Time.timeScale = 0;
        BoxSetting.SetActive(true);
    }
    public void OffBoxSetting()
    {
        Time.timeScale = 1;
        BoxSetting.SetActive(false);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void OnBoxThua()
    {
        Time.timeScale = 0;
        BoxThua.SetActive(true);
    }
    void SetUpUIMusic()
    {
        if(isMusic)
        {
            btnMusic.GetComponent<Image>().color = OnMusicColor;
            foreach (AudioSource audio in audioSource)
            {
                audio.volume = 1;
            }
        }
        else
        {
            btnMusic.GetComponent<Image>().color = OffMusicColor;
            foreach (AudioSource audio in audioSource)
            {
                audio.volume = 0;
            }
        }
    }
    public void ChangeStatusMusic()
    {
        isMusic = !isMusic;
        SetUpUIMusic();
    }
}
