using UnityEngine;
using UnityEngine.UI;

public class ForceBarUI : MonoBehaviour
{
    public static ForceBarUI Instance;


    [Header("Player Bars")]
    [SerializeField] private Image player1Fill;
    [SerializeField] private Image player2Fill;


    private void Awake()
    {
        Instance = this;
    }


    public void UpdatePlayer1Bar(float value)
    {
        player1Fill.fillAmount = value;
    }


    public void UpdatePlayer2Bar(float value)
    {
        player2Fill.fillAmount = value;
    }
}