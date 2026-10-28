using UnityEngine;
using UnityEngine.InputSystem;

// SAHİBİ: Hamza
public class RobotMove : MonoBehaviour
{
    [SerializeField] private float acceleration = 20f;      // hız ne kadar çabuk değişir
    [SerializeField] private float mouseSensitivity = 0.1f; // fare 1 piksel = kaç derece
    [SerializeField] private float minPitch = -60f;         // en fazla ne kadar yukarı bakılır
    [SerializeField] private float maxPitch = 60f;          // en fazla ne kadar aşağı bakılır
    [SerializeField] private Transform cameraPivot;         // kameranın tutulduğu nokta (yukarı/aşağı bakış)

    private Rigidbody rb;
    private RobotSpeed robotSpeed;
    private RobotBattery battery;
    private GroundCheck groundCheck;

    private InputAction moveAction;    // WASD
    private InputAction lookAction;    // fare
    private InputAction attackAction;  // sol tık (imleci tekrar kilitlemek için)

    private Vector2 moveInput;   // Update okur, FixedUpdate kullanır
    private float yaw;           // sağa-sola bakış açısı (robotun tamamı)
    private float pitch;         // yukarı-aşağı bakış açısı (sadece kamera)
    private bool controlEnabled = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        robotSpeed = GetComponent<RobotSpeed>();
        battery = GetComponent<RobotBattery>();
        groundCheck = GetComponent<GroundCheck>();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        attackAction = InputSystem.actions.FindAction("Attack");

        yaw = rb.rotation.eulerAngles.y;   // robotun başlangıçta baktığı yönden başla
        LockCursor(true);
    }

    void Update()
    {
        // Market açıkken: hiçbir girdi okuma
        if (!controlEnabled)
        {
            moveInput = Vector2.zero;
            return;
        }

        // İmleç serbest kaldıysa (örn. Esc'ye basıldı), sol tıkla tekrar kilitle
        if (Cursor.lockState != CursorLockMode.Locked && attackAction.WasPressedThisFrame())
        {
            LockCursor(true);
        }

        // Hareket tuşlarını oku
        moveInput = moveAction.ReadValue<Vector2>();

        // İmleç kilitli değilse bakma (fare pencere dışındayken robot dönmesin)
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Fare: son kareden beri kaç piksel kaydı? (deltaTime ile ÇARPILMAZ)
        Vector2 look = lookAction.ReadValue<Vector2>();
        yaw += look.x * mouseSensitivity;
        pitch -= look.y * mouseSensitivity;               // fare yukarı → yukarı bak (açı küçülür)
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);   // takla atmasın

        // Pitch'i sadece kameraya uygula
        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    void FixedUpdate()
    {
        // Yaw'ı robotun tamamına, fizik yoluyla uygula
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));

        // Havadaysak direksiyon yok
        if (!groundCheck.IsGrounded()) return;

        // Robotun ileri ve sağ yönleri (fizik dönüşüne göre)
        Vector3 forward = rb.rotation * Vector3.forward;
        Vector3 right = rb.rotation * Vector3.right;

        // Baktığımız yöne göre hareket yönü
        Vector3 direction = forward * moveInput.y + right * moveInput.x;
        direction = Vector3.ClampMagnitude(direction, 1f);   // çapraz giderken hızlanmasın

        // İstenen hız
        Vector3 wanted = direction * robotSpeed.GetSpeed();
        if (battery.IsEmpty()) wanted = Vector3.zero;

        // Hıza yavaşça yaklaş. Grip küçükse (yağ) çok yavaş yaklaşır, yani kayar.
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);
        float step = acceleration * robotSpeed.GetGrip() * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);
    }

    // Market açılınca false, kapanınca true (Esra çağırır)
    public void SetControlEnabled(bool enabled)
    {
        controlEnabled = enabled;
        LockCursor(enabled);
    }

    private void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;   // ekranın ortasına kilitle
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;     // serbest bırak
            Cursor.visible = true;
        }
    }
}