using UnityEngine;

[CreateAssetMenu(fileName = "BallData", menuName = "Sumo/Ball Data")]
public class BallData : ScriptableObject
{
    public string ballName;

    public Material material;

    public float speed;
    public float pushPower;
    public float weight;
    public float size;
}