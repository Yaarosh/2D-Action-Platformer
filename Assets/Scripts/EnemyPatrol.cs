using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header ("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header ("Movement Parameters")]
    [SerializeField] private float speed;
    private Vector3 currentTarget;

    private void Start()
    {
        // Na starcie wróg idzie w stronę punktu A
        currentTarget = pointA.position;
    }

    private void Update()
    {
        // Ruch w stronę aktualnego celu (tylko w osi X, żeby nie zapadał się pod ziemię)
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentTarget.x, transform.position.y, transform.position.z), speed * Time.deltaTime);

        // Sprawdzamy, czy wróg dotarł bardzo blisko celu
        if (Vector2.Distance(transform.transform.position, new Vector2(currentTarget.x, transform.position.y)) < 0.1f)
        {
            // Zmiana celu i obrót postaci
            if (currentTarget == pointA.position)
            {
                currentTarget = pointB.position;
                Flip(1); // Obróć w prawo
            }
            else
            {
                currentTarget = pointA.position;
                Flip(-1); // Obróć w lewo
            }
        }
    }

    private void Flip(float direction)
    {
        // Zmieniamy lokalną skalę wroga na podstawie kierunku (tak jak u Twojego gracza!)
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10, transform.position);

                if (playerHealth.currentHealth != 0)
                    Debug.Log("liczba pozostalych punktow zycia: " + playerHealth.currentHealth);
            }
        }
    }
}