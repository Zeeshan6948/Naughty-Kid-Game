//AmmoText.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmmoText : MonoBehaviour {
	//draw ammo amount on screen
	[HideInInspector]
	public int ammoGui;//bullets remaining in clip
	[HideInInspector]
	public int ammoGui2;//total ammo in inventory
	[HideInInspector]
	public bool showMags = true;
	private int oldAmmo = -512;
	private int oldAmmo2 = -512;
	[Tooltip("Color of GUIText.")]
	public Color textColor;
	[HideInInspector]
	public Text uiTextComponent;
	
	void OnEnable(){
		uiTextComponent = GetComponent<Text>();
		oldAmmo = -512;
		oldAmmo2 = -512;
	}
	
	void Update(){
		//only update GUIText if value to be displayed has changed
		if(ammoGui != oldAmmo || ammoGui2 != oldAmmo2) {

			if(showMags){
				uiTextComponent.text = "Ammo : "+ ammoGui.ToString()+" / "+ ammoGui2.ToString();
			}else{
				uiTextComponent.text = "Ammo : "+ ammoGui2.ToString();
			}
			
//			uiTextComponent.material.color = textColor;
		    oldAmmo = ammoGui;
			oldAmmo2 = ammoGui2;
			
	    }
	
	}
}