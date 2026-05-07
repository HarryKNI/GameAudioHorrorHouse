using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class BathroomReverbTrigger : MonoBehaviour
{

    private EventInstance reverbSnapshot;

    private void Awake()
    {
        reverbSnapshot = RuntimeManager.CreateInstance("snapshot:/Reverb/Reverb");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetReverb(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetReverb(false);
        }
    }

    private void SetReverb(bool isOn)
    {
        if (isOn)
        {
            reverbSnapshot.start();
        }
        else
        {
            reverbSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
