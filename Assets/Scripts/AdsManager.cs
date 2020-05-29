using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.Advertisements;
using ChartboostSDK;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    public static AdsManager Instance;

    [Header("Admob Properties")]
    public string AdmobAppId;
    public string AdmobBannerId;
    public string AdmobInterstialId;
    public string AdmobRewardedId;
    public AdPosition AdmobBannerPosition;

    private BannerView bannerView;
    private BannerView bannerView2;
    private InterstitialAd interstitial;
    private RewardBasedVideoAd rewardBasedVideo;

    public ShowInterstitialScript IronSourceInterstitial;
    public InterstitialAdScene FacebookInterstitial;
    public RewardedVideoAdScene FacebookRewarded;

    [Header("Unity Properties")]
    public string UnityGameId = "1234567";
    public bool UnityTestMode = true;
    string myPlacementId = "rewardedVideo";

    [Header("ChartBoost Properties")]
    private List<string> delegateHistory;
    private bool ageGate = false;
    public bool autocache = true;
    private bool showInterstitial = true;
    private bool showRewardedVideo = true;

    GameObject RefObject;
    String FunctionName;
    bool rewarded;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            this.name = "AdsManager";
        }
        else
        {
            Destroy(this.gameObject);
        }
        /////////////////////////////////////////
        delegateHistory = new List<string>();
#if UNITY_IPHONE
		Chartboost.setShouldPauseClickForConfirmation(ageGate);
#endif
        Chartboost.setAutoCacheAds(autocache);
        Chartboost.setPIDataUseConsent(CBPIDataUseConsent.YesBehavioral);
        AddLog("Is Initialized: " + Chartboost.isInitialized());
        Chartboost.Create();
        //Chartboost.cacheRewardedVideo(CBLocation.MainMenu);
        Chartboost.cacheInterstitial(CBLocation.HomeScreen);
        ////////////////////////////////////////////
        Advertisement.AddListener(this);
        Advertisement.Initialize(UnityGameId, UnityTestMode);
        // Initialize the Google Mobile Ads SDK.
        //MobileAds.Initialize(initStatus => { });

        if (AdmobAppId == "")
        {
#if UNITY_ANDROID
            AdmobAppId = "ca-app-pub-3940256099942544~3347511713";
#elif UNITY_IPHONE
            AdmobAppId = "ca-app-pub-3940256099942544~1458002511";
#else
            AdmobAppId = "unexpected_platform";
#endif
        }
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(AdmobAppId);

        this.RequestBanner();
        this.RequestBanner2();
        this.RequestInterstitial();
        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;
        this.RequestRewardBasedVideo();
        FacebookInterstitial.LoadInterstitial();
        FacebookRewarded.LoadRewardedVideo();
        //FacebookInterstitial.GetComponent<AdViewScene>().LoadBanner();
        ShowBannerAd();
    }
    private void RequestBanner()
    {
        if (AdmobBannerId == "")
        {
#if UNITY_ANDROID
            AdmobBannerId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            AdmobBannerId = "ca-app-pub-3940256099942544/2934735716";
#else
            AdmobBannerId = "unexpected_platform";
#endif
        }
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(AdmobBannerId, AdSize.SmartBanner, AdmobBannerPosition);

        // Called when an ad request has successfully loaded.
        bannerView.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        //bannerView.OnAdOpening += HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        //bannerView.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        //bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
        HideBannerAd();
        //Custom Ad size
        //AdSize adSize = new AdSize(250, 250);
        //BannerView bannerView = new BannerView(adUnitId, adSize, AdPosition.Bottom);
    }
    private void RequestBanner2()
    {
        if (AdmobBannerId == "")
        {
#if UNITY_ANDROID
            AdmobBannerId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            AdmobBannerId = "ca-app-pub-3940256099942544/2934735716";
#else
            AdmobBannerId = "unexpected_platform";
#endif
        }
        bannerView2 = new BannerView(AdmobBannerId, AdSize.Banner, AdmobBannerPosition);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView2.LoadAd(request);
        HideBanner2Ad();
    }
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    //public void HandleOnAdOpened(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdOpened event received");
    //}

    //public void HandleOnAdClosed(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdClosed event received");
    //}

    //public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdLeavingApplication event received");
    //}
    public void DestroyBannerAd()
    {
        bannerView.Destroy();
    }
    public void HideBannerAd()
    {
        bannerView.Hide();
    }
    public void ShowBannerAd()
    {
        bannerView.Show();
    }
    public void DestroyBanner2Ad()
    {
        bannerView2.Destroy();
    }
    public void HideBanner2Ad()
    {
        bannerView2.Hide();
    }
    public void ShowBanner2Ad()
    {
        bannerView2.Show();
    }
    private void RequestInterstitial()
    {
        if (AdmobInterstialId == "")
        {
#if UNITY_ANDROID
            AdmobInterstialId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
            AdmobInterstialId = "ca-app-pub-3940256099942544/4411468910";
#else
            AdmobInterstialId = "unexpected_platform";
#endif
        }
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(AdmobInterstialId);

        // Called when an ad request has successfully loaded.
        //this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        //this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        //this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        //this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        //this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;


        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }
    //public void HandleOnAdLoaded(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdLoaded event received");
    //}

    //public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    //{
    //    MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
    //                        + args.Message);
    //}

    //public void HandleOnAdOpened(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdOpened event received");
    //}

    //public void HandleOnAdClosed(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdClosed event received");
    //}

    //public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdLeavingApplication event received");
    //}
    public void ShowAdmobIntersitial()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else
        {
            this.RequestInterstitial();
        }
    }
    public void DestroyAdmobIntersitial()
    {
        interstitial.Destroy();
    }

    private void RequestRewardBasedVideo()
    {
        if (AdmobRewardedId == "")
        {
#if UNITY_ANDROID
            AdmobRewardedId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            AdmobRewardedId = "ca-app-pub-3940256099942544/1712485313";
#else
            AdmobRewardedId = "unexpected_platform";
#endif
        }

        //Called when an ad request has successfully loaded.
        //rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        //Called when an ad request failed to load.
        //rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        //Called when an ad is shown.
        //rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        //Called when the ad starts to play.
        //rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
        //Called when the user should be rewarded for watching a video.
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        //Called when the ad is closed.
        //rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        //rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;


        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        this.rewardBasedVideo.LoadAd(request, AdmobRewardedId);
    }
    //public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    //}

    //public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    //{
    //    MonoBehaviour.print(
    //        "HandleRewardBasedVideoFailedToLoad event received with message: "
    //                         + args.Message);
    //}

    //public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
    //}

    //public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
    //}

    //public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
    //}

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        if (rewarded)
            RefObject.SendMessage(FunctionName);
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print("HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);
    }

    //public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
    //}


    private void ShowadmobRewarded()
    {
        if (rewardBasedVideo.IsLoaded())
        {
            rewardBasedVideo.Show();
        }
    }
    /// <summary>
    /// ///////////////////////////////////////Unity ADs Integration////////////////////////////////////////////////////
    /// </summary>
    /// 
    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            if (rewarded)
                RefObject.SendMessage(FunctionName);
            // Reward the user for watching the ad to completion.
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, show the ad:
        //if (placementId == myPlacementId)
        //{
        //    Advertisement.Show(myPlacementId);
        //}
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }
    /// <summary>
    /// ///////////////////////////////////////ChartBoost Integration////////////////////////////////////////////////////
    /// </summary>
    /// 
    public void ShowCBIntersitial()
    {
        if (Chartboost.hasInterstitial(CBLocation.HomeScreen))
        {
            Chartboost.showInterstitial(CBLocation.HomeScreen);
        }
        else
        {
            Chartboost.cacheInterstitial(CBLocation.HomeScreen);
        }
    }
    public void ShowCBRewarded()
    {
        if (Chartboost.hasRewardedVideo(CBLocation.MainMenu))
        {
            Chartboost.showRewardedVideo(CBLocation.MainMenu);
        }
        else
        {
            Chartboost.cacheRewardedVideo(CBLocation.MainMenu);
        }
    }
    void OnEnable()
    {
        SetupDelegates();
    }
    void SetupDelegates()
    {
        // Listen to all impression-related events
        Chartboost.didInitialize += didInitialize;
        Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
        Chartboost.didDismissInterstitial += didDismissInterstitial;
        Chartboost.didCloseInterstitial += didCloseInterstitial;
        Chartboost.didClickInterstitial += didClickInterstitial;
        Chartboost.didCacheInterstitial += didCacheInterstitial;
        Chartboost.shouldDisplayInterstitial += shouldDisplayInterstitial;
        Chartboost.didDisplayInterstitial += didDisplayInterstitial;
        Chartboost.didFailToRecordClick += didFailToRecordClick;
        Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
        Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
        Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
        Chartboost.didClickRewardedVideo += didClickRewardedVideo;
        Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
        Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
        Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
        Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
        Chartboost.didPauseClickForConfirmation += didPauseClickForConfirmation;
        Chartboost.willDisplayVideo += willDisplayVideo;
#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow += didCompleteAppStoreSheetFlow;
#endif
    }
    void AddLog(string text)
    {
        Debug.Log(text);
        delegateHistory.Insert(0, text + "\n");
        int count = delegateHistory.Count;
        if (count > 20)
        {
            delegateHistory.RemoveRange(20, count - 20);
        }
    }
    void didInitialize(bool status)
    {
        AddLog(string.Format("didInitialize: {0}", status));
    }

    void didFailToLoadInterstitial(CBLocation location, CBImpressionError error)
    {
        AddLog(string.Format("didFailToLoadInterstitial: {0} at location {1}", error, location));
    }

    void didDismissInterstitial(CBLocation location)
    {
        AddLog("didDismissInterstitial: " + location);
    }

    void didCloseInterstitial(CBLocation location)
    {
        AddLog("didCloseInterstitial: " + location);
    }

    void didClickInterstitial(CBLocation location)
    {
        AddLog("didClickInterstitial: " + location);
    }

    void didCacheInterstitial(CBLocation location)
    {
        AddLog("didCacheInterstitial: " + location);
    }

    bool shouldDisplayInterstitial(CBLocation location)
    {
        // return true if you want to allow the interstitial to be displayed
        // AddLog("shouldDisplayInterstitial @" + location + " : " + showInterstitial);
        return showInterstitial;
    }

    void didDisplayInterstitial(CBLocation location)
    {
        AddLog("didDisplayInterstitial: " + location);
    }

    void didFailToRecordClick(CBLocation location, CBClickError error)
    {
        AddLog(string.Format("didFailToRecordClick: {0} at location: {1}", error, location));
    }

    void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error)
    {
        AddLog(string.Format("didFailToLoadRewardedVideo: {0} at location {1}", error, location));
    }

    void didDismissRewardedVideo(CBLocation location)
    {
        AddLog("didDismissRewardedVideo: " + location);
    }

    void didCloseRewardedVideo(CBLocation location)
    {
        AddLog("didCloseRewardedVideo: " + location);
    }

    void didClickRewardedVideo(CBLocation location)
    {
        AddLog("didClickRewardedVideo: " + location);
    }

    void didCacheRewardedVideo(CBLocation location)
    {
        AddLog("didCacheRewardedVideo: " + location);
    }

    bool shouldDisplayRewardedVideo(CBLocation location)
    {
        // AddLog("shouldDisplayRewardedVideo @" + location + " : " + showRewardedVideo);
        return showRewardedVideo;
    }

    void didCompleteRewardedVideo(CBLocation location, int reward)
    {
        AddLog(string.Format("didCompleteRewardedVideo: reward {0} at location {1}", reward, location));
    }

    void didDisplayRewardedVideo(CBLocation location)
    {
        AddLog("didDisplayRewardedVideo: " + location);
    }

    void didPauseClickForConfirmation()
    {
#if UNITY_IPHONE
		AddLog("didPauseClickForConfirmation called");
		activeAgeGate = true;
#endif
    }

    void willDisplayVideo(CBLocation location)
    {
        AddLog("willDisplayVideo: " + location);
    }










    public void functioncalling(GameObject ob, string fn)
    {
        RefObject = ob;
        FunctionName = fn;
    }

    public void ShowUnityInterstitial()
    {
        rewarded = false;
        if (Advertisement.IsReady())
            Advertisement.Show();
    }
    public void ShowUnityRewarded()
    {
        rewarded = true;
        if (Advertisement.IsReady(myPlacementId))
            Advertisement.Show(myPlacementId);
    }

    public void MediationAd()
    {
        rewarded = false;
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else if (Advertisement.IsReady())
        {
            Advertisement.Show();
            this.RequestInterstitial();
        }
        else if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else if (FacebookInterstitial.isLoaded)
        {
            FacebookInterstitial.interstitialAd.Show();
            FacebookInterstitial.isLoaded = false;
            IronSource.Agent.loadInterstitial();
        }
        else
        {
            FacebookInterstitial.LoadInterstitial();
        }
    }

    public void RewardedMediation()
    {
        rewarded = true;
        if (rewardBasedVideo.IsLoaded())
        {
            ShowadmobRewarded();
        }
        else if (FacebookRewarded.isLoaded)
        {
            FacebookRewarded.ShowRewardedVideo();
        }
        else if (Advertisement.IsReady(myPlacementId))
            Advertisement.Show(myPlacementId);
    }

    public void SendReward()
    {
        if (rewarded)
            RefObject.SendMessage(FunctionName);
    }

    void Update()
    {

    }

}
