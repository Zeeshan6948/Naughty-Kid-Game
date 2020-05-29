//SmoothMouseLook.cs by Azuline Studios© All Rights Reserved
//Smoothes mouse input, manages angle limits, enables/unlocks cursor on pause, and compensates for non-recovering weapon recoil. 
using UnityEngine;
using System.Collections;

public class SmoothMouseLook : MonoBehaviour {
	private InputControl InputComponent;
	private GameObject playerObj;
	private FPSPlayer FPSPlayerComponent;

	[Tooltip("Mouse look sensitivity/camera move speed.")]
    public float sensitivity = 4.0f;
	[HideInInspector]
	public float sensitivityAmt = 4.0f;//actual sensitivity modified by IronSights Script

    private float minimumX = -360.0f;
    private float maximumX = 360.0f;

	[Tooltip("Minumum pitch of camera for mouselook.")]
	public float minimumY = -85f;
	[Tooltip("Maximum pitch of camera for mouselook.")]
	public float maximumY = 85f;
	[HideInInspector]
    public float rotationX = 0.0f;
	[HideInInspector]
    public float rotationY = 0.0f;
	[HideInInspector]
	public Quaternion xQuaternion;
	[HideInInspector]
	public Quaternion yQuaternion;
	[HideInInspector]
    public float inputY = 0.0f;
	[HideInInspector]
	public float horizontalDelta;
   
	[Tooltip("Smooth speed of camera angles for mouse look.")]
	public float smoothSpeed = 0.35f;
	[HideInInspector]
	public float playerMovedTime;
	
	[HideInInspector]
	public Quaternion originalRotation;
	[HideInInspector]
	public Transform myTransform;
	[HideInInspector]
	public float recoilX;//non recovering recoil amount managed by WeaponKick function of WeaponBehavior.cs
	[HideInInspector]
	public float recoilY;//non recovering recoil amount managed by WeaponKick function of WeaponBehavior.cs

	[HideInInspector]
	public bool dzAiming;
	[Tooltip("Reverse vertical input for mouselook.")]
	public bool invertVerticalLook;
	[HideInInspector]
	public bool thirdPerson;
	[HideInInspector]
	public bool tpIdleCamRotate;
	
	void Start(){ 
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		InputComponent = playerObj.GetComponent<InputControl>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();

        if (GetComponent<Rigidbody>()){GetComponent<Rigidbody>().freezeRotation = true;}
		
		myTransform = transform;//cache transform for efficiency
		
		//sync the initial rotation of the main camera to the y rotation set in editor
		originalRotation = Quaternion.Euler(myTransform.parent.transform.eulerAngles.x, myTransform.parent.transform.eulerAngles.y, 0.0f);
		
		sensitivityAmt = sensitivity;//initialize sensitivity amount from var set by player
		
		//Hide the cursor
		ControlFreak2.CFCursor.visible = false;
    }

	#if UNITY_EDITOR || UNITY_WEBPLAYER
	void OnGUI(){//lock cursor - don't use OnGUI in standalone for performance reasons
		if(Time.timeScale > 0.0f && FPSPlayerComponent.pauseHidesCursor){
			ControlFreak2.CFCursor.lockState = CursorLockMode.Locked;
			ControlFreak2.CFCursor.visible = false;
		}
	}
	#endif

    void Update(){
		
		if(Time.timeScale > 0.0f && Time.smoothDeltaTime > 0.0f){//allow pausing by setting timescale to 0

			//Hide the cursor
			ControlFreak2.CFCursor.lockState = CursorLockMode.Locked;
			ControlFreak2.CFCursor.visible = false;

			horizontalDelta = rotationX;//old rotationX

			// Read the mouse input axis
			if(!dzAiming){
				rotationX += InputComponent.lookX * sensitivityAmt * Time.timeScale;//lower sensitivity at slower time settings
				if(!invertVerticalLook){
					rotationY += InputComponent.lookY * sensitivityAmt * Time.timeScale;
				}else{
					rotationY -= InputComponent.lookY * sensitivityAmt * Time.timeScale;
				}
			}
			
			//reset vertical recoilY value if it would exceed maximumY amount 
			if(maximumY - InputComponent.lookY * sensitivityAmt * Time.timeScale < recoilY){
				rotationY += recoilY;
				recoilY = 0.0f;	
			}
			//reset horizontal recoilX value if it would exceed maximumX amount 
			if(maximumX - InputComponent.lookX * sensitivityAmt * Time.timeScale < recoilX){
				rotationX += recoilX;
				recoilX = 0.0f;	
			}
			 
			rotationX = ClampAngle (rotationX, minimumX, maximumX);
			rotationY = ClampAngle (rotationY, minimumY - recoilY, maximumY - recoilY);
			
			inputY = rotationY + recoilY;//set public inputY value for use in other scripts
			 
			xQuaternion = Quaternion.AngleAxis (rotationX + recoilX, Vector3.up);
			yQuaternion = Quaternion.AngleAxis (rotationY + recoilY, -Vector3.right);

			horizontalDelta = Mathf.DeltaAngle(horizontalDelta, rotationX);

			if(!thirdPerson){
				if(playerMovedTime + 0.1f < Time.time){
					//smooth the mouse input
					myTransform.rotation = Quaternion.Slerp(myTransform.rotation , originalRotation * xQuaternion * yQuaternion, smoothSpeed * Time.smoothDeltaTime * 60.0f / Time.timeScale);
				}else{
					myTransform.rotation = originalRotation * xQuaternion * yQuaternion;//snap camera instantly to angles with no smoothing
				}
				//lock mouselook roll to prevent gun rotating with fast mouse movements
				myTransform.rotation = Quaternion.Euler(myTransform.rotation.eulerAngles.x, myTransform.rotation.eulerAngles.y, 0.0f);
			}
			
		}else{
			//Show the cursor
			ControlFreak2.CFCursor.lockState = CursorLockMode.None;
			ControlFreak2.CFCursor.visible = true;
		}
		
    }
   
    public static float ClampAngle (float angle, float min, float max){
        angle = angle % 360;
        if((angle >= -360.0f) && (angle <= 360.0f)){
            if(angle < -360.0f){
                angle += 360.0f;
            }
            if(angle > 360.0f){
                angle -= 360.0f;
            }         
        }
        return Mathf.Clamp (angle, min, max);
    }
	
}