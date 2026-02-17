using System.Collections;
using UnityEngine;

namespace RealisticHorrorGameSystem
{
    public class CaseScript : MonoBehaviour
    {
        public bool isOpened = false;
        public bool isLocked = false;
        public bool giveLockMessage = false;
        public int KeyID_ToOpen = 0;
        private Animation dooranimation;
        public Collider caseDoorCollider;
        private float LastTimeTry = 0;

        public GameObject itemInCabinet;
        void Start()
        {
            dooranimation = GetComponentInChildren<Animation>();
            if (itemInCabinet != null)
            {
                itemInCabinet.GetComponent<Collider>().enabled = false;
            }
        }

        IEnumerator OpenTheDoor()
        {
            LastTimeTry = LastTimeTry - 1;
            yield return new WaitForSeconds(0.5f);
            TryToOpen();
        }

        public void TryToOpen()
        {
            if (Time.time > LastTimeTry + 1)
            {
                LastTimeTry = Time.time;
                if (isLocked)
                {
                    if (HeroPlayerScript.Instance.Keys_Grabbed.Contains(KeyID_ToOpen))
                    {
                        isLocked = false;
                        AudioManager.Instance.Play_Door_UnLock();
                        StartCoroutine(OpenTheDoor());
                    }
                    else
                    {
                        dooranimation.Play("Custom_Animation_CaseDoor_Try");
                        AudioManager.Instance.Play_Door_TryOpen();
                        if (giveLockMessage)
                        {
                            GameCanvas.Instance.Show_Warning("Locked. Find the key!");
                        }
                        return;
                    }
                }
                else
                {
                    if (isOpened == false)
                    {
                        AudioManager.Instance.Play_Audio_Cabinet_Open();
                        StartCoroutine(ActivateInsideCollider());
                        Destroy(GetComponent<ItemScript>());
                        dooranimation.Play("Custom_Animation_CaseDoor_Open");
                        isOpened = true;
                        caseDoorCollider.enabled = false;
                        AudioManager.Instance.Play_Door_Wooden_Open();
                    }
                }
            }
        }

        IEnumerator ActivateInsideCollider()
        {
            yield return new WaitForSeconds(1);
            if (itemInCabinet != null)
            {
                itemInCabinet.GetComponent<Collider>().enabled = true;
            }
            Destroy(this);
        }
    }
}