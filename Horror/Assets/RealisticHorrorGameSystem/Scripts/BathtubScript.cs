using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealisticHorrorGameSystem
{
    public class BathtubScript : MonoBehaviour
    {
        public Animation bloodanimation;
        public Collider collider;
        public AudioSource audioSource;
        private bool isInteracted = false;

        public void Interact()
        {
            if (!isInteracted)
            {
                isInteracted = true;
                bloodanimation.Play();
                audioSource.Play();
                collider.enabled = false;
            }
        }
    }
}
