using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Analytics;

public class MainMenuManager : MonoBehaviour
{
    public GameObject[] m_Panels;
    public int m_CurrentPanel;
    public List<int> PanelFlow;
    public Text CoinsText;
    public Button MainMusicBtn;
    public Button MainSoundBtn;
    public Button SettingMusicBtn;
    public Button SettingSoundBtn;
    public AudioSource MusicSource;
    public AudioSource SoundSource;
    public AudioSource BtnClickSound;
    public GameObject BarProfBtn;
    public GameObject BarBackBtn;
    public GameObject m_LevelObject;

    void Start()
    {
        m_CurrentPanel = 1;
        OpenSpecficPanel(1);
        BarValueUpdate();
        if (AdsManager.Instance)
        {
            AdsManager.Instance.ShowBannerAd();
            AdsManager.Instance.HideBanner2Ad();
            FirebaseAnalytics.LogEvent("GameOpened");
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void BackBtnFun()
    {
        int temp = PanelFlow.IndexOf(m_CurrentPanel);
        OpenSpecficPanel(PanelFlow[temp - 1]);
        BarValueUpdate();
        BtnClickSound.Play();
        if (m_CurrentPanel == 1)
        {
            PanelFlow.Clear();
            PanelFlow.Add(1);
        }
    }

    void OpenSpecficPanel(int number)
    {
        m_Panels[m_CurrentPanel].SetActive(false);
        m_CurrentPanel = number;
        m_Panels[m_CurrentPanel].SetActive(true);
        PanelFlow.Add(number);
        if (m_CurrentPanel == 1)
        {
            BarProfBtn.SetActive(true);
            BarBackBtn.SetActive(false);
        }
        else
        {
            BarProfBtn.SetActive(false);
            BarBackBtn.SetActive(true);
        }
        if (AdsManager.Instance)
            AdsManager.Instance.MediationAd();
    }

    public void MainMenuSinglePlayer()
    {
        OpenSpecficPanel(2);
        BtnClickSound.Play();
        LevelLockingUnlocking();
    }
    void LevelLockingUnlocking()
    {
        for (int i = 1; i < m_LevelObject.transform.childCount; i++)
        {
            if (i <= PlayerPrefs.GetInt("LevelCompleted"))
            {
                m_LevelObject.transform.GetChild(i).GetComponent<Button>().interactable = true;
                m_LevelObject.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                m_LevelObject.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                m_LevelObject.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
        }
    }

    public void MainMenuShare()
    {
        BtnClickSound.Play();
    }

    public void MainMenuMusic()
    {
        if (MusicSource.volume == 0)
        {
            MainMusicBtn.GetComponent<Image>().sprite = MainMusicBtn.spriteState.pressedSprite;
            SettingMusicBtn.GetComponent<Image>().sprite = SettingMusicBtn.spriteState.pressedSprite;
            MusicSource.volume = 1;
        }
        else
        {
            MainMusicBtn.GetComponent<Image>().sprite = MainMusicBtn.spriteState.disabledSprite;
            SettingMusicBtn.GetComponent<Image>().sprite = SettingMusicBtn.spriteState.disabledSprite;
            MusicSource.volume = 0;
        }
        BtnClickSound.Play();
    }

    public void MainMenuSound()
    {
        if (SoundSource.volume == 0)
        {
            MainSoundBtn.GetComponent<Image>().sprite = MainSoundBtn.spriteState.pressedSprite;
            SettingSoundBtn.GetComponent<Image>().sprite = SettingSoundBtn.spriteState.pressedSprite;
            SoundSource.volume = 1;
        }
        else
        {
            MainSoundBtn.GetComponent<Image>().sprite = MainSoundBtn.spriteState.disabledSprite;
            SettingSoundBtn.GetComponent<Image>().sprite = SettingSoundBtn.spriteState.disabledSprite;
            SoundSource.volume = 0;
        }
        BtnClickSound.Play();
    }
    public void MainMenuRateUS()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
        BtnClickSound.Play();
    }

    public void BarProfileBtn()
    {
        BtnClickSound.Play();
    }

    public void BarFreeGift()
    {
        BtnClickSound.Play();
        if (AdsManager.Instance)
        {
            AdsManager.Instance.functioncalling(this.gameObject, "RewardofFree");
            AdsManager.Instance.ShowUnityRewarded();
        }
    }

    public void BarFreeCoins()
    {
        BtnClickSound.Play();
        if (AdsManager.Instance)
        {
            AdsManager.Instance.functioncalling(this.gameObject, "RewardofFree");
            AdsManager.Instance.ShowUnityRewarded();
        }
    }
    public void RewardofFree()
    {
        PlayerPrefs.SetInt("TotalReward", 300);
    }
    void BarValueUpdate()
    {
        CoinsText.text = PlayerPrefs.GetInt("TotalReward").ToString();
    }

    public void BarSetting()
    {
        OpenSpecficPanel(3);
        BtnClickSound.Play();
    }

    public void BarStore()
    {
        BtnClickSound.Play();

    }
    
    public void CarSelectPurchase()
    {
        
    }
    public void LevelSelected(int number)
    {
        OpenSpecficPanel(2);
        PlayerPrefs.SetInt("LevelSelected", number);
        bl_SceneLoaderUtils.GetLoader.LoadLevel("Game");
        BtnClickSound.Play();
    }
    public void PrivacyPolicyBtn()
    {
        Application.OpenURL("http://www.redleos.com/privacy.html");
    }
    public void RateUSBtn()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
    }

    public void ReviewbtnFun()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
    }

    public void CoinSpeedUpgrade()
    {
    }
    public void CoinNitroUpgrade()
    {
    }
    public void CoinSteeringUpgrade()
    {
    }
    public void CoinBrakeUpgrade()
    {
    }
    public void AdsSpeedUpgrade()
    {
    }
    public void AdsNitroUpgrade()
    {
    }
    public void AdsSteeringUpgrade()
    {
    }
    public void AdsBrakeUpgrade()
    {
    }
    public void UpdateCarValuesComponenets(GameObject CarButton)
    {

    }
}
