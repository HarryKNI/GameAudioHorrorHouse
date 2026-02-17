using System.Collections;
using UnityEngine;

namespace RealisticHorrorGameSystem
{
    public class NoteScript : MonoBehaviour
    {
        [Multiline]
        public string noteText = "";
        [HideInInspector]
        public bool isReading = false;
        public MeshRenderer meshRenderer;
        public Collider collider;

        void Start()
        {

        }

        public void Interact()
        {
            if (isReading == false)
            {
                isReading = true;
                meshRenderer.enabled = collider.enabled = false;
                GameCanvas.Instance.Show_Note(noteText);
                GameCanvas.Instance.currentNote = this;
            }
            else
            {
                StartCoroutine(ActivateBack());
                GameCanvas.Instance.Hide_Note();
            }
        }

        IEnumerator ActivateBack()
        {
            yield return new WaitForSeconds(0.25f);
            isReading = false;
            meshRenderer.enabled = collider.enabled = true;
        }
    }
}