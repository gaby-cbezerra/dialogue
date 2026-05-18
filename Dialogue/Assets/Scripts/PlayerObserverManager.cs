using System;

public static class PlayerObserverManager
{
    // Valor atual de moedas
    private static int currentCoins = 0;

    // Evento
    public static Action<int> OnCoinCollected;

    // Método chamado ao coletar moeda
    public static void CoinCollected(int amount)
    {
        currentCoins += amount;

        OnCoinCollected?.Invoke(currentCoins);
    }
}