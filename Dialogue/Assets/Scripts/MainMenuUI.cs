using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnStartButton()
    {
        GameManager.Instance.StartGame();
    }

    public void OnQuitButton()
    {
        GameManager.Instance.QuitGame();
    }
}