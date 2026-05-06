using UnityEngine;
using System;

public static class InteractOM
{
    public static event Action OnInteract;

    public static void Interact()
    {
        OnInteract?.Invoke();
    }
    
}
