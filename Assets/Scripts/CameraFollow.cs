using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // Nasz gracz, którego kamera ma śledzić
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Podstawowe przesunięcie (Z musi być -10!)

    [Header("Smooth Settings")]
    [SerializeField] private float smoothTimeX = 0.08f; // Bardzo szybkie doganianie w poziomie (żeby kamera nie zostawała w tyle)
    [SerializeField] private float smoothTimeY = 0.25f; // Miękkie, wolniejsze doganianie w pionie (przy skokach w górę)

    [Header("Look Ahead")]
    [SerializeField] private float lookAheadDistance = 3f; // Jak daleko przed gracza ma wysuwać się kamera
    [SerializeField] private float lookAheadSpeed = 6f;    // Jak szybko kamera ma przejeżdżać z lewej na prawą stronę gracza

    [Header("Position Restrictions (Clamping)")]
    [SerializeField] private bool useMinY = true;          // Czy blokada dolna ma być włączona
    [SerializeField] private float minYPosition = 0f;      // Najniższy punkt w pionie
    [SerializeField] private bool useMaxY = false;         // Czy blokada górna ma być włączona
    [SerializeField] private float maxYPosition = 10f;     // Najwyższy punkt w pionie

    [System.Serializable] private struct Dummy {} // Czyszczenie formatowania w inspektorze

    [SerializeField] private bool useMinX = true;          // Czy blokada na lewo ma być włączona
    [SerializeField] private float minXPosition = 0f;      // Najbardziej wysunięty punkt na lewo
    [SerializeField] private bool useMaxX = false;         // NOWE: Czy blokada na prawo ma być włączona
    [SerializeField] private float maxXPosition = 50f;     // NOWE: Najbardziej wysunięty punkt na prawo (koło drzwi)

    // Prywatne zmienne techniczne do obsługi płynności i fizyki
    private float currentXOffset;
    private float xVelocity = 0f;
    private float yVelocity = 0f;
    private Rigidbody2D targetRb; // Komponent fizyki gracza do sprawdzania prędkości spadania

    private void Start()
    {
        currentXOffset = offset.x;

        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. OŚ X: Kierunek patrzenia
        float targetXOffset = target.localScale.x < 0 ? -lookAheadDistance : lookAheadDistance;
        currentXOffset = Mathf.MoveTowards(currentXOffset, targetXOffset, lookAheadSpeed * Time.deltaTime);
        float targetX = target.position.x + currentXOffset;

        // 2. DYNAMICZNA OŚ Y
        float currentSmoothTimeY = smoothTimeY; 
        if (targetRb != null && targetRb.linearVelocity.y < -1f) 
        {
            currentSmoothTimeY = 0.05f; 
        }
        float targetY = target.position.y + offset.y;

        // 3. FINALE: Matematyczne wygładzamy ruch
        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref xVelocity, smoothTimeX);
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref yVelocity, currentSmoothTimeY);

        // Aplikacja blokad (Clamping)
        if (useMinY) newY = Mathf.Max(newY, minYPosition);
        if (useMaxY) newY = Mathf.Min(newY, maxYPosition); // Blokada sufitu
        
        if (useMinX) newX = Mathf.Max(newX, minXPosition);
        if (useMaxX) newX = Mathf.Min(newX, maxXPosition); // NOWE: Blokada ściany przy drzwiach

        // Przypisujemy nową pozycję do kamery
        transform.position = new Vector3(newX, newY, offset.z);
    }
}