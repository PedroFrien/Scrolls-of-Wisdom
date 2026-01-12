using System.Collections;
using Unity.Cinemachine;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;



[RequireComponent(typeof(CharacterController))]
public class FPController : BaseLooping
{
    [Header("Movement Parameters")]
    public float MaxSpeed => SprintInput ? SprintSpeed : WalkSpeed;

    

    public float Acceleration = 15f;

    [SerializeField] float WalkSpeed = 3.5f;
    [SerializeField] float SprintSpeed = 8f;

    public bool MovementEnabled = true;
    public bool LookEnabled = true;

    [Space(15)]
    [Tooltip("This is how high the character can jump.")]
    [SerializeField] float JumpHeight = 2f;

    [SerializeField] float DashVerticalMult = 3f;
    [SerializeField] float DashBase = 10f;
    bool CanDash = true;




    

    public bool Sprinting
    {
        get
        {
            return SprintInput && CurrentSpeed > 0.1f;
        }
    }

    [Header("Looking Parameters")]
    public Vector2 LookSensitivity = new Vector2(0.1f, 0.1f);

    public float PitchLimit = 85f;

    [SerializeField] float currentPitch = 0f;

    public float CurrentPitch
    {
        get => currentPitch;

        set
        {
            currentPitch = Mathf.Clamp(value, -PitchLimit, PitchLimit);
        }
    }

    [Header("Camera Parameters")]
    [SerializeField] float CameraNormalFOV = 60f;
    [SerializeField] float CameraSprintFOV = 80f;
    [SerializeField] float CameraFOVSmoothing = 1f;

    float TargetCameraFOV
    {
        get
        {
            return Sprinting ? CameraSprintFOV : CameraNormalFOV;
        }
    }

    [Header("Physics Parameters")]
    [SerializeField] float GravityScale = 3f;

    public float VerticalVelocity = 0f;

    public Vector3 CurrentVelocity { get; private set; }
    public float CurrentSpeed { get; private set; }

    public bool IsGrounded => characterController.isGrounded;


    [Header("Input")]
    public Vector2 MoveInput;
    public Vector2 LookInput;
    public bool SprintInput;

    [Header("Interacting")]
    [SerializeField] float InteractDistance = 5f;
    private IInteractable selectedInteractable;

    [Header("Components")]
    [SerializeField] CinemachineCamera fpCamera;
    [SerializeField] CharacterController characterController;
    [SerializeField] Image interactPopup;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    #region Unity Methods

    private void OnValidate()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        if (MovementEnabled)
        {
            MoveUpdate();
            

            CameraUpdate();
        }


        if (LookEnabled) LookUpdate();
        

        CheckForInteract();


        if (IsGrounded)
        {
            CanDash = true;
        }
    }

    private void Start()
    {
        SetSpawnPoint(transform.position, transform.rotation);
    }


    #endregion

    #region Controller Methods


    public void TryJump()
    {


        if (IsGrounded == false || MovementEnabled == false)
        {
            return;
        }

        VerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y * GravityScale);
    }

    public void TryDash()
    {
        Debug.Log("Trying dash");

        if (IsGrounded == false && MovementEnabled == true && CanDash)
        {
            float storedVelocity = DashBase;
            VerticalVelocity = 0f;

            Vector3 lookDir = transform.forward;

            CurrentVelocity = CurrentVelocity + (lookDir.normalized * storedVelocity);

            CanDash = false;
        }
        
    }

    void MoveUpdate()
    {
        Vector3 motion = transform.forward * MoveInput.y + transform.right * MoveInput.x;
        motion.y = 0f;
        motion.Normalize();

        

        if (motion.sqrMagnitude >= 0.01f)
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, motion * MaxSpeed, Acceleration * Time.deltaTime);
        }
        else
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, Vector3.zero, Acceleration * Time.deltaTime);
        }

        if (IsGrounded && VerticalVelocity <= 0.01f)
        {
            VerticalVelocity = -3f;
        }
        else
        {
            VerticalVelocity += Physics.gravity.y * GravityScale * Time.deltaTime;
        }


        Vector3 fullVelocity = new Vector3(CurrentVelocity.x, VerticalVelocity, CurrentVelocity.z);

        characterController.Move(fullVelocity * Time.deltaTime);

        CurrentSpeed = CurrentVelocity.magnitude;
    }

    void LookUpdate()
    {
        Vector2 input = new Vector2(LookInput.x * LookSensitivity.x, LookInput.y * LookSensitivity.y);

        // looking up and down
        CurrentPitch -= input.y;

        fpCamera.transform.localRotation = Quaternion.Euler(CurrentPitch, 0f, 0f);


        // looking left and right
        transform.Rotate(Vector3.up * input.x);
    }

    void CameraUpdate()
    {
        float targetFOV = CameraNormalFOV;

        if (Sprinting)
        {
            float speedRatio = CurrentSpeed / SprintSpeed;

            targetFOV = Mathf.Lerp(CameraNormalFOV, CameraSprintFOV, speedRatio);
        }


        fpCamera.Lens.FieldOfView = Mathf.Lerp(fpCamera.Lens.FieldOfView, targetFOV, CameraFOVSmoothing * Time.deltaTime);
    }

    public void Restart()
    {
        GameManager.Instance.Restart();
    }

    public void TryInteract()
    {
        if (selectedInteractable != null)
        {
            Debug.Log("Trying Interact");
            selectedInteractable.OnInteract();
        }
    }


    #endregion

    #region Looping Methods

    

    public override void SpecificReset()
    {
        characterController.enabled = false;
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        characterController.enabled = true;

        fpCamera.Lens.FieldOfView = CameraNormalFOV;
        CurrentPitch = 0f;
        CurrentVelocity = Vector3.zero;
        CurrentSpeed = 0f;

        
    }










    #endregion

    #region Interact

    void CheckForInteract()
    {
        RaycastHit hit;

        if (Physics.Raycast(fpCamera.transform.position, fpCamera.transform.forward, out hit, InteractDistance))
        {
            if (hit.collider.GetComponent<IInteractable>() != null)
            {
                interactPopup.gameObject.SetActive(true);
                selectedInteractable = hit.collider.GetComponent<IInteractable>();              
            }
            else
            {
                interactPopup.gameObject.SetActive(false);
                selectedInteractable = null;
            }
        }
        else
        {
            interactPopup.gameObject.SetActive(false);
            selectedInteractable = null;
        }
    }

    #endregion

}
