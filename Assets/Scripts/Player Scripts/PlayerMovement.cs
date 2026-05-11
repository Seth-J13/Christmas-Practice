using System.Collections;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    //Variables
        //Unity Basics Variables
    Rigidbody body;
    Camera cam;
        //Scripts & Components
    PlayerCameraMovement playerCamComponent;
    PlayerGunManager playerGunComponent;
        //Moving
    private Vector3 p_Pos = Vector3.zero;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float increaseLerp = 0.1f;
    private float lerpAmount = 0;
    private bool isRunning = false;
        //Looking
    public float lookSpeedX = 5f;
    public float lookSpeedY = 5f;
    private float lookAngleX = 0f;
    private float lookAngleY = 0f;
    [SerializeField] private int angleY = 100;
        //Jumping
    public float jumpForce = 500f;
    public float gravityScale = 3f;
    private bool canJump = true;
    private bool wantsToJump = false;
    private bool canPushDown = false;
        //Crouching
    private bool crouching = false;

    //Delegates
    public delegate void Look(float spdX, float spdY, float angX, float angY, Vector3 plyrPos);
    public static event Look look;

    //Unity Basics Functions
    private void Awake()
    {
        //Remove Cursor while playing
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Setting variables
        body = GetComponent<Rigidbody>();
        cam = Instantiate(Resources.Load<Camera>("Prefabs/Critical Assets/PlayerCam"));
        cam.name = "Player Cam";
        playerCamComponent = cam.GetComponent<PlayerCameraMovement>();
        playerGunComponent = GetComponent<PlayerGunManager>();
    }
    private void FixedUpdate()
    {
        //XZ movement
        if (isRunning && body.linearVelocity.x < 30 && body.linearVelocity.y < 30)
        {
            if(crouching)
                UnCrouch();
            lerpAmount += increaseLerp; 
            float lerp = Mathf.Lerp(moveSpeed, runSpeed, lerpAmount);
            body.AddForce(Quaternion.FromToRotation(Vector3.forward, new Vector3(transform.forward.x, 0, transform.forward.z)) * p_Pos * lerp * Time.deltaTime, ForceMode.Force);
        }
        else if (body.linearVelocity.x < 30 && body.linearVelocity.y < 30)
        {
            body.AddForce(Quaternion.FromToRotation(Vector3.forward, new Vector3(transform.forward.x, 0, transform.forward.z)) * p_Pos * moveSpeed * Time.deltaTime, ForceMode.Force);
        }
       
        //Push Player down (Heavier Gravity)
        if(canPushDown && (!canJump || crouching))
            body.AddForce(Vector3.down * gravityScale * Time.deltaTime, ForceMode.Force);
        
        //Y rotation <>
        body.rotation = Quaternion.Euler(0, lookAngleX * lookSpeedX * Time.deltaTime, 0);
        //Update Camera Position to Players Position
        look?.Invoke(lookSpeedX, lookSpeedY, lookAngleX, lookAngleY, transform.position);
        //Update gun look rotation
        playerGunComponent.UpdateVerticalLookPosition(lookSpeedX, lookSpeedY, lookAngleX, lookAngleY);
        //Player jump
        if (wantsToJump && canJump)
        {
            body.AddForce(Vector3.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
            StartCoroutine(WaitToPushPlayerDown());
            canJump = false;
        }
        canPushDown = body.linearVelocity.y > -0.01f ? false : true;
    }
    //Methods
    public void Grouded() => canJump = true; 
    public void SetCam(Camera cam) => this.cam = cam; 
    private void Crouch()
    {
        crouching = true;
        moveSpeed = moveSpeed / 1.5f;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        capsule.height = 1.25f;
        capsule.center = new Vector3(0, 0.375f, 0);
    }
    private void UnCrouch()
    {
        crouching = false;
        moveSpeed = moveSpeed * 1.5f;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        capsule.height = 2;
        capsule.center = Vector3.zero;
    }
    //Movement
    void OnMove(InputValue v)
    {
        p_Pos = new Vector3(v.Get<Vector2>().x, 0, v.Get<Vector2>().y);
    }
    void OnSprint(InputValue v)
    {
        isRunning = v.Get<float>() == 1 ? true : false;
        if(!isRunning)
        {
            lerpAmount = 0f;
        }
    }
    void OnLook(InputValue v)
    {
        lookAngleX += v.Get<Vector2>().x;
        //If the look angle is not going past the limit (angleY)
        if(lookAngleY < angleY && lookAngleY > -angleY)
            lookAngleY += v.Get<Vector2>().y;
        else
            lookAngleY = lookAngleY > 0 ? angleY - 1 : -(angleY - 1);
    }
    void OnJump(InputValue v)
    {
        wantsToJump = v.Get<float>() == 1 ? true : false;
    }
    void OnCrouch(InputValue v)
    {
        if(crouching)
            UnCrouch();
        else
            Crouch();
    }
    //Enumerators
    IEnumerator WaitToPushPlayerDown()
    {
        yield return new WaitUntil(() => body.linearVelocity.y < 0);
        canPushDown = true;        
    }
    //Extra
    void OnRestart(InputValue v)
    {
        if (v.Get<float>() == 1)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
