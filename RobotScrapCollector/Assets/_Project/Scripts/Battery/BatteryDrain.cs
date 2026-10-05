using UnityEngine;

public class BatteryDrain : MonoBehaviour
{
    [SerializeField] private float maxBattery = 100f;    // bataryanın tam dolu hâli
    [SerializeField] private float drainPerSecond = 5f;  // saniyede ne kadar azalacak

    private float currentBattery;      // şu anki batarya
    private bool emptyMessageShown;    // "bitti" mesajı yazıldı mı?

    void Start()
    {
        currentBattery = maxBattery;
        Debug.Log("Battery start: " + currentBattery);
    }

    void Update()
    {
        currentBattery -= drainPerSecond * Time.deltaTime;

        if (currentBattery < 0f)
        {
            currentBattery = 0f;
        }

        float percent = currentBattery / maxBattery;
        transform.localScale = new Vector3(percent, 1f, 1f);

        if (currentBattery == 0f && !emptyMessageShown)
        {
            Debug.Log("Battery empty!");
            emptyMessageShown = true;
        }
    }
}