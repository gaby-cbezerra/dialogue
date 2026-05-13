using System;
using UnityEngine;

namespace Core
{
    public class DoorController : MonoBehaviour
    {
        public static event Action<bool> OnOpenedOrLocked;
        private bool _isOpen;
        public Animator animator;
        // track whether player is currently in interaction range
        private bool playerInRange = false;

        public bool IsPlayerInRange => playerInRange;
        [Header("Interaction")]
        [Tooltip("Distance used as a fallback check to decide if the player is still near the door after animations move colliders.")]
        public float interactionRadius = 2f;
        [Tooltip("Time in seconds to ignore exit events immediately after toggling the door. Helps avoid false exits caused by door animation moving colliders.")]
        public float exitIgnoreDuration = 0.5f;

        // internal timestamp until which exit events are ignored
        private float ignoreExitUntilTime = 0f;
        [Header("Probing")]
        [Tooltip("How often (seconds) to check player's distance as a fallback when collision/trigger events are missed.")]
        public float checkInterval = 0.2f;
        private float nextCheckTime = 0f;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("DoorController: Player collided with door.");
                playerInRange = true;
                GameEventSystem.Invoke();
                // ensure UI knows current door state when player enters so text shows correctly
                OnOpenedOrLocked?.Invoke(_isOpen);
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("DoorController: Player left door collision. Verifying distance before hiding UI.");

                if (Time.time < ignoreExitUntilTime)
                {
                    Debug.Log("DoorController: Ignoring exit because we are within the exit ignore window after toggle.");
                    return;
                }
                // Check actual distance to player; sometimes animations move colliders and cause a false exit event.
                var player = collision.gameObject;
                float d = Vector3.Distance(player.transform.position, transform.position);
                if (d <= interactionRadius)
                {
                    Debug.Log($"DoorController: Ignoring exit because player is still within interactionRadius ({d} <= {interactionRadius}).");
                    playerInRange = true;
                    return;
                }

                playerInRange = false;
                GameEventSystem.InvokeLeft();
            }
        }

        // Also support trigger-based interaction zones (recommended: use a child trigger collider that doesn't move with the door animation)
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("DoorController: Player entered trigger interaction zone.");
                playerInRange = true;
                GameEventSystem.Invoke();
                // ensure UI knows current door state when player enters so text shows correctly
                OnOpenedOrLocked?.Invoke(_isOpen);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("DoorController: Player exited trigger interaction zone. Verifying distance before hiding UI.");

                if (Time.time < ignoreExitUntilTime)
                {
                    Debug.Log("DoorController: Ignoring trigger exit because we are within the exit ignore window after toggle.");
                    return;
                }
                var player = other.gameObject;
                float d = Vector3.Distance(player.transform.position, transform.position);
                if (d <= interactionRadius)
                {
                    Debug.Log($"DoorController: Ignoring trigger exit because player is still within interactionRadius ({d} <= {interactionRadius}).");
                    playerInRange = true;
                    return;
                }

                playerInRange = false;
                GameEventSystem.InvokeLeft();
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
            
            // Notify listeners about the new state
            OnOpenedOrLocked?.Invoke(_isOpen);
            Debug.Log($"DoorController: OpenDoor -> new state: {_isOpen}. PlayerInRange={playerInRange}");

            // If the player is still in range after toggling, ensure the UI is updated/shown
            if (playerInRange)
            {
                Debug.Log("DoorController: Player still in range after toggle - re-invoking show event.");
                GameEventSystem.Invoke();
            }
            else
            {
                // Fallback: check player's distance to door in case animation moved colliders and produced a false exit.
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    float d = Vector3.Distance(player.transform.position, transform.position);
                    if (d <= interactionRadius)
                    {
                        Debug.Log($"DoorController: Player within fallback interactionRadius ({d} <= {interactionRadius}) after toggle — treating as in-range.");
                        playerInRange = true;
                        GameEventSystem.Invoke();
                    }
                    else
                    {
                        Debug.Log($"DoorController: Player not within fallback radius after toggle ({d} > {interactionRadius}).");
                    }
                }
            }
            // Ignore any exit events for a short time to allow animations to complete without hiding UI.
            ignoreExitUntilTime = Time.time + exitIgnoreDuration;
            //animator.Play("Padrao");
        }

        // Periodically probe player's distance to ensure UI shows/hides correctly even if physics events were missed
        void Update()
        {
            if (Time.time < nextCheckTime) return;
            nextCheckTime = Time.time + Mathf.Max(0.01f, checkInterval);

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            float d = Vector3.Distance(player.transform.position, transform.position);

            // If player is within interaction radius and we currently think they're out of range, re-enter
            if (d <= interactionRadius && !playerInRange)
            {
                // If we're within the ignore window for exits, still allow re-showing
                Debug.Log($"DoorController(Update): Detected player within radius ({d} <= {interactionRadius}). Showing UI.");
                playerInRange = true;
                GameEventSystem.Invoke();
                OnOpenedOrLocked?.Invoke(_isOpen);
            }

            // If player is outside radius and we currently think they're in range, consider exiting (respect ignore window)
            if (d > interactionRadius && playerInRange)
            {
                if (Time.time < ignoreExitUntilTime)
                {
                    // still ignore
                    return;
                }

                Debug.Log($"DoorController(Update): Player moved out of radius ({d} > {interactionRadius}). Hiding UI.");
                playerInRange = false;
                GameEventSystem.InvokeLeft();
            }
        }
    }
}