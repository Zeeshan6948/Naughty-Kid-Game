//Ironsights.cs by Azuline Studios© All Rights Reserved
//Adjusts weapon position and bobbing speeds and magnitudes 
//for various player states like zooming, sprinting, and crouching.
using UnityEngine;
using System.Collections;

public class Ironsights : MonoBehaviour {
	//Set up external script references
	[HideInInspector]
	public SmoothMouseLook SmoothMouseLook;
	[HideInInspector]
	public CamAndWeapAnims CamAndWeapAnimsComponent;
	[HideInInspector]
	public Animator CamAndWeapAnimator;
	private PlayerWeapons PlayerWeaponsComponent;
	private FPSRigidBodyWalker FPSWalker;
	private FPSPlayer FPSPlayerComponent;
	private InputControl InputComponent;
	[HideInInspector]
	public WeaponBehavior WeaponBehaviorComponent;
	[HideInInspector]
	public WeaponPivot WeaponPivotComponent;
	private Animation GunAnimationComponent;
	private CameraControl CameraControlComponent;
	//other objects accessed by this script
	[HideInInspector]
	public GameObject playerObj;
	[HideInInspector]
	public GameObject weaponObj;
	[HideInInspector]
	public GameObject CameraObj;
	[HideInInspector]
	public Camera WeapCameraObj;
	//weapon object (weapon object child) set by PlayerWeapons.cs script
	[HideInInspector]
	public GameObject gunObj;
	//Var set to sprint animation time of weapon
	[HideInInspector]
	public Transform gun;//this set by PlayerWeapons script to active weapon transform
		
	//weapon positioning	
	[HideInInspector]
	public float zPosRecNext = 0.0f;//weapon recoil z position that is smoothed using smoothDamp function
	[HideInInspector]
	public float zPosRec = 0.0f;//target weapon recoil z position that is smoothed using smoothDamp function
	private float recZDamp = 0.0f;//velocity that is used by smoothDamp function
	[HideInInspector]
	public  Vector3 nextPos;//weapon z position that is smoothed using smoothDamp function
	[HideInInspector]
	public  Vector3 newPos;//target weapon z position that is smoothed using smoothDamp function
	private Vector3 bobPos;
	private Vector3 dampVel = Vector3.zero;//velocities that are used by smoothDamp function
	[HideInInspector]
	public Vector3 tempGunPos = Vector3.zero;

	//camera FOV handling
	[Tooltip("Default camera field of view value.")]
	public float defaultFov = 75.0f;
	[Tooltip("Default camera field of view value while sprinting.")]
	public float sprintFov = 85.0f;
	[Tooltip("Amount to subtract from main camera FOV for weapon camera FOV.")]
	public float weaponCamFovDiff = 20.0f;//amount to subtract from main camera FOV for weapon camera FOV
	[HideInInspector]
	public float nextFov = 75.0f;//camera field of view that is smoothed using smoothDamp
	[HideInInspector]
	public float newFov = 75.0f;//camera field of view that is smoothed using smoothDamp
	private float FovSmoothSpeed = 0.15f;//speed that camera FOV is smoothed
	private float dampFOV = 0.0f;//target weapon z position that is smoothed using smoothDamp function
		
	//zooming
	public enum zoomType{
		hold,
		toggle,
		both
	}
	[HideInInspector]
	public bool dzAiming;//true if deadzone aiming
	[Tooltip("User may set zoom mode to toggle, hold, or both (toggle on zoom button press, hold on zoom button hold).")]
	public zoomType zoomMode = zoomType.both;
	[Tooltip("Percentage to reduce mouse sensitivity when zoomed.")]
	public float zoomSensitivity = 0.5f;
	public AudioClip sightsUpSnd;
	public AudioClip sightsDownSnd;
	[HideInInspector]
	public bool zoomSfxState = true;//var for only playing sights sound effects once
	[HideInInspector]
	public bool reloading = false;//this variable true when player is reloading
	
	[Header ("Bobbing Speeds and Amounts", order = 0)]
	[Space (10, order = 1)]
	//speeds and camera bobbing amounts for player movement states	
	[Tooltip("Camera position bobbing amount when walking (X = horizontal, Y = vertical).")]
	public Vector2 walkPositionBob = Vector2.one;
	[Tooltip("Camera angle bobbing amount when walking (yaw, pitch, roll).")]
	public Vector3 walkAngleBob = Vector3.one;
	[Tooltip("Camera and weapon bobbing speed when walking.")]
	public float walkBobSpeed = 1f;

	[Tooltip("Camera position bobbing amount when sprinting (X = horizontal, Y = vertical).")]
	public Vector2 sprintPositionBob = Vector2.one;
	[Tooltip("Camera angle bobbing amount when sprinting (yaw, pitch, roll).")]
	public Vector3 sprintAngleBob = Vector3.one;
	[Tooltip("Camera and weapon bobbing speed when sprinting.")]
	public float sprintBobSpeed = 1f;

	[Tooltip("Camera position bobbing amount when crouching (X = horizontal, Y = vertical).")]
	public Vector2 crouchPositionBob = Vector2.one;
	[Tooltip("Camera angle bobbing amount when crouching (yaw, pitch, roll).")]
	public Vector3 crouchAngleBob = Vector3.one;
	[Tooltip("Camera and weapon bobbing speed when crouching.")]
	public float crouchBobSpeed = 1f;

	[Tooltip("Camera position bobbing amount when prone (X = horizontal, Y = vertical).")]
	public Vector2 pronePositionBob = Vector2.one;
	[Tooltip("Camera angle bobbing amount when prone (yaw, pitch, roll).")]
	public Vector3 proneAngleBob = Vector3.one;
	[Tooltip("Camera and weapon bobbing speed when prone.")]
	public float proneBobSpeed = 1f;

	[Tooltip("Camera position bobbing amount when zoomed (X = horizontal, Y = vertical).")]
	public Vector2 zoomPositionBob = Vector2.one;
	[Tooltip("Camera angle bobbing amount when zoomed (yaw, pitch, roll).")]
	public Vector3 zoomAngleBob = Vector3.one;
	[Tooltip("Camera and weapon bobbing speed when zoomed.")]
	public float zoomBobSpeed = 1f;
	[Tooltip("Camera and weapon bobbing speed when zoomed and crouched.")]
	public float zoomBobSpeedCrouch = 1f;
	[Tooltip("Camera and weapon bobbing speed multiplier when swimming.")]
	public float swimBobSpeedFactor = 0.6f;
	private float swimBobSpeedAmt = 0.0f;
	private float moveInputAmt = 0.0f;
	private float moveInputSpeed = 0.0f;

	//actual bobbing vars that are passed to position and rotation calculations
	//these are switched to above amounts depending on movement state
	[HideInInspector]
	public Vector2 camPositionBobAmt = Vector2.zero;
	[HideInInspector]
	public Vector3 camAngleBobAmt = Vector3.zero;
	[HideInInspector]
	public Vector3 weapPositionBobAmt = Vector3.zero;
	[HideInInspector]
	public Vector3 weapAngleBobAmt = Vector3.zero;

	[Tooltip("Amount to roll the screen left or right when strafing and sprinting.")]
	public float sprintStrafeRoll = 2.0f;
	[Tooltip("Amount to roll the screen left or right when strafing and walking.")]
	public float walkStrafeRoll = 1.0f;
	[Tooltip("Amount to roll the screen left or right when moving view horizontally.")]
	public float lookRoll = 1f;
	[Tooltip("Amount to roll the screen left or right when moving view horizontally during bullet time.")]
	public float btLookRoll = 1f;
	[Tooltip("Amount to roll the screen left or right when moving view horizontally and underwater.")]
	public float swimLookRoll = 1f;
	[Tooltip("Speed to return to neutral roll values when above water.")]
	public float rollReturnSpeed = 4.0f;
	[Tooltip("Speed to return to neutral roll values when underwater.")]
	public float rollReturnSpeedSwim = 2.0f;
	[Tooltip("Amount the camera should bob vertically to simulate player breathing.")]
	public float idleBobAmt = 1f;
	[Tooltip("Amount the camera should bob vertically to simulate floating in water.")]
	public float swimBobAmt = 1f;//true if camera should bob slightly up and down when player is swimming
	
	private float strafeSideAmt;//amount to move weapon left or right when strafing
	private float pivotBobAmt;
		
	//gun X position amount for tweaking ironsights position
	private float horizontalGunPosAmt = -0.02f;
	private float sprintXPositionAmt = 0.0f;
	//weapon sprinting positioning
	private float gunup = 0.015f;//amount to move weapon up while sprinting
	private float gunRunUp = 1.0f;//to control vertical bobbing of weapon during sprinting
	//weapon positioning
	private float yDampSpeed= 0.0f;//this value used to control speed that weapon Y position is smoothed
	private float yDampSpeedAmt;
	private float zDampSpeed= 0.0f;//this value used to control speed that weapon Z position is smoothed
	public float bobDampSpeed = 0.1f;
	private float bobMove = 0.0f;
	private float sideMove = 0.0f;
	[HideInInspector]
	public float switchMove = 0.0f;//for moving weapon down while switching weapons
	[HideInInspector]
	public float climbMove = 0.0f;//for moving weapon down while climbing
	private float jumpmove = 0.0f;//for moving weapon down while jumping
	[HideInInspector]
	public float jumpAmt = 0.0f;
	private float idleX = 0.0f;//amount of weapon movement when idle
	private float idleY = 0.0f;
	[HideInInspector]
	public float side = 0.0f;//amount to sway weapon position horizontally
	[HideInInspector]
	public float raise = 0.0f;//amount to sway weapon position vertically

	private AudioSource aSource;
	[Tooltip("Point to rotate weapon models for vertical bobbing effect.")]
	public Transform pivot;
	private float pivotAmt;
	private float dampVel2;
	private float rotateAmtNeutral;

	void Start(){
		//Set up external script references
		SmoothMouseLook = CameraObj.GetComponent<SmoothMouseLook>();
		PlayerWeaponsComponent = weaponObj.GetComponent<PlayerWeapons>();
		FPSWalker = playerObj.GetComponent<FPSRigidBodyWalker>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		InputComponent = playerObj.GetComponent<InputControl>();
		WeaponPivotComponent = FPSPlayerComponent.WeaponPivotComponent;
		CamAndWeapAnimsComponent = CameraObj.GetComponent<CamAndWeapAnims>();
		CamAndWeapAnimator = CameraObj.GetComponent<Animator>();
		CameraControlComponent = Camera.main.transform.GetComponent<CameraControl>();

		tempGunPos = Vector3.zero;

		aSource = playerObj.AddComponent<AudioSource>(); 
		aSource.spatialBlend = 0.0f;
		aSource.playOnAwake = false;

		CamAndWeapAnimator.SetBool("Walking", false);
		CamAndWeapAnimator.SetBool("Sprinting", false);
		CamAndWeapAnimator.SetBool("Idle", true);

	}
	
	void Update (){
		
		if(Time.timeScale > 0.0f && Time.deltaTime > 0.0f && Time.smoothDeltaTime > 0.0f){//allow pausing by setting timescale to 0

			yDampSpeedAmt = Mathf.MoveTowards(yDampSpeedAmt, yDampSpeed, Time.deltaTime);
				
			if(SmoothMouseLook.playerMovedTime + 0.1 < Time.time || FPSWalker.moving){
				//main weapon position smoothing happens here
				newPos.x = Mathf.SmoothDamp(newPos.x, nextPos.x, ref dampVel.x, yDampSpeedAmt, Mathf.Infinity, Time.deltaTime);
				newPos.y = Mathf.SmoothDamp(newPos.y, nextPos.y, ref dampVel.y, yDampSpeedAmt, Mathf.Infinity, Time.deltaTime);
				newPos.z = Mathf.SmoothDamp(newPos.z, nextPos.z, ref dampVel.z, zDampSpeed, Mathf.Infinity, Time.deltaTime);
				zPosRec = Mathf.SmoothDamp(zPosRec, zPosRecNext, ref recZDamp, 0.25f, Mathf.Infinity, Time.deltaTime);//smooth recoil kick back of weapon
			}else{
				//main weapon position smoothing happens here
				newPos.x = nextPos.x;
				newPos.y = nextPos.y;
				newPos.z = nextPos.z;
				zPosRec = zPosRecNext;
			}
			
			newFov = Mathf.SmoothDamp(Camera.main.fieldOfView, nextFov, ref dampFOV, FovSmoothSpeed, Mathf.Infinity, Time.deltaTime);//smooth camera FOV
			
			//Sync camera FOVs
			if(WeapCameraObj){
				WeapCameraObj.fieldOfView = Camera.main.fieldOfView - weaponCamFovDiff;
			}
			Camera.main.fieldOfView = newFov;
			//Get input from player movement script
			float horizontal = FPSWalker.inputX;
			float vertical = FPSWalker.inputY;
			
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Adjust weapon position and bobbing amounts dynamicly based on movement and player states
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			//move weapon back towards camera based on kickBack amount in WeaponBehavior.cs					
			if(WeaponBehaviorComponent.shootStartTime + 0.1f > Time.time){
				if(FPSPlayerComponent.zoomed){
					zPosRecNext = WeaponBehaviorComponent.kickBackAmtZoom;	
				}else{
					zPosRecNext = WeaponBehaviorComponent.kickBackAmtUnzoom;	
				}
			}else{
				if(WeaponBehaviorComponent.pullBackAmt != 0.0f){
					zPosRecNext = WeaponBehaviorComponent.pullBackAmt * WeaponBehaviorComponent.fireHoldMult;
				}else{
					zPosRecNext = 0.0f;
				}
			}
				
			if(FPSWalker.moving){
				idleY = 0;
				idleX = 0;
				//check for sprinting or prone
				if(
					(
						(//player has stood up from crouch or prone and is sprinting
							FPSWalker.sprintActive
							&& !FPSWalker.crouched
							&& !FPSWalker.cancelSprint
							&& (FPSWalker.midPos >= FPSWalker.standingCamHeight && FPSWalker.proneRisen)	
						) 
						|| (!reloading && (!FPSWalker.proneRisen && !FPSWalker.crouched) 
					    && (FPSWalker.prone || FPSWalker.sprintActive))//player is prone
					)
				&& FPSWalker.fallingDistance < 0.75f	
				&& !FPSPlayerComponent.zoomed
				&& !FPSWalker.jumping){
					
					if (!FPSWalker.cancelSprint
					&& (!reloading || FPSWalker.sprintReload)
					&& FPSWalker.fallingDistance < 0.75f
					&& !FPSWalker.jumping){//actually sprinting now
						if(FPSWalker.grounded){
							PlaySprintingAnim();
						}else{
							PlayIdleAnim();
						}
						//set the camera's fov back to normal if the player has sprinted into a wall, but the sprint is still active
						if(((FPSWalker.inputY != 0 && FPSWalker.forwardSprintOnly) || (!FPSWalker.forwardSprintOnly && FPSWalker.moving))
						&& !FPSWalker.prone){
							nextFov = sprintFov;
						}else{
							nextFov = defaultFov;	
						}
						
						if(!reloading){
							//gradually move weapon more towards center while sprinting
							sprintXPositionAmt = Mathf.MoveTowards(sprintXPositionAmt, WeaponBehaviorComponent.sprintXPosition, Time.deltaTime * 16);
							horizontalGunPosAmt = WeaponBehaviorComponent.unzoomXPosition + sprintXPositionAmt;
							
							if(gunRunUp < 1.4f){gunRunUp += Time.deltaTime / 4.0f;}//gradually increase for smoother transition
							bobMove = gunup + WeaponBehaviorComponent.sprintYPosition;//raise weapon while sprinting
							sideMove = 0.0f;
						}else{//weapon positioning for sprinting and reloading
							gunRunUp = 1.0f;
							bobMove = 0.0f;
							sideMove = 0.0f;
							sprintXPositionAmt = Mathf.MoveTowards(sprintXPositionAmt, 0, Time.deltaTime * 16);
							horizontalGunPosAmt = WeaponBehaviorComponent.unzoomXPosition + sprintXPositionAmt;
						}
						
					}else{//not sprinting
						nextFov = defaultFov;
						gunRunUp = 1.0f;
						bobMove = -0.01f;
						//make this check to prevent weapon occasionally not lowering during switch while prone and moving 
						if(!FPSWalker.prone){
							switchMove = 0.0f;
						}
					}
				}else{//walking
					if(FPSWalker.grounded){
						PlayWalkingAnim();
					}else{
						PlayIdleAnim();
					}
					gunRunUp = 1.0f;
					//reset horizontal weapon positioning var and make sure it returns to zero when not sprinting to prevent unwanted side movement
					sprintXPositionAmt = Mathf.MoveTowards(sprintXPositionAmt, 0, Time.deltaTime * 16);
					horizontalGunPosAmt = WeaponBehaviorComponent.unzoomXPosition + sprintXPositionAmt;
					if(reloading){//move weapon position up when reloading and moving for full view of animation
						nextFov = defaultFov;
						bobMove = 0.0f;
						sideMove = 0.0f;
					}else{
						nextFov = defaultFov;
						if(FPSPlayerComponent.zoomed && WeaponBehaviorComponent.meleeSwingDelay == 0) {//zoomed and not melee weapon
							bobMove = 0.0F;//move weapon to idle
						}else{//not zoomed
							//move weapon down and left when crouching
							if (FPSWalker.crouched || FPSWalker.midPos < FPSWalker.standingCamHeight * 0.85f) {
								bobMove = WeaponBehaviorComponent.crouchWalkYPosition;
								sideMove = WeaponBehaviorComponent.crouchXPosition;
							}else{
								bobMove = WeaponBehaviorComponent.walkYPosition;
								sideMove = 0.00f;
							}
						}
					}
				}
			}else{//if not moving (no player movement input)
				PlayIdleAnim();
				nextFov = defaultFov;
				horizontalGunPosAmt = WeaponBehaviorComponent.unzoomXPosition;
				if(sprintXPositionAmt > 0){sprintXPositionAmt -= Time.deltaTime / 4;}
				if(reloading){
					nextFov = defaultFov;
					bobMove = 0.0F;
					sideMove = 0.0f;
				}else{
					//move weapon to idle
					if((FPSWalker.crouched || FPSWalker.midPos < FPSWalker.standingCamHeight * 0.85f) && !FPSPlayerComponent.zoomed) {
						bobMove = WeaponBehaviorComponent.crouchYPosition;
						sideMove = WeaponBehaviorComponent.crouchXPosition;
					}else{
						bobMove = 0.0f;
						sideMove = 0.0f;
					}
				}
				//weapon idle motion
				if(FPSPlayerComponent.zoomed && WeaponBehaviorComponent.meleeSwingDelay == 0) {
					idleX = Mathf.Sin(Time.time * 1.25f) * 0.0005f * WeaponBehaviorComponent.zoomIdleSwayAmt;
					idleY = Mathf.Sin(Time.time * 1.5f) * 0.0005f * WeaponBehaviorComponent.zoomIdleSwayAmt;
				}else{
					if(!FPSWalker.swimming){
						idleX = Mathf.Sin(Time.time * 1.25f) * 0.0012f * WeaponBehaviorComponent.idleSwayAmt;
						idleY = Mathf.Sin(Time.time * 1.5f) * 0.0012f * WeaponBehaviorComponent.idleSwayAmt;
					}else{
						idleX = Mathf.Sin(Time.time * 1.25f) * 0.003f * WeaponBehaviorComponent.swimIdleSwayAmt;
						idleY = Mathf.Sin(Time.time * 1.5f) * 0.003f * WeaponBehaviorComponent.swimIdleSwayAmt;	
					}
				}
			}
			
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Weapon Swaying/Bobbing while moving
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			//set smoothed weapon position to actual gun position vector
			tempGunPos.x = newPos.x + (CamAndWeapAnimsComponent.weapPosAnim.x * weapPositionBobAmt.x);
			tempGunPos.y = newPos.y + (CamAndWeapAnimsComponent.weapPosAnim.y * weapPositionBobAmt.y);
			tempGunPos.z = newPos.z + zPosRec - (CamAndWeapAnimsComponent.weapPosAnim.z * weapPositionBobAmt.z );//add weapon z position and recoil kick back


			if(!WeaponBehaviorComponent.unarmed && Time.timeSinceLevelLoad > 0.3f){
				//apply temporary vector to gun's transform position
				gun.localPosition = tempGunPos;
			}

			//compensate for floating point imprecision in RotateAround when player is a large distance from scene origin
			gun.transform.parent.transform.localPosition = Vector3.MoveTowards(gun.transform.parent.transform.localPosition, Vector3.zero, 0.005f * Time.smoothDeltaTime);

			pivotAmt = Mathf.SmoothDampAngle(pivotAmt, (WeaponPivotComponent.animOffsetTarg.x * 6.0f) + (CamAndWeapAnimsComponent.weapAngleAnim.y * weapAngleBobAmt.y * 6.5f), 
													   ref dampVel2, 0.04f, Mathf.Infinity, Time.smoothDeltaTime);

			rotateAmtNeutral = Mathf.DeltaAngle(gun.transform.parent.transform.localEulerAngles.x, pivotAmt);

			//rotate weapon vertically along pivot for bobbing effect
			gun.transform.parent.transform.RotateAround(pivot.position, gun.transform.parent.transform.right, rotateAmtNeutral);
		
		
			//lower weapon when jumping, falling, or slipping off ledge
			if(FPSWalker.jumping || FPSWalker.fallingDistance > 1.25f){
				//lower weapon less when zoomed
				if (!FPSPlayerComponent.zoomed){
					//raise weapon when jump is ascending and lower when descending
					if((FPSWalker.airTime + 0.175f) > Time.time){
						jumpmove = 0.015f;
					}else{
						jumpmove = -0.025f;
					}
				}else{
					jumpmove = -0.01f;
				}
			}else{
				jumpmove = 0.0f;
			}
		   
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Adjust vars for zoom and other states
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			//use this variable to slow down bobbing speeds or increase forward bobbing amount if player is swimming
			if(!FPSWalker.swimming){
				swimBobSpeedAmt = 1f;
			}else{
				swimBobSpeedAmt = Mathf.Max(0.01f, swimBobSpeedFactor);//actual swim bobbing factor, don't allow divide by zero
			}
				
			//scale the weapon and camera animation speeds with the amount the joystick is pressed
			if(!FPSWalker.sprintActive){
				moveInputSpeed = Mathf.Clamp01(Mathf.Max(Mathf.Abs(FPSWalker.inputX), Mathf.Abs(FPSWalker.inputY) + InputComponent.deadzone));
			}else{
				moveInputSpeed = 1f;
			}
			//gradually set the animation speed multiplier (moveInputAmt) to prevent jerky transitions between moving and stopping
			moveInputAmt = Mathf.MoveTowards(moveInputAmt, moveInputSpeed, Time.deltaTime * 3.5f);

			//if zoomed
			//check time of weapon sprinting anim to make weapon return to center, then zoom normally 
			if((FPSPlayerComponent.zoomed || (FPSPlayerComponent.canBackstab && !WeaponBehaviorComponent.shooting))
			&& FPSPlayerComponent.hitPoints > 1.0f
			&& PlayerWeaponsComponent.switchTime + WeaponBehaviorComponent.readyTimeAmt < Time.time//don't raise sights when readying weapon 
			&& !reloading 
			//&& WeaponBehaviorComponent.meleeSwingDelay == 0//not a melee weapon
			&& PlayerWeaponsComponent.currentWeapon != 0
			//move weapon to zoom values if zoomIsBlock is true, also
			&& (FPSPlayerComponent.canBackstab || (((WeaponBehaviorComponent.zoomIsBlock && ((WeaponBehaviorComponent.shootStartTime + WeaponBehaviorComponent.fireRate < Time.time && !WeaponBehaviorComponent.shootFromBlock) || WeaponBehaviorComponent.shootFromBlock)) || !WeaponBehaviorComponent.zoomIsBlock)))
			&& WeaponBehaviorComponent.reloadLastStartTime + WeaponBehaviorComponent.reloadLastTime < Time.time){

				if(!dzAiming){
					if(!reloading){
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideZoom, Time.deltaTime * 16);
					}else{
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0.0f, Time.deltaTime * 16);
					}
					if(!FPSPlayerComponent.canBackstab){
						//X pos with idle movement
						nextPos.x = WeaponBehaviorComponent.zoomXPosition + (side / 1.5f) + idleX + (FPSWalker.inputX * 0.1f * strafeSideAmt);
						//Y pos with idle movement
						nextPos.y = WeaponBehaviorComponent.zoomYPosition + (raise / 1.5f) + idleY + (bobMove + switchMove + climbMove + jumpAmt + jumpmove); 
						//Z pos
						nextPos.z = WeaponBehaviorComponent.zoomZPosition;
					}else{
						//X pos with idle movement
						nextPos.x = WeaponBehaviorComponent.backstabXPosition + (side / 1.5f) + idleX + (FPSWalker.inputX * 0.1f * strafeSideAmt);
						//Y pos with idle movement
						nextPos.y = WeaponBehaviorComponent.backstabYPosition + (raise / 1.5f) + idleY + (bobMove + switchMove + climbMove + jumpAmt + jumpmove); 
						//Z pos
						nextPos.z = WeaponBehaviorComponent.backstabZPosition;
					}

					if(WeaponBehaviorComponent.zoomFOVTp > 0.0f && CameraControlComponent.thirdPersonActive){
						nextFov = WeaponBehaviorComponent.zoomFOVTp;
					}else{
						nextFov = WeaponBehaviorComponent.zoomFOV;
					}

					//If not a melee weapon, play sound effect when raising sights
					if(zoomSfxState && WeaponBehaviorComponent.meleeSwingDelay == 0 && !WeaponBehaviorComponent.unarmed){
						aSource.clip = sightsUpSnd;
						aSource.volume = 1.0f;
						aSource.pitch = 1.0f * Time.timeScale;
						aSource.Play();
						zoomSfxState = false;
					}
				}else{//zooming with deadzone (goldeneye/perfect dark style)
					if(!reloading){
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideZoom, Time.deltaTime * 16);
					}else{
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0.0f, Time.deltaTime * 16);
					}
					//X pos with idle movement
					nextPos.x = side + idleX + sideMove + horizontalGunPosAmt + (FPSWalker.leanAmt / 60.0f) + (FPSWalker.inputX * 0.1f * strafeSideAmt);
					//Y pos with idle movement
					nextPos.y = raise + idleY + (bobMove + climbMove + switchMove + jumpAmt + jumpmove) + WeaponBehaviorComponent.unzoomYPosition;
					//Z pos
					nextPos.z = WeaponBehaviorComponent.unzoomZPosition;
					nextFov = WeaponBehaviorComponent.zoomFOVDz;
				}

				//adjust FOV and weapon position for zoom
				FovSmoothSpeed = 0.09f;//faster FOV zoom speed when zooming in
				yDampSpeed = 0.09f;
				zDampSpeed = 0.15f;
				
				if(!FPSPlayerComponent.canBackstab){
					//slow down turning and movement speed for zoom
					FPSWalker.zoomSpeed = true;
				
					if(!WeaponBehaviorComponent.zoomIsBlock){
						//Reduce mouse sensitivity when zoomed, but maintain sensitivity set in SmoothMouseLook script
						SmoothMouseLook.sensitivityAmt = SmoothMouseLook.sensitivity * WeaponBehaviorComponent.zoomSensitivity;
					}
					
					//gradually increase or decrease bobbing amounts for smooth transitions between movement states
					//zoomed bobbing amounts
					camPositionBobAmt = Vector2.MoveTowards(camPositionBobAmt, zoomPositionBob, Time.smoothDeltaTime * 24f);
					camAngleBobAmt = Vector3.MoveTowards(camAngleBobAmt, zoomAngleBob, Time.smoothDeltaTime * 24f);

					weapPositionBobAmt = Vector3.MoveTowards(weapPositionBobAmt, WeaponBehaviorComponent.zoomBobPosition, Time.smoothDeltaTime * 24f);
					weapAngleBobAmt = Vector3.MoveTowards(weapAngleBobAmt, WeaponBehaviorComponent.zoomBobAngles, Time.smoothDeltaTime * 24f);

					//camera and weapon bobbing speeds when zoomed
					if(FPSWalker.crouched){
						CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed, zoomBobSpeedCrouch * swimBobSpeedAmt * moveInputAmt, Time.smoothDeltaTime * 16f);
					}else{
						CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed, zoomBobSpeed * swimBobSpeedAmt * moveInputAmt, Time.smoothDeltaTime * 16f);
					}

				}
				
			}else{//not zoomed
				
				FovSmoothSpeed = 0.18f;//slower FOV zoom speed when zooming out
				
				//adjust weapon Y position smoothing speed for unzoom and switching weapons
				if(!PlayerWeaponsComponent.switching){
					yDampSpeed = 0.18f;//weapon swaying speed
				}else{
					yDampSpeed = 0.2f;//weapon switch raising speed
				}
				zDampSpeed = 0.1f;
				//X pos with idle movement
				nextPos.x = side + idleX + sideMove + horizontalGunPosAmt + (FPSWalker.leanAmt / 60.0f) + (FPSWalker.inputX * 0.1f * strafeSideAmt);
				//Y pos with idle movement
				nextPos.y = raise + idleY + (bobMove + climbMove + switchMove + jumpAmt + jumpmove) + WeaponBehaviorComponent.unzoomYPosition;
				//Z pos
				if(!FPSWalker.prone){
					nextPos.z = WeaponBehaviorComponent.unzoomZPosition;
				}else{
					nextPos.z = WeaponBehaviorComponent.unzoomZPosition;
				}
				//Set turning and movement speed for unzoom
				FPSWalker.zoomSpeed = false;	
				//If not a melee weapon, play sound effect when lowering sights	
				if(!zoomSfxState && WeaponBehaviorComponent.meleeSwingDelay == 0 && !WeaponBehaviorComponent.unarmed){
					aSource.clip = sightsDownSnd;
					aSource.volume = 1.0f;
					aSource.pitch = 1.0f * Time.timeScale;
					aSource.Play();
					zoomSfxState = true;
				}
				//Return mouse sensitivity to normal
				SmoothMouseLook.sensitivityAmt = SmoothMouseLook.sensitivity;
				
				//Set weapon and view bobbing amounts
				if (FPSWalker.sprintActive
				&& !(FPSWalker.forwardSprintOnly && (Mathf.Abs(horizontal) != 0.0f) && (Mathf.Abs(vertical) < 0.75f))
				&& (Mathf.Abs(vertical) != 0.0f || (!FPSWalker.forwardSprintOnly && FPSWalker.moving))
				&& !FPSWalker.cancelSprint
				&& !FPSWalker.crouched
				&& !FPSWalker.prone
				&& FPSWalker.midPos >= FPSWalker.standingCamHeight
				&& !FPSPlayerComponent.zoomed
				&& !InputComponent.fireHold){

					//distance to move weapon left and right for visual strafing feedback
					if(!reloading){
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideSprint, Time.deltaTime * 16);
					}else{
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0.0f, Time.deltaTime * 16);
					}
					
					//scale up bob speeds slowly to prevent jerky transition
					camPositionBobAmt = Vector2.MoveTowards(camPositionBobAmt, sprintPositionBob, Time.smoothDeltaTime * 16f);
					camAngleBobAmt = Vector3.MoveTowards(camAngleBobAmt, sprintAngleBob, Time.smoothDeltaTime * 16f);

					weapPositionBobAmt = Vector3.MoveTowards(weapPositionBobAmt, WeaponBehaviorComponent.sprintBobPosition, Time.smoothDeltaTime * 24f);
					weapAngleBobAmt = Vector3.MoveTowards(weapAngleBobAmt, WeaponBehaviorComponent.sprintBobAngles, Time.smoothDeltaTime * 24f);

					CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed * moveInputAmt, sprintBobSpeed, Time.smoothDeltaTime * 16f);

					if(!reloading){
						//move weapon toward or away from camera while sprinting
						nextPos.z = WeaponBehaviorComponent.sprintZPosition;
					}

				}else{

					if(!reloading){
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideUnzoom, Time.deltaTime * 16);
					}else{
						strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0.0f, Time.deltaTime * 16);
					}
					
					//scale up bob speeds slowly to prevent jerky transition
					if (!FPSWalker.crouched && !FPSWalker.prone && FPSWalker.midPos >= FPSWalker.standingCamHeight){
						
						////walking bob amounts///
						camPositionBobAmt = Vector2.MoveTowards(camPositionBobAmt, walkPositionBob, Time.smoothDeltaTime * 16f);
						camAngleBobAmt.x = Mathf.MoveTowards(camAngleBobAmt.x, walkAngleBob.x, Time.smoothDeltaTime * 16f);
						camAngleBobAmt.y =  Mathf.MoveTowards(camAngleBobAmt.y, walkAngleBob.y, Time.smoothDeltaTime * 16f);

						weapPositionBobAmt = Vector3.MoveTowards(weapPositionBobAmt, WeaponBehaviorComponent.walkBobPosition, Time.smoothDeltaTime * 24f);
						weapAngleBobAmt.x = Mathf.MoveTowards(weapAngleBobAmt.x, WeaponBehaviorComponent.walkBobAngles.x, Time.smoothDeltaTime * 24f);
						weapAngleBobAmt.y = Mathf.MoveTowards(weapAngleBobAmt.y, WeaponBehaviorComponent.walkBobAngles.y, Time.smoothDeltaTime * 24f);
			
						//walk forward bobbing amount, greater if swimming
						camAngleBobAmt.z = Mathf.MoveTowards(camAngleBobAmt.z, (walkAngleBob.z / swimBobSpeedAmt), Time.smoothDeltaTime * 16f);
						weapAngleBobAmt.z = Mathf.MoveTowards(weapAngleBobAmt.z, WeaponBehaviorComponent.walkBobAngles.z / swimBobSpeedAmt, Time.smoothDeltaTime * 24f);

						//walk bobbing speed, slower if swimming
						CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed, walkBobSpeed * swimBobSpeedAmt * moveInputAmt, Time.smoothDeltaTime * 16f);

					}else{
						
						if(FPSWalker.crouched){
							////crouching bob amounts////
							camPositionBobAmt = Vector2.MoveTowards(camPositionBobAmt, crouchPositionBob, Time.smoothDeltaTime * 16f);
							camAngleBobAmt = Vector3.MoveTowards(camAngleBobAmt, crouchAngleBob, Time.smoothDeltaTime * 16f);

							weapPositionBobAmt = Vector3.MoveTowards(weapPositionBobAmt, WeaponBehaviorComponent.crouchBobPosition, Time.smoothDeltaTime * 24f);
							weapAngleBobAmt = Vector3.MoveTowards(weapAngleBobAmt, WeaponBehaviorComponent.crouchBobAngles, Time.smoothDeltaTime * 24f);

							CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed * moveInputAmt, crouchBobSpeed, Time.smoothDeltaTime * 16f);

						}else if(FPSWalker.prone){
							////prone bob amounts////
							camPositionBobAmt = Vector2.MoveTowards(camPositionBobAmt, pronePositionBob, Time.smoothDeltaTime * 16f);
							camAngleBobAmt = Vector3.MoveTowards(camAngleBobAmt, proneAngleBob, Time.smoothDeltaTime * 16f);

							weapPositionBobAmt = Vector3.MoveTowards(weapPositionBobAmt, WeaponBehaviorComponent.proneBobPosition, Time.smoothDeltaTime * 24f);
							weapAngleBobAmt = Vector3.MoveTowards(weapAngleBobAmt, WeaponBehaviorComponent.proneBobAngles, Time.smoothDeltaTime * 24f);

							CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed * moveInputAmt, proneBobSpeed, Time.smoothDeltaTime * 16f);
						}

					}
				}
			}
		}
	}

	private void PlayWalkingAnim () {
		CamAndWeapAnimator.SetBool("Walking", true);
		CamAndWeapAnimator.SetBool("Sprinting", false);
		CamAndWeapAnimator.SetBool("Idle", false);
	}

	private void PlaySprintingAnim () {
		CamAndWeapAnimator.SetBool("Walking", false);
		CamAndWeapAnimator.SetBool("Sprinting", true);
		CamAndWeapAnimator.SetBool("Idle", false);
	}

	private void PlayIdleAnim () {
		CamAndWeapAnimator.SetBool("Walking", false);
		CamAndWeapAnimator.SetBool("Sprinting", false);
		CamAndWeapAnimator.SetBool("Idle", true);
	}
	
}