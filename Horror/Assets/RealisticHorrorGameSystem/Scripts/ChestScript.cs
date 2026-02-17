using UnityEngine;

namespace RealisticHorrorGameSystem
{
    public class ChestScript : MonoBehaviour
    {
        public bool isOpened = false;
        public GameObject Cover;
        public GameObject itemInChest;
        private float lastInteractedTime = 0;

        private void Start()
        {
            if (itemInChest != null)
            {
                itemInChest.GetComponent<Collider>().enabled = false;
            }
        }

        public void Loot()
        {
            if (Time.time > lastInteractedTime + 0.25f)
            {
                lastInteractedTime = Time.time;
                if (isOpened)
                {
                    Cover.GetComponent<Animation>().Play("CloseChest");
                    AudioManager.Instance.Play_Door_Close();
                    isOpened = false;
                    if (itemInChest != null)
                    {
                        itemInChest.SetActive(false);
                        if (itemInChest.GetComponent<Collider>() != null)
                        {
                            itemInChest.GetComponent<Collider>().enabled = false;
                        }
                    }
                }
                else
                {
                    Cover.GetComponent<Animation>().Play("OpenChest");
                    AudioManager.Instance.Play_Door_Wooden_Open();
                    isOpened = true;
                    if (itemInChest != null)
                    {
                        itemInChest.SetActive(true);
                        if(itemInChest.GetComponent<Collider>() !=null)
                        {
                            itemInChest.GetComponent<Collider>().enabled = true;
                        }
                    }
                }
            }
        }
    }

    public enum ChestType 
    { 
        Hideable, 
        Lootable 
    }
}
