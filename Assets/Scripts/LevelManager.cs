using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Firebase.Analytics;

[System.Serializable]
public class Level
{
    public Transform PlayerPos;
    public int LevelTime;
    public int LevelReward;
    public GameObject LevelObject;
    public string LevelStatement;
    public Transform CamPos;
    public AudioClip[] ThiefLines;
    public Transform ThiefPos;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager m_Instance;
    public Level[] m_level;
    public GameObject PlayerObject;
    public static int m_currentLevelNo;
    public CanvasControl CanvasObject;
    public GameObject FPSCanvas;
    public int AnimationTime;
    public GameObject SecondCamera;
    public AudioSource TheifSoundOut;
    public PrankChecker CurrentPrank;
    public AI Theif;
    public GameObject Environment;
    public GameObject Inventory;
    public Sprite[] InventoryObjects;
    public List<int> InventoryNumber;

    int waitnumber;
    double Timer;
    double FirstTimer;
    double levelrecord;
    int soundnumber;
    bool once;
    void Start()
    {
        soundnumber = 0;
        AudioListener.volume = 1;
        m_currentLevelNo = PlayerPrefs.GetInt("LevelSelected");
        levelrecord = (float)m_currentLevelNo;
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        ActivationofObjects();
        if (AdsManager.Instance)
        {
            AdsManager.Instance.ShowBannerAd();
            AdsManager.Instance.HideBanner2Ad();
            AdsManager.Instance.FacebookInterstitial.GetComponent<AdViewScene>().HideFacebookBanner();
            FirebaseAnalytics.LogEvent("Level", "LevelOpened", levelrecord);
        }
    }

    public void ActivationofObjects()
    {
        StartingAnimation();
        ThiefSoundSystem();
        PlayerObject.transform.position = m_level[m_currentLevelNo].PlayerPos.position;
        PlayerObject.transform.rotation = m_level[m_currentLevelNo].PlayerPos.rotation;
        PlayerObject.SetActive(true);
        CanvasObject.PlayerMessage(m_level[m_currentLevelNo].LevelStatement);
        Timer = m_level[m_currentLevelNo].LevelTime;
        CurrentPrank = m_level[m_currentLevelNo].LevelObject.GetComponent<PrankChecker>();
        FirstTimer = Timer;
        InvokeRepeating("TickTick", 1, 1);
    }

    void TickTick()
    {
        Timer--;
        CanvasObject.TimerText.text = ((int)Timer / 60).ToString() + ":" + ((int)Timer % 60).ToString();
        if (Timer < (FirstTimer / 2) && !once)
        {
            ThiefSoundSystem();
            once = true;
        }
        if (Timer == 0)
        {
            StartThiefOperation();
            ThiefSoundSystem();
            CancelInvoke("TickTick");
        }
    }

    public void LevelFailed()
    {
        Invoke("GeneralWait", 3);
        waitnumber = 2;
        FPSCanvas.SetActive(false);
    }

    public void LevelCompleted()
    {
        print("completed");
        if (PlayerPrefs.GetInt("LevelCompleted") <= PlayerPrefs.GetInt("LevelSelected"))
            PlayerPrefs.SetInt("LevelCompleted", PlayerPrefs.GetInt("LevelSelected") + 1);
        PlayerPrefs.SetInt("TotalReward", PlayerPrefs.GetInt("TotalReward") + m_level[m_currentLevelNo].LevelReward);
        Invoke("GeneralWait", 5);
        waitnumber = 0;
        FPSCanvas.SetActive(false);
        //AudioListener.volume = 0;
    }

    public void GeneralWait()
    {
        switch (waitnumber)
        {
            case 0:
                CanvasObject.OpenSpecficPanel(2);
                if (AdsManager.Instance)
                {
                    AdsManager.Instance.HideBannerAd();
                    AdsManager.Instance.ShowBanner2Ad();
                    AdsManager.Instance.MediationAd();
                    FirebaseAnalytics.LogEvent("Level", "LevelCompleted", levelrecord);
                }
                break;
            case 1:
                CanvasObject.OpenSpecficPanel(5);
                break;
            case 2:
                CanvasObject.OpenSpecficPanel(3);
                if (AdsManager.Instance)
                {
                    AdsManager.Instance.HideBannerAd();
                    AdsManager.Instance.ShowBanner2Ad();
                    AdsManager.Instance.MediationAd();
                    FirebaseAnalytics.LogEvent("Level", "LevelFailed", levelrecord);
                }
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }
    void Update()
    {
        
    }
    
    public void StartingAnimation()
    {
        SecondCamera.SetActive(true);
        CanvasObject.m_Panels[0].SetActive(false);
        Invoke("StopAnimation", AnimationTime);
    }

    void StopAnimation()
    {
        SecondCamera.SetActive(false);
        CanvasObject.m_Panels[0].SetActive(true);
        m_level[m_currentLevelNo].LevelObject.SetActive(true);
    }

    public void ThiefSoundSystem()
    {
        if (m_level[m_currentLevelNo].ThiefLines[0] == null)
        {
            return;
        }
        else
        {
            TheifSoundOut.clip = m_level[m_currentLevelNo].ThiefLines[soundnumber];
            soundnumber++;
            TheifSoundOut.Play();
        }
    }

    public void StartThiefOperation()
    {
        DoorOpen();
        MovePLayer();
    }

    void DoorOpen()
    {
        SecondCamera.transform.position = m_level[m_currentLevelNo].CamPos.position;
        SecondCamera.transform.rotation = m_level[m_currentLevelNo].CamPos.rotation;
        SecondCamera.SetActive(true);
        Environment.GetComponent<Animator>().SetTrigger("OpenDoor");
        SecondCamera.GetComponent<LookAtConstraint>().enabled = true;
    }

    void MovePLayer()
    {
        Theif.GoToPosition(m_level[m_currentLevelNo].ThiefPos.position, false);
    }

    public void AddtoInventory(int number)
    {
        ObjectDetail ob;
        int temp;
        for(int i = 0; i < Inventory.transform.childCount; i++)
        {
            ob = Inventory.transform.GetChild(i).GetComponent<ObjectDetail>();
            if (ob.ObjectNo == -1)
            {
                if (InventoryNumber.Contains(number))
                {
                    temp = InventoryNumber.IndexOf(number);
                    ob.SetValues(InventoryObjects[temp], InventoryNumber[temp]);
                    return;
                }
            }

        }
    }

    public void RemovefromInventory(int number)
    {
        ObjectDetail ob;
        for (int i = 0; i < Inventory.transform.childCount; i++)
        {
            ob = Inventory.transform.GetChild(i).GetComponent<ObjectDetail>();
            if (ob.ObjectNo == number)
            {
                ob.RemoveValue();
                return;
            }

        }
    }
}
