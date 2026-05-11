using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    public class DoorController : MonoBehaviour
    {
        public static event Action<bool> OnOpenedOrLocked;
        private bool _isOpen;
        public Animator animator;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("DoorController: Player collided with door.");
                GameEventSystem.Invoke();
            }
        }

        public void OpenDoor()
        {
            if (!_isOpen)
            {
                animator.Play("PortaAbrindo");
                _isOpen = true;
            }
            else
            {
                animator.Play("PortaFechando");
                _isOpen = false;
            }
            
            //animator.Play("Padrao");
        }
    }
}