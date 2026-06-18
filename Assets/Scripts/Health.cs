using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHealth;
    /*private*/ public float currentHealth;
    [SerializeField] private HealthBar healthBar;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            TakeDamage(10, transform.position);
    }

    public void TakeDamage(float value, Vector2 damageSourcePosition)
    {
        if (isDead) 
            return;

        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit!");
            
            PlayerMovement movement = GetComponent<PlayerMovement>();
            if (movement != null)
            {
                StartCoroutine(movement.TriggerKnockback(damageSourcePosition));
            }
        }

        currentHealth -= value;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("YOU DIED");
            
            GetComponent<PlayerMovement>().enabled = false;
            
            Collider2D playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.enabled = false;
            }

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = false;
            }

            Animator anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("die");
            }

            DeathManager.Instance.StartDeathSequence();
        }
        else
        {
            Debug.Log(gameObject.name + " is dead!");
            Destroy(gameObject);
        }
    }
}