//PlayerCharacter.cs by Azuline Studios© All Rights Reserved
//Plays third person character animations and 
//instantiates and animates first person visible body. 
using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {
	private Animator anim;//third person animator component
	private Transform headBone;
	private Transform chestBone;
	private Transform spineBone;
	[HideInInspector]
	public Animator shotgunAnimComponent;
	[HideInInspector]
	public GameObject playerObj;
	private Transform playerTransform;
	[HideInInspector]
	public GameObject weaponObj;
	private InputControl InputComponent;
	private FPSRigidBodyWalker walkerComponent;
	private FPSPlayer FPSPlayerComponent;
	private SmoothMouseLook SmoothMouseLookComponent;
	private CameraControl CameraControlComponent;
	private GunSway GunSwayComponent;
	private WeaponBehavior WeaponBehaviorComponent;
	private PlayerWeapons PlayerWeaponsComponent;
	private Transform myTransform;
	[HideInInspector]
	public SkinnedMeshRenderer[] AllSkinnedMeshes;
	[HideInInspector]
	public MeshRenderer[] AllMeshRenderers;

	[HideInInspector]
	public GameObject fpBodyObj;
	private Transform fpBodyTransform;
	private Animator fpBodyAnim;
	private SkinnedMeshRenderer fpBodySkinnedMesh;
	[Tooltip("True if the visible body should be displayed in first person mode.")]
	public bool displayVisibleBody = true;
	[Tooltip("The animator controller needed to animated the first person visible body.")]
	public RuntimeAnimatorController fpBodyController;
	[Tooltip("Alternate material to use for the first person visible body.")]
	public Material alternateBodyMaterial;
	[TooltipAttribute("Index number of character skinned mesh material to replace with alternate body material.")]
	public int materialToReplace;
	[Tooltip("Reference to the ThirdPersonWeapons.cs script for third person weapons (assigned automatically).")]
	public ThirdPersonWeapons weaponModels;
	[Tooltip("Angles of the third person weapon parent object.")]
	public Vector3 weaponModelParentAngles = new Vector3(355.03f, 276.66f, 269.42f);

	private float bodyAng;
	private float bodyAngAmt;
	private float bodyAngSpeed;
	private Vector3 tempBodyAngles;
	[Tooltip("Speed to rotate the player model's bones.")]
	public float boneSpeed = 4f;
	private float boneSpeedAmt;
	private float boneAng1;
	private float boneAng1Amt;
	[Tooltip("Speed to rotate the player model's yaw angle.")]
	public float slerpSpeed = 14f;
	private float slerpSpeedAmt;
	private float idleTime = -32f;
	private float moveTime = -32f;
	private Vector2 moveInputs;
	[Tooltip("Amount to offset the player's aiming angle.")]
	public float aimOffset = 0.0f;
	private float aimOffsetAmt;
	[Tooltip("Amount to offset the player's aiming angle when swimming.")]
	public float swimFireAng;
	private float swimFireAngAmt;

	private Vector3 dampvel;
	[HideInInspector]
	public Vector3 tempBodyPosition;
	private Vector3 tempSmoothedPos;

	[TooltipAttribute("Scale of model while standing.")]
	public float modelStandScale = 1.4f;
	[TooltipAttribute("Scale of model while crouching.")]
	public float modelCrouchScale = 1.1f;
	private float modelScaleAmt = 1.1f;
	[Tooltip("Scale of player model in third person mode.")]
	public float tpMeshScale = 0.95f;
	[Tooltip("Amount to offset the player's yaw angle when moving forward.")]
	public float forwardYawAng;
	[Tooltip("Amount to offset the player's yaw angle.")]
	public float idleYawAng;
	[Tooltip("Amount to offset the player's yaw angle when unarmed.")]
	public float idleYawAngUnarmed;
	[Tooltip("Amount to offset the player's yaw angle when unarmed and crouched.")]
	public float yawAngUnarmedCrouchStrafe = -25.0f;
	private float idleYawAngAmt;
	[Tooltip("Pitch angle of visible body in first person mode.")]
	public float modelPitch = -7.0f;
	[Tooltip("Pitch angle of visible body in first person mode when crouched.")]
	public float modelPitchCrouch = -7.0f;
	[Tooltip("Pitch angle of visible body when sprinting in first person mode.")]
	public float modelPitchRun = -20f;
	[HideInInspector]
	public float modelPitchAmt;
	[Tooltip("Amount to add to forward position of visible player model when standing in first person mode.")]
	public float modelForward = -0.35f;
	[Tooltip("Amount to add to forward position of visible player model when sprinting in first person mode.")]
	public float modelForwardSprint = -0.35f;
	[Tooltip("Amount to add to forward position of visible player model when crouching in first person mode.")]
	public float modelForwardCrouch = -0.3f;
	[Tooltip("Amount to add to forward position of visible player model when swimming in first person mode.")]
	public float modelForwardDown = -4.5f;
	public float sprintStrafeRight;
	private float modelForwardAmt;
	[HideInInspector]
	public float modelRightAmt;
	[Tooltip("Amount to add to upward position of visible player model when standing in first person mode.")]
	public float modelUpFP = -0.15f;
	[Tooltip("Amount to add to upward position of FP shadow and third person player model when standing.")]
	public float modelUp = 1.8f;
	[Tooltip("Amount to add to upward position of FP shadow and third person player model when crouching.")]
	public float modelUpCrouch = 1.1f;
	[Tooltip("Amount to add to upward position of FP shadow and third person player model when crouching and unarmed.")]
	public float modelUpCrouchUnarmed = 0.05f;
	[Tooltip("Amount to add to upward position of FP shadow and third person player model when sprinting.")]
	public float modelUpRun = 1.1f;
	[HideInInspector]
	public float modelUpAmt;
	[HideInInspector]
	public float verticalPos;
	private float rotAngleAmt;
	private float facingAngleDelta;

	private Vector2 moveSmoothed;
	public float inputSmoothingSpeed = 4f;

	[HideInInspector]
	public float fireTime = -32f;
	[Tooltip("Time to keep the weapon raised after firing.")]
	public float fireUpTime = 2.0f;
	[Tooltip("Time to keep the weapon raised when firing a single shot.")]
	public float fireShotTime = 0.4f;

	private float forwardSpeedAmt;
	private float sprintTime;
	private float proneMoveTime;
	private bool proneState;
	private float proneStanceTime;
	private int proneTransition;
	[Tooltip("Duration of prone stance change transition.")]
	public float proneTransitionLength = 0.7f;
	private float weapDownTime;

	//player movement direction: 0 = idle, 1 = forward, 2 = left, 3 = right, 4 = back
	private int moveDir;
	//player movement states: 0 = walking, 1 = sprinting, 2 = crouching, 3 = jumping, 4 = swimming, 5 = swimming and firing, 6 = prone, 7 = dead
	private int moveState;

	[Tooltip("The xyz amounts of mouse input to apply to chest angles to make the player model look up or down.")]
	public Vector3 mLookAng;
	[Tooltip("The xyz amounts of mouse input to apply to spine angles to make the player model look up or down.")]
	public Vector3 mLookAng2;
	[Tooltip("The xyz amounts of mouse input to apply to chest angles to make the player model look up or down when prone.")]
	public Vector3 mLookAngProne;
	[Tooltip("The xyz amounts of mouse input to apply to spine angles to make the player model look up or down when prone.")]
	public Vector3 mLookAngProne2;
	[Tooltip("The xyz amounts of mouse input to apply to chest angles to make the player model look up or down when aiming bow.")]
	public Vector3 mLookAngBow;
	[Tooltip("The xyz amounts of mouse input to apply to spine angles to make the player model look up or down when aiming bow.")]
	public Vector3 mLookAngBow2;
	[Tooltip("True if player model should always aim in first person mode (shadow will always aim in look direction).")]
	public bool fpTorsoAlwaysAims;

	private float torsoLayerWeight;
	private float torsoLayerWeight2;

	private Vector3 chestAngles;
	private Vector3 spineAngles;

	private Vector3 chestAnglesAmt;
	private Vector3 spineAnglesAmt;

	[Tooltip("Amount to rotate player's head to face forward when moving.")]
	public float headRotation = 0.7f;
	[Tooltip("Amount to rotate player's head to face forward when crouched.")]
	public float headRotationCrouch = 0.4f;

	private bool reloadStartTimeState;
	[HideInInspector]
	public float reloadStartTime = -32.0f;
	private float reloadDurationAmt;
	[Tooltip("The duration of the reloading animation for rifles.")]
	public float reloadDurationRifle = 2.25f;
	[Tooltip("The duration of the reloading animation for pistols.")]
	public float reloadDurationPistol = 1.0f;
	[Tooltip("The duration of the reloading animation for shotguns.")]
	public float reloadDurationShotgun = 1.0f;

	private float swimBlendAmt;
	private float swimBlend;

	[Tooltip("The duration of the weapon switching animation.")]
	public float switchDuration = 0.3f;
	[HideInInspector]
	public int lastWeapon;

	private float angOffset;
	private bool meleeMove;
	private float fireDelay;
	[Tooltip("The duration of the offhand melee animation.")]
	public float offhandMeleeTime = 0.14f;
	[Tooltip("Delay before making arrow object visible after firing.")]
	public float arrowVisibleDelay = 0.4f;

	private bool offhandState;
	private bool rotateHead;
	private bool tpModeState;
	private bool deadState;
	//time since look input has exceeded an amount 
	//used for making turning animations, ignoring max input sensitivity, since sensitivity is lower for joystick vs higher for mouse
	private float turnTimer;

	private Transform mainCamTransform;
	[HideInInspector]
	public float tpswitchTime;
	private float modelPosSmoothSpeed;

	void Start () {
		mainCamTransform = Camera.main.transform;
		weaponModels = GetComponentInChildren<ThirdPersonWeapons>();
		weaponModels.gameObject.SetActive(true);
		anim = GetComponentInChildren<Animator>();
		if(anim.GetBoneTransform(HumanBodyBones.Head)){
			headBone = anim.GetBoneTransform(HumanBodyBones.Head);
			chestBone = anim.GetBoneTransform(HumanBodyBones.Chest);
			spineBone = anim.GetBoneTransform(HumanBodyBones.Spine);
		}else{
			Debug.Log("<color=red>Player model avatar error:</color> Please check your player character setup and animator avatar reference.");
		}
		anim.applyRootMotion = false;
		CameraControlComponent = mainCamTransform.GetComponent<CameraControl>();
		playerObj = CameraControlComponent.playerObj;
		weaponObj = CameraControlComponent.weaponObj;
		playerTransform = playerObj.transform;
		InputComponent = playerObj.GetComponent<InputControl>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		walkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		GunSwayComponent = weaponObj.GetComponent<GunSway>();
		SmoothMouseLookComponent = GunSwayComponent.cameraObj.GetComponent<SmoothMouseLook>();
		PlayerWeaponsComponent = FPSPlayerComponent.PlayerWeaponsComponent;
		myTransform = transform;
		AllSkinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>(true);
		AllMeshRenderers = GetComponentsInChildren<MeshRenderer>(true);
		tpModeState = CameraControlComponent.thirdPersonActive;//synchronize third person state var with actual third person mode state
		
		CameraControlComponent.PlayerCharacterComponent = this;
		CameraControlComponent.PlayerCharacterObj = this.gameObject;
		walkerComponent.PlayerCharacterComponent = this;
		walkerComponent.PlayerCharacterObj = this.gameObject;

		///Set up first person visible body object///
		fpBodyObj = Instantiate(myTransform.gameObject, myTransform.position, myTransform.rotation) as GameObject;//duplicate this object
		Destroy(fpBodyObj.GetComponent<PlayerCharacter>());//remove playercharacter.cs component 
		fpBodyAnim = fpBodyObj.GetComponent<Animator>();
		GameObject tempWeapons = fpBodyObj.GetComponentInChildren<ThirdPersonWeapons>().gameObject as GameObject;
		Destroy(tempWeapons);//first person visible body doesn't have arms, so we don't need an extra copy of weapon models
		fpBodyTransform = fpBodyObj.transform;
		fpBodyAnim.runtimeAnimatorController = fpBodyController;
		fpBodySkinnedMesh = fpBodyObj.GetComponentInChildren<SkinnedMeshRenderer>();
		fpBodySkinnedMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		//scale down arms and head of first person visible body object, so they don't show in the camera view
		if(fpBodyAnim.GetBoneTransform(HumanBodyBones.Head)){
			fpBodyAnim.GetBoneTransform(HumanBodyBones.Head).localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
			fpBodyAnim.GetBoneTransform(HumanBodyBones.RightUpperArm).localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
			fpBodyAnim.GetBoneTransform(HumanBodyBones.LeftUpperArm).localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
		}else{
			Debug.Log("<color=red>Player model avatar error:</color> Please check your player character setup and animator avatar reference.");
		}

		if(alternateBodyMaterial){
			Material[] mats = fpBodySkinnedMesh.gameObject.GetComponent<Renderer>().materials;
			mats[materialToReplace] = alternateBodyMaterial;
			fpBodySkinnedMesh.gameObject.GetComponent<Renderer>().materials = mats;
		}

		//position third person weapon models in player character's right hand
		weaponModels.transform.parent = anim.GetBoneTransform(HumanBodyBones.RightHand);
		weaponModels.transform.localPosition = Vector3.zero;
		weaponModels.transform.localScale = Vector3.one;
		weaponModels.transform.localEulerAngles = weaponModelParentAngles;

		//Initialize all weapon muzzle flash and smoke/shell ejection positions for third person mode
		for(int i = 0; i < weaponModels.thirdPersonWeaponModels.Count; i++){
			WeaponBehavior thisWeapBehavior = PlayerWeaponsComponent.weaponOrder[i].GetComponent<WeaponBehavior>();
			thisWeapBehavior.PlayerModelAnim = anim;
			if(weaponModels.thirdPersonWeaponModels[i].weaponObject){
				thisWeapBehavior.thirdPersonSmokePos = weaponModels.thirdPersonWeaponModels[i].muzzleSmokePos;
				thisWeapBehavior.TPmuzzleRenderer = weaponModels.thirdPersonWeaponModels[i].muzzleFlashRenderer;
				thisWeapBehavior.shellEjectPositionTP = weaponModels.thirdPersonWeaponModels[i].shellEjectPos;
				weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(true);//set obj to true before deactivating to make sure animator is initialized
				if(thisWeapBehavior.tpUseShotgunAnims){
					thisWeapBehavior.tpShotgunAnim = weaponModels.thirdPersonWeaponModels[i].weaponObject.GetComponent<Animator>();
				}
				if(thisWeapBehavior.tpWeaponAnimType == 3){
					thisWeapBehavior.tpPistolAnim = weaponModels.thirdPersonWeaponModels[i].weaponObject.GetComponent<Animator>();
				}
				weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(false);
			}
			if(weaponModels.thirdPersonWeaponModels[i].weaponObject2 != null){
				weaponModels.thirdPersonWeaponModels[i].weaponObject2.SetActive(true);
				if(thisWeapBehavior.tpWeaponAnimType == 6){
					thisWeapBehavior.tpBowAnim = weaponModels.thirdPersonWeaponModels[i].weaponObject2.GetComponentInChildren<Animator>();
				}
				weaponModels.thirdPersonWeaponModels[i].weaponObject2.SetActive(false);
				weaponModels.thirdPersonWeaponModels[i].weaponObject2.transform.parent = anim.GetBoneTransform(HumanBodyBones.LeftHand);
			}
			if(weaponModels.thirdPersonWeaponModels[i].weaponObjectBack != null){
				weaponModels.thirdPersonWeaponModels[i].weaponObjectBack.SetActive(false);
				weaponModels.thirdPersonWeaponModels[i].weaponObjectBack.transform.parent = anim.GetBoneTransform(HumanBodyBones.Chest);
			}
		}

	}


	void Update () {

		if(anim.isInitialized && fpBodyAnim.isInitialized && Time.timeScale > 0.0f){

			WeaponBehaviorComponent = FPSPlayerComponent.WeaponBehaviorComponent;

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Player Model Positioning
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	

			if(!CameraControlComponent.thirdPersonActive && displayVisibleBody){

				if(!walkerComponent.crouched){

					if(walkerComponent.prone || walkerComponent.swimming){//prone or swimming

						rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, 0.0f, 100.0f * Time.smoothDeltaTime);
						modelForwardAmt = Mathf.MoveTowards(modelForwardAmt, modelForwardDown, Time.smoothDeltaTime * 10.0f);
						modelPitchAmt = Mathf.MoveTowards(modelPitchAmt, modelPitch, Time.smoothDeltaTime * 32.0f);
						modelUpAmt = Mathf.MoveTowards(modelUpAmt, modelUpFP, Time.smoothDeltaTime * 8.0f);
						modelScaleAmt = Mathf.MoveTowards(modelScaleAmt, modelStandScale, Time.smoothDeltaTime);
						modelRightAmt = Mathf.Lerp(modelRightAmt, 0.0f, Time.smoothDeltaTime * 7.0f);

					}else{

						if(!walkerComponent.sprintActive){//standing

							if(walkerComponent.moving){
								rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, forwardYawAng, 100.0f * Time.smoothDeltaTime);
							}else{

								if(WeaponBehaviorComponent.tpWeaponAnimType == 0){
									idleYawAngAmt = idleYawAngUnarmed;
								}else{
									idleYawAngAmt = idleYawAng;
								}
								
								if(moveDir == 2){
									rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, idleYawAngAmt + 40.0f, 200.0f * Time.smoothDeltaTime);
								}else if(moveDir == 3){
									rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, idleYawAngAmt - 20.0f, 200.0f * Time.smoothDeltaTime);
								}else{
									rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, idleYawAngAmt, 100.0f * Time.smoothDeltaTime);
								}
							}

							modelForwardAmt = Mathf.Lerp(modelForwardAmt, modelForward, Time.smoothDeltaTime);
							modelPitchAmt = Mathf.MoveTowards(modelPitchAmt, modelPitch, Time.smoothDeltaTime * 32.0f);
							modelUpAmt = Mathf.MoveTowards(modelUpAmt, modelUpFP, Time.smoothDeltaTime * 8.0f);
							modelScaleAmt = Mathf.MoveTowards(modelScaleAmt, modelStandScale, Time.smoothDeltaTime);
							modelRightAmt = Mathf.Lerp(modelRightAmt, 0.0f, Time.smoothDeltaTime * 7.0f);

						}else{//sprinting

							rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, 0.0f, 100.0f * Time.smoothDeltaTime);
							if(moveDir == 2){
								modelRightAmt = Mathf.Lerp(modelRightAmt, sprintStrafeRight, Time.smoothDeltaTime * 7.0f);
							}else if(moveDir == 3){
								modelRightAmt = Mathf.Lerp(modelRightAmt, -sprintStrafeRight, Time.smoothDeltaTime * 7.0f);
							}else{
								modelRightAmt = Mathf.Lerp(modelRightAmt, 0.0f, Time.smoothDeltaTime * 7.0f);
							}
							modelForwardAmt = Mathf.Lerp(modelForwardAmt, modelForwardSprint, Time.smoothDeltaTime);
							modelPitchAmt = Mathf.MoveTowards(modelPitchAmt, modelPitchRun, Time.smoothDeltaTime * 32.0f);
							modelUpAmt = Mathf.MoveTowards(modelUpAmt, modelUpRun, Time.smoothDeltaTime * 8.0f);
							modelScaleAmt = Mathf.MoveTowards(modelScaleAmt, modelStandScale, Time.smoothDeltaTime);

						}

					}

				}else{//crouched

					if(WeaponBehaviorComponent.tpWeaponAnimType == 0 && walkerComponent.moving && (moveDir == 2 || moveDir == 3)){
						rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, yawAngUnarmedCrouchStrafe, 100.0f * Time.smoothDeltaTime);
					}else{
						rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, 0.0f, 100.0f * Time.smoothDeltaTime);
					}
					modelForwardAmt = Mathf.Lerp(modelForwardAmt, modelForwardCrouch, Time.smoothDeltaTime);
					modelPitchAmt = Mathf.MoveTowards(modelPitchAmt, modelPitchCrouch, Time.smoothDeltaTime * 32.0f);
					if(WeaponBehaviorComponent.tpWeaponAnimType == 0){
						modelUpAmt = Mathf.MoveTowards(modelUpAmt, modelUpCrouchUnarmed, Time.smoothDeltaTime * 8.0f);
					}else{
						modelUpAmt = Mathf.MoveTowards(modelUpAmt, modelUpCrouch, Time.smoothDeltaTime * 8.0f);
					}
					modelScaleAmt = Mathf.MoveTowards(modelScaleAmt, modelCrouchScale, Time.smoothDeltaTime);
					modelRightAmt = Mathf.Lerp(modelRightAmt, 0.0f, Time.smoothDeltaTime * 7.0f);

				}

				//set player model scale, position, and rotation from amounts adjusted by stance above for third person model in first person mode (shadow only)
				//fp visible body will be set to same amounts

				myTransform.localScale = new Vector3(modelScaleAmt, modelScaleAmt, modelScaleAmt);

				tempSmoothedPos = new Vector3(playerTransform.position.x, playerTransform.position.y - modelUpAmt, playerTransform.position.z); 
				tempBodyPosition = Vector3.SmoothDamp(tempBodyPosition, tempSmoothedPos, ref dampvel, modelPosSmoothSpeed, Mathf.Infinity, Time.smoothDeltaTime);
				myTransform.position = tempBodyPosition + (playerTransform.forward * modelForwardAmt) + (playerTransform.right * modelRightAmt);


				tempBodyAngles = new Vector3(modelPitchAmt, bodyAngAmt + swimFireAngAmt + playerTransform.eulerAngles.y + rotAngleAmt, 0.0f);
				myTransform.eulerAngles = tempBodyAngles;

				//enable first person visible body object after delay to prevent misalignment of mesh for one frame when transitioning
				//there is probably a better way to do this, but it works for now
				if(tpswitchTime + 0.001 < Time.time || walkerComponent.sprintActive){
					fpBodySkinnedMesh.enabled = true;
					modelPosSmoothSpeed = CameraControlComponent.lerpSpeedAmt;
				}else{
					modelPosSmoothSpeed = 0.0001f;
				}

			}else{

				//set player model scale, position, and rotation for third person model (visible character and shadow, hide first person visible body)

				myTransform.localScale = new Vector3(tpMeshScale, tpMeshScale, tpMeshScale);

				tempSmoothedPos = new Vector3(playerTransform.position.x, playerTransform.position.y - modelUp, playerTransform.position.z); 
				tempBodyPosition = Vector3.SmoothDamp(tempBodyPosition, tempSmoothedPos, ref dampvel, CameraControlComponent.lerpSpeedAmt, Mathf.Infinity, Time.smoothDeltaTime);
				myTransform.position = tempBodyPosition;


				tempBodyAngles = new Vector3(0f, bodyAngAmt + swimFireAngAmt + playerTransform.eulerAngles.y, 0.0f);
				myTransform.localRotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(tempBodyAngles), Time.smoothDeltaTime * slerpSpeedAmt);

				fpBodySkinnedMesh.enabled = false;

			}

			//actually set position, rotation, and scale for first person visible body
			fpBodyTransform.position = myTransform.position;
			fpBodyTransform.rotation = myTransform.rotation;
			fpBodyTransform.localScale = myTransform.localScale;


			//set mesh shadow and visibility modes based on third person and displayVisiblieBody settings
			if(CameraControlComponent.thirdPersonActive){
				if(displayVisibleBody){
					if(tpModeState){
						foreach(SkinnedMeshRenderer sMesh in AllSkinnedMeshes){
							sMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
						}
						foreach(MeshRenderer mesh in AllMeshRenderers){
							mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
						}
						tpModeState = false;
					}
				}else{
					if(tpModeState){
						foreach(SkinnedMeshRenderer sMesh in AllSkinnedMeshes){
							sMesh.enabled = true;
							sMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
						}
						foreach(MeshRenderer mesh in AllMeshRenderers){
							mesh.enabled = true;
							mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
						}
						tpModeState = false;
					}
				}
			}else{
				if(displayVisibleBody){
					if(!tpModeState){
						foreach(SkinnedMeshRenderer sMesh in AllSkinnedMeshes){
							sMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
						}
						foreach(MeshRenderer mesh in AllMeshRenderers){
							mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
						}
						tpModeState = true;
					}
				}else{
					if(!tpModeState){
						foreach(SkinnedMeshRenderer sMesh in AllSkinnedMeshes){
							sMesh.enabled = false;
							sMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
						}
						foreach(MeshRenderer mesh in AllMeshRenderers){
							mesh.enabled = false;
							mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
						}
						tpModeState = true;
					}
				}
			}


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Input and Movement States
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	

			moveInputs = new Vector2(walkerComponent.inputX, walkerComponent.inputY);
			moveSmoothed =  Vector2.Lerp(moveSmoothed, moveInputs, Time.smoothDeltaTime * inputSmoothingSpeed);

			//smoothly blend strafing angles for animators
			if(Mathf.Abs(walkerComponent.inputY) > 0.5f){
				anim.SetFloat("StrafeAmt", 0.0f, 0.4f, Time.deltaTime);
				fpBodyAnim.SetFloat("StrafeAmt", 0.0f, 0.4f, Time.deltaTime);
			}else{
				anim.SetFloat("StrafeAmt", Mathf.Abs(walkerComponent.inputX), 0.4f, Time.deltaTime);
				fpBodyAnim.SetFloat("StrafeAmt", Mathf.Abs(walkerComponent.inputX), 0.4f, Time.deltaTime);
			}

			anim.SetFloat("MoveX", walkerComponent.inputX, 0.3f, Time.deltaTime);
			anim.SetFloat("MoveY", walkerComponent.inputY, 0.4f, Time.deltaTime);

			fpBodyAnim.SetFloat("MoveX", walkerComponent.inputX, 0.3f, Time.deltaTime);
			fpBodyAnim.SetFloat("MoveY", walkerComponent.inputY, 0.4f, Time.deltaTime);

			if(walkerComponent.moving){
				moveTime = Time.time;
			}else{ 
				idleTime = Time.time;
			}

			//get angle difference between character model and camera yaw for turning animations
			if(!walkerComponent.swimming && CameraControlComponent.tpPressTime + 0.15f < Time.time){
				if(CameraControlComponent.thirdPersonActive){
					facingAngleDelta = Mathf.DeltaAngle(myTransform.eulerAngles.y, tempBodyAngles.y);
				}else{
					facingAngleDelta = Mathf.Lerp(facingAngleDelta, SmoothMouseLookComponent.horizontalDelta, Time.smoothDeltaTime * 10.0f);
				}
			}else{
				facingAngleDelta = 0.0f;
			}

			//track the time that view rotate input has exceeded 0.5
			//so turning animations can be played, even if joystick input
			//has a lower max value than the mouse
			if(Mathf.Abs(SmoothMouseLookComponent.horizontalDelta) > 0.5f
			&& (FPSPlayerComponent.zoomed || fireTime + (fireUpTime * 0.4f) > Time.time)){
				turnTimer += Time.deltaTime;
			}else{
				turnTimer = 0.0f;
			}

			//make the facingAngleDelta exceed the turning animation threshold if turnTimer is longer than 0.6sec
			if(turnTimer > 0.6f){
				facingAngleDelta = 16.5f;
			}

			//play forward animation for all directions if prone, otherwise, move normally
			if(!walkerComponent.prone){
				forwardSpeedAmt = Mathf.Max(Mathf.Abs(moveSmoothed.y), Mathf.Abs(moveSmoothed.x));
			}else{
				forwardSpeedAmt = Mathf.Max(Mathf.Abs(moveSmoothed.y), Mathf.Abs(moveSmoothed.x)) * Mathf.Sign(moveSmoothed.y);
			}

			//set moveDir to movement direction for animator MoveDir parameter
			if(moveInputs.y > InputComponent.deadzone){
				moveDir = 1;//forwards
			}else if(moveInputs.y < -InputComponent.deadzone){
				moveDir = 4;//backwards
			}else if(moveInputs.x > InputComponent.deadzone || (meleeMove && WeaponBehaviorComponent.swingSide && !walkerComponent.prone)){
				if(meleeMove){
					forwardSpeedAmt = 0.5f;//play forwards anim so legs do something when playing torso left melee attack anim
				}
				moveDir = 3;//strafe right
			}else if(moveInputs.x < -InputComponent.deadzone || (meleeMove && !WeaponBehaviorComponent.swingSide && !walkerComponent.prone)){
				if(meleeMove){
					forwardSpeedAmt = -0.4f;//play forwards anim so legs do something when playing torso right melee attack anim
				}
				moveDir = 2;//strafe left
			}else{//return to idle
				if(moveTime + 0.35f < Time.time){//delay idle to prevent jerky blending
					if(walkerComponent.swimming && (InputComponent.jumpHold || InputComponent.crouchHold)){
						moveDir = 1;//use to play forward animation when swimming straight up or down
						forwardSpeedAmt = 1.0f;
					}else{
						if((CameraControlComponent.thirdPersonActive && facingAngleDelta > 16.0f) 
						|| (!CameraControlComponent.thirdPersonActive && facingAngleDelta > 0.4f)){
							anim.SetFloat("StrafeAmt", 1f, 0.6f, Time.deltaTime);
							fpBodyAnim.SetFloat("StrafeAmt", 1f, 0.6f, Time.deltaTime);
							moveDir = 3;//turn right
							forwardSpeedAmt = 1.0f;
							if(walkerComponent.prone){
								proneMoveTime = Time.time;
							}
						}else if((CameraControlComponent.thirdPersonActive && facingAngleDelta < -16.0f) 
						|| (!CameraControlComponent.thirdPersonActive && facingAngleDelta < -0.4f)){
							anim.SetFloat("StrafeAmt", 1f, 0.6f, Time.deltaTime);
							fpBodyAnim.SetFloat("StrafeAmt", 1f, 0.6f, Time.deltaTime);
							moveDir = 2;//turn left
							forwardSpeedAmt =  1.0f;
							if(walkerComponent.prone){
								proneMoveTime = Time.time;
							}
						}else{
							moveDir = 0;//idle
						}
					}
				}else{
					moveDir = 0;//idle
				}
			}
	
			anim.SetInteger("MoveDir", moveDir);
			fpBodyAnim.SetInteger("MoveDir", moveDir);

			//track player's progress from standing, crouching, and prone for animator ProneTransition parameter
			if(walkerComponent.grounded){
				if(walkerComponent.prone){
					if(!proneState){
						proneStanceTime = Time.time;
						proneState = true;
					}
					if(proneStanceTime + proneTransitionLength > Time.time){
						proneTransition = 1;
					}else{
						proneTransition = 0;
					}
				}else{
					if(proneState){
						proneStanceTime = Time.time;
						proneState = false;
					}
					if(proneStanceTime + proneTransitionLength > Time.time){
						if(!walkerComponent.crouched){
							proneTransition = 2;
						}else{
							proneTransition = 3;
						}
					}else{
						proneTransition = 0;
					}

					//do this to allow quicker return from prone to standing arms animation when not using a rifle type weapon
					if(proneStanceTime + (proneTransitionLength * 0.8f) < Time.time && !walkerComponent.crouched){
						moveState = 0;
					}

				}
			}else{
				proneTransition = 0;
			}

			//don't allow firing in third person if changing to or from prone stance
			if(proneTransition == 0){
				WeaponBehaviorComponent.disableFiring = false;
			}else{
				WeaponBehaviorComponent.disableFiring = true;
			}

			anim.SetInteger("ProneTransition", proneTransition);
			fpBodyAnim.SetInteger("ProneTransition", proneTransition);

			//set player movement state/stance for animator MoveState parameter
			if(FPSPlayerComponent.hitPoints > 0){
				if(proneTransition == 0){
					if(walkerComponent.grounded){
						if(walkerComponent.sprintActive && forwardSpeedAmt > InputComponent.deadzone){
							if(moveInputs.y > -0.5f){
								forwardSpeedAmt = Mathf.Abs(forwardSpeedAmt);
							}else{
								forwardSpeedAmt = -Mathf.Abs(forwardSpeedAmt);
							}
							if(Mathf.Abs(moveInputs.y) >= InputComponent.deadzone || Mathf.Abs(moveInputs.x) >= InputComponent.deadzone){
								moveState = 1;//sprinting
								sprintTime = Time.time;
								modelForwardAmt = Mathf.Lerp(modelForwardAmt, modelForwardSprint, Time.smoothDeltaTime * 7.0f);
							}
							fireTime = -16.0f;

						}else{
							if(walkerComponent.crouched){
								moveState = 2;//crouching
							}else if(walkerComponent.swimming){
								if((!FPSPlayerComponent.zoomed && fireTime + (fireUpTime * 0.4f) < Time.time && WeaponBehaviorComponent.tpWeaponAnimType != 6) 
								|| (WeaponBehaviorComponent.tpWeaponAnimType == 6 && !WeaponBehaviorComponent.pullAnimState)){
									moveState = 4;//swimming
								}else{
									moveState = 5;//swimming and firing
								}
							}else if(walkerComponent.prone){
								moveState = 6;//prone
							}else{
								moveState = 0;//walking
							}
							modelForwardAmt = Mathf.Lerp(modelForwardAmt, modelForward, Time.smoothDeltaTime * 7.0f);
						}
					}else{
						moveState = 3;//jumping/airborn
					}
				}
			}else{
				moveState = 7;//dead
			}



			//blend swim<->treading water anims with a lerp because angle differences are large and 
			//lerp-blending looks better than default linear-type blending for large transitions
			if(moveDir != 0){
				swimBlendAmt = 1.0f;
			}else{
				swimBlendAmt = 0.0f;
			}
			swimBlend = Mathf.Lerp(swimBlend, swimBlendAmt, Time.deltaTime * 2.0f);
			anim.SetFloat("SwimBlend", swimBlend);
			fpBodyAnim.SetFloat("SwimBlend", swimBlend);

			anim.SetInteger("MoveState", moveState);
			fpBodyAnim.SetInteger("MoveState", moveState);

			if(WeaponBehaviorComponent.tpWeaponAnimType == 0){
				if(moveInputs.y > -0.5f){
					forwardSpeedAmt = Mathf.Abs(forwardSpeedAmt);
				}else{
					forwardSpeedAmt = -Mathf.Abs(forwardSpeedAmt);
				}
			}

			if(FPSPlayerComponent.zoomed){
				anim.SetFloat("ForwardSpeed", forwardSpeedAmt * 0.85f);
				fpBodyAnim.SetFloat("ForwardSpeed", forwardSpeedAmt * 0.85f);
			}else{
				anim.SetFloat("ForwardSpeed", forwardSpeedAmt);
				fpBodyAnim.SetFloat("ForwardSpeed", forwardSpeedAmt);
			}

			if(WeaponBehaviorComponent.tpUseShotgunAnims){
				anim.SetBool("ShotgunAnim", true);
			}else{
				anim.SetBool("ShotgunAnim", false);
			}

			if(WeaponBehaviorComponent.tpOffhandMeleeIsBash){
				anim.SetBool("OffhandMeleeBash", true);
			}else{
				anim.SetBool("OffhandMeleeBash", false);
			}


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Angles and Weapons
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	

			//rotate player model in movement/input direction
			if(((!FPSPlayerComponent.zoomed && (fireTime + (fireUpTime * fireDelay) < Time.time && !WeaponBehaviorComponent.meleeActive)) 
			|| (walkerComponent.sprintActive && (fireTime + (fireUpTime * fireDelay) < Time.time))
			|| (walkerComponent.swimming && moveState != 5)
			|| (WeaponBehaviorComponent.tpWeaponAnimType == 0 && (walkerComponent.crouched || walkerComponent.moving)))
			&& !PlayerWeaponsComponent.offhandThrowActive
			&& !WeaponBehaviorComponent.pullAnimState){

				if(moveInputs.y >= InputComponent.deadzone){
					if(walkerComponent.sprintActive || walkerComponent.swimming 
					|| (WeaponBehaviorComponent.tpWeaponAnimType == 0 && walkerComponent.moving)){
						bodyAng = Mathf.Atan2(moveInputs.x, moveInputs.y) * Mathf.Rad2Deg;
					}else{
						if(moveInputs.y > 0.5f){
							bodyAng = Mathf.Atan2(moveInputs.x, moveInputs.y) * Mathf.Rad2Deg;
						}else{
							bodyAng = 0.0f;
						}
					}
				}else{
					if(moveInputs.y < -0.5f){//backpedal
						bodyAng = Mathf.Atan2(-moveInputs.x, -moveInputs.y) * Mathf.Rad2Deg;
					}else{
						if(walkerComponent.sprintActive || walkerComponent.swimming 
						|| (WeaponBehaviorComponent.tpWeaponAnimType == 0 && walkerComponent.moving)){
							bodyAng = Mathf.Atan2(moveInputs.x, moveInputs.y) * Mathf.Rad2Deg;
						}else{
							bodyAng = 0.0f;
						}
					}
				}
			}else{//keep player model angle facing forward
				bodyAng = 0.0f;
			}

			bodyAngAmt = Mathf.LerpAngle(bodyAngAmt, bodyAng, Time.deltaTime * 5f);

			//make player face forward with offset when firing underwater
			if(walkerComponent.swimming && ((moveState == 5 && WeaponBehaviorComponent.tpWeaponAnimType != 6) || (WeaponBehaviorComponent.pullAnimState && WeaponBehaviorComponent.tpWeaponAnimType == 6))
			&& (WeaponBehaviorComponent.tpWeaponAnimType != 1 
		    && WeaponBehaviorComponent.tpWeaponAnimType != 2
		    && !WeaponBehaviorComponent.meleeActive)){

				swimFireAngAmt = Mathf.MoveTowards(swimFireAngAmt, swimFireAng, Time.deltaTime * 448.0f);

			}else{
				swimFireAngAmt = Mathf.MoveTowards(swimFireAngAmt, 0.0f, Time.deltaTime * 64.0f);
			}

			//adjust speed of player model yaw angle smoothing (increase smooth speed if player is rapidly turning to prevent unnatural twisting of player bones)
			if((Mathf.Abs(Mathf.DeltaAngle(myTransform.eulerAngles.y, tempBodyAngles.y)) > 90f || FPSPlayerComponent.zoomed || fireTime + fireUpTime > Time.time) && idleTime + 0.6f < Time.time){
				slerpSpeedAmt = Mathf.MoveTowards(slerpSpeedAmt, 36f, Time.deltaTime * 128f);
			}else{
				slerpSpeedAmt = Mathf.MoveTowards(slerpSpeedAmt, slerpSpeed, Time.deltaTime * 64f);
			}

			//find angle difference, taking into account negative angles
			boneAng1 = Vector3.Angle(playerTransform.forward, myTransform.forward);
			Vector3 cross = Vector3.Cross(playerTransform.forward, myTransform.forward);
			if(cross.y < 0.0f){
				boneAng1 = -boneAng1;
			}

			//track time that player last moved when prone
			if(walkerComponent.prone && walkerComponent.moving){
				proneMoveTime = Time.time;
			}

			//delay firing effects until player model has centered weapon forward aim, but allow instant firing (feels more responsive)
			if(weapDownTime + 0.14f > Time.time 
			|| (Mathf.Abs(Mathf.DeltaAngle(mainCamTransform.eulerAngles.y, myTransform.eulerAngles.y - swimFireAngAmt)) > 15f)
			|| sprintTime + 0.07f > Time.time
			|| proneMoveTime + 0.06f > Time.time){
				WeaponBehaviorComponent.muzzActive = false; 
			}else{
				WeaponBehaviorComponent.muzzActive = true; 
			}

			//set speed of bone rotation 
			if(idleTime + 0.6f < Time.time && (FPSPlayerComponent.zoomed || WeaponBehaviorComponent.shooting)){
				boneSpeedAmt = Mathf.MoveTowards(boneSpeedAmt, 64f, Time.deltaTime * 120);
			}else{
				if(Mathf.Abs(Mathf.DeltaAngle(mainCamTransform.eulerAngles.y, playerObj.transform.eulerAngles.y)) > 30f){
					boneSpeedAmt = Mathf.MoveTowards(boneSpeedAmt, boneSpeed, Time.deltaTime * 256f);
				}else{
					boneSpeedAmt = Mathf.MoveTowards(boneSpeedAmt, 16f, Time.deltaTime * 120f);
				}

			}

			//set yaw angle of player when firing or zoomed underwater
			float swimAng;

			if(walkerComponent.swimming && (FPSPlayerComponent.zoomed || fireTime + fireUpTime > Time.time)){
				swimAng = -35f;
			}else{
				swimAng = 0.0f;
			}

			//set yaw angle of player when firing or zoomed
			if((FPSPlayerComponent.zoomed || fireTime + fireUpTime > Time.time) && WeaponBehaviorComponent.tpWeaponAnimType != 3){
				aimOffsetAmt = aimOffset;
			}else{
				aimOffsetAmt = 0.0f;
			}

			//hide arrow in player's hand until they grab it out of the quiver on their back
			if(WeaponBehaviorComponent.tpWeaponAnimType == 6){
				if(WeaponBehaviorComponent.pullAnimState){
					if(weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject != null
					&& ((WeaponBehaviorComponent.pullTimer + arrowVisibleDelay < Time.time && !walkerComponent.prone) 
					||(WeaponBehaviorComponent.pullTimer + (arrowVisibleDelay * 0.3f) < Time.time && walkerComponent.prone))){
						weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject.SetActive(true);
					}
				}else{
					if(weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject != null){
						weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject.SetActive(false);
					}
				}
			}

			//smooth and limit bone angles
			boneAng1Amt = Mathf.LerpAngle(boneAng1Amt, boneAng1 + swimAng + aimOffsetAmt, Time.deltaTime * boneSpeedAmt);
			boneAng1Amt = Mathf.Clamp(boneAng1Amt, -90f, 90f);

			//set firing timer for animations
			if(WeaponBehaviorComponent.shooting){
				if(proneTransition == 0){
					fireTime = Time.time;
				}
			}else{
				if(proneTransition != 0 || (walkerComponent.prone && walkerComponent.moving)){
					fireTime = -16.0f;
				}
			}

			//set reloading timer and state for animations
			if(FPSPlayerComponent.IronsightsComponent.reloading){
				if(!reloadStartTimeState){
					if(WeaponBehaviorComponent.tpPistolAnim && WeaponBehaviorComponent.tpPistolAnim.isInitialized){
						WeaponBehaviorComponent.tpPistolAnim.SetTrigger("Reload");
					}
					reloadStartTime = Time.time;
					reloadStartTimeState = true;
				}
			}else{
				if(reloadStartTimeState){
					if(WeaponBehaviorComponent.tpPistolAnim && WeaponBehaviorComponent.tpPistolAnim.isInitialized){
						WeaponBehaviorComponent.tpPistolAnim.SetTrigger("Idle");
					}
					reloadStartTimeState = false;
				}
			}

			//set different reloading times for the type of weapon being used
			if(WeaponBehaviorComponent.tpWeaponAnimType == 5){
				if (WeaponBehaviorComponent.tpShotgunAnim){
					reloadDurationAmt = reloadDurationShotgun;
				}else{
					reloadDurationAmt = reloadDurationRifle;
				}
			}else if (WeaponBehaviorComponent.tpWeaponAnimType == 3){
				reloadDurationAmt = reloadDurationPistol;
			}

			//modify weapon aiming time after firing by type of weapon used		
			if(!WeaponBehaviorComponent.meleeActive){
				if(WeaponBehaviorComponent.tpWeaponAnimType == 1 || WeaponBehaviorComponent.tpWeaponAnimType == 2){
					if(WeaponBehaviorComponent.swingSide || walkerComponent.prone){
						fireDelay = WeaponBehaviorComponent.tpSwingTimeL;
					}else{
						fireDelay = WeaponBehaviorComponent.tpSwingTimeR;
					}
				}else{
					fireDelay = 1.0f;
				}
			}else{
				if(WeaponBehaviorComponent.tpOffhandMeleeIsBash){
					fireDelay = offhandMeleeTime * 0.25f;
				}else{
					fireDelay = offhandMeleeTime;
				}
				reloadStartTime = -16.0f;//cancel reload if performing offhand melee attack
			}


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Player Stances and States
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	

			if(FPSPlayerComponent.hitPoints > 0){
				//detect reloading state
				if(reloadStartTime + reloadDurationAmt > Time.time 
				&& PlayerWeaponsComponent.switchTime + switchDuration < Time.time 
				&& !WeaponBehaviorComponent.meleeActive){
					torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1.0f, Time.deltaTime * 9f);
					if(CameraControlComponent.thirdPersonActive){
						chestAnglesAmt = Vector3.Lerp(chestAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 32.0f);
						spineAnglesAmt =  Vector3.Lerp(spineAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 32.0f);
					}

					fireTime = -16.0f;			
					anim.SetInteger("ArmsState", 3);//reload
					rotateHead = true;

				}else{
					//detect firing or zoomed state
					if((FPSPlayerComponent.zoomed 
					|| (fireTime + (fireUpTime * fireDelay) > Time.time && !walkerComponent.swimming) 
					|| (walkerComponent.swimming && fireTime + (fireUpTime * 0.5f) > Time.time && WeaponBehaviorComponent.tpWeaponAnimType != 6)
					|| WeaponBehaviorComponent.pullAnimState
					//make player shadow look in camera forward direction if in first person and not switching weapons or throwing grenade
					|| (!CameraControlComponent.thirdPersonActive 
						&& lastWeapon == PlayerWeaponsComponent.CurrentWeaponBehaviorComponent.weaponNumber 
						&& !PlayerWeaponsComponent.offhandThrowActive
						&& !walkerComponent.swimming
						&& fpTorsoAlwaysAims)
					|| PlayerWeaponsComponent.offhandThrowActive) 
					&& proneTransition == 0
					&& PlayerWeaponsComponent.switchTime + (switchDuration * 1.1f) < Time.time){ 

						///when prone, rotate player chest and spine angles to look in aiming direction by the mLookAngProne values///

						if(walkerComponent.prone){

							spineAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAngProne.x, 
														 SmoothMouseLookComponent.inputY * mLookAngProne.y, 
														 SmoothMouseLookComponent.inputY * mLookAngProne.z);


							chestAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAngProne2.x, 
														 SmoothMouseLookComponent.inputY * mLookAngProne2.y, 
														 SmoothMouseLookComponent.inputY * mLookAngProne2.z);

						}else{

							///when aiming a bow, rotate player chest and spine angles to look in aiming direction by the mLookAngBow values///

							if(WeaponBehaviorComponent.tpWeaponAnimType == 6 && WeaponBehaviorComponent.pullAnimState && !CameraControlComponent.rotating){

								spineAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAngBow.x, 
															 SmoothMouseLookComponent.inputY * mLookAngBow.y - (angOffset * 0.6f), 
															 SmoothMouseLookComponent.inputY * mLookAngBow.z);


								chestAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAngBow2.x, 
															 SmoothMouseLookComponent.inputY * mLookAngBow2.y - (angOffset * 0.4f), 
															 SmoothMouseLookComponent.inputY * mLookAngBow2.z);

							}else if(!CameraControlComponent.rotating){

								///rotate player chest and spine angles to look in aiming direction by the mLookAng values for all other weapons and states///

								spineAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAng.x, 
															 SmoothMouseLookComponent.inputY * mLookAng.y - (angOffset * 0.6f), 
															 SmoothMouseLookComponent.inputY * mLookAng.z);


								chestAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAng2.x, 
															 SmoothMouseLookComponent.inputY * mLookAng2.y - (angOffset * 0.4f), 
															 SmoothMouseLookComponent.inputY * mLookAng2.z);

							}

						}

						//handle the animator layer weights based on weapon type and states
						if((!walkerComponent.prone || WeaponBehaviorComponent.tpWeaponAnimType != 5)){
							torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1.0f, Time.deltaTime * 9f);
						}else{
							if(fireTime + fireUpTime > Time.time && !WeaponBehaviorComponent.meleeActive){
								torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1.0f, Time.deltaTime * 9f);
							}else{
								torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0.0f, Time.deltaTime * 9f);
							}
						}

						//adjust torso layer weights and yaw angles for various player stance and weapon states
						if((((WeaponBehaviorComponent.tpWeaponAnimType == 1 || WeaponBehaviorComponent.tpWeaponAnimType == 2 || WeaponBehaviorComponent.meleeActive) 
						&& (fireTime + (fireUpTime * fireDelay) > Time.time)) || WeaponBehaviorComponent.pullAnimState || PlayerWeaponsComponent.offhandThrowActive)){

							if((walkerComponent.moving && !walkerComponent.swimming) 
							|| (WeaponBehaviorComponent.pullAnimState && WeaponBehaviorComponent.tpWeaponAnimType == 6)){
								if(WeaponBehaviorComponent.pullAnimState && WeaponBehaviorComponent.tpWeaponAnimType == 6 && !walkerComponent.swimming){
									if(moveDir == 1 || moveDir == 4){
										angOffset = Mathf.LerpAngle(angOffset, -30.0f, Time.deltaTime * 7.0f);//adjust torso yaw angle to aim bow animation forward when strafing
									}else if(walkerComponent.crouched){
										angOffset = Mathf.LerpAngle(angOffset, -20.0f, Time.deltaTime * 7.0f);//adjust torso yaw angle to aim bow animation forward when crouched
									}else{
										angOffset = Mathf.LerpAngle(angOffset, 0.0f, Time.deltaTime * 7.0f);//rest torso yaw angle to zero
									}
								}else{
									if(walkerComponent.swimming && WeaponBehaviorComponent.pullAnimState && WeaponBehaviorComponent.tpWeaponAnimType == 6){//bow firing underwater
										angOffset = Mathf.LerpAngle(angOffset, 10.0f, Time.deltaTime * 7.0f);
									}else{
										angOffset = Mathf.LerpAngle(angOffset, 0.0f, Time.deltaTime * 7.0f);
									}
								}
								meleeMove = false;
							}else{
								angOffset = Mathf.LerpAngle(angOffset, 0.0f, Time.deltaTime * 7.0f);
								if(fireTime + 0.0175f > Time.time){
									meleeMove = true;//make player move their legs when torso is performing melee animation 
								}else{
									meleeMove = false;
								}	
							}
							
							torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 1.0f, Time.deltaTime * 7.0f);

						}else{//reset player yaw angles and attack parameters
							torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 0.0f, Time.deltaTime * 7.0f);
							angOffset = Mathf.LerpAngle(angOffset, 0.0f, Time.deltaTime * 9.0f);
							meleeMove = false;
							if(WeaponBehaviorComponent.meleeActive){
								fireTime = -16.0f;
							}
						}

						if(fireTime + fireShotTime > Time.time){
							anim.SetInteger("ArmsState", 2);//weapon fire
						}else{
							if(WeaponBehaviorComponent.zoomIsBlock && FPSPlayerComponent.zoomed){
								anim.SetInteger("ArmsState", 5);//weapon block
							}else{
								anim.SetInteger("ArmsState", 1);//weapon up
							}
						}

						rotateHead = false;//don't rotate head when firing (torso will already be facing correct direction)

					}else{//NOT FIRING (idle state)

						//reset chest and spine angles to zero when idle
						chestAnglesAmt = Vector3.Lerp(chestAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 24.0f);
						spineAnglesAmt =  Vector3.Lerp(spineAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 24.0f);

						if(PlayerWeaponsComponent.switchTime + switchDuration > Time.time){
							
							anim.SetInteger("ArmsState", 4);//weapon switching
							reloadStartTime = -16.0f;
							fireTime = -16.0f;
							torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1.0f, Time.deltaTime * 3f);
							torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 0.0f, Time.deltaTime * 4f);

						}else{

							if(!walkerComponent.grounded){
								torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1.0f, Time.deltaTime * 3f);
							}else{
								if(WeaponBehaviorComponent.tpWeaponAnimType == 5){//use default torso anim for rifle weapons, but use torso layer for other weapon types
									if(walkerComponent.crouched && proneTransition == 0){//use torso layer when crouched or transitioning to or from prone
										torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1.0f, Time.deltaTime * 3f);
									}else{
										torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0.0f, Time.deltaTime * 3f);
									}
								}else{//need some extra logic here to determine when to use blending on non-rifle type weapons
									if(walkerComponent.crouched && proneTransition == 0){
										if(WeaponBehaviorComponent.tpWeaponAnimType == 0){
											torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0.0f, Time.deltaTime * 3f);
										}else{
											torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1.0f, Time.deltaTime * 3f);
										}
									}else{
										//play body anims when rising from prone and moving when prone
										if((walkerComponent.prone && moveDir != 0) || proneTransition == 1 || (walkerComponent.swimming && moveState != 5)){
											torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0.0f, Time.deltaTime * 3f);
										}else if(walkerComponent.prone && WeaponBehaviorComponent.tpWeaponAnimType == 0){
											torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1.0f, Time.deltaTime * 3f);
										}else{//otherwise, play arm animations for idle, aiming, and firing
											if(WeaponBehaviorComponent.tpWeaponAnimType == 0){
												torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0.0f, Time.deltaTime * 3f);
											}else{
												torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1.0f, Time.deltaTime * 3f);
											}
										}
									}
								}
							}

							anim.SetInteger("ArmsState", 0);//weapon down

							SelectCurrentWeapon();//hide or show weapon model from third person weapons list based on currently selected weapon

						}

						//hide weapon models if the player is offhand-throwing a grenade
						if(!PlayerWeaponsComponent.offhandThrowActive){
							torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 0.0f, Time.deltaTime * 4f);
							offhandState = false;
						}else{
							if(!offhandState && lastWeapon != PlayerWeaponsComponent.CurrentWeaponBehaviorComponent.weaponNumber){
								anim.SetTrigger("Pull");
								if(weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject != null){
									weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject.SetActive(false);
								}
								if(!PlayerWeaponsComponent.offhandThrowActive){
									if(weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject2 != null){
										weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject2.SetActive(false);
									}
									if(weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObjectBack != null){
										weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObjectBack.SetActive(false);
									}
								}
								offhandState = true;
							}
							torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 1.0f, Time.deltaTime * 4f);
							lastWeapon = -lastWeapon;//to trigger weapon model activation after canceling reload with offhand throw
						}

						//reset weapon firing timers and states when idle
						anim.ResetTrigger("Fire");
						weapDownTime = Time.time;
						fireTime = -16.0f;//to reset weapon states, parameters, and anims if idle
						meleeMove = false;//reset feet shifting for throwing, melee
						angOffset = 0.0f;//reset torso yaw angle offset when idle (prevents rifles from rotating like the bow animation needs to)
						rotateHead = true;

					}
				}
			}else{//player is dead, reset all yaw and bone angles to zero
				rotateHead = false;
				chestAnglesAmt = Vector3.Lerp(chestAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 24.0f);
				spineAnglesAmt =  Vector3.Lerp(spineAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 24.0f);
				torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 0.0f, Time.deltaTime * 8f);
				torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0.0f, Time.deltaTime * 8f);

				//hide third person weapon models when player dies
				if(!deadState){
					for(int i = 0; i < weaponModels.thirdPersonWeaponModels.Count; i++){
						if(weaponModels.thirdPersonWeaponModels[i].weaponObject){
							weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(false);
						}
						if(weaponModels.thirdPersonWeaponModels[i].weaponObject2 != null){
							weaponModels.thirdPersonWeaponModels[i].weaponObject2.SetActive(false);
						}
						if(weaponModels.thirdPersonWeaponModels[i].weaponObjectBack != null){
							weaponModels.thirdPersonWeaponModels[i].weaponObjectBack.SetActive(false);
						}
					}
					deadState = true;
				}
			}

			//this might not be needed anymore, will test
			if(!CameraControlComponent.thirdPersonActive){
				CameraControlComponent.constantLooking = true;
			}else{
				CameraControlComponent.constantLooking = false;
			}

			//set animator layer weights from torsoLayerWeight values
			anim.SetLayerWeight(1, torsoLayerWeight);
			anim.SetLayerWeight(2, torsoLayerWeight2);
			//smooth spine angles
			spineAngles.x = Mathf.LerpAngle(spineAngles.x, spineAnglesAmt.x, Time.deltaTime * 12f);
			spineAngles.y = Mathf.LerpAngle(spineAngles.y, spineAnglesAmt.y, Time.deltaTime * 24f);
			spineAngles.z = Mathf.LerpAngle(spineAngles.z, spineAnglesAmt.z, Time.deltaTime * 12f);
			//smooth chest angles
			chestAngles.x = Mathf.LerpAngle(chestAngles.x, chestAnglesAmt.x, Time.deltaTime * 12f);
			chestAngles.y = Mathf.LerpAngle(chestAngles.y, chestAnglesAmt.y, Time.deltaTime * 24f);
			chestAngles.z = Mathf.LerpAngle(chestAngles.z, chestAnglesAmt.z, Time.deltaTime * 12f);

		}

	}


	void LateUpdate () {//use LateUpdate() to actually set the bone angles because they are overwitten if set in Update()
		
		if(rotateHead && headBone && headRotation > 0.0f){
			if(walkerComponent.crouched){
				headBone.eulerAngles += new Vector3(0f, 0.0f - boneAng1Amt * headRotationCrouch, 0.0f);
			}else{
				headBone.eulerAngles += new Vector3(0f, 0.0f - boneAng1Amt * headRotation, 0.0f);
			}
		}

		if(spineBone){
			spineBone.eulerAngles += spineAngles;
		}

		if(chestBone){
			chestBone.eulerAngles += chestAngles;
		}
	}


	public void SelectCurrentWeapon(){//hide or show weapon model from third person weapons list based on currently selected weapon
		if(lastWeapon != PlayerWeaponsComponent.CurrentWeaponBehaviorComponent.weaponNumber){
			for(int i = 0; i < weaponModels.thirdPersonWeaponModels.Count; i++){
				
				if(i == WeaponBehaviorComponent.weaponNumber){
					if(weaponModels.thirdPersonWeaponModels[i].weaponObject != null){
						if(WeaponBehaviorComponent.tpWeaponAnimType == 6){
							weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(false);//hide arrow object if bow after equipping
						}else{
							weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(true);//show main weapon model if this is current weapon
						}
					}
					if(weaponModels.thirdPersonWeaponModels[i].weaponObject2 != null){
						weaponModels.thirdPersonWeaponModels[i].weaponObject2.SetActive(true);//show left hand model if this weapon has one
					}
					if(weaponModels.thirdPersonWeaponModels[i].weaponObjectBack != null){
						weaponModels.thirdPersonWeaponModels[i].weaponObjectBack.SetActive(true);//show back model if this weapon has one
					}
					anim.SetInteger("WeaponType", WeaponBehaviorComponent.tpWeaponAnimType);
					fpBodyAnim.SetInteger("WeaponType", WeaponBehaviorComponent.tpWeaponAnimType);
					lastWeapon = WeaponBehaviorComponent.weaponNumber;
				}else{//hide weapon model if this is not the currently selected weapon
					if(weaponModels.thirdPersonWeaponModels[i].weaponObject != null){
						weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(false);
					}

					if(!PlayerWeaponsComponent.offhandThrowActive){
						if(weaponModels.thirdPersonWeaponModels[i].weaponObject2 != null){
							weaponModels.thirdPersonWeaponModels[i].weaponObject2.SetActive(false);
						}
						if(weaponModels.thirdPersonWeaponModels[i].weaponObjectBack != null){
							weaponModels.thirdPersonWeaponModels[i].weaponObjectBack.SetActive(false);
						}
					}
				}
			}
		}

	}



}
