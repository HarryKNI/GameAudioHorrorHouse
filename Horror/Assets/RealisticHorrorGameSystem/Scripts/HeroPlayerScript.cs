using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

namespace RealisticHorrorGameSystem
{
    public class HeroPlayerScript : MonoBehaviour
    {
        public static HeroPlayerScript Instance;
        public GameObject LadderPointInCamera;
        public FirstPersonController firstPersonController;
        public CharacterController characterController;
        public Transform DemonComingPoint;
        public int Health = 100;
        public List<int> Keys_Grabbed = new List<int>();
        [HideInInspector]
        public GameObject Carrying_Ladder = null;
        public GameObject FPSHands;
        public FlashLightScript FlashLight;
        public AnomalyDetector AnomalyDetector;
        public MedkitScript MedkitManager;
        public KnifeScript Knife;
        public CrossScript Cross;
        public bool isHoldingBox = false;
        public GameObject Hand_FlashLight;
        public GameObject Hand_AnomalyDetector;
        public GameObject Hand_Cross;
        public GameObject Hand_Knife;
        public bool isHiding = false;
        public bool isSitting = false;
        public HideScript hidingPlace;
        public GameObject MainCamera;
        public bool isCaughtByEnemy = false;
        private bool isHeroBusy = false;
        public float lastInteractionTime = 0;
        private InputAction interactAction;
        private InputAction mouseAction;

        [Header("FMOD Event Parameters")]
        //FMOD.Studio.ParameterInstance surfaceParam;
        [SerializeField] private AudioManager audioManager;
        public FMODUnity.ParamRef surfaceParam;
        public int SurfaceIndex;

        void Start()
        {
            Time.timeScale = 1;
            GameCanvas.Instance.UpdateHealth();
        }

        public void ResetHands()
        {
            HeroPlayerScript.Instance.FPSHands.SetActive(true);
            HeroPlayerScript.Instance.FlashLight.enabled = false;
            HeroPlayerScript.Instance.Cross.enabled = false;
            HeroPlayerScript.Instance.AnomalyDetector.enabled = false;
            HeroPlayerScript.Instance.Knife.enabled = false;
            HeroPlayerScript.Instance.MedkitManager.enabled = false;
            Hand_AnomalyDetector.SetActive(false);
            Hand_Cross.SetActive(false);
            Hand_FlashLight.SetActive(false);
            Hand_Knife.SetActive(false);
        }

        public void SetHeroBusy(bool b)
        {
            isHeroBusy = b;
        }

        public void ToggleHandObjects()
        {
            StartCoroutine(HideHands());
        }

        IEnumerator HideHands()
        {
            yield return new WaitForSeconds(0.1f);
            FPSHands.SetActive(false);
        }

        public bool GetHeroBusy()
        {
            return isHeroBusy;
        }

        void ShowInteractionWarning(ItemScript item)
        {
            switch (item.interactionType)
            {
                case InteractionType.Grab:
                    GameCanvas.Instance.Show_Warning("Press E to Grab");
                    break;
                case InteractionType.KnockDown:
                    GameCanvas.Instance.Show_Warning("Press E to Knock Down");
                    break;
                case InteractionType.Carry:
                    GameCanvas.Instance.Show_Warning("Press E to Carry");
                    break;
                case InteractionType.Put:
                    GameCanvas.Instance.Show_Warning("Press E to Put");
                    break;
                case InteractionType.Hide:
                    GameCanvas.Instance.Show_Warning("Press E to Hide");
                    break;
                case InteractionType.Sit:
                    GameCanvas.Instance.Show_Warning("Press E to Sit");
                    break;
                case InteractionType.Interact:
                    GameCanvas.Instance.Show_Warning("Press E to Interact");
                    break;
                case InteractionType.Open:
                    GameCanvas.Instance.Show_Warning("Press E to Open");
                    break;
                case InteractionType.Read:
                    GameCanvas.Instance.Show_Warning("Press E to Read");
                    break;
                case InteractionType.PressAndHold:
                    GameCanvas.Instance.Show_Warning("Press and Hold to Maintain");
                    break;
                default:
                    GameCanvas.Instance.Show_Warning("Press E");
                    break;
            }
        }

        public void Escape()
        {
            isCaughtByEnemy = false;
            ActivatePlayer();
            GameCanvas.Instance.Image_CaughtBlood.SetActive(false);
        }

        public void GetCaught()
        {
            isCaughtByEnemy = true;
            GameCanvas.Instance.Image_CaughtBlood.SetActive(true);
            GameCanvas.Instance.Panel_ClickToEscape.SetActive(true);
            StartCoroutine(Die());
        }

        public IEnumerator Die()
        {
            yield return new WaitForSeconds(AdvancedGameManager.Instance.durationForClickingTime);
            if(isCaughtByEnemy)
            {
                GetDamage(100);
            }
        }

        public void GetDamage(int Damage)
        {
            Health = Health - Damage;
            isCaughtByEnemy = false;
            GameCanvas.Instance.Show_Blood_Effect();
            GameCanvas.Instance.Panel_ClickToEscape.SetActive(false);
            if (Health < 0) Health = 0;
            GameCanvas.Instance.UpdateHealth();
            if (Health <= 0)
            {
                DeactivatePlayer();
                CameraLook.Instance.fCamShakeImpulse = 1;
                GameCanvas.Instance.Show_GameOverPanel();
                HeroPlayerScript.Instance.FPSHands.SetActive(false);
                HeroPlayerScript.Instance.Hand_FlashLight.SetActive(false);
                HeroPlayerScript.Instance.Hand_Cross.SetActive(false);
                HeroPlayerScript.Instance.Hand_AnomalyDetector.SetActive(false);
                if (AdvancedGameManager.Instance.gameOverEventToInvoke != null)
                {
                    AdvancedGameManager.Instance.gameOverEventToInvoke.Invoke();
                }
            }
            else
            {
                CameraLook.Instance.fCamShakeImpulse = 0.5f;
            }
        }

        private void Awake()
        {
            Instance = this;
            interactAction = new UnityEngine.InputSystem.InputAction(type: UnityEngine.InputSystem.InputActionType.Button, binding: "<Keyboard>/e");
            interactAction.Enable();
            mouseAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
            mouseAction.Enable();

            //surfaceParam = audioManager.footstepEvent.getParameter("Surface");
            for (int i = 0; i < audioManager.footstepEvent.Params.Length; i++)
            {
                if (audioManager.footstepEvent.Params[i].Name == "Surface")
                {
                    surfaceParam = audioManager.footstepEvent.Params[i];
                    Debug.Log(surfaceParam.Name);
                }
            }
        }

        private void Update()
        {
            if(isCaughtByEnemy && AdvancedGameManager.Instance.canPlayerEscapeByClickEnough)
            {
                CameraLook.Instance.fCamShakeImpulse = 0.25f;
                if (mouseAction.WasPressedThisFrame())
                {
                    GameCanvas.Instance.Click_Escape();
                }
            }
            HandleRaycastInteraction();
        }

        public float interactDistance = 3f;
        [HideInInspector]
        public ItemScript currentItem;

        void HandleRaycastInteraction()
        {
            if (!isHoldingBox)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, interactDistance))
                {
                    ItemScript item = hit.collider.GetComponent<ItemScript>();
                    if (item != null)
                    {
                        ShowInteractionWarning(item);
                        if (interactAction.WasPressedThisFrame())
                        {
                            item.Interact();
                            currentItem = item;
                        }
                    }
                    else
                    {
                        GameCanvas.Instance.Hide_Warning();
                    }
                }
                else
                {
                    GameCanvas.Instance.Hide_Warning();
                }
            }
            else
            {
                if (interactAction.WasPressedThisFrame() && currentItem != null)
                {
                    currentItem.Interact();
                }
            }
        }

        public void Grab_Key(int ID)
        {
            Keys_Grabbed.Add(ID);
        }

        public void ActivatePlayer()
        {
            transform.eulerAngles = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            firstPersonController.enabled = true;
            characterController.enabled = true;
            transform.eulerAngles = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (other.gameObject.tag == "CarpetSurface")
            //{
            //    surfaceParam.Value = 0;
            //    SurfaceIndex = 0;
            //    audioManager.footstepEvent.SetParameter("Surface", surfaceParam.Value, false);
            //}
            //else if (other.gameObject.tag == "GrassSurface")
            //{
            //    surfaceParam.Value = 1;
            //    SurfaceIndex = 1;
            //    audioManager.footstepEvent.SetParameter("Surface", 1, false);
            //}
            //else if (other.gameObject.tag == "WoodSurface")
            //{
            //    surfaceParam.Value = 2;
            //    SurfaceIndex = 2;
            //    audioManager.footstepEvent.SetParameter("Surface", surfaceParam.Value, false);
            //}
            //else if (other.gameObject.tag == "MetalSurface" || other.gameObject.tag == "Vent")
            //{
            //    surfaceParam.Value = 3;
            //    SurfaceIndex = 3;
            //    audioManager.footstepEvent.SetParameter("Surface", surfaceParam.Value, false);
            //}
        }

        private void OnTriggerStay(Collider other)
        {
            //Debug.Log("collision");
            //if(other.gameObject.CompareTag("Vent") && !firstPersonController.isInVent && firstPersonController.isCrouching)
            //{
            //    firstPersonController.isInVent = true;
            //}
            if (other.gameObject.CompareTag("CarpetSurface"))
            {
                surfaceParam.Value = 0;
                SurfaceIndex = 0;
                audioManager.footstepEvent.SetParameter("Surface", 0, false);
            }
            else if (other.gameObject.CompareTag("GrassSurface"))
            {
                surfaceParam.Value = 1;
                SurfaceIndex = 1;
                audioManager.footstepEvent.SetParameter("Surface", 1, false);
            }
            else if (other.gameObject.CompareTag("WoodSurface"))
            {
                surfaceParam.Value = 2;
                SurfaceIndex = 2;
                audioManager.footstepEvent.SetParameter("Surface", 2, false);
            }
            else if (other.gameObject.CompareTag("MetalSurface") || other.gameObject.CompareTag("Vent"))
            {
                surfaceParam.Value = 3;
                SurfaceIndex = 3;
                audioManager.footstepEvent.SetParameter("Surface", 3, false);
            }
            else if (other.gameObject.CompareTag("ConcreteSurface"))
            {
                surfaceParam.Value = 4;
                SurfaceIndex = 4;
                audioManager.footstepEvent.SetParameter("Surface", 4, false);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Vent") && firstPersonController.isInVent)
            {
                firstPersonController.isInVent = false;
            }
        }

        public void DeactivatePlayer()
        {
            firstPersonController.enabled = false;
            characterController.enabled = false;
        }

        public void ChangeTag(bool hide)
        {
            if (hide)
            {
                gameObject.tag = "Untagged";
            }
            else
            {
                gameObject.tag = "Player";
            }
        }

        public void ActivatePlayerInputs()
        {
            firstPersonController.enabled = true;
            characterController.enabled = true;
        }
    }
}