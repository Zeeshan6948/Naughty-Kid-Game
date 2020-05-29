//DamageZone.cs by Azuline Studios© All Rights Reserved
//Damages player by damage var amount when they enter a trigger with this script attached
using UnityEngine;
using System.Collections;

public class DamageZone : MonoBehaviour {
	[Tooltip("Amount of damage to apply to player while in damage trigger.")]
	public float damage = 1.0f;
	[Tooltip("Amount of damage to gradually apply to NPCs while in damage trigger.")]
	public float NpcDamage = 0.0f;
	[Tooltip("Delay before player is damaged again by this damage zone.")]
	public float delay = 1.75f;
	private float damageTime;
	private FPSPlayer FPSPlayerComponent;
	
	void Start () {
		FPSPlayerComponent =  Camera.main.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>();
	}
	
	void OnTriggerStay ( Collider col  ){
		if(col.gameObject.tag == "Player"){
			if(damageTime < Time.time){
				FPSPlayerComponent.ApplyDamage(damage);
				damageTime = Time.time + delay;
			}
		}
		if(col.gameObject.layer == 13){//also damage NPCs
			if(col.GetComponent<LocationDamage>()){//use LocationDamage reference instead of CharacterDamage if using newer version of the RFPSP
				LocationDamage NPC = col.GetComponent<LocationDamage>();
				NPC.ApplyDamage(NpcDamage, Vector3.zero, transform.position, null, false, false);
			}
		}
	}

}




