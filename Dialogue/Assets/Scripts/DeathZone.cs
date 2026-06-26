using UnityEngine;


public class DeathZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        PlayerController player =
            other.GetComponent<PlayerController>();


        if(player == null)
            return;



        if(player.IsPlayer1())
        {
            RoundManager.Instance.Player2WonRound();
        }
        else
        {
            RoundManager.Instance.Player1WonRound();
        }


    }
}