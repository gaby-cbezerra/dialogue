using TMPro;
using UnityEngine;


public class CoinUI : MonoBehaviour
{
    [SerializeField] private TMP_Text player1CoinText;

    [SerializeField] private TMP_Text player2CoinText;



    private void OnEnable()
    {
        PlayerObserverManager.OnPlayer1CoinsChanged += UpdatePlayer1;

        PlayerObserverManager.OnPlayer2CoinsChanged += UpdatePlayer2;
    }



    private void OnDisable()
    {
        PlayerObserverManager.OnPlayer1CoinsChanged -= UpdatePlayer1;

        PlayerObserverManager.OnPlayer2CoinsChanged -= UpdatePlayer2;
    }



    private void UpdatePlayer1(int value)
    {
        player1CoinText.text = "P1 Coins: " + value;
    }



    private void UpdatePlayer2(int value)
    {
        player2CoinText.text = "P2 Coins: " + value;
    }
}