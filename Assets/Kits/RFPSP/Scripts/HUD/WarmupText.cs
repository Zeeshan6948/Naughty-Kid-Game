//WarmupText.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WarmupText : MonoBehaviour {
	//draw ammo amount on screen
	[HideInInspector]
	public float warmupGui;//bullets remaining in clip
	private float oldWarmup = -512;
	[Tooltip("Color of GUIText.")]
	public Color textColor;
	[HideInInspector]
	public bool waveBegins;
	[HideInInspector]
	public bool waveComplete;
	private Text uiTextComponent;
	
	void OnEnable(){
		uiTextComponent = GetComponent<Text>();
		oldWarmup = -512;
	}
	
	void Update(){
		//only update GUIText if value to be displayed has changed
		if(warmupGui != oldWarmup) {

			if(!waveComplete){
				if(!waveBegins){
					uiTextComponent.text = "Warmup Time : "+  Mathf.Round(warmupGui).ToString();
				}else{
					uiTextComponent.text = "INCOMING WAVE";
				}
			}else{
				uiTextComponent.text = "WAVE COMPLETE";
			}
			
			uiTextComponent.color = textColor;
			oldWarmup = warmupGui;
	    }
	
	}
}