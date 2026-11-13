
using UnityEngine;

// SAHİBİ: Nagihan — tek bir yaya: yürür, durur, çöp fırlatır, gider (ya da kaçar)
public class Pedestrian : MonoBehaviour
{
    private enum State { Walking, WindUp, Throwing, Recover, Leaving, Fleeing }

    private const float WindUpTime = 0.6f;
    private const float ThrowTime = 0.2f;
    private const float RecoverTime = 0.3f;
    private const float ArmBackAngle = 150f;
    private const float ArmForwardAngle = -60f;
    private const float PanicArmAngle = 165f;
    private const float FleeSpeedMultiplier = 3f;

    private PedestrianSpawner spawner;
    private Vector3 start;
    private Vector3 end;
    private float speed;
    private float throwAt;
    private bool willThrow;
    private bool hasThrown;

    private Transform armPivot;
    private Transform handPoint;

    private State state = State.Walking;
    private float stateTimer;
    private Vector3 target;
    private bool released;

    public void Init(PedestrianSpawner owner, Vector3 from, Vector3 to,
                     float walkSpeed, float throwPoint, bool throwsTrash)
    {
        spawner = owner;
        start = from;
        end = to;
        speed = walkSpeed;
        throwAt = throwPoint;
        willThrow = throwsTrash;

        transform.position = start;
        FaceTowards(end);

        armPivot = transform.Find("ArmPivot");
        if (armPivot != null) handPoint = armPivot.Find("HandPoint");

        if (armPivot == null || handPoint == null)
        {
            Debug.LogError(name + ": ArmPivot ya da HandPoint bulunamadı, bu yaya çöp atamaz");
            willThrow = false;
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.Walking: UpdateWalking(); break;
            case State.WindUp: UpdateWindUp(); break;
            case State.Throwing: UpdateThrowing(); break;
            case State.Recover: UpdateRecover(); break;
            case State.Leaving: Walk(); break;
            case State.Fleeing: Walk(); break;
        }
    }

    // ===== SÖZLEŞME =====

    // Sümeyra'nın sopası çağırır: fırlattığım çöp havada vuruldu
    public void Flee()
    {
        if (state == State.Fleeing) return;

        state = State.Fleeing;
        willThrow = false;
        hasThrown = true;
        released = true;

        speed *= FleeSpeedMultiplier;
        SetArm(PanicArmAngle);
        FaceTowards(end);

        Debug.Log(name + " kaçıyor!");
    }

    // ----- DURUMLAR -----

    private void UpdateWalking()
    {
        Walk();

        if (!willThrow || hasThrown) return;
        if (Progress() < throwAt) return;

        hasThrown = true;
        if (!spawner.CanThrow()) return;
        if (!spawner.TryPickTarget(out target)) return;

        FaceTowards(target);
        EnterState(State.WindUp);
    }

    private void UpdateWindUp()
    {
        float t = Tick(WindUpTime);
        SetArm(Mathf.Lerp(0f, ArmBackAngle, t));
        if (t >= 1f) EnterState(State.Throwing);
    }

    private void UpdateThrowing()
    {
        float t = Tick(ThrowTime);
        SetArm(Mathf.Lerp(ArmBackAngle, ArmForwardAngle, t));

        if (!released && t >= 0.5f)
        {
            ReleaseTrash();
            released = true;
        }

        if (t >= 1f) EnterState(State.Recover);
    }

    private void UpdateRecover()
    {
        float t = Tick(RecoverTime);
        SetArm(Mathf.Lerp(ArmForwardAngle, 0f, t));

        if (t >= 1f)
        {
            FaceTowards(end);
            EnterState(State.Leaving);
        }
    }

    // ----- YARDIMCILAR -----

    private void EnterState(State next)
    {
        state = next;
        stateTimer = 0f;
    }

    private float Tick(float duration)
    {
        stateTimer += Time.deltaTime;
        return Mathf.Clamp01(stateTimer / duration);
    }

    private void Walk()
    {
        transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, end) < 0.05f)
        {
            Destroy(gameObject);
        }
    }

    private float Progress()
    {
        float total = Vector3.Distance(start, end);
        if (total <= 0f) return 1f;
        return Vector3.Distance(start, transform.position) / total;
    }

    private void FaceTowards(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void SetArm(float angle)
    {
        if (armPivot != null) armPivot.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    // ----- FIRLATMA -----

    private void ReleaseTrash()
    {
        TrashItem prefab = spawner.PickTrashPrefab();
        if (prefab == null) return;

        Vector3 from = handPoint.position;

        // Uçuş süresini önce hesapla: hem hız hem işaret aynı süreyi kullanır
        float flightTime = FlightTimeFor(from, target);
        Vector3 velocity = BallisticVelocity(from, target, flightTime);

        TrashItem trash = Instantiate(prefab, from, Quaternion.identity);
        trash.SetThrower(this);
        trash.Launch(velocity);

        // Düşeceği yere küçülen halka
        LandingMarker marker = spawner.CreateMarker(target, flightTime);
        if (marker != null) trash.SetMarker(marker.gameObject);
    }

    private float FlightTimeFor(Vector3 from, Vector3 to)
    {
        Vector3 difference = to - from;
        Vector3 flat = new Vector3(difference.x, 0f, difference.z);
        return Mathf.Clamp(flat.magnitude / 12f, 0.8f, 1.6f);
    }

    private Vector3 BallisticVelocity(Vector3 from, Vector3 to, float flightTime)
    {
        Vector3 difference = to - from;
        return difference / flightTime - 0.5f * Physics.gravity * flightTime;
    }
}