using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{ 
    [Header("Dependencies")]
    public Transform cameraTransform;
    private CharacterController cc;

    [SerializeField]
    Animator cameraAnimator;

    [Header("Movement Settings")]
    [Tooltip("Speed of horizontal movement")]
    public float moveSpeed = 5f;

    public float runSpeed = 8f;


    [Tooltip("Friction applied when no input is given")]
    public float friction = 8f;

    [Header("Jump / Gravity")]
    [Tooltip("Maximum jump height")]
    public float jumpStrength = 2f;
    [Tooltip("Base gravity value")]
    public float gravity = -9.81f;

    public float maxFallingSpeed = -15;


    public float velocityChagneMultipler = 5;

    private Vector3 velocity;

    bool isRunning => Input.GetKey(KeyCode.LeftShift);

    public float stamina = 1;

    public float staminaDegradation = 4;
    public float staminaRegeneration = 4;


    public bool isFrozen = false;

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    Vector3 keyboardInput = Vector3.zero;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        // Sync CharacterController slope limit
        //cc.slopeLimit = maxWalkSlope;
        GameObject ddol = GameObject.FindGameObjectWithTag("DontDestroyOnLoad");
        if (ddol != null)
        {
            Settings settings = ddol.GetComponent<Settings>();
            if (settings != null)
            {
                audioSource.volume = settings.soundVolume / 100f;
            }
        }
    }

    void Update()
    {
        if (isFrozen)
            return;

        CalculateStamina();
        UpdateVelocity();
        UpdateAnimator();
        HandleSounds();
    }

    void HandleSounds()
    {
        if (isRunning)
        {
            audioSource.pitch = 1.3f;
        }
        else
        {
            audioSource.pitch = 1f;
        }
        if (keyboardInput.magnitude > 0.1f && cc.isGrounded)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }


    void UpdateVelocity()
    {
        if (velocity.y > maxFallingSpeed)
            velocity.y += gravity * Time.deltaTime;

        if (cc.isGrounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                velocity.y = jumpStrength;
            }
        }



        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = Quaternion.Euler(0, 90, 0) * cameraForward;

        keyboardInput = VectorKeyboard();



        Vector3 newVelocity = (cameraForward * keyboardInput.z + cameraRight * keyboardInput.x) * currentSpeed;
        newVelocity.y = velocity.y;



        velocity = Vector3.Lerp(velocity, newVelocity, velocityChagneMultipler * Time.deltaTime);


        cc.Move(velocity * Time.deltaTime);
    }

    void CalculateStamina()
    {
        if (isRunning)
        {
            if(stamina > 0)
                stamina -= staminaDegradation * Time.deltaTime;
        }
        else
        {
            if(stamina < 1)
                stamina += staminaRegeneration * Time.deltaTime;
        }
    }


    float currentSpeed => isRunning && stamina > 0 ? runSpeed : moveSpeed;


    Vector3 VectorKeyboard()
    {
        Vector3 velocity = Vector3.zero;

        if(Input.GetKey(KeyCode.A))
        {
            velocity.x = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            velocity.x = 1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            velocity.z = 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            velocity.z = -1;
        }

        return velocity.normalized;
    }


    void UpdateAnimator()
    {
        if(keyboardInput.magnitude != 0)
        {
            cameraAnimator.SetBool("isWalking", true);
            cameraAnimator.SetBool("isRun", currentSpeed > moveSpeed || !cc.isGrounded);
            return;
        }
        
        cameraAnimator.SetBool("isWalking", false);
        cameraAnimator.SetBool("isRun", false);
    }


}

// Extension to simplify zeroing the Y component
public static class Vector3Extensions
{
    public static Vector3 WithY(this Vector3 v, float newY)
    {
        return new Vector3(v.x, newY, v.z);
    }
}
