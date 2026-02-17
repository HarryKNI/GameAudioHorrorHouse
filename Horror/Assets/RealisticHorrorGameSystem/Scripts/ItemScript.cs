using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RealisticHorrorGameSystem
{
    public class ItemScript : MonoBehaviour
    {
        public ItemType itemType;
        public string Name = "";
        [HideInInspector]
        public bool playerUsing = false;
        public InteractionType interactionType;
        public UnityEvent eventToInvokeWhenInteract;
        private bool isGrabbedBefore = false;

        public void Interact()
        {
            if (Time.time < HeroPlayerScript.Instance.lastInteractionTime + 0.5f) return;

            HeroPlayerScript.Instance.lastInteractionTime = Time.time;
            GameCanvas.Instance.Hide_Warning();
            if (itemType == ItemType.Flashlight)
            {
                AudioManager.Instance.Play_Item_Grab();
                FlashLightScript.Instance.Grabbed();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                Destroy(gameObject);
            }
            else if (itemType == ItemType.Door)
            {
                GetComponent<DoorScript>().TryToOpen();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Light)
            {
                GetComponent<LightScript>().Interact();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.ElectricLight)
            {
                GetComponent<ElectricLightScript>().Interact();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Sink)
            {
                GetComponent<SinkScript>().Interact();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.SecurityMonitor)
            {
                GetComponent<MonitorScript>().Interact();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Key)
            {
                AudioManager.Instance.Play_Item_Grab();
                GetComponent<KeyScript>().isGrabbed = true;
                HeroPlayerScript.Instance.Grab_Key(GetComponent<KeyScript>().KeyID);
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                Destroy(gameObject);
            }
            else if (itemType == ItemType.Note)
            {
                GetComponent<NoteScript>().Interact();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.BigKnife)
            {
                isGrabbedBefore = true;
                AudioManager.Instance.Play_Item_Grab();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                Destroy(gameObject);
            }
            else if (itemType == ItemType.Knife)
            {
                AudioManager.Instance.Play_Item_Grab();
                HeroPlayerScript.Instance.Knife.Grabbed();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                Destroy(gameObject);
            }
            else if (itemType == ItemType.AnomalyDetector)
            {
                AudioManager.Instance.Play_Item_Grab();
                HeroPlayerScript.Instance.AnomalyDetector.Grabbed();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                Destroy(gameObject);
            }
            else if (itemType == ItemType.Bathtub)
            {
                GetComponent<BathtubScript>().Interact();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Cross)
            {
                AudioManager.Instance.Play_Item_Grab();
                HeroPlayerScript.Instance.Cross.Grabbed();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                Destroy(gameObject);
            }
            else if (itemType == ItemType.Note && GetComponent<NoteScript>().isReading)
            {
                GameCanvas.Instance.Hide_Note();
            }
            else if (itemType == ItemType.Box)
            {
                GetComponent<BoxScript>().Interact();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.LadderPuttingArea)
            {
                if (HeroPlayerScript.Instance.Carrying_Ladder != null && !HeroPlayerScript.Instance.Carrying_Ladder.GetComponent<LadderScript>().isPut)
                {
                    GameCanvas.Instance.Drop_GrabbedLadder(transform);
                }
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Case)
            {
                GetComponent<CaseScript>().TryToOpen();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Drawer)
            {
                GetComponent<DrawerScript>().Interact();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.MedKit)
            {
                HeroPlayerScript.Instance.MedkitManager.Grabbed();
                AudioManager.Instance.Play_Item_Grab();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                Destroy(gameObject);
            }
            else if (itemType == ItemType.WoodToBreak)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().AddExplosionForce(100, transform.position, 1);
                GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-90, 90), Random.Range(-90, 90), Random.Range(-90, 90)));
                AudioManager.Instance.Play_WoodBreakable();
                GetComponent<BoxCollider>().enabled = false;
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
                Destroy(gameObject, 2);
            }
            else if (itemType == ItemType.Ladder && !GetComponent<LadderScript>().isPut)
            {
                HeroPlayerScript.Instance.Carrying_Ladder = this.gameObject;
                SphereCollider[] colliders = GetComponents<SphereCollider>();
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = false;
                }
                GetComponent<BoxCollider>().enabled = false;
                AudioManager.Instance.Play_Item_Grab();
                transform.parent = HeroPlayerScript.Instance.LadderPointInCamera.transform.parent;
                transform.position = HeroPlayerScript.Instance.LadderPointInCamera.transform.position;
                transform.eulerAngles = HeroPlayerScript.Instance.LadderPointInCamera.transform.eulerAngles;
                transform.localScale = HeroPlayerScript.Instance.LadderPointInCamera.transform.localScale;
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Chest)
            {
                if (GetComponent<HideScript>() != null)
                {
                    GetComponent<HideScript>().Hide();
                }
                else if (GetComponent<ChestScript>() != null)
                {
                    GetComponent<ChestScript>().Loot();
                }
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Bed)
            {
                GetComponent<HideScript>().Hide();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Coffin)
            {
                GetComponent<HideScript>().Hide();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            else if (itemType == ItemType.Armchair)
            {
                GetComponent<ChairScript>().Sit();
                if (eventToInvokeWhenInteract != null)
                {
                    eventToInvokeWhenInteract.Invoke();
                }
            }
            StartCoroutine(UpdateStatus());
        }

        IEnumerator UpdateStatus()
        {
            yield return new WaitForSeconds(0.25f);
            playerUsing = !playerUsing;
        }

        public void DeactivateCollidersAndRigidbody()
        {
            Collider[] allColliders = GetComponentsInChildren<Collider>();
            for (int i = 0; i < allColliders.Length; i++)
            {
                allColliders[i].enabled = false;
                transform.tag = "Untagged";
            }
            if (GetComponent<Rigidbody>() != null)
            {
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Rigidbody>().useGravity = false;
            }
        }

        public void ActivateCollidersAndRigidbody()
        {
            transform.localScale = new Vector3(1, 1, 1);
            Collider[] allColliders = GetComponentsInChildren<Collider>();
            for (int i = 0; i < allColliders.Length; i++)
            {
                allColliders[i].enabled = true;
                transform.tag = "Item";
            }
            if (GetComponent<Rigidbody>() != null)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }

    public enum ItemType
    {
        Door,
        Flashlight,
        Key,
        Note,
        Cabinet,
        Ladder,
        WoodToBreak,
        LadderPuttingArea,
        Chest,
        None,
        Drawer,
        Box,
        MedKit,
        Cross,
        Knife,
        AnomalyDetector,
        Bed,
        Case,
        Armchair,
        Light,
        Sink,
        Coffin,
        BigKnife,
        Bathtub,
        ElectricLight,
        SecurityMonitor
    }

    public enum InteractionType
    {
        Grab,
        Hide,
        Carry,
        Read,
        Open,
        PressAndHold,
        Put,
        KnockDown,
        Sit,
        Interact,
        Loot
    }
}