using System;
using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    [SerializeField] private DialogueNPCSO dialogueNpcSo;
    
    public string NPCName => dialogueNpcSo.npcName;
    public Sprite NPCImage => dialogueNpcSo.npcImage;
    public Color NPCColor => dialogueNpcSo.npcColor;
    public string[] DialogueLines => dialogueNpcSo.dialogueLines.ToArray();
    
    private bool isInteractable;

    private void Start()
    {
        GetComponent<MeshRenderer>().material.color = NPCColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isInteractable)
        {
            InteractOM.OnInteract += ShowDialogue;
            isInteractable = true;
            InteractOM.ShowInteraction(isInteractable);
            InteractOM.PositionChange(transform.position);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && isInteractable)
        {
            InteractOM.OnInteract -= ShowDialogue;
            isInteractable = false;
            InteractOM.ShowInteraction(isInteractable);
        }
    }

    private void ShowDialogue()
    {
        Debug.Log(NPCName+": "+DialogueLines[0]);
    }
}
