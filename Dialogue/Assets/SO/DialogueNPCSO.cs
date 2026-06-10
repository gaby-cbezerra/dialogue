using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue NPC", menuName = "MyGame/Data/New Dialogue NPC")]
public class DialogueNPCSO : ScriptableObject
{
    public string npcName;
    public Sprite npcImage;
    public Color npcColor;
    public List<string> dialogueLines;
}