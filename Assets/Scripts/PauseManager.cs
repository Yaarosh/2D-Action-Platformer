using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;

    private bool isPaused = false;

    void Update()
    {
        // Sprawdzamy, czy gracz wcisnął Escape lub P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true); // Pokazuje przyciemnienie i przyciski
        
        // ZATRZYMANIE CZASU W GRZE:
        Time.timeScale = 0f; 

        // Odblokowanie myszki
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        
        Time.timeScale = 1f; 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // PAMIĘTAJ: Zawsze przywracaj czas przed przeładowaniem sceny!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // PAMIĘTAJ: Zawsze przywracaj czas przed przeładowaniem sceny!
        SceneManager.LoadScene("MainMenu"); // Wpisz tu dokładną nazwę swojej sceny z menu
    }
}