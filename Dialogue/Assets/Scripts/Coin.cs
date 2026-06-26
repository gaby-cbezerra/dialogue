using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool collected = false;


    private void OnTriggerEnter(Collider other)
    {
        if(collected)
            return;


        if(other.CompareTag("Player"))
        {
            collected = true;


            PlayerController player =
                other.GetComponent<PlayerController>();


            if(player != null)
            {
                player.AddCoin(1);
            }


            Destroy(gameObject);
        }
    }
}