using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


namespace RealisticHorrorGameSystem
{
    public class KnifeScript : MonoBehaviour
    {
        public static KnifeScript Instance;
        public int Damage = 40;
        public bool isHitting = false;
        public bool isGrabbed = false;
        public int EquipmentUIIndex = 2;
        private InputAction mouseAction;

        private void Awake()
        {
            Instance = this;
            mouseAction = new UnityEngine.InputSystem.InputAction(type: UnityEngine.InputSystem.InputActionType.Button, binding: "<Mouse>/leftButton");
            mouseAction.Enable();
        }

        void Start()
        {

        }

        void Update()
        {
            if (!isGrabbed) return;
            if (HeroPlayerScript.Instance.isHiding) return;

            if (mouseAction.WasPressedThisFrame() && !HeroPlayerScript.Instance.GetHeroBusy())
            {
                Hit();
            }
            if (mouseAction.WasReleasedThisFrame() && HeroPlayerScript.Instance.GetHeroBusy())
            {
                HeroPlayerScript.Instance.SetHeroBusy(false);
            }
        }

        public void Hit()
        {
            if (isHitting) return;
            isHitting = true;
            HeroPlayerScript.Instance.Hand_Knife.GetComponent<Animation>().Play();
            StartCoroutine(ReleaseHit(1.5f));
        }

        public IEnumerator ReleaseHit(float time)
        {
            yield return new WaitForSeconds(0.5f);
            CheckTheTarget();
            AudioManager.Instance.Play_Audio_BaseBallHit();
            yield return new WaitForSeconds(1f);
            isHitting = false;
        }

        public void CheckTheTarget()
        {
            Ray ray = Camera.main.ScreenPointToRay(GameCanvas.Instance.Crosshair.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 3))
            {
                if (hit.collider.GetComponent<DemonScript>() != null)
                {
                    hit.collider.GetComponent<DemonScript>().GetDamageByPistolOrBaseBallStick(Damage);
                }
            }
        }

        public void Grabbed()
        {
            //HeroPlayerScript.Instance.ResetHands();
            //HeroPlayerScript.Instance.Hand_Knife.SetActive(true);
            //HeroPlayerScript.Instance.Knife.enabled = true;
            GameCanvas.Instance.ActivateEquipments(EquipmentUIIndex);
            isGrabbed = true;

        }
    }
}