using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

namespace RealisticHorrorGameSystem
{
    public class AnomalyDetector : MonoBehaviour
    {
        public static AnomalyDetector Instance;
        public bool isGrabbed = false;
        public Image Image_Filler;
        public AudioSource audioSource;
        public AudioClip audioDot;
        public int EquipmentUIIndex = 3;
        private float smoothedDist = 20.0f;

        [SerializeField] private StudioEventEmitter trackerEvent;
        public EventInstance tracker;

        void Awake()
        {
            Instance = this;
        }
        public void Grabbed()
        {
            //HeroPlayerScript.Instance.ResetHands();
            //HeroPlayerScript.Instance.Hand_AnomalyDetector.SetActive(true);
            //HeroPlayerScript.Instance.AnomalyDetector.enabled = true;
            GameCanvas.Instance.ActivateEquipments(EquipmentUIIndex);
            isGrabbed = true;
        }

        private float LastDetectionTime = 0f;
        private float LastDetectionTimeDot = 0f;
        public float DetectionRange = 15f;
        private float DetectionCheckPeriod = 0.5f;
        private void Update()
        {
            if (!isGrabbed) return;
            tracker.set3DAttributes(RuntimeUtils.To3DAttributes(HeroPlayerScript.Instance.MainCamera.transform));
            if (Time.time > LastDetectionTime + DetectionCheckPeriod)
            {
                LastDetectionTime = Time.time;
                CheckForParanormalActivity();
            }
        }

        void CheckForParanormalActivity()
        {
            Collider[] hitColliders = Physics.OverlapSphere(HeroPlayerScript.Instance.MainCamera.transform.position, DetectionRange);
            float closestDistance = DetectionRange + 1f;
            foreach (var hitCollider in hitColliders)
            {
                var anomaly = hitCollider.GetComponent<AnomalyScript>();
                if (anomaly != null)
                {
                    float dist = Vector3.Distance(HeroPlayerScript.Instance.MainCamera.transform.position, hitCollider.transform.position);
                    if (dist < closestDistance)
                        closestDistance = dist;
                }
            }

            float clampedDist = Mathf.Clamp(closestDistance, 0.0f, DetectionRange);
            float scaledDist = (clampedDist / DetectionRange) * 20.0f;

            float smooth = Mathf.Lerp(smoothedDist, scaledDist, Time.deltaTime * 5f);

            tracker.setParameterByName("Distance", scaledDist);

            Image_Filler.fillAmount = 1f - (clampedDist / DetectionRange);

            //float minInterval = 0.1f;
            //float maxInterval = 1.0f;
            //float interval = Mathf.Lerp(minInterval, maxInterval, closestDistance / DetectionRange);

            //if (Time.time > LastDetectionTimeDot + interval)
            //{
            //    LastDetectionTimeDot = Time.time;
            //    PlayAudio();
            //}
        }

        public void PlayAudio()
        {
            //audioSource.PlayOneShot(audioDot);
            tracker = RuntimeManager.CreateInstance("event:/Character/Tracker");
            tracker.set3DAttributes(RuntimeUtils.To3DAttributes(HeroPlayerScript.Instance.MainCamera.transform));
            tracker.start();
        }

        public void StopAudio()
        {
            //audioSource.Stop();
            //trackerEvent.Stop();

            if (tracker.isValid())
            {
                tracker.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                tracker.release();
            }
        }
    }
}