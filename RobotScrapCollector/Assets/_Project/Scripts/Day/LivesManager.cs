using System.Collections;
using UnityEngine;

// SAHİBİ: Ali — 3 can, batarya bitince çekici, canlar bitince game over
public class LivesManager : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float towHours = 1f;        // çekici saati kaç saat ileri alır
    [SerializeField] private float fadeTime = 0.6f;      // ekran kararma/açılma süresi
    [SerializeField] private float holdTime = 0.8f;      // karanlıkta bekleme (yazı okunsun)
    [SerializeField] private Vector3 towOffset = new Vector3(0f, 0f, -3f);   // YENİ: marketin önü (kapı güneyde)

    private int lives;
    private bool towing;
    private bool gameOver;

    private RobotBattery battery;
    private RobotStorage storage;
    private RobotMove move;
    private DayClock clock;
    private HUDController hud;
    private GameOverUI gameOverUI;
    private Transform market;

    void Awake()
    {
        lives = maxLives;   // HUD Start'ta sorduğunda hazır olsun
    }

    void Start()
    {
        GameObject robot = GameObject.FindWithTag("Player");
        if (robot != null)
        {
            battery = robot.GetComponent<RobotBattery>();
            storage = robot.GetComponent<RobotStorage>();
            move = robot.GetComponent<RobotMove>();
        }

        clock = GetComponent<DayClock>();   // aynı objede
        hud = FindFirstObjectByType<HUDController>();
        gameOverUI = FindFirstObjectByType<GameOverUI>(FindObjectsInactive.Include);

        MarketZone zone = FindFirstObjectByType<MarketZone>();
        if (zone != null) market = zone.transform;
    }

    void Update()
    {
        if (towing || gameOver) return;
        if (battery == null || !battery.IsEmpty()) return;
        if (clock != null && clock.IsDayOver()) return;

        // Batarya bitti!
        StartCoroutine(TowRoutine());
    }

    private IEnumerator TowRoutine()
    {
        towing = true;
        lives--;
        Debug.Log("Batarya bitti! Kalan can: " + lives);

        if (move != null) move.SetControlEnabled(false);

        // 1) Ekranı karart
        yield return Fade(0f, 1f);

        // 2) Can kalmadıysa: GAME OVER
        if (lives <= 0)
        {
            ShowGameOver();
            yield break;   // coroutine burada biter
        }

        // 3) Robotu marketin önüne çek, bataryayı doldur
        if (market != null && move != null)
        {
            Vector3 spot = market.position + towOffset;            // YENİ: marketin ortası değil, kapının önü
            spot.y = 0f;                                           // zemine
            move.TeleportTo(spot, Quaternion.Euler(0f, 180f, 0f)); // yüzü bahçeye (güneye) dönük
        }
        if (battery != null) battery.RechargeFull();

        // 4) Kurtarma süresi: saat ileri (20:00'ı bulursa gün biter)
        if (clock != null) clock.AddHours(towHours);

        // 5) Karanlıkta biraz bekle, yazı okunsun
        yield return new WaitForSeconds(holdTime);

        // 6) Ekranı aç
        yield return Fade(1f, 0f);

        // 7) Gün bitmediyse kontrol geri gelsin (bittiyse özet ekranı açık, kontrol kapalı kalsın)
        if (move != null && (clock == null || !clock.IsDayOver()))
        {
            move.SetControlEnabled(true);
        }

        towing = false;
    }

    // Perdeyi 'from'dan 'to'ya fadeTime sürede götür
    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            if (hud != null) hud.SetTowOverlay(Mathf.Lerp(from, to, t / fadeTime));
            yield return null;   // sonraki kare
        }
        if (hud != null) hud.SetTowOverlay(to);
    }

    private void ShowGameOver()
    {
        gameOver = true;
        if (hud != null) hud.SetTowOverlay(0f);

        int day = clock != null ? clock.GetDay() : 1;
        int scrap = storage != null ? storage.GetScrap() : 0;

        if (gameOverUI != null) gameOverUI.Show(day, scrap);

        Time.timeScale = 0f;   // oyunu dondur (Esra'nın "Tekrar Oyna" butonu 1'e geri alır)
        Debug.Log("GAME OVER · Gün " + day + " · Scrap " + scrap);
    }

    // HUD sorar
    public int GetLives() { return lives; }
    public int GetMaxLives() { return maxLives; }
}