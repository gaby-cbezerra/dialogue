using TMPro;
using UnityEngine;

public class VictoryManager : MonoBehaviour
{

    public static VictoryManager Instance;


    [SerializeField]
    private TMP_Text winnerText;


    [SerializeField]
    private TMP_Text ballText;



    private void Awake()
    {
        Instance = this;
    }



    private void Start()
    {
        ShowWinner();
    }



    public void ShowWinner()
    {

        if(RoundManager.Instance.player1Wins >= 2)
        {

            winnerText.text =
                "Jogador 1 venceu!";


            ballText.text =
                "Bola: " + 
                PlayerPrefs.GetString("Player1Ball");


        }

        else
        {

            winnerText.text =
                "Jogador 2 venceu!";


            ballText.text =
                "Bola: " +
                PlayerPrefs.GetString("Player2Ball");

        }

    }


    public void BackToSelect()
    {
        GameManager.Instance.GoToCharacterSelect();
    }

}