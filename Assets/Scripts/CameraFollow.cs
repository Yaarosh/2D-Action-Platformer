using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // Nasz gracz, którego kamera ma śledzić
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Podstawowe przesunięcie (Z musi być -10!)

    [Header("Smooth Settings")]
    [SerializeField] private float smoothTimeX = 0.08f; // Bardzo szybkie doganianie w poziomie (żeby kamera nie zostawała w tyle)
    [SerializeField] private float smoothTimeY = 0.25f; // Miękkie, wolniejsze doganianie w pionie (przy skokach w górę)

    [Header("Look Ahead (Kierunek biegu)")]
    [SerializeField] private float lookAheadDistance = 3f; // Jak daleko przed gracza ma wysuwać się kamera
    [SerializeField] private float lookAheadSpeed = 6f;    // Jak szybko kamera ma przejeżdżać z lewej na prawą stronę gracza

    [Header("Ograniczenia Pozycji (Clamping)")]
    [SerializeField] private bool useMinY = true;          // Czy blokada dolna ma być włączona
    [SerializeField] private float minYPosition = 0f;      // najniższy punkt
    [SerializeField] private bool useMinX = true;          // Czy blokada na lewo ma być włączona
    [SerializeField] private float minXPosition = 0f;      // najniższy punkt

    // Prywatne zmienne techniczne do obsługi płynności i fizyki
    private float currentXOffset;
    private float xVelocity = 0f;
    private float yVelocity = 0f;
    private Rigidbody2D targetRb; // Komponent fizyki gracza do sprawdzania prędkości spadania

    private void Start()
    {
        // Na starcie ustawiamy początkowy offset w osi X
        currentXOffset = offset.x;

        // Automatycznie pobieramy Rigidbody2D z naszego gracza
        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
        }
    }

    private void LateUpdate()
    {
        // Bezpiecznik: jeśli gracz nie jest przypisany, nic nie rób
        if (target == null) return;

        // 1. OŚ X: Sprawdzamy kierunek: jeśli lokalna skala X gracza jest ujemna, patrzy w lewo (-3), jeśli dodatnia – w prawo (3)
        float targetXOffset = target.localScale.x < 0 ? -lookAheadDistance : lookAheadDistance;

        // Płynnie przesuwamy sam offset w osi X, żeby nie było szarpnięcia przy zmianie kierunku
        currentXOffset = Mathf.MoveTowards(currentXOffset, targetXOffset, lookAheadSpeed * Time.deltaTime);
        float targetX = target.position.x + currentXOffset;


        // 2. DYNAMICZNA OŚ Y: Domyślnie używamy miękkiego czasu (0.25s)
        float currentSmoothTimeY = smoothTimeY; 

        // Jeśli gracz spada w dół (jego prędkość pionowa jest ujemna), kamera przełącza się na tryb agresywny (0.05s)
        if (targetRb != null && targetRb.linearVelocity.y < -1f) 
        {
            currentSmoothTimeY = 0.05f; 
        }

        float targetY = target.position.y + offset.y;


        // 3. FINALE: Matematycznie wygładzamy ruch osobno dla X i dla Y
        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref xVelocity, smoothTimeX);
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref yVelocity, currentSmoothTimeY);

        if (useMinY)
        {
            // Zabezpieczamy 'newY' – jeśli wyliczona pozycja jest mniejsza niż minYPosition, weźmie minYPosition
            newY = Mathf.Max(newY, minYPosition);
        }
        if (useMinX)
        {
            // Zabezpieczamy 'newX' – jeśli wyliczona pozycja jest mniejsza niż minXPosition, weźmie minXPosition
            newX = Mathf.Max(newX, minXPosition);
        }

        // Przypisujemy nową pozycję do kamery (oś Z zostaje bez zmian z offsetu)
        transform.position = new Vector3(newX, newY, offset.z);
    }
}