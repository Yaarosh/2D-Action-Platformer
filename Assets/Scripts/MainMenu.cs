using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string firstLevelName = "Level1"; // Wpisz tu nazwę swojej pierwszej sceny z grą
    [SerializeField] private string levelSelectSceneName = "LevelSelect"; // Opcjonalna scena wyboru poziomów

    public void StartGame()
    {
        // Ładuje pierwszy poziom gry
        SceneManager.LoadScene(firstLevelName);
    }

    public void OpenLevelSelect()
    {
        // Tutaj w przyszłości załadujemy scenę wyboru poziomów
        Debug.Log("LevelSelecting...");
        // SceneManager.LoadScene(levelSelectSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("GameTurnedOff");
        Application.Quit(); // Zamyka zbudowaną grę (.exe)
    }
}