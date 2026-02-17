using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

        void Awake()
        {
            Instance = this;
        }
        public void Grabbed()
        {
            HeroPlayerScript.Instance.ResetHands();
            HeroPlayerScript.Instance.Hand_AnomalyDetector.SetActive(true);
            HeroPlayerScript.Instance.AnomalyDetector.enabled = true;
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
            if(Time.time > LastDetectionTime + DetectionCheckPeriod)
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

            if (closestDistance <= DetectionRange)
            {
                float normalized = 1f - Mathf.Clamp01(closestDistance / DetectionRange);
                Image_Filler.fillAmount = normalized;

                float minInterval = 0.1f;
                float maxInterval = 1.0f;
                float interval = Mathf.Lerp(minInterval, maxInterval, closestDistance / DetectionRange);

                if (Time.time > LastDetectionTimeDot + interval)
                {
                    LastDetectionTimeDot = Time.time;
                    PlayAudio();
                }
            }
            else
            {
                Image_Filler.fillAmount = 0f;
                StopAudio();
            }
        }

        public void PlayAudio()
        {
            audioSource.PlayOneShot(audioDot);
        }

        public void StopAudio()
        {
            audioSource.Stop();
        }
    }
}