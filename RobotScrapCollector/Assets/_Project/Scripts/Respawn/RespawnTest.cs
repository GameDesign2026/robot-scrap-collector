using UnityEngine;

public class RespawnTest : MonoBehaviour
{
    [SerializeField] private Transform visual;          // gizlenip gösterilecek görünüş parçası
    [SerializeField] private float visibleTime = 3f;    // kaç saniye görünür kalacak
    [SerializeField] private float respawnTime = 5f;    // kaybolduktan kaç saniye sonra geri gelecek
    [SerializeField] private float growTime = 0.5f;     // belirirken kaç saniyede tam boyuta ulaşacak

    private Vector3 fullScale;      // Visual'ın normal boyutu
    private bool isVisible = true;  // çöp şu an görünüyor mu?
    private float timer;            // geçen süreyi sayan sayaç

    void Start()
    {
        // Visual'ın normal boyutunu hatırla
        fullScale = visual.localScale;
    }

    void Update()
    {
        // Her karede, geçen süreyi sayaca ekle
        timer += Time.deltaTime;

        if (isVisible)
        {
            // Büyüme oranı: 0'dan başlar, growTime saniyede 1'e ulaşır
            float growth = timer / growTime;
            if (growth > 1f)
            {
                growth = 1f;
            }
            visual.localScale = fullScale * growth;

            // Görünür kalma süresi doldu mu? → "toplandı", gizle
            if (timer >= visibleTime)
            {
                visual.gameObject.SetActive(false);
                isVisible = false;
                timer = 0f;
                Debug.Log(name + " collected");
            }
        }
        else
        {
            // Bekleme süresi doldu mu? → yeniden göster
            if (timer >= respawnTime)
            {
                visual.gameObject.SetActive(true);
                isVisible = true;
                timer = 0f;
                Debug.Log(name + " respawned");
            }
        }
    }
}