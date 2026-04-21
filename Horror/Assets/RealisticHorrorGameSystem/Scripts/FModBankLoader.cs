using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class FModBankLoader : MonoBehaviour
{

    private FMOD.Studio.VCA VcaController;
    public string VcaName;

    private Slider slider;


    void Start()
    {
        VcaController = FMODUnity.RuntimeManager.GetVCA("vca:/" + VcaName);
        slider = GetComponent<Slider>();
    }

    public void SetVolume(float volume)
    {
        VcaController.setVolume(volume);
    }
}
