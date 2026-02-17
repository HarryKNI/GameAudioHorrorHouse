using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace RealisticHorrorGameSystem
{
    public class AdvancedGameManager : MonoBehaviour
    {
        [Tooltip("Player will die when Enemy AI catches him OR Health Bar will appear and Player will die when his health is run out.")]
        public GameType gameType;

        public bool canJump;
        public bool canCrouch;
        public bool canSprint;
        public bool canCarryBoxes;
        public bool canCarryMultipleWeapon = false;
        public bool showHealthBar = true;
        public bool hasInventory = true;

        public bool showCrosshair = true;
        public UnityEvent gameOverEventToInvoke;

        [Header("Escape from Enemy AI Feature")]
        public bool canPlayerEscapeByClickEnough = true;
        public int neededClickAmountToEscape = 10;
        public float durationForClickingTime = 3;

        public static AdvancedGameManager Instance;
        public SaverScript[] SavingAreas;

        [HideInInspector]
        public int Difficulty = 1;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Difficulty = PlayerPrefs.GetInt("Difficulty", 1);
            GameCanvas.Instance.Crosshair.SetActive(showCrosshair);
            if (!showHealthBar)
            {
                GameCanvas.Instance.Panel_Health.SetActive(false);
            }
            else if (gameType == GameType.DieWhenYourHealthIsRunOut)
            {
                GameCanvas.Instance.Panel_Health.SetActive(true);
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            HeroPlayerScript.Instance.characterController.enabled = false;
            HeroPlayerScript.Instance.firstPersonController.enabled = false;
            int lastSavedIndex = PlayerPrefs.GetInt("SavingIndex", -1);
            if (lastSavedIndex != -1 && SavingAreas != null && SavingAreas.Length > 0)
            {
                SaverScript lastSavedPoint = SavingAreas.Where(x => x.SavingIndex == lastSavedIndex).FirstOrDefault();
                if(lastSavedPoint != null)
                {
                    HeroPlayerScript.Instance.transform.position = lastSavedPoint.transform.position;
                }
            }
            HeroPlayerScript.Instance.characterController.enabled = true;
            HeroPlayerScript.Instance.firstPersonController.enabled = true;
        }
    }


    public enum GameType
    {
        DieWhenYouAreCaught,
        DieWhenYourHealthIsRunOut
    }
    public enum ControllerType
    {
        PcAndConsole,
        Mobile
    }
}