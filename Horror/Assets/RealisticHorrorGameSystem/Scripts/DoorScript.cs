using System.Collections;
using UnityEngine;
using System.Linq;
using FMODUnity;
using UnityEngine.Audio;

namespace RealisticHorrorGameSystem
{
    public class DoorScript : MonoBehaviour
    {
        public bool isLocked = false;
        public bool giveLockMessage = false;
        public bool isOpened = false;
        public int KeyID_ToOpen = 0;
        private Animation dooranimation;
        public Collider doorCollider;

        [SerializeField] private StudioEventEmitter doorOpenEvent;
        [SerializeField] private StudioEventEmitter doorCloseEvent;

        private void Start()
        {
            dooranimation = GetComponent<Animation>();
        }

        IEnumerator OpenTheDoor()
        {
            LastTimeTry = LastTimeTry - 1;
            yield return new WaitForSeconds(0.5f);
            TryToOpen();
        }

        private float LastTimeTry = 0;
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
                        dooranimation["Custom_Animation_Door_Try"].time = 0;
                        dooranimation["Custom_Animation_Door_Try"].speed = 1;
                        dooranimation.Play("Custom_Animation_Door_Try");
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
                        dooranimation["Custom_Animation_Door_Open"].time = 0;
                        dooranimation["Custom_Animation_Door_Open"].speed = 1;
                        dooranimation.Play("Custom_Animation_Door_Open");
                        isOpened = true;
                        doorCollider.enabled = false;
                        Play_Door_Wooden_Open();
                    }
                    else
                    {
                        isOpened = false;
                        Play_Door_Close();
                        dooranimation["Custom_Animation_Door_Open"].time = dooranimation["Custom_Animation_Door_Open"].length;
                        dooranimation["Custom_Animation_Door_Open"].speed = -1;
                        dooranimation.Play("Custom_Animation_Door_Open");
                        StartCoroutine(ActivateCollider());
                    }
                }
            }

            IEnumerator ActivateCollider()
            {
                yield return new WaitForSeconds(1);
                doorCollider.enabled = true;
            }
        }

        public void Play_Door_Wooden_Open()
        {
            //audioSource.PlayOneShot(Door_Wooden_Open[UnityEngine.Random.Range(0, Door_Wooden_Open.Length)]);
            doorOpenEvent.Play();
        }

        public void Play_Door_Close()
        {
            //audioSource.PlayOneShot(Door_Close[UnityEngine.Random.Range(0, Door_Close.Length)]);
            doorCloseEvent.Play();
        }
    }
}