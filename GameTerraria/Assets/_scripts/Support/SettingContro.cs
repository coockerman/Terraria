using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingContro : MonoBehaviour
{
    public GameObject BoxSetting;
    public GameObject BoxThua;
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
}
