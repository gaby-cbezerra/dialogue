using System;

public static class PlayerObserverManager
{
    // Player 1
    public static Action<int> OnPlayer1CoinsChanged;

    // Player 2
    public static Action<int> OnPlayer2CoinsChanged;



    public static void UpdatePlayer1Coins(int amount)
    {
        OnPlayer1CoinsChanged?.Invoke(amount);
    }


    public static void UpdatePlayer2Coins(int amount)
    {
        OnPlayer2CoinsChanged?.Invoke(amount);
    }
}