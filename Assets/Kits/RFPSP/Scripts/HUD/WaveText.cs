//WaveText.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveText : MonoBehaviour {
	//draw ammo amount on screen
	[HideInInspector]
	public int waveGui;
	[HideInInspector]
	public int waveGui2;
	private int oldWave = -512;
	private int oldWave2 = -512;
	[Tooltip("Color of GUIText.")]
	public Color textColor;
	private Text uiTextComponent;
	
	void OnEnable(){
		uiTextComponent = GetComponent<Text>();
		oldWave = -512;
		oldWave2 = -512;
	}
	
	void Update(){
		//only update GUIText if value to be displayed has changed
		if(waveGui != oldWave || waveGui2 != oldWave2) {
			
			uiTextComponent.text = "Wave "+ waveGui.ToString()+" - Remaining : "+ waveGui2.ToString();
			
			uiTextComponent.color = textColor;
			oldWave = waveGui;
			oldWave2 = waveGui2;
	    }
	
	}
}