using TMPro;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    public static RoundUI Instance;


    [Header("Rounds UI")]
    [SerializeField] private TMP_Text roundTextP1;
    [SerializeField] private TMP_Text roundTextP2;



    private void Awake()
    {
        Instance = this;
    }



    public void UpdateRounds(int p1, int p2)
    {
        roundTextP1.text = "P1: " + p1;
        roundTextP2.text = "P2: " + p2;
    }
}