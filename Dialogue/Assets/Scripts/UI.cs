using TMPro;
using UnityEngine;

namespace Core
{
    public class UIGameplayManager : MonoBehaviour
    {
        public static  UIGameplayManager Instance;
        public GameObject buttonGameObject;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            Instance = this;
            // ensure button starts hidden to avoid showing incorrectly
            if (buttonGameObject != null)
                buttonGameObject.SetActive(false);
        }

        void OnEnable()
        {
            GameEventSystem.OnPlayerCollidedWithDoor += ShowButton;
            GameEventSystem.OnPlayerLeftDoor += HideButton;
            DoorController.OnOpenedOrLocked += HandleOpenOrLockDoor;
        }

        void OnDisable()
        {
            GameEventSystem.OnPlayerCollidedWithDoor -= ShowButton;
            GameEventSystem.OnPlayerLeftDoor -= HideButton;
            DoorController.OnOpenedOrLocked -= HandleOpenOrLockDoor;
        }

        public void ShowButton()
        {
            if (buttonGameObject == null)
            {
                Debug.LogWarning("UIGameplayManager: buttonGameObject is not assigned.");
                return;
            }

            buttonGameObject.SetActive(true);
        }

        public void HideButton()
        {
            if (buttonGameObject == null) return;
            buttonGameObject.SetActive(false);
        }

        void HandleOpenOrLockDoor(bool ctx)
        {
            if (buttonGameObject == null)
            {
                Debug.LogWarning("UIGameplayManager: buttonGameObject not set when updating text.");
                return;
            }

            var txtMesh = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (txtMesh == null)
            {
                Debug.LogWarning("UIGameplayManager: No TextMeshProUGUI found under buttonGameObject.");
                return;
            }

            txtMesh.text = ctx ? "Close" : "Open";
        }
    }
}