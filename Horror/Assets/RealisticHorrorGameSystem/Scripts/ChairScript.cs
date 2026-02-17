using UnityEngine;

namespace RealisticHorrorGameSystem
{
    public class ChairScript : MonoBehaviour
    {
        public Camera camera;
        private float lastSittingTime = 0;

        public void Sit()
        {
            if(Time.time > lastSittingTime + 0.25f)
            {
                lastSittingTime = Time.time;
                if (HeroPlayerScript.Instance.isSitting)
                {
                    GameCanvas.Instance.Blink();
                    HeroPlayerScript.Instance.isSitting = false;
                    camera.gameObject.SetActive(false);
                    HeroPlayerScript.Instance.MainCamera.SetActive(true);
                    HeroPlayerScript.Instance.gameObject.SetActive(true);
                    FPSHandRotator.Instance.gameObject.SetActive(true);
                }
                else
                {
                    GameCanvas.Instance.Blink();
                    HeroPlayerScript.Instance.isSitting = true;
                    HeroPlayerScript.Instance.MainCamera.SetActive(false);
                    HeroPlayerScript.Instance.gameObject.SetActive(false);
                    FPSHandRotator.Instance.gameObject.SetActive(false);
                    camera.gameObject.SetActive(true);
                }
            }
        }
    }
}
