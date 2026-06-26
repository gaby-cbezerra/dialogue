using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;


    [Header("Rounds")]
    public int player1Wins = 0;
    public int player2Wins = 0;


    [Header("Players")]
    public Rigidbody player1;
    public Rigidbody player2;


    private Vector3 player1Start;
    private Vector3 player2Start;



    private void Awake()
    {
        Instance = this;
    }



    private void Start()
    {
        player1Start = player1.position;
        player2Start = player2.position;
    }



    public void Player1WonRound()
    {
        player1Wins++;


        RoundUI.Instance.UpdateRounds(
            player1Wins,
            player2Wins
        );


        CheckWinner();
    }


    public void Player2WonRound()
    {
        player2Wins++;


        RoundUI.Instance.UpdateRounds(
            player1Wins,
            player2Wins
        );


        CheckWinner();
    }




    private void CheckWinner()
    {

        if(player1Wins >= 2)
        {
            GameManager.Instance.GoToVictory();
            return;
        }


        if(player2Wins >= 2)
        {
            GameManager.Instance.GoToVictory();
            return;
        }


        ResetRound();
    }



    private void ResetRound()
    {

        player1.position = player1Start;
        player2.position = player2Start;


        player1.linearVelocity = Vector3.zero;
        player2.linearVelocity = Vector3.zero;


        Debug.Log("Novo round");
    }
}