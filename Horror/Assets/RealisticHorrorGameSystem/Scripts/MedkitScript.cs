using RealisticHorrorGameSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RealisticHorrorGameSystem
{
    public class MedkitScript : MonoBehaviour
    {
        public static MedkitScript Instance;
        public bool isGrabbed = false;
        public AudioSource audioSource;
        public AudioClip audioHealing;
        public float healingAmountPerMedkit = 50f;
        public int EquipmentUIIndex = 4;
        private InputAction mouseAction;

        void Awake()
        {
            Instance = this;
            mouseAction = new UnityEngine.InputSystem.InputAction(type: UnityEngine.InputSystem.InputActionType.Button, binding: "<Keyboard>/u");
            mouseAction.Enable();
        }

        public void Grabbed()
        {
            GameCanvas.Instance.AddEquipments(EquipmentUIIndex);
        }

        void Update()
        {
            if (mouseAction.WasPressedThisFrame())
            {
                if (HeroPlayerScript.Instance.Health < 100)
                {
                    GameCanvas.Instance.IncreaseHealth();
                    GameCanvas.Instance.UpdateHealth();
                    audioSource.PlayOneShot(audioHealing);
                    isGrabbed = false;
                    GameCanvas.Instance.DeactivateEquipments(EquipmentUIIndex);
                }
            }
        }
    }
}