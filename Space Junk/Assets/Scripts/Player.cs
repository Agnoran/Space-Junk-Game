using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int HP = 100;
    [SerializeField] int speed = 5;
    [SerializeField] int sprintMod = 2;
    [SerializeField] int jumpSpeed = 5;
    [SerializeField] int jumpMax = 2;
    [SerializeField] int gravity = 10;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] CharacterController controller;

    [SerializeField] float groundCheckDistance = 0.5f;

    int flightSpeed = 0;
    [SerializeField] float thrusterForce = 10f;
    [SerializeField] float thrusterFuel = 100f;
    [SerializeField] float maxThrusterFuel = 100f;
    [SerializeField] float thrusterFuelConsumptionRate = 10f;
    [SerializeField] float thrusterFuelRegenRate = 5f;
    [SerializeField] bool thrusterFuelRegen = false;
    [SerializeField] float thrusterFuelRegenDelay = 2f;

    int jumpCount;
    int HPOrig;

    bool isJumping = false;
    bool isAttractedToGround = true;
    bool thrustersActive = false;
    bool dampenThrusters = false;


    Vector3 moveDir;
    Vector3 playerVel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        if (thrustersActive == false)
        {
            thrusterFuelRegen = true;
            if (thrusterFuel < maxThrusterFuel)
            {
                thrusterFuel += thrusterFuelRegenRate * Time.deltaTime;
            }
                
        }
        else
        {
            thrusterFuelRegen = false;
        }
    }


    void Movement()
    {
        if(isAttractedToGround)
        {
            
            GroundMovement();
            Sprint();
        }
        else
        {
            ThrusterMovement();
        }
    }
    void GroundMovement()
    {
        if (isJumping && jumpCount >= jumpMax && Input.GetButtonDown("Jump"))
        {
            isAttractedToGround = false;
            isJumping = false;
            jumpCount = 0;
            playerVel = Vector3.zero;
            controller.Move(playerVel * Time.deltaTime);
            flightSpeed = 0;
            return;
        }
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        Jump();
        controller.Move(playerVel * Time.deltaTime);

        playerVel.y -= gravity * Time.deltaTime;


    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            isJumping = true;
            jumpCount++;
        }
    }
    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }
    void ThrusterMovement()
    {
        if(closeToGround() && Input.GetButtonDown("Jump"))
        {
            isAttractedToGround = true;
            thrustersActive = false;
            return;
        }
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Depth") * transform.up;
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetAxis("Depth") != 0)
        {
            if (thrusterFuel > 0)
            {
                playerVel.y += Input.GetAxis("Depth") * thrusterForce * Time.deltaTime;
                playerVel.x += Input.GetAxis("Horizontal") * thrusterForce * Time.deltaTime;
                playerVel.z += Input.GetAxis("Vertical") * thrusterForce * Time.deltaTime;
                
                thrustersActive = true;
                thrusterFuel -= thrusterFuelConsumptionRate * Time.deltaTime;
            }
            else
            {
                thrustersActive = false;
            }
            
        }
        else
        {
            thrustersActive = false;
        }
        controller.Move(playerVel * Time.deltaTime);
    }    
    void Reorient()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 5f);
    }
    bool closeToGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}
