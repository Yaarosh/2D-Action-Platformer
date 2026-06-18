using System.Collections; 
using UnityEngine; 
using UnityEngine.SceneManagement; 
using TMPro; 

public class DeathManager : MonoBehaviour 
{ 
    public static DeathManager Instance; 

    [Header("UI References")] 
    [SerializeField] private CanvasGroup faderCanvasGroup; 
    [SerializeField] private TextMeshProUGUI youDiedText; 
    [SerializeField] private CanvasGroup buttonsCanvasGroup; 
    [SerializeField] private string menuSceneName = "MainMenu"; 
    [SerializeField] private float fadeDuration = 3.0f; 

    private void Awake() 
    { 
        if (Instance == null) 
        { 
            Instance = this; 
        } 
        else 
        { 
            Destroy(gameObject); 
            return; 
        } 

        if (faderCanvasGroup != null) 
            faderCanvasGroup.alpha = 0f; 
        
        if (youDiedText != null)
        {
            youDiedText.color = new Color(youDiedText.color.r, youDiedText.color.g, youDiedText.color.b, 0f); 
            youDiedText.transform.localScale = new Vector3(0.8f, 0.8f, 1f); 
        }

        // --- Ukrywamy przyciski na samym starcie gry ---
        if (buttonsCanvasGroup != null)
        {
            buttonsCanvasGroup.alpha = 0f;
            buttonsCanvasGroup.interactable = false;
            buttonsCanvasGroup.blocksRaycasts = false;
        }
    } 

    public void StartDeathSequence() 
    { 
        StartCoroutine(DeathSequenceRoutine());
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    } 

    private IEnumerator DeathSequenceRoutine() 
    { 
        float timer = 0f; 
        Vector3 initialScale = new Vector3(0.8f, 0.8f, 1f); 
        Vector3 targetScale = new Vector3(1.1f, 1.1f, 1f); 

        // 1. i 2. Ściemnianie tła i pojawianie się napisu YOU DIED
        while (timer < fadeDuration) 
        { 
            timer += Time.deltaTime; 
            float progress = timer / fadeDuration; 

            faderCanvasGroup.alpha = Mathf.Lerp(0f, 1f, progress); 

            Color txtColor = youDiedText.color; 
            txtColor.a = Mathf.Lerp(0f, 1f, progress); 
            youDiedText.color = txtColor; 
            youDiedText.transform.localScale = Vector3.Lerp(initialScale, targetScale, progress); 

            yield return null; 
        } 

        timer = 0f;
        float buttonsFadeDuration = 1.0f; // Przyciski będą się pojawiać przez 1 sekundę

        if (buttonsCanvasGroup != null)
        {
            buttonsCanvasGroup.interactable = true; // Pozwalamy na klikanie
            buttonsCanvasGroup.blocksRaycasts = true; // Pozwalamy myszce na najeżdżanie

            while (timer < buttonsFadeDuration)
            {
                timer += Time.deltaTime;
                buttonsCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / buttonsFadeDuration);
                yield return null;
            }
        }
    } 

    public void RestartLevel() 
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
    public void GoToMenu() 
    { 
        SceneManager.LoadScene(menuSceneName); 
    }
}