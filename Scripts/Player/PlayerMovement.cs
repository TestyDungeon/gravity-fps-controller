using UnityEngine;

/// <summary>
/// Quake-esque player movement from my other project https://github.com/TestyDungeon/quake-character-controller
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform playerPivot;

    private MovementController movementController;

    [SerializeField] private float MAX_SPEED = 30f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float accel = 11f;
    [SerializeField] private float airMaxSpeed = 2f;
    [SerializeField] private float airAccel = 11f;
    [SerializeField] private float friction = 7f;
    [SerializeField] private float stopSpeed = 0.1f;
    [SerializeField] private float jumpStrength = 10f;

    private Vector3 playerVelocity = Vector3.zero;

    private bool grounded;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        movementController = GetComponent<MovementController>();
    }

    private void Update()
    {
        AirMove();
        JumpButton();
    }

    private void FixedUpdate()
    {
        grounded = movementController.GroundCheck();
        playerVelocity = movementController.Move(playerVelocity);
    }

    private void AirMove()
    {
        Vector3 wishdir;
        Vector3 wishvel = new Vector3();
        float wishspeed;

        Vector3 forward;
        Vector3 right;

        float fmove, smove;
        
        forward = playerPivot.forward;
        right = playerPivot.right;

        fmove = Input.GetAxisRaw("Horizontal");
        smove = Input.GetAxisRaw("Vertical");

        Vector3.Normalize(forward);
        Vector3.Normalize(right);

        for (int i = 0; i < 3; i++)
            wishvel[i] = forward[i] * smove + right[i] * fmove;

        wishdir = wishvel;

        wishspeed = wishdir.magnitude * speed;

        Vector3.Normalize(wishdir);

        if (wishspeed > MAX_SPEED)
        {
            wishspeed = MAX_SPEED;
        }

        if (grounded)
        {
            Friction();
            Accelerate(wishdir, wishspeed);
        }
        else
        {
            AirAccelerate(wishdir, wishspeed);
        }
    }

    private void Accelerate(Vector3 wishDir, float wishSpeed)
    {
        float currentSpeed, addSpeed, accelSpeed;

        currentSpeed = Vector3.Dot(playerVelocity, wishDir);
        addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0)
            return;

        accelSpeed = accel * Time.deltaTime * wishSpeed;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        for (int i = 0; i < 3; i++)
            playerVelocity[i] += wishDir[i] * accelSpeed;
    }

    private void AirAccelerate(Vector3 wishDir, float wishSpeed)
    {
        float wishSpd = wishSpeed;

        if (wishSpd > airMaxSpeed)
            wishSpd = airMaxSpeed;

        float currentSpeed = Vector3.Dot(playerVelocity, wishDir);
        float addSpeed = wishSpd - currentSpeed;

        if (addSpeed <= 0)
            return;

        float accelSpeed = airAccel * Time.deltaTime * wishSpeed;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        for (int i = 0; i < 3; i++)
            playerVelocity[i] += wishDir[i] * accelSpeed;
    }

    private void Friction()
    {
        float control, drop, newspeed;

        float speed = playerVelocity.magnitude;

        if (speed < 0.01)
        {
            playerVelocity = Vector3.zero;
            return;
        }

        drop = 0;

        if (movementController.GroundCheck())
        {
            control = speed < stopSpeed ? stopSpeed : speed;
            drop += control * friction * Time.deltaTime;
        }

        newspeed = speed - drop;
        if (newspeed < 0)
            newspeed = 0;
        newspeed /= speed;

        playerVelocity[0] *= newspeed;
        playerVelocity[1] *= newspeed;
        playerVelocity[2] *= newspeed;
    }

    private void JumpButton()
    {
        if (!movementController.GroundCheck())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerVelocity -= Vector3.Project(playerVelocity, transform.up);
            playerVelocity += transform.up * jumpStrength;
        }
    }


    //private void OnGUI()
    //{
    //    GUI.color = Color.green;
    //    var ups = playerVelocity;
    //    GUI.Label(new Rect(0, 15, 400, 100),
    //    "Speed: " + Mathf.Round(ups.magnitude * 100) / 100 + "ups\n" +
    //    "Velocity: " + ups + "\n" +
    //    "Grounded: " + movementController.GroundCheck());
    //}
}