using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private int currentCoins = 0;

    private void OnEnable()
    {
        PlayerObserverManager.OnCoinCollected += UpdateCoins;
    }

    private void OnDisable()
    {
        PlayerObserverManager.OnCoinCollected -= UpdateCoins;
    }

    private void UpdateCoins(int amount)
    {
        currentCoins += amount;
        
        Debug.Log("Moeda recebida: " + currentCoins);
        
        coinText.text = "Coins: " + currentCoins;
    }
}