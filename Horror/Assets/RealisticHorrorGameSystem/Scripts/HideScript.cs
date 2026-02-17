using UnityEngine;

namespace RealisticHorrorGameSystem
{
    public class HideScript : MonoBehaviour
    {
        public Camera camera;
        private float lastHidingTime = 0;

        public void Hide()
        {
            if(Time.time > lastHidingTime + 0.25f)
            {
                lastHidingTime = Time.time;
                if (HeroPlayerScript.Instance.isHiding)
                {
                    GameCanvas.Instance.Blink();
                    AudioManager.Instance.Play_Audio_Hide();
                    HeroPlayerScript.Instance.isHiding = false;
                    camera.gameObject.SetActive(false);
                    HeroPlayerScript.Instance.MainCamera.SetActive(true);
                    HeroPlayerScript.Instance.gameObject.SetActive(true);
                    FPSHandRotator.Instance.gameObject.SetActive(true);
                    GameCanvas.Instance.Show_GameUI();
                }
                else
                {
                    GameCanvas.Instance.Blink();
                    AudioManager.Instance.Play_Audio_Hide();
                    HeroPlayerScript.Instance.isHiding = true;
                    HeroPlayerScript.Instance.MainCamera.SetActive(false);
                    HeroPlayerScript.Instance.gameObject.SetActive(false);
                    FPSHandRotator.Instance.gameObject.SetActive(false);
                    camera.gameObject.SetActive(true);
                    HeroPlayerScript.Instance.hidingPlace = this;
                    GameCanvas.Instance.Hide_Warning();
                    GameCanvas.Instance.Hide_GameUI();
                }
            }
        }
    }
}
