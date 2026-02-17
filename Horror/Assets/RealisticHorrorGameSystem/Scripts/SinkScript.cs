using UnityEngine;

namespace RealisticHorrorGameSystem
{
    public class SinkScript : MonoBehaviour
    {
        public GameObject ParticleSystem_Valve;
        public GameObject ValveKey;
        public GameObject ParticleSystem_Sink;
        public AudioSource AudioSource;
        public AudioClip Audio_Key;
        private float lastInteractingTime = 0;

        public void Interact()
        {
            if(Time.time > lastInteractingTime + 0.25f)
            {
                lastInteractingTime = Time.time;
                ParticleSystem_Valve.SetActive(!ParticleSystem_Valve.activeSelf);
                ParticleSystem_Sink.SetActive(!ParticleSystem_Sink.activeSelf);
                if (ParticleSystem_Valve.activeSelf)
                {
                    AudioSource.Play();
                    AudioSource.PlayOneShot(Audio_Key);
                    ValveKey.transform.localEulerAngles = new Vector3(0f, -180f, 90f);
                }
                else
                {
                    AudioSource.Stop();
                    AudioSource.PlayOneShot(Audio_Key);
                    ValveKey.transform.localEulerAngles = new Vector3(0f, -180f, 0f);
                }
            }
        }
    }
}
