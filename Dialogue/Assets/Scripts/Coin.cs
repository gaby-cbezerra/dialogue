using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Procura PlayerController
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.AddCoin(1);
            }

            Destroy(gameObject);
        }
    }
}