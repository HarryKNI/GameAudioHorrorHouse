using System.Collections;
using UnityEngine;

namespace RealisticHorrorGameSystem
{
    public class DrawerScript : MonoBehaviour
    {
        public bool isOpened = false;
        public bool isLocked = false;
        public int KeyID_ToOpen = 0;
        private float LastTimeTry = 0;
        public GameObject itemInDrawer;
        public DrawerSide drawerSide;
        void Start()
        {
            if (itemInDrawer != null)
            {
                itemInDrawer.GetComponent<Collider>().enabled = false;
            }
        }

        public void Interact()
        {
            if (isOpened == false)
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
                            isOpened = true;
                            AudioManager.Instance.Play_Audio_Drawer_Open();
                            GetComponent<Animation>().Play("DrawerOpen" + drawerSide.ToString());
                            StartCoroutine(ActivateInsideCollider());
                        }
                        else
                        {
                            AudioManager.Instance.Play_Door_TryOpen();
                            GameCanvas.Instance.Show_Warning("Locked! Find the key.");
                            return;
                        }
                    }
                    else
                    {
                        isOpened = true;
                        AudioManager.Instance.Play_Audio_Drawer_Open();
                        GetComponent<Animation>().Play("DrawerOpen" + drawerSide.ToString());
                        StartCoroutine(ActivateInsideCollider());
                    }
                }
            }
            else
            {
                isOpened = false;
                AudioManager.Instance.Play_Audio_Drawer_Open();
                GetComponent<Animation>().Play("DrawerClose" + drawerSide.ToString());
            }
        }

        IEnumerator ActivateInsideCollider()
        {
            yield return new WaitForSeconds(1);
            if (itemInDrawer != null)
            {
                itemInDrawer.GetComponent<Collider>().enabled = true;
            }
        }
    }
    public enum DrawerSide
    {
        Left, Right
    }
}