//ThirdPersonWeapons.cs by Azuline Studios© All Rights Reserved
//Contains references to all related third person weapon objects. 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MultiDimensionalWeapons{
	[Tooltip("Main weapon object that player holds in third person for this weapon.")]
	public GameObject weaponObject;
	[Tooltip("Weapon object that player holds in their left hand in third person.")]
	public GameObject weaponObject2;
	[Tooltip("Weapon object that player holds on their back when in third person.")]
	public GameObject weaponObjectBack;
	[Tooltip("The muzzle flash object to display for this weapon in third person when firing.")] 
	public Renderer muzzleFlashRenderer;
	[Tooltip("The position to emit smoke effects for this weapon.")] 
	public Transform muzzleSmokePos;
	[Tooltip("The position to emit shells for this weapon.")] 
	public Transform shellEjectPos;
}

public class ThirdPersonWeapons : MonoBehaviour {

	[Tooltip("List of weapon objects that correspond with the Weapon Order list of PlayerWeapons.cs.")]
	public List<MultiDimensionalWeapons> thirdPersonWeaponModels = new List<MultiDimensionalWeapons>();

}
