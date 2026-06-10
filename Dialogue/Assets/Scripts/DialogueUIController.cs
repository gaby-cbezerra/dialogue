using TMPro;
using UnityEngine;

public class DialogueUIController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private SpriteRenderer portraitImage;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void SetName(string name)
    {
        nameText.text = npcName;
    }

    private void SetPortrait(Sprite npc)
    {
        
    }
}
