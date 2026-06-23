using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelExit : MonoBehaviour
{
    [Header("Success interface")]
    [SerializeField] private GameObject levelCompletePanel; // Główny panel trzymający UI
    [SerializeField] private Image blackScreenFader;        // Czarny obrazek na cały ekran

    [Header("Animation Components")]
    [SerializeField] private CanvasGroup levelCompletedTextCanvasGroup; // NOWE: Canvas Group wrzucony bezpośrednio na napis
    [SerializeField] private CanvasGroup buttonsCanvasGroup;           // Canvas Group wrzucony na przyciski

    [Header("Door Graphics")]
    [SerializeField] private Sprite openedDoorSprite;        // Grafika OTWARTYCH drzwi
    private SpriteRenderer doorSpriteRenderer;

    [Header("Fade Duration")]
    [SerializeField] private float fadeDuration = 3.0f;
    
    [Header("Text Delay Settings")]
    [SerializeField] private float textDelay = 1.0f;          // Po ilu sekundach napis ZACZYNA się pojawiać
    [SerializeField] private float textFadeDuration = 2.5f;     // Ile sekund ma się ściemniać ekran

    private bool playerIsClose = false;
    private bool isLevelCompleted = false;

    private void Start()
    {
        doorSpriteRenderer = GetComponent<SpriteRenderer>();

        // Na starcie gry ukrywamy czarny ekran
        if (blackScreenFader != null)
        {
            Color c = blackScreenFader.color;
            c.a = 0f;
            blackScreenFader.color = c;
            blackScreenFader.gameObject.SetActive(false);
        }

        // Ukrywamy napis poprzez Canvas Group
        if (levelCompletedTextCanvasGroup != null)
        {
            levelCompletedTextCanvasGroup.alpha = 0f;
        }

        // Ukrywamy przyciski
        if (buttonsCanvasGroup != null)
        {
            buttonsCanvasGroup.alpha = 0f;
            buttonsCanvasGroup.interactable = false;
            buttonsCanvasGroup.blocksRaycasts = false;
        }

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerIsClose && Input.GetKeyDown(KeyCode.E) && !isLevelCompleted)
        {
            OpenExit();
        }
    }

    private void OpenExit()
    {
        isLevelCompleted = true;

        if (doorSpriteRenderer != null && openedDoorSprite != null)
        {
            doorSpriteRenderer.sprite = openedDoorSprite;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static; 
            }

            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.DisableControls();
            }
        }

        StartCoroutine(FadeToBlackRoutine());
    }

    private IEnumerator FadeToBlackRoutine()
    {
        // Włączamy panel i czarne tło
        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
        if (blackScreenFader != null) blackScreenFader.gameObject.SetActive(true);

        // Na starcie napis jest całkowicie przezroczysty
        if (levelCompletedTextCanvasGroup != null) levelCompletedTextCanvasGroup.alpha = 0f;

        float timer = 0f;

        // ETAP 1: Ściemnianie czarnego tła (trwa tyle, ile wynosi 'fadeDuration')
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            if (blackScreenFader != null)
            {
                Color canvasColor = blackScreenFader.color;
                canvasColor.a = Mathf.Lerp(0f, 1f, progress);
                blackScreenFader.color = canvasColor;
            }

            // W trakcie tego ściemniania sprawdzamy też, czy tekst już powinien zacząć się pojawiać
            if (timer >= textDelay && levelCompletedTextCanvasGroup != null)
            {
                float textProgress = (timer - textDelay) / textFadeDuration;
                levelCompletedTextCanvasGroup.alpha = Mathf.Clamp01(textProgress);
            }

            yield return null;
        }

        // ETAP 2: Ekran jest już całkiem czarny, włączamy przyciski, 
        // ale pętla ODMIERZA CZAS DALEJ, żeby napis mógł dokończyć swoje pojawianie się!
        float buttonsTimer = 0f;
        float buttonsFadeDuration = 1.0f; // Przyciski pojawiają się przez 1 sekundę

        if (buttonsCanvasGroup != null)
        {
            buttonsCanvasGroup.interactable = true;
            buttonsCanvasGroup.blocksRaycasts = true;
        }

        // Ta pętla animuje pojawianie się przycisków ORAZ pozwala tekstowi dalej się rozjaśniać
        while (buttonsTimer < buttonsFadeDuration || (levelCompletedTextCanvasGroup != null && levelCompletedTextCanvasGroup.alpha < 1f))
        {
            float dt = Time.deltaTime;
            timer += dt;        // Całkowity czas od początku sekwencji
            buttonsTimer += dt; // Czas od momentu włączenia przycisków

            // Animacja przycisków
            if (buttonsCanvasGroup != null)
            {
                buttonsCanvasGroup.alpha = Mathf.Clamp01(buttonsTimer / buttonsFadeDuration);
            }

            // Dokończenie animacji tekstu (jeśli jeszcze się nie skończyła)
            if (timer >= textDelay && levelCompletedTextCanvasGroup != null)
            {
                float textProgress = (timer - textDelay) / textFadeDuration;
                levelCompletedTextCanvasGroup.alpha = Mathf.Clamp01(textProgress);
            }

            yield return null;
        }

        // Pokazujemy kursor myszki na samym końcu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsClose = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsClose = false;
    }
}