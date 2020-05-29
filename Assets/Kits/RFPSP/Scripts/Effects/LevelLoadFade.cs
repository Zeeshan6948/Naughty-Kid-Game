//LevelLoadFade.cs by Azuline Studios© All Rights Reserved
//To fade in from black and fade out to black
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class LevelLoadFade : MonoBehaviour {

	[HideInInspector]
	public GameObject LevelLoadFadeobj;
	[HideInInspector]
	public Image fadeImage;
	
	public void FadeAndLoadLevel ( Color color, float fadeLength, bool fadeIn ){
//		Texture2D fadeTexture = new Texture2D (1, 1);//Create texture for screen fade
//		fadeTexture.SetPixel(0, 0, color);
//		fadeTexture.Apply();
		
		LevelLoadFadeobj.layer = 14;//set fade object's layer to one not ignored by weapon camera
		fadeImage.color = color;
//		LevelLoadFadeobj.AddComponent<GUITexture>();
//		LevelLoadFadeobj.transform.position = new Vector3 (0.5f, 0.5f, 1000);
//		LevelLoadFadeobj.GetComponent<GUITexture>().texture = fadeTexture;
	
//		DontDestroyOnLoad(fadeTexture);

		LevelLoadFadeobj.SetActive(true);
	
		if(fadeIn){//Call DoFadeIn or DoFadeOut functions based on which argument is called
			StartCoroutine(DoFadeIn(fadeLength));
		}else{
			StartCoroutine(DoFadeOut(fadeLength));	
		}
	}

	IEnumerator DoFadeIn ( float fadeLength){
		 // Dont destroy the fade game object during level load
//		DontDestroyOnLoad(LevelLoadFadeobj);
//		GUITexture GUITextureRef = GetComponent<GUITexture>();
		
		// Fade texture in
		float time = 0.0f;
		while (time < fadeLength){
			if(Time.timeSinceLevelLoad > 0.8f){		//Create a temporary Color var and make alpha of color = 0 (transparent for starting fade out)
				Color tempColor = fadeImage.color; 
				time += Time.deltaTime;
				tempColor.a = Mathf.InverseLerp(fadeLength, 0.0f, time);//smoothly fade alpha in
				fadeImage.color = tempColor;
			}
			yield return null;
		}

		LevelLoadFadeobj.SetActive(false);
	
//		Destroy (LevelLoadFadeobj);//destroy temporary texture 
	
		// If we created the texture from code we used DontDestroyOnLoad,
		// which means we have to clean it up manually to avoid leaks
//		if (destroyTexture){
//			Destroy (GUITextureRef.texture);
//		}
	}
	
	IEnumerator DoFadeOut (float fadeLength){
		
//		GUITexture GUITextureRef = GetComponent<GUITexture>();
		
		Color tempColor = fadeImage.color; 
   		tempColor.a = 0.0f;//store the color's alpha amount
		fadeImage.color = tempColor;//set the guiTexture's color to the value(s) of our temporary color var
		
		//Fade texture in
		float time = 0.0f;
		while (time < fadeLength){
			time += Time.deltaTime;
			tempColor.a = Mathf.InverseLerp(0.0f, fadeLength, time);//smoothly fade alpha out
			fadeImage.color = tempColor;
			yield return null;
		}
	
		//Complete the fade out (Load a level or reset player position, not needed if using checkpoint spawning)
		#if UNITY_5_3_OR_NEWER
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		#else
		Application.LoadLevel(Application.loadedLevel);
		#endif
		
//		FPSPlayer FPSPlayerComponent = Camera.main.transform.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>();
//		GameObject llf = Instantiate(FPSPlayerComponent.levelLoadFadeObj) as GameObject;
//		llf.GetComponent<LevelLoadFade>().FadeAndLoadLevel(Color.black, 2.0f, true);
//		
//		//respawn the player at the checkpoint position with full health
//		FPSPlayerComponent.HealPlayer(100.0f - FPSPlayerComponent.hitPoints);
//		FPSPlayerComponent.MouseLookComponent.enabled = true;
//		Camera.main.transform.GetComponent<CameraControl>().playerObj.GetComponent<Rigidbody>().freezeRotation = true;
//		FPSPlayerComponent.FPSWalkerComponent.myTransform.position = FPSPlayerComponent.FPSWalkerComponent.startingPos;
		
		yield return new WaitForSeconds(1.0f);

		LevelLoadFadeobj.SetActive(false);

	}
}