//PainFade.cs by Azuline Studios© All Rights Reserved
//script to make screen flash red when damage taken
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PainFade : MonoBehaviour {
	
	[HideInInspector]
	public Image painImageComponent;

	void Start (){
		painImageComponent = GetComponent<Image>();
	}
	
	public IEnumerator FadeIn ( Color color, float fadeLength ){
		//Create a temporary Color var and make alpha of color = 0 (transparent for starting fade out)
		Color tempColor = color; 
   		tempColor.a = 0.0f;//store the color's alpha amount
		painImageComponent.color = tempColor;//set the guiTexture's color to the value of our temporary color var
		color.a = Mathf.Clamp01(color.a);
		
		//Fade texture out
		float time = 0.0f;
		while (time < fadeLength * 3.0f){
			time += Time.deltaTime * 1.15f;
			tempColor.a = Mathf.InverseLerp(fadeLength, 0.0f, time) * color.a;
			painImageComponent.color = tempColor;
			yield return null;
		}

	}
}