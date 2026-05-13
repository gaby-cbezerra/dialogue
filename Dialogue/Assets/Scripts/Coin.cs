using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Notifica o Observer Manager
            PlayerObserverManager.CoinCollected(1);

            // Destroi moeda
            Destroy(gameObject);
        }
    }
}