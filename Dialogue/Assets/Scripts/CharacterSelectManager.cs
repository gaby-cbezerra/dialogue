using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Ball Data")]
    [SerializeField] private BallData[] balls;

    [Header("Player 1 UI")]
    [SerializeField] private TMP_Text p1Name;
    [SerializeField] private TMP_Text p1ReadyText;
    [SerializeField] private Image p1Preview;

    [Header("Player 2 UI")]
    [SerializeField] private TMP_Text p2Name;
    [SerializeField] private TMP_Text p2ReadyText;
    [SerializeField] private Image p2Preview;

    private int p1Index = 0;
    private int p2Index = 0;

    private bool p1Ready = false;
    private bool p2Ready = false;

    private void Start()
    {
        UpdateUI();

        p1ReadyText.text = "";
        p2ReadyText.text = "";
    }

    private void Update()
    {
        // PLAYER 1

        if (!p1Ready)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                p1Index--;
                if (p1Index < 0)
                    p1Index = balls.Length - 1;

                UpdateUI();
            }

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                p1Index++;
                if (p1Index >= balls.Length)
                    p1Index = 0;

                UpdateUI();
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                p1Ready = true;
                p1ReadyText.text = "READY!";
            }
        }

        // PLAYER 2

        if (!p2Ready)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                p2Index--;
                if (p2Index < 0)
                    p2Index = balls.Length - 1;

                UpdateUI();
            }

            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                p2Index++;
                if (p2Index >= balls.Length)
                    p2Index = 0;

                UpdateUI();
            }

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                p2Ready = true;
                p2ReadyText.text = "READY!";
            }
        }

        // Ambos confirmaram

        if (p1Ready && p2Ready)
        {
            GameManager.Instance.StartMatch();

            // Aqui depois vamos carregar a Gameplay
        }
    }

    private void UpdateUI()
    {
        p1Name.text = balls[p1Index].ballName;
        p2Name.text = balls[p2Index].ballName;

        p1Preview.sprite = balls[p1Index].previewSprite;
        p2Preview.sprite = balls[p2Index].previewSprite;
    }
}