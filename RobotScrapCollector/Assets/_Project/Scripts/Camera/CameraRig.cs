using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

// SAHİBİ: Hamza — FPS ve üçüncü şahıs kamera arasında geçiş
public class CameraRig : MonoBehaviour
{
    [SerializeField] private CinemachineCamera fpsCamera;   // robotun gözünden
    [SerializeField] private CinemachineCamera tpsCamera;   // robotun arkasından
    [SerializeField] private bool startInFirstPerson = true;

    private InputAction toggleAction;   // V tuşu
    private bool firstPerson;

    void Start()
    {
        // Robotu "Player" tag'i ile bul, kameraların takip edeceği noktayı ver
        GameObject robot = GameObject.FindWithTag("Player");
        if (robot != null)
        {
            Transform pivot = robot.transform.Find("CameraPivot");
            if (pivot == null) pivot = robot.transform;   // bulunamazsa robotun kendisi

            fpsCamera.Follow = pivot;
            tpsCamera.Follow = pivot;
        }
        else
        {
            Debug.LogWarning("CameraRig: sahnede Player tag'li robot yok");
        }

        toggleAction = InputSystem.actions.FindAction("CameraToggle");
        SetMode(startInFirstPerson);
    }

    void Update()
    {
        // V'ye basıldıysa modu değiştir
        if (toggleAction != null && toggleAction.WasPressedThisFrame())
        {
            SetMode(!firstPerson);
        }
    }

    private void SetMode(bool useFirstPerson)
    {
        firstPerson = useFirstPerson;

        // Hocanın CameraZone kuralı: açık olan sanal kamera kazanır
        fpsCamera.gameObject.SetActive(useFirstPerson);
        tpsCamera.gameObject.SetActive(!useFirstPerson);
    }

    // Şu an FPS'te miyiz? (ileride başka script'ler sorabilir)
    public bool IsFirstPerson()
    {
        return firstPerson;
    }
}