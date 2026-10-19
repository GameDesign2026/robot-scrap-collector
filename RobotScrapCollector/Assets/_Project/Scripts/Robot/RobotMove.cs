using UnityEngine;
using UnityEngine.InputSystem;

// SAHİBİ: Hamza
public class RobotMove : MonoBehaviour
{
    // Oyun hissinin iki ayar düğmesi (üçüncüsü olan hız, RobotSpeed'de)
    [SerializeField] private float acceleration = 20f;   // hız saniyede en fazla ne kadar değişebilir
    [SerializeField] private float turnSpeed = 360f;     // saniyede en fazla kaç derece dönebilir

    private Rigidbody rb;
    private RobotSpeed robotSpeed;     // hızı buradan alıyoruz (Nazife)
    private RobotBattery battery;      // batarya bitti mi? (Ali)
    private GroundCheck groundCheck;   // yerde miyiz? (Esra)
    private InputAction moveAction;    // "Move" action'ı: WASD, oklar, sol çubuk
    private Vector3 direction;         // oyuncunun gitmek istediği yön

    void Start()
    {
        // Gereken her şeyi bir kere bul ve sakla
        rb = GetComponent<Rigidbody>();
        robotSpeed = GetComponent<RobotSpeed>();
        battery = GetComponent<RobotBattery>();
        groundCheck = GetComponent<GroundCheck>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        // Her karede: tuşları ya da çubuğu oku
        Vector2 input = moveAction.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);
    }

    void FixedUpdate()
    {
        // Havadaysak direksiyon yok: hareketi fiziğe bırak
        if (!groundCheck.IsGrounded()) return;

        // İstediğimiz hız: yön × RobotSpeed'in verdiği hız (çamur, yağ, ağırlık dahil)
        Vector3 wanted = direction * robotSpeed.GetSpeed();

        // Batarya bittiyse robot durmak ister
        if (battery.IsEmpty()) wanted = Vector3.zero;

        // Şu anki hızımız (sadece yatay kısmı; düşme fiziğin işi)
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        // Hızı, istenen hıza biraz yaklaştır (en fazla 'step' kadar)
        float step = acceleration * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);

        // Dönme: tuşa basılmıyorsa ya da batarya bittiyse dönme
        if (direction == Vector3.zero) return;
        if (battery.IsEmpty()) return;

        Quaternion look = Quaternion.LookRotation(direction);
        float turn = turnSpeed * Time.fixedDeltaTime;
        look = Quaternion.RotateTowards(rb.rotation, look, turn);
        rb.MoveRotation(look);
    }
}