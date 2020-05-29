//ThirstText.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThirstText : MonoBehaviour {
	//draw Thirst amount on screen
	[HideInInspector]
	public float thirstGui;
	private float oldThirstGui = -512;
	[Tooltip("Color of GUIText.")]
	public Color textColor; 
	private Text uiTextComponent;
	
	void Start(){
		uiTextComponent = GetComponent<Text>();
		uiTextComponent.color = textColor;
		oldThirstGui = -512;
	}
	
	void Update (){
		//only update GUIText if value to be displayed has changed
		if(thirstGui != oldThirstGui){
			uiTextComponent.text = "Thirst : "+ thirstGui.ToString();
			oldThirstGui = thirstGui;
		}
	}
	
}