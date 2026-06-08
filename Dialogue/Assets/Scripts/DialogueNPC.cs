using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    [SerializeField] private DialogueNPCSO dialogueNpcSo;
    
    public string NPCName => dialogueNpcSo.npcName;
    public Sprite NPCImage => dialogueNpcSo.npcImage;
    public Color NPCColor => dialogueNpcSo.npcColor;
    public List<string> dialogueLines;

}
