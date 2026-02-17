using System.Collections;
using UnityEngine;

namespace RealisticHorrorGameSystem
{
    public class ElectricLightScript : MonoBehaviour
    {
        public bool canFlick = true;
        public GameObject Light;
        public AudioSource AudioSource;
        private float lastInteractingTime = 0;
        public MeshRenderer meshRenderer;
        public Material on;
        public Material off;
        private Coroutine flickerCoroutine;
        public AudioClip flickingSound;
        public bool lightIsOnWhenStart = false;

        private void Start()
        {
            if (lightIsOnWhenStart)
            {
                Light.SetActive(true);
                meshRenderer.material = on;
                if (canFlick)
                {
                    if (flickerCoroutine == null)
                        flickerCoroutine = StartCoroutine(FlickerRoutine());
                }
            }
        }


        public void Interact()
        {
            if (Time.time > lastInteractingTime + 0.25f)
            {
                lastInteractingTime = Time.time;
                Light.SetActive(!Light.activeSelf);
                if (Light.activeSelf)
                {
                    AudioSource.Play();
                    meshRenderer.material = on;
                    if (flickerCoroutine == null)
                        flickerCoroutine = StartCoroutine(FlickerRoutine());
                }
                else
                {
                    meshRenderer.material = off;
                    if (flickerCoroutine != null)
                    {
                        StopCoroutine(flickerCoroutine);
                        flickerCoroutine = null;
                        Light.SetActive(false);
                    }
                }
            }
        }

        private IEnumerator FlickerRoutine()
        {
            if (canFlick)
            {
                while (Light.activeSelf)
                {
                    yield return new WaitForSeconds(Random.Range(4f, 12f));
                    AudioSource.PlayOneShot(flickingSound);
                    int flickerCount = Random.Range(3, 8);
                    for (int i = 0; i < flickerCount; i++)
                    {
                        Light.SetActive(false);
                        meshRenderer.material = off;
                        yield return new WaitForSeconds(Random.Range(0.03f, 0.12f));
                        Light.SetActive(true);
                        meshRenderer.material = on;
                        yield return new WaitForSeconds(Random.Range(0.04f, 0.18f));
                    }
                }
            }
        }
    }
}
