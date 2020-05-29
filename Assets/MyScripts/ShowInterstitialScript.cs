using UnityEngine;
using System;
using System.Collections;

public class ShowInterstitialScript : MonoBehaviour
{
 	String INTERSTITIAL_INSTANCE_ID = "0";

	// Use this for initialization
	void Start ()
	{
		Debug.Log ("unity-script: ShowInterstitialScript Start called");
		// Add Interstitial Events
		IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
		IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;		
		IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent; 
		IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent; 
		IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
		IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
		IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;

		// Add Interstitial DemandOnly Events
		IronSourceEvents.onInterstitialAdReadyDemandOnlyEvent += InterstitialAdReadyDemandOnlyEvent;
		IronSourceEvents.onInterstitialAdLoadFailedDemandOnlyEvent += InterstitialAdLoadFailedDemandOnlyEvent;		
		IronSourceEvents.onInterstitialAdShowFailedDemandOnlyEvent += InterstitialAdShowFailedDemandOnlyEvent; 
		IronSourceEvents.onInterstitialAdClickedDemandOnlyEvent += InterstitialAdClickedDemandOnlyEvent;
		IronSourceEvents.onInterstitialAdOpenedDemandOnlyEvent += InterstitialAdOpenedDemandOnlyEvent;
		IronSourceEvents.onInterstitialAdClosedDemandOnlyEvent += InterstitialAdClosedDemandOnlyEvent;

		// Add Rewarded Interstitial Events
		IronSourceEvents.onInterstitialAdRewardedEvent += InterstitialAdRewardedEvent;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	/************* Interstitial API *************/ 
	public void LoadInterstitialButtonClicked ()
	{
		Debug.Log ("unity-script: LoadInterstitialButtonClicked");
		IronSource.Agent.loadInterstitial ();

		//DemandOnly
		// LoadDemandOnlyInterstitial ();
	}
	
	public void ShowInterstitialButtonClicked ()
	{
		Debug.Log ("unity-script: ShowInterstitialButtonClicked");
		if (IronSource.Agent.isInterstitialReady ()) {
			IronSource.Agent.showInterstitial ();
		} else {
			Debug.Log ("unity-script: IronSource.Agent.isInterstitialReady - False");
		}

		// DemandOnly
		// ShowDemandOnlyInterstitial ();
	}

	void LoadDemandOnlyInterstitial ()
	{
		Debug.Log ("unity-script: LoadDemandOnlyInterstitialButtonClicked");
		IronSource.Agent.loadISDemandOnlyInterstitial (INTERSTITIAL_INSTANCE_ID);  
	}
	
	void ShowDemandOnlyInterstitial ()
	{
		Debug.Log ("unity-script: ShowDemandOnlyInterstitialButtonClicked");
		if (IronSource.Agent.isISDemandOnlyInterstitialReady (INTERSTITIAL_INSTANCE_ID)) {
			IronSource.Agent.showISDemandOnlyInterstitial (INTERSTITIAL_INSTANCE_ID);
		} else {
			Debug.Log ("unity-script: IronSource.Agent.isISDemandOnlyInterstitialReady - False");
		}
	}

	/************* Interstitial Delegates *************/ 
	void InterstitialAdReadyEvent ()
	{
		Debug.Log ("unity-script: I got InterstitialAdReadyEvent");
	}
	
	void InterstitialAdLoadFailedEvent (IronSourceError error)
	{
		Debug.Log ("unity-script: I got InterstitialAdLoadFailedEvent, code: " + error.getCode () + ", description : " + error.getDescription ());
	}
	
	void InterstitialAdShowSucceededEvent ()
	{
		Debug.Log ("unity-script: I got InterstitialAdShowSucceededEvent");
	}
	
	void InterstitialAdShowFailedEvent (IronSourceError error)
	{
		Debug.Log ("unity-script: I got InterstitialAdShowFailedEvent, code :  " + error.getCode () + ", description : " + error.getDescription ());
		
	}
	
	void InterstitialAdClickedEvent ()
	{
		Debug.Log ("unity-script: I got InterstitialAdClickedEvent");
	}
	
	void InterstitialAdOpenedEvent ()
	{
		Debug.Log ("unity-script: I got InterstitialAdOpenedEvent");
	}

	void InterstitialAdClosedEvent ()
	{
		Debug.Log ("unity-script: I got InterstitialAdClosedEvent");
	}

	void InterstitialAdRewardedEvent ()
	{
		Debug.Log ("unity-script: I got InterstitialAdRewardedEvent");
	}

	/************* Interstitial DemandOnly Delegates *************/ 

	void InterstitialAdReadyDemandOnlyEvent (string instanceId)
	{
		Debug.Log ("unity-script: I got InterstitialAdReadyDemandOnlyEvent for instance: " + instanceId);
		
	}
	
	void InterstitialAdLoadFailedDemandOnlyEvent (string instanceId, IronSourceError error)
	{
		Debug.Log ("unity-script: I got InterstitialAdLoadFailedDemandOnlyEvent for instance: " + instanceId + ", error code: " + error.getCode () + ",error description : " + error.getDescription ());
	}
	
	void InterstitialAdShowFailedDemandOnlyEvent (string instanceId, IronSourceError error)
	{
		Debug.Log ("unity-script: I got InterstitialAdShowFailedDemandOnlyEvent for instance: " + instanceId + ", error code :  " + error.getCode () + ",error description : " + error.getDescription ());
		
	}
	
	void InterstitialAdClickedDemandOnlyEvent (string instanceId)
	{
		Debug.Log ("unity-script: I got InterstitialAdClickedDemandOnlyEvent for instance: " + instanceId);
	}
	
	void InterstitialAdOpenedDemandOnlyEvent (string instanceId)
	{
		Debug.Log ("unity-script: I got InterstitialAdOpenedDemandOnlyEvent for instance: " + instanceId);
        
    }

    void InterstitialAdClosedDemandOnlyEvent (string instanceId)
	{
		Debug.Log ("unity-script: I got InterstitialAdClosedDemandOnlyEvent for instance: " + instanceId);
	}

	void InterstitialAdRewardedDemandOnlyEvent (string instanceId)
	{
		Debug.Log ("unity-script: I got InterstitialAdRewardedDemandOnlyEvent for instance: " + instanceId);
	}

}

