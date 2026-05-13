using UnityEngine;
using System;

namespace Core
{
    public class GameEventSystem : MonoBehaviour
    {
        public static event Action OnPlayerCollidedWithDoor;
        // New event for leaving the door interaction area
        public static event Action OnPlayerLeftDoor;

        public static void Invoke()
        {
            OnPlayerCollidedWithDoor?.Invoke();
        }

        // Invoke when the player exits the door collider
        public static void InvokeLeft()
        {
            OnPlayerLeftDoor?.Invoke();
        }
    }    
}