using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRandomSpawnChance : MonoBehaviour {

    [Tooltip("Chance between 0 and 1 that object will spawn. Used to randomize object locations and surprise the player.")]
	[Range(0.0f, 1.0f)]
	public float randomSpawnChance = 1.0f;

	void Start () {

         Mathf.Clamp01(randomSpawnChance);

		if(Random.value > randomSpawnChance){
			Destroy(transform.gameObject);
		}
	}

}
