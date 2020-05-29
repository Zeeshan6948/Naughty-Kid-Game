//HealthText.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HungerText : MonoBehaviour {
	//draw hunger amount on screen
	[HideInInspector]
	public float hungerGui;
	private float oldHungerGui = -512;
	[Tooltip("Color of GUIText.")]
	public Color textColor;
	private Text uiTextComponent;
	
	void Start(){
		uiTextComponent = GetComponent<Text>();
		uiTextComponent.color = textColor;
		oldHungerGui = -512;
	}
	
	void Update (){
		//only update GUIText if value to be displayed has changed
		if(hungerGui != oldHungerGui){
			uiTextComponent.text = "Hunger : "+ hungerGui.ToString();
			oldHungerGui = hungerGui;
		}
	}
	
}