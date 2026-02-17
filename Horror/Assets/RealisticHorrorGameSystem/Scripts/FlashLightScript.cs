using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RealisticHorrorGameSystem
{
    public class FlashLightScript : MonoBehaviour
    {
        public static FlashLightScript Instance;
        public bool isGrabbed = false;
        public Light Light;
        public AudioSource audioSource;
        private bool isOn = false;
        public int EquipmentUIIndex = 0;
        private InputAction flashlightAction;

        void Awake()
        {
            Instance = this;
            flashlightAction = new UnityEngine.InputSystem.InputAction(type: UnityEngine.InputSystem.InputActionType.Button, binding: "<Keyboard>/u");
            flashlightAction.Enable();
        }
        public void FlashLight_Decision(bool decision)
        {
            Light.enabled = decision;
        }

        public void Grabbed()
        {
            GameCanvas.Instance.ShowInfo("Press U to use Flashlight!");
            HeroPlayerScript.Instance.ResetHands();
            HeroPlayerScript.Instance.Hand_FlashLight.SetActive(true);
            HeroPlayerScript.Instance.FlashLight.enabled = true;
            GameCanvas.Instance.ActivateEquipments(EquipmentUIIndex);
            isGrabbed = true;
        }
        private void Update()
        {
            if (!isGrabbed) return;
            if (flashlightAction.WasReleasedThisFrame())
            {
                if (!isOn)
                {
                    isOn = true;
                    FlashLightScript.Instance.FlashLight_Decision(true);
                    AudioManager.Instance.Play_Flashlight_Open();
                }
                else
                {
                    isOn = false;
                    FlashLightScript.Instance.FlashLight_Decision(false);
                }
                AudioManager.Instance.Play_Flashlight_Open();
            }
        }

        public void PlayAudioBlueLight()
        {
            audioSource.Play();
        }

        public void StopAudioBlueLight()
        {
            audioSource.Stop();
        }
    }
}