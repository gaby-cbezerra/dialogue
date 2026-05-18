using System;

public static class PlayerObserverManager
{
    // Evento da UI
    public static Action<int> OnCoinCollected;

    // Apenas comunica o valor atual
    public static void UpdateCoins(int currentCoins)
    {
        OnCoinCollected?.Invoke(currentCoins);
    }
}