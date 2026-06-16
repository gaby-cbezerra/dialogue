using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnStartButton()
    {
        GameManager.Instance.GoToCharacterSelect();
    }

    public void OnQuitButton()
    {
        GameManager.Instance.QuitGame();
    }
}