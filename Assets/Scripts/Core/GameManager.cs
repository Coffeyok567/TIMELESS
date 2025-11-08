using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CharacterManager characterManager;
    //public UIManager uiManager;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeGame();
    }

    void InitializeGame()
    {
        // TO DO: всякие всеигровые проверки
    }
}