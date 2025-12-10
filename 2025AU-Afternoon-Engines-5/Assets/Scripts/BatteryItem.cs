using UnityEngine;

public class BatteryItem : MonoBehaviour
{
    public float rechargeAmount = 25f;
    public string playerTag = "Player";

    private Flashlight _flashlight;

    private void Start()
    {
        _flashlight = GameObject.Find("/Player/Flashlight").GetComponent<Flashlight>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        _flashlight.GiveBattery(rechargeAmount);
        
        Destroy(gameObject);
    }
}
