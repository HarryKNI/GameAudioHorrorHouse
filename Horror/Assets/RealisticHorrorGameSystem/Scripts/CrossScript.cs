using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RealisticHorrorGameSystem
{
    public class CrossScript : MonoBehaviour
    {
        public static CrossScript Instance;
        public bool isGrabbed = false;
        public AudioSource audioSource;
        public AudioClip audioHolyWolds;
        public Animation handAnimation;
        public LayerMask enemyLayerMask;
        public float crossDamage = 25f;
        public int EquipmentUIIndex = 1;
        private InputAction mouseAction;
        void Awake()
        {
            Instance = this;
            mouseAction = new UnityEngine.InputSystem.InputAction(type: UnityEngine.InputSystem.InputActionType.Button, binding: "<Mouse>/leftButton");
            mouseAction.Enable();
        }
        public void Grabbed()
        {
            HeroPlayerScript.Instance.ResetHands();
            HeroPlayerScript.Instance.Hand_Cross.SetActive(true);
            HeroPlayerScript.Instance.Cross.enabled = true;
            GameCanvas.Instance.ActivateEquipments(EquipmentUIIndex);
            isGrabbed = true;
        }

        private float LastEffectTime = 0f;
        public float EffectRange = 15f;
        private float EffectCheckPeriod = 0.5f;
        private void Update()
        {
            if (!isGrabbed) return;
            if (Time.time > LastEffectTime + EffectCheckPeriod)
            {
                SayHolyWords();
            }
        }

        void SayHolyWords()
        {
            if (mouseAction.WasPressedThisFrame())
            {
                LastEffectTime = Time.time;
                if (handAnimation != null && !handAnimation.isPlaying)
                {
                    handAnimation.Play();
                    PlayAudio();
                }

                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, EffectRange, enemyLayerMask))
                {
                    GameObject hitObj = hit.collider.gameObject;
                    if (hitObj.CompareTag("Enemy"))
                    {
                        DemonScript demon = hitObj.GetComponent<DemonScript>();
                        if (demon != null && demon.health > 0)
                        {
                            StartCoroutine(SetDamage(demon));
                        }
                    }
                }
            }
        }

        public void PlayAudio()
        {
            audioSource.PlayOneShot(audioHolyWolds);
        }

        IEnumerator SetDamage(DemonScript demon)
        {
            yield return new WaitForSeconds(1);
            demon.GetDamageByCross(crossDamage);
        }

        public void StopAudio()
        {
            audioSource.Stop();
        }
    }
}
