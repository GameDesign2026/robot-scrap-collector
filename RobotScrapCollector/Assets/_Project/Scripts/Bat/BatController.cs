using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

// SAHİBİ: Sümeyra — sol tıkla sopayı salla, havadaki çöpü bahçenin dışına vur
public class BatController : MonoBehaviour
{
    private enum Phase { Ready, Swinging, Returning, Cooldown }

    [Header("Parçalar (boşsa kendisi bulur)")]
    [SerializeField] private Transform bat;        // Furkan'ın sopası (robotun çocuğu, adı "Bat")
    [SerializeField] private Transform hitPoint;   // Sopanın içindeki "HitPoint"

    [Header("Sallama")]
    [SerializeField] private float restYaw = 80f;      // beklerken: sağda
    [SerializeField] private float endYaw = -80f;      // sallama bitince: solda
    [SerializeField] private float tilt = -10f;        // sopa hafif yukarı baksın
    [SerializeField] private float swingTime = 0.18f;  // sallama süresi
    [SerializeField] private float returnTime = 0.25f; // geri dönüş süresi
    [SerializeField] private float cooldown = 0.15f;   // tekrar sallamadan önce bekleme

    [Header("Vuruş")]
    [SerializeField] private float hitRadius = 1.0f;   // HitPoint'in etrafında vuruş alanı (güçlü sopa büyütür)
    [SerializeField] private float hitSpeed = 16f;     // çöpü ne kadar hızlı fırlatır
    [SerializeField] private float hitLift = 7f;       // ne kadar yukarı

    [Header("Efekt")]
    [SerializeField] private GameObject hitFx;         // FX_HitSparks

    private Phase phase = Phase.Ready;
    private float timer;
    private InputAction attackAction;
    private CinemachineImpulseSource impulse;
    private List<TrashItem> hitThisSwing = new List<TrashItem>();

    void Start()
    {
        // Furkan'ın sopasını ve vuruş noktasını isimleriyle bul
        if (bat == null) bat = transform.Find("Bat");
        if (bat != null && hitPoint == null) hitPoint = bat.Find("HitPoint");

        if (bat == null) Debug.LogWarning("BatController: robotun içinde 'Bat' bulunamadı");

        attackAction = InputSystem.actions.FindAction("Attack");

        // Kamera sarsıntısı göndericisi: robotta yoksa kendimiz ekleyelim
        impulse = GetComponent<CinemachineImpulseSource>();
        if (impulse == null)
        {
            impulse = gameObject.AddComponent<CinemachineImpulseSource>();
            impulse.DefaultVelocity = new Vector3(0f, -0.3f, 0f);   // kısa, hafif bir sarsıntı
        }

        SetBatYaw(restYaw);
    }

    void Update()
    {
        switch (phase)
        {
            case Phase.Ready:     UpdateReady();     break;
            case Phase.Swinging:  UpdateSwinging();  break;
            case Phase.Returning: UpdateReturning(); break;
            case Phase.Cooldown:  UpdateCooldown();  break;
        }
    }

    // ----- EVRELER -----

    private void UpdateReady()
    {
        if (bat == null || attackAction == null) return;
        if (Cursor.lockState != CursorLockMode.Locked) return;   // menü/market açıkken sallama
        if (!attackAction.WasPressedThisFrame()) return;

        hitThisSwing.Clear();
        EnterPhase(Phase.Swinging);
    }

    private void UpdateSwinging()
    {
        float t = Tick(swingTime);
        SetBatYaw(Mathf.Lerp(restYaw, endYaw, t));

        CheckHits();   // sallarken her karede: alanda havadaki çöp var mı?

        if (t >= 1f) EnterPhase(Phase.Returning);
    }

    private void UpdateReturning()
    {
        float t = Tick(returnTime);
        SetBatYaw(Mathf.Lerp(endYaw, restYaw, t));
        if (t >= 1f) EnterPhase(Phase.Cooldown);
    }

    private void UpdateCooldown()
    {
        timer += Time.deltaTime;
        if (timer >= cooldown) EnterPhase(Phase.Ready);
    }

    // ----- VURUŞ -----

    private void CheckHits()
    {
        if (hitPoint == null) return;

        // HitPoint'in etrafındaki küreye değen her şey (trigger'lar dahil)
        Collider[] hits = Physics.OverlapSphere(hitPoint.position, hitRadius, ~0, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            TrashItem item = hit.GetComponent<TrashItem>();
            if (item == null) continue;                  // çöp değil
            if (!item.CanBeHit()) continue;              // havada değil ya da zaten vurulmuş
            if (hitThisSwing.Contains(item)) continue;   // bu sallamada zaten vurdum

            hitThisSwing.Add(item);
            Hit(item);
        }
    }

    private void Hit(TrashItem item)
    {
        Vector3 position = item.transform.position;

        // Bahçenin ortasından dışarı doğru
        Vector3 outward = position;
        outward.y = 0f;
        if (outward.sqrMagnitude < 0.01f) outward = transform.forward;   // tam ortadaysa ileri
        outward.Normalize();

        // Robotun baktığı yön
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        // %70 dışarı + %30 ileri, sonra yukarı ekle
        Vector3 direction = (outward * 0.7f + forward * 0.3f).normalized;
        Vector3 velocity = direction * hitSpeed + Vector3.up * hitLift;

        item.HitAway(velocity);

        // Hissettir: kıvılcım, sarsıntı, yazı
        if (hitFx != null) Instantiate(hitFx, position, Quaternion.identity);
        if (impulse != null) impulse.GenerateImpulse();
        FloatingText.Spawn(position + Vector3.up * 0.5f, "VURDUN!", new Color(1f, 0.6f, 0.1f));
    }

    // ----- YARDIMCILAR -----

    private void EnterPhase(Phase next)
    {
        phase = next;
        timer = 0f;
    }

    private float Tick(float duration)
    {
        timer += Time.deltaTime;
        return Mathf.Clamp01(timer / duration);
    }

    private void SetBatYaw(float yaw)
    {
        if (bat != null) bat.localRotation = Quaternion.Euler(tilt, yaw, 0f);
    }

    // ===== SÖZLEŞME =====

    // Nazife'nin güçlü sopa geliştirmesi
    public void SetHitRadius(float radius)
    {
        if (radius > 0f) hitRadius = radius;
    }

    // Furkan'ın ekipman kutusu sorar
    public bool IsSwinging()
    {
        return phase == Phase.Swinging || phase == Phase.Returning;
    }
}
