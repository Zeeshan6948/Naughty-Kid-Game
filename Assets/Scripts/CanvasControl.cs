using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasControl : MonoBehaviour
{
    public GameObject[] m_Panels;
    bool Paused;
    public Text TimerText;
    public int m_CurrentPanel;
    public GameObject Messenger;
    public Text MessageText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void OpenSpecficPanel(int number)
    {
        m_Panels[m_CurrentPanel].SetActive(false);
        m_CurrentPanel = number;
        m_Panels[m_CurrentPanel].SetActive(true);
    }

    public void PausedBtnFun()
    {
        Paused = !Paused;
        if (Paused)
        {
            OpenSpecficPanel(1);
            Time.timeScale = 0;
            AudioListener.volume = 0;
        }
        else
        {
            OpenSpecficPanel(0);
            Time.timeScale = 1;
            AudioListener.volume = 1;
        }
    }

    public void ReloadBtnFun()
    {
        bl_SceneLoaderUtils.GetLoader.LoadLevel("Game");
        Time.timeScale = 1;
        AudioListener.volume = 1;
    }

    public void NextBtnFun()
    {
        PlayerPrefs.SetInt("LevelSelected", PlayerPrefs.GetInt("LevelSelected")+1);
        AudioListener.volume = 1;
        bl_SceneLoaderUtils.GetLoader.LoadLevel("Game");
        Time.timeScale = 1;
    }

    public void HomeBtnFun()
    {
        AudioListener.volume = 1;
        bl_SceneLoaderUtils.GetLoader.LoadLevel("Main");
        Time.timeScale = 1;
    }

    public void WatchAdBtn()
    {
        AdsManager.Instance.functioncalling(this.gameObject, "RewardReturn");
        AdsManager.Instance.ShowUnityRewarded();
    }

    void RewardReturn()
    {
        PlayerPrefs.SetInt("TotalCoins", PlayerPrefs.GetInt("TotalCoins") + 500);
    }

    public void PlayerMessage(string Message)
    {
        MessageText.GetComponent<Typer>().message = Message;
        MessageText.GetComponent<Typer>().enabled = true;
        Messenger.gameObject.SetActive(true);
    }
}
