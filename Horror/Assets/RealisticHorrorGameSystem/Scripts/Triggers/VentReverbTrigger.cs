using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class VentReverbTrigger : MonoBehaviour
{
    private int playerInsideCount = 0;
    private EventInstance ventReverbSnapshot;

    private void Awake()
    {
        if (!ventReverbSnapshot.isValid())
        {
            ventReverbSnapshot = RuntimeManager.CreateInstance("snapshot:/Reverb/Reverb Vent");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInsideCount++;

        if (playerInsideCount == 1)
        {
            ventReverbSnapshot.start();
            Debug.Log("Vent Reverb Started");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInsideCount--;

        if (playerInsideCount <= 0)
        {
            playerInsideCount = 0;
            ventReverbSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Debug.Log("Vent Reverb Ended");
        }
    }
}