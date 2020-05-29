//WaveManager.cs by Azuline Studios© All Rights Reserved
//Spawns NPCs from NPC Spawners for successive waves using several 
//parameters to control spawn timing and amounts.
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MultiDimensionalInt
{
	[Tooltip("Total number of NPCs to spawn for this wave.")] 
	public int[] NpcCounts;
	[Tooltip("Maximum number of NPCs from the spawner that can be active in the scene at once.")]
	public int[] NpcLoads;
	[Tooltip("Delay between spawning of NPCs for this wave.")]
	public float[] NpcDelay;
	[Tooltip("The NPC Prefabs that will be spawned for this wave.")]
	public GameObject[] NpcTypes;
}

public class WaveManager : MonoBehaviour {
	private FPSPlayer FPSPlayerComponent;
	[Tooltip("The NPC Spawner objects that the Wave Manager will spawn NPC's from. The Waves list parameters correspond the order of these spawners from top to bottom.")]
	public List<NPCSpawner> NpcSpawners = new List<NPCSpawner>();
	[Tooltip("This list contains information for NPC wave spawning. The array sizes and order correspond with the Npc Spawners list. The Waves list can be expanded to add new waves of varying combinations of NPCs and parameters.")] 
	public MultiDimensionalInt[] waves;
	[Tooltip("Time before wave begins.")]
	public float warmupTime = 30.0f;
	private float startTime = -512;
	private float countDown;
	[HideInInspector]
	public int NpcsToSpawn;
	[HideInInspector]
	public int killedNpcs;
	[HideInInspector]
	public int waveNumber;

	[Tooltip("Sound FX played when wave starts.")]
	public AudioClip waveStartFx;
	[Tooltip("Sound FX played when wave ends.")]
	public AudioClip waveEndFx;
	private AudioSource asource;
	private bool fxPlayed;
	private bool fxPlayed2;
	private bool lastWave;

	[HideInInspector]
	public WaveText WaveText;
	[HideInInspector]
	public WaveText WaveTextShadow;
	[HideInInspector]
	public WarmupText WarmupText;
	[HideInInspector]
	public WarmupText WarmupTextShadow;

	private Color tempColor;
	private Color tempColor2;

	private Text WarmupUIText1;
	private Text WarmupUIText2;
	private Vector2 warmupTextPos1Orig;
	private Vector2 warmupTextPos2Orig;
	private Vector2 warmupTextPos1;
	private Vector2 warmupTextPos2;

	void Start () {
		FPSPlayerComponent =  Camera.main.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>();
		asource = gameObject.AddComponent<AudioSource>();
		asource.spatialBlend = 0.0f;

		WaveText = FPSPlayerComponent.waveUiObj.GetComponent<WaveText>();
		WaveTextShadow = FPSPlayerComponent.waveUiObjShadow.GetComponent<WaveText>();
		
		//initialize health amounts on GUIText objects
		WaveText.waveGui = waveNumber;
		WaveTextShadow.waveGui = waveNumber;	
		WaveText.waveGui2 = NpcsToSpawn - killedNpcs;
		WaveTextShadow.waveGui2 = NpcsToSpawn - killedNpcs;

		WarmupText = FPSPlayerComponent.warmupUiObj.GetComponent<WarmupText>();
		WarmupTextShadow = FPSPlayerComponent.warmupUiObjShadow.GetComponent<WarmupText>();

		tempColor = WarmupText.textColor;
		tempColor2 = WarmupTextShadow.textColor; 

		WarmupText.warmupGui = countDown;
		WarmupTextShadow.warmupGui = countDown;

		WarmupUIText1 = WarmupText.GetComponent<Text>();
		WarmupUIText2 = WarmupTextShadow.GetComponent<Text>();

		warmupTextPos1Orig = WarmupUIText1.rectTransform.anchoredPosition;
		warmupTextPos2Orig = WarmupUIText2.rectTransform.anchoredPosition;
		
		StartCoroutine(StartWave());
	}

	void FixedUpdate(){	
		if(WaveText.waveGui2 != NpcsToSpawn - killedNpcs){
			WaveText.waveGui2 = NpcsToSpawn - killedNpcs;
			WaveTextShadow.waveGui2 = NpcsToSpawn - killedNpcs;
		}
	}

	public IEnumerator StartWave(){

		countDown = warmupTime;
		WarmupText.warmupGui = countDown;
		WarmupTextShadow.warmupGui = countDown;	
		killedNpcs = 0;
		NpcsToSpawn = 0;
		if(waveNumber <= waves.Length){
			if(waveNumber < waves.Length){
				waveNumber ++;
			}else{
				//start again from first wave if last wave was completed
				lastWave = true;
				waveNumber = 1;
			}
		}else{
			waveNumber = 1;
		}
		WaveText.waveGui = waveNumber;
		WaveTextShadow.waveGui = waveNumber;	

		tempColor.a = 1.0f;
		tempColor2.a = 1.0f;

		WarmupText.waveBegins = false;
		WarmupTextShadow.waveBegins = false;

		if(waveNumber > 1 || lastWave){
			startTime = Time.time;
			WarmupText.waveComplete = true;
			WarmupTextShadow.waveComplete = true;
			if(waveEndFx && !fxPlayed2){
				asource.PlayOneShot(waveEndFx, 1.0f);
				FPSPlayerComponent.StartCoroutine(FPSPlayerComponent.ActivateBulletTime(1.0f));
				fxPlayed2 = true;
			}
			if(lastWave){lastWave = false;}
		}

		//initialize NPC Spawner objects for spawning of this wave
		for(int i = 0; i < NpcSpawners.Count; i++){
			NpcSpawners[i].NPCPrefab = waves[waveNumber - 1].NpcTypes[i];
			NpcSpawners[i].NpcsToSpawn = waves[waveNumber - 1].NpcCounts[i];
			NpcSpawners[i].maxActiveNpcs = waves[waveNumber - 1].NpcLoads[i];
			NpcSpawners[i].spawnDelay = waves[waveNumber - 1].NpcDelay[i];
			NpcsToSpawn += NpcSpawners[i].NpcsToSpawn;
			NpcSpawners[i].pauseSpawning = true;
			NpcSpawners[i].spawnedNpcAmt = 0;
			NpcSpawners[i].huntPlayer = true;
			NpcSpawners[i].unlimitedSpawning = false;
		}

		//spawn wave
		while(true){

			WarmupUIText1.rectTransform.anchoredPosition = warmupTextPos1Orig;
			WarmupUIText2.rectTransform.anchoredPosition = warmupTextPos2Orig;
			warmupTextPos1 = warmupTextPos1Orig;
			warmupTextPos2 = warmupTextPos2Orig;

			if(startTime + 3.00 < Time.time){
				WarmupText.waveComplete = false;
				WarmupTextShadow.waveComplete = false;
				countDown -= Time.deltaTime;
				WarmupText.warmupGui = countDown;
				WarmupTextShadow.warmupGui = countDown;
			}

			WarmupUIText1.enabled = true;
			WarmupUIText2.enabled = true;
			WarmupUIText1.color = tempColor; 
			WarmupUIText2.color = tempColor2; 

			//start spawning NPCs for this wave
			if(countDown <= 0.0f){
				if(waveStartFx && !fxPlayed){
					for(int i = 0; i < NpcSpawners.Count; i++){
						NpcSpawners[i].pauseSpawning = false;
					}
	
					WarmupText.waveBegins = true;
					WarmupTextShadow.waveBegins = true;

					fxPlayed = true;
					fxPlayed2 = false;
					asource.PlayOneShot(waveStartFx, 1.0f);
				}
			}

			if(countDown <= -2.75f){
				StartCoroutine(FadeWarmupText());
				fxPlayed = false;
				yield break;
			}

			yield return null;

		}

	}

	IEnumerator FadeWarmupText(){

		while(true){

			tempColor.a -= Time.deltaTime;
			tempColor2.a -= Time.deltaTime;
			
			WarmupUIText1.color = tempColor; 
			WarmupUIText2.color = tempColor2; 

			warmupTextPos1.y -= Time.deltaTime * 9.0f;
			warmupTextPos2.y -= Time.deltaTime * 9.0f;

			WarmupUIText1.rectTransform.anchoredPosition = warmupTextPos1;
			WarmupUIText2.rectTransform.anchoredPosition = warmupTextPos2;
			
			if(tempColor.a <= 0.0f && tempColor2.a <= 0.0f){
				WarmupUIText1.enabled = false;
				WarmupUIText2.enabled = false;
				yield break;
			}

			yield return null;
			
		}
	}

}
	