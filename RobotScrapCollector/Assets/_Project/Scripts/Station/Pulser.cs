using UnityEngine;

public class Pulser : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseAmount = 0.15f;

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float multiplier = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = startScale * multiplier;
    }
}