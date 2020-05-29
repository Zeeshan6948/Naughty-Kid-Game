//HelpText.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HelpText : MonoBehaviour {
	//draw ammo amount on screen
	[Tooltip("Color of GUIText.")]
	public Color textColor;
	[Tooltip("Offset from total screen width to position GUIText.")]
	public float horizontalOffset = 0.0425f;

	private bool helpTextState = true;
	private bool helpTextEnabled = false;
	private float startTime = 0.0f;
	private bool initialHide = true;
	private bool moveState = true;
	private bool F1pressed = false;
	private bool fadeState = false;
	private float moveTime = 0.0f;
	private float fadeTime = 5.0f;
	[HideInInspector]
	public GameObject playerObj;
	private Text uiTextComponent;
	
	void Start(){
		uiTextComponent = GetComponent<Text>();
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;

		uiTextComponent.enabled = true;
		uiTextComponent.text = "Press F1 for controls";
		uiTextComponent.color = textColor;
		uiTextComponent.enabled = true;
		startTime = Time.time;
	}
	
	void Update (){
		//Initialize script references
		FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		InputControl InputComponent = playerObj.GetComponent<InputControl>();
		float horizontal = FPSWalkerComponent.inputX;//Get input from player movement script
		float vertical = FPSWalkerComponent.inputY;
		
		Color tempColor = uiTextComponent.color; 
		
		if(moveState &&(Mathf.Abs(horizontal) > 0.75f || Mathf.Abs(vertical) > 0.75f)){
			moveState = false;	
			if(startTime + fadeTime < Time.time){
				moveTime = Time.time;//fade F1 message if moved after fadeTime
			}else{
				moveTime = startTime + fadeTime;//if moved before fade started, start fade at fadeTime	
			}
		}
		
		//if F1 is pressed before fade, bypass fading of F1 message and show help text
		if(InputComponent.helpPress && (moveState || moveTime > Time.time)){
			moveState = false;	
			F1pressed = true;
			moveTime = Time.time;
		}
		
		if(!fadeState && !F1pressed){
			if(!moveState && (startTime + fadeTime < Time.time)){
				if(moveTime + 1.0f > Time.time){
					tempColor.a -= Time.deltaTime;//fade out color alpha element for one second
					uiTextComponent.color = tempColor;
				}else{
					fadeState = true;//F1 message has faded, allow controls to be shown with F1 press
				}
			}
		}else{
			
			if(initialHide){
				uiTextComponent.text = "Mouse 1 : fire weapon\nMouse 2 : raise sights or block\nMouse 3 or C : toggle camera mode, hold in third person to zoom and rotate when moving\nW : forward\nS : backward\nA : strafe left\nD : strafe right\nLeft Shift : sprint\nLeft Ctrl : crouch\nX : prone\nQ : lean left\nE : lean right\nSpace : jump\nF : use item, move item, move NPC\nR : reload\nB : fire mode\nH : holster weapon\nBackspace : drop weapon\nG : throw grenade\nBackslash : select grenade\nV : melee attack\nL : toggle flashlight (if weapon has one)\nT : enter bullet time\nZ : toggle deadzone aiming\nEsc or F2: Main Menu\nTab : Pause\n";
				uiTextComponent.enabled = false;
				tempColor.a = 1.0f;//reset alpha to opaque
				uiTextComponent.color = tempColor;
				initialHide = false;//do these actions only once after F1 help notice has faded out
			}
			
			if(InputComponent.helpPress){
				if(helpTextState){
					if(!helpTextEnabled){
						uiTextComponent.enabled = true;
						helpTextEnabled = true;
					}else{
						uiTextComponent.enabled = false;
						helpTextEnabled = false;
					}
					helpTextState = false;
				}
			}else{
				helpTextState = true;		
			}
		}
		
	}
	
}