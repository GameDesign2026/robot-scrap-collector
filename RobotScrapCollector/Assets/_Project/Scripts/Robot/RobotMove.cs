using UnityEngine;
using UnityEngine.InputSystem;

// SAHİBİ: Hamza
public class RobotMove : MonoBehaviour
{
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private float sprintMultiplier = 1.6f;   // YENİ: Shift'le ne kadar hızlı
    [SerializeField] private Transform cameraPivot;

    private Rigidbody rb;
    private RobotSpeed robotSpeed;
    private RobotBattery battery;
    private GroundCheck groundCheck;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction attackAction;
    private InputAction sprintAction;   // YENİ

    private Vector2 moveInput;
    private float yaw;
    private float pitch;
    private bool controlEnabled = true;
    private bool sprinting;             // YENİ

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        robotSpeed = GetComponent<RobotSpeed>();
        battery = GetComponent<RobotBattery>();
        groundCheck = GetComponent<GroundCheck>();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        attackAction = InputSystem.actions.FindAction("Attack");
        sprintAction = InputSystem.actions.FindAction("Sprint");

        yaw = rb.rotation.eulerAngles.y;
        LockCursor(true);
    }

    void Update()
    {
        if (!controlEnabled)
        {
            moveInput = Vector2.zero;
            sprinting = false;
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked && attackAction.WasPressedThisFrame())
        {
            LockCursor(true);
        }

        moveInput = moveAction.ReadValue<Vector2>();

        // YENİ: koşuyor mu? Shift basılı + hareket ediyor + batarya var
        bool shiftHeld = sprintAction != null && sprintAction.IsPressed();
        bool moving = moveInput.sqrMagnitude > 0.01f;
        sprinting = shiftHeld && moving && !battery.IsEmpty();

        if (Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 look = lookAction.ReadValue<Vector2>();
        yaw += look.x * mouseSensitivity;
        pitch -= look.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
        if (!groundCheck.IsGrounded()) return;

        Vector3 forward = rb.rotation * Vector3.forward;
        Vector3 right = rb.rotation * Vector3.right;
        Vector3 direction = forward * moveInput.y + right * moveInput.x;
        direction = Vector3.ClampMagnitude(direction, 1f);

        float speed = robotSpeed.GetSpeed();
        if (sprinting) speed *= sprintMultiplier;   // YENİ

        Vector3 wanted = direction * speed;
        if (battery.IsEmpty()) wanted = Vector3.zero;

        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);
        float step = acceleration * robotSpeed.GetGrip() * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);
    }

    // ===== SÖZLEŞME =====

    public void SetControlEnabled(bool enabled)
    {
        controlEnabled = enabled;
        LockCursor(enabled);
    }

    // YENİ: Ali'nin BatteryUsage'ı sorar (koşarken iki kat harcama)
    public bool IsSprinting()
    {
        return sprinting;
    }

    // YENİ: Ali'nin çekicisi çağırır: robotu bir anda başka bir yere taşı
    public void TeleportTo(Vector3 position, Quaternion rotation)
    {
        rb.linearVelocity = Vector3.zero;     // eski hızla fırlamasın
        rb.angularVelocity = Vector3.zero;

        rb.position = position;               // fizik konumu
        transform.position = position;        // görünen konum (Interpolate yüzünden ikisi de)
        rb.rotation = rotation;
        transform.rotation = rotation;

        yaw = rotation.eulerAngles.y;         // bir sonraki adımda eski yöne dönmesin
        pitch = 0f;
        if (cameraPivot != null) cameraPivot.localRotation = Quaternion.identity;
    }

    private void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}