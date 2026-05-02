using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RealisticHorrorGameSystem
{
    public class GameCanvas : MonoBehaviour
    {
        public static GameCanvas Instance;
        public Image image_Blinking;
        public Image image_Healing;
        public GameObject Panel_ClickToEscape;
        public Image Image_ClickToEscape;
        public GameObject Panel_WarningPanel;
        public GameObject Image_CaughtBlood;
        public LayerMask layerMaskForInteract;
        public GameObject Panel_GameUI;
        public GameObject Panel_Pause;
        public GameObject Panel_Health;
        public Image Slider_Stamina;
        public GameObject Panel_Settings;
        public GameObject Panel_Note;
        public GameObject Panel_Note_Text;
        public Text Button_Close_Note_Text;
        public GameObject Panel_GameOver;
        public Image Image_Sprite_Blood;
        public Text Text_Health;
        private bool isGameOver = false;
        [HideInInspector]
        public bool isPaused = false;
        public GameObject Crosshair;
        [HideInInspector]
        public NoteScript currentNote = null;
        public GameObject[] equipments;
        public Image[] equipment_backgrounds;

        private InputAction escapeAction;
        private InputAction eAction;
        private InputAction[] equipmentActions;

        private void Awake()
        {
            Instance = this;
            escapeAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");
            escapeAction.Enable();

            eAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/e");
            eAction.Enable();

            equipmentActions = new InputAction[5];
            equipmentActions[0] = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/1");
            equipmentActions[1] = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/2");
            equipmentActions[2] = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/3");
            equipmentActions[3] = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/4");
            equipmentActions[4] = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/5");
            foreach (var action in equipmentActions) action.Enable();
        }

        public void Click_BacktoMenu()
        {
            SceneManager.LoadScene("Scene_MainMenu");
        }
        public void Click_Escape()
        {
            if (AdvancedGameManager.Instance.canPlayerEscapeByClickEnough)
            {
                if (Image_ClickToEscape.fillAmount < 1)
                {
                    float factor = 1f / AdvancedGameManager.Instance.neededClickAmountToEscape;
                    Image_ClickToEscape.fillAmount = Image_ClickToEscape.fillAmount + factor;
                    CameraLook.Instance.fCamShakeImpulse = 0.5f;
                    if (Image_ClickToEscape.fillAmount >= 1)
                    {
                        Image_ClickToEscape.fillAmount = 0;
                        Panel_ClickToEscape.SetActive(false);
                        HeroPlayerScript.Instance.Escape();
                    }
                }
            }
        }

        public void Show_Blood_Effect()
        {
            Image_Sprite_Blood.gameObject.SetActive(true);
            Image_Sprite_Blood.GetComponent<Animation>().Play("BloodEffect");
            StartCoroutine(HideEffect());
        }

        public void Blink()
        {
            image_Blinking.GetComponent<Animation>().Play();
        }

        IEnumerator HideEffect()
        {
            yield return new WaitForSeconds(1);
            Image_Sprite_Blood.gameObject.SetActive(false);
        }

        public void Click_Continue()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            HeroPlayerScript.Instance.ActivatePlayerInputs();
            CameraLook.Instance.enabled = true;
            Time.timeScale = 1;
            isPaused = false;
            Panel_Pause.SetActive(false);
            Panel_Settings.SetActive(false);
            Panel_GameUI.SetActive(true);

            AudioManager.Instance.SetPaused(false);
        }

        public void ShowHint(string text)
        {
            StartCoroutine(ShowHintInTime(text));
        }
        public Text Text_Info;
        public void ShowInfo(string text)
        {
            StartCoroutine(ShowInfoPanel(text));
        }

        private IEnumerator ShowInfoPanel(string text)
        {
            Text_Info.gameObject.SetActive(true);
            Text_Info.text = text;
            yield return new WaitForSeconds(3);
            Text_Info.gameObject.SetActive(false);
        }

        IEnumerator ShowHintInTime(string text)
        {
            yield return new WaitForSeconds(0.25f);
            GameCanvas.Instance.Show_Warning(text);
            yield return new WaitForSeconds(3);
            GameCanvas.Instance.Hide_Warning();
        }

        public void Click_Pause()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            HeroPlayerScript.Instance.DeactivatePlayer();
            CameraLook.Instance.enabled = false;
            Time.timeScale = 0;
            isPaused = true;
            Panel_Pause.SetActive(true);
            Panel_GameUI.SetActive(false);

            AudioManager.Instance.SetPaused(true);
        }

        public void UpdateHealth()
        {
            Text_Health.text = HeroPlayerScript.Instance.Health.ToString();
        }

        public void IncreaseHealth()
        {
            HeroPlayerScript.Instance.Health = 100;
            StartCoroutine(HealingEffect());
        }

        IEnumerator HealingEffect()
        {
            image_Healing.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            image_Healing.gameObject.SetActive(false);
        }

        void Update()
        {
            Input_Control();
            EscapeControl();
            EquipmentSwitch_Control();
        }

        public void ActivateEquipments(int index)
        {
            equipments[index].SetActive(true);
            ResetEquipmentBackgrounds(index);
        }

        public void AddEquipments(int index)
        {
            equipments[index].SetActive(true);
        }

        public void DeactivateEquipments(int index)
        {
            equipments[index].SetActive(false);
            ResetEquipmentBackgrounds(-1);
        }

        public void ResetEquipmentBackgrounds(int index)
        {
            for (int i = 0; i < equipment_backgrounds.Length; i++)
            {
                equipment_backgrounds[i].enabled = false;
            }
            if(index != -1)
            {
                equipment_backgrounds[index].enabled = true;
            }
        }

        private void EquipmentSwitch_Control()
        {
            if (equipmentActions[0].WasPressedThisFrame())
            {
                if (FlashLightScript.Instance.isGrabbed)
                {
                    AnomalyDetector.Instance.StopAudio();
                    HeroPlayerScript.Instance.ResetHands();
                    HeroPlayerScript.Instance.Hand_FlashLight.SetActive(true);
                    HeroPlayerScript.Instance.FlashLight.enabled = true;
                    ResetEquipmentBackgrounds(HeroPlayerScript.Instance.FlashLight.EquipmentUIIndex);
                    ShowInfo("Press U to use Flashlight!");
                }
            }
            else if (equipmentActions[1].WasPressedThisFrame())
            {
                if (CrossScript.Instance.isGrabbed)
                {
                    AnomalyDetector.Instance.StopAudio();
                    HeroPlayerScript.Instance.ResetHands();
                    HeroPlayerScript.Instance.Hand_Cross.SetActive(true);
                    HeroPlayerScript.Instance.Cross.enabled = true;
                    ResetEquipmentBackgrounds(HeroPlayerScript.Instance.Cross.EquipmentUIIndex);
                }
            }
            else if (equipmentActions[2].WasPressedThisFrame())
            {
                if (KnifeScript.Instance.isGrabbed)
                {
                    AnomalyDetector.Instance.StopAudio();
                    HeroPlayerScript.Instance.ResetHands();
                    HeroPlayerScript.Instance.Hand_Knife.SetActive(true);
                    HeroPlayerScript.Instance.Knife.enabled = true;
                    ResetEquipmentBackgrounds(HeroPlayerScript.Instance.Knife.EquipmentUIIndex);
                }
            }
            else if (equipmentActions[3].WasPressedThisFrame())
            {
                if (AnomalyDetector.Instance.isGrabbed)
                {
                    AnomalyDetector.Instance.PlayAudio();
                    HeroPlayerScript.Instance.ResetHands();
                    HeroPlayerScript.Instance.Hand_AnomalyDetector.SetActive(true);
                    HeroPlayerScript.Instance.AnomalyDetector.enabled = true;
                    ResetEquipmentBackgrounds(HeroPlayerScript.Instance.AnomalyDetector.EquipmentUIIndex);
                }
            }
            else if (equipmentActions[4].WasPressedThisFrame())
            {
                AnomalyDetector.Instance.StopAudio();
                HeroPlayerScript.Instance.ResetHands();
                HeroPlayerScript.Instance.MedkitManager.enabled = true;
                ResetEquipmentBackgrounds(HeroPlayerScript.Instance.MedkitManager.EquipmentUIIndex);
                ShowInfo("Press U to use Medkit!");
            }
        }

        private void Input_Control()
        {
            if (escapeAction.WasPressedThisFrame() && !isGameOver && !Panel_Note.activeSelf)
            {
                if (isPaused)
                {
                    Click_Continue();
                }
                else
                {
                    Click_Pause();
                }
            }
            else if (eAction.WasPressedThisFrame() && Panel_Note.activeSelf && currentNote.isReading)
            {
                currentNote.Interact();
            }
            else if (eAction.WasPressedThisFrame() && HeroPlayerScript.Instance.currentItem != null && !HeroPlayerScript.Instance.gameObject.activeSelf)
            {
                if (HeroPlayerScript.Instance.currentItem.playerUsing)
                {
                    HeroPlayerScript.Instance.currentItem.Interact();
                }
                else if (HeroPlayerScript.Instance.currentItem.itemType == ItemType.Box && GetComponent<BoxScript>().isHolding)
                {
                    HeroPlayerScript.Instance.currentItem.Interact();
                }
            }
        }
        private void EscapeControl()
        {
            if (AdvancedGameManager.Instance.canPlayerEscapeByClickEnough)
            {
                if (Panel_ClickToEscape.activeSelf)
                {
                    Image_ClickToEscape.fillAmount -= Time.deltaTime * 0.2f;
                }
            }
        }

        public void Show_GameOverPanel()
        {
            HeroPlayerScript.Instance.DeactivatePlayer();
            CameraLook.Instance.enabled = false;
            isGameOver = true;
            if (AdvancedGameManager.Instance.gameType == GameType.DieWhenYouAreCaught)
            {
                StartCoroutine(WaitAndShowGameOver(3));
            }
            else
            {
                StartCoroutine(WaitAndShowGameOver(1));
            }
        }

        IEnumerator WaitAndShowGameOver(int time)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            CameraLook.Instance.enabled = false;
            if (AdvancedGameManager.Instance.gameType == GameType.DieWhenYourHealthIsRunOut)
            {
                CameraLook.Instance.GetComponent<Animation>().Play();
            }
            yield return new WaitForSeconds(time);
            Time.timeScale = 0;
            Panel_GameUI.SetActive(false);
            Panel_GameOver.SetActive(true);
        }

        public void Click_Settings()
        {
            Panel_Settings.SetActive(true);
            Panel_Pause.SetActive(false);
        }

        public void Click_Close_Settings()
        {
            Panel_Settings.SetActive(false);
            Panel_Pause.SetActive(true);
        }

        public void Click_ShowNote()
        {
            Panel_GameUI.SetActive(false);
            HeroPlayerScript.Instance.DeactivatePlayer();
        }

        public void Click_Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Show_Note(string text)
        {
            Panel_GameUI.SetActive(false);
            HeroPlayerScript.Instance.DeactivatePlayer();
            Panel_Note.SetActive(true);
            Panel_Note_Text.GetComponent<Text>().text = text;
            AudioManager.Instance.Play_Note_Reading();
            Button_Close_Note_Text.text = "CLOSE (E)";
        }


        public void Hide_Note()
        {
            Panel_GameUI.SetActive(true);
            HeroPlayerScript.Instance.ActivatePlayerInputs();
            Panel_Note.SetActive(false);
            Panel_Note_Text.GetComponent<Text>().text = "";
            AudioManager.Instance.Play_Item_Close();
        }


        public void Show_GameUI()
        {
            Panel_GameUI.SetActive(true);
        }

        public void Hide_GameUI()
        {
            Panel_GameUI.SetActive(false);
        }

        public void Show_Warning(String textID)
        {
            ShowWarningPanel(textID);
        }

        void ShowWarningPanel(String text)
        {
            if(!Panel_WarningPanel.activeSelf)
            {
                Panel_WarningPanel.SetActive(true);
                Panel_WarningPanel.GetComponentInChildren<Text>().text = text;
            }
        }

        public void Hide_Warning()
        {
            Panel_WarningPanel.SetActive(false);
            Panel_WarningPanel.GetComponentInChildren<Text>().text = "";
        }

        public void Drop_GrabbedLadder(Transform LadderPutPoint)
        {
            HeroPlayerScript.Instance.Carrying_Ladder.SetActive(true);
            HeroPlayerScript.Instance.Carrying_Ladder.transform.parent = null;
            HeroPlayerScript.Instance.Carrying_Ladder.transform.position = LadderPutPoint.transform.position;
            HeroPlayerScript.Instance.Carrying_Ladder.transform.eulerAngles = LadderPutPoint.transform.eulerAngles;
            HeroPlayerScript.Instance.Carrying_Ladder.transform.localScale = LadderPutPoint.transform.localScale;
            HeroPlayerScript.Instance.Carrying_Ladder.GetComponent<BoxCollider>().enabled = true;
            HeroPlayerScript.Instance.Carrying_Ladder.tag = "Untagged";
            if (HeroPlayerScript.Instance.Carrying_Ladder.GetComponent<ItemScript>() != null)
            {
                Destroy(HeroPlayerScript.Instance.Carrying_Ladder.GetComponent<ItemScript>());
                Destroy(HeroPlayerScript.Instance.Carrying_Ladder.GetComponent<LadderScript>());
            }
            AudioManager.Instance.Play_Item_Grab();
            HeroPlayerScript.Instance.Carrying_Ladder.GetComponent<LadderScript>().isPut = true;
            HeroPlayerScript.Instance.Carrying_Ladder = null;
            Destroy(LadderPutPoint.gameObject);
        }
    }
}