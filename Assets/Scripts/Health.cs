using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHealth;
    /*private*/ public float currentHealth;
    [SerializeField] private HealthBar healthBar;

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
        //if (Input.GetKeyDown(KeyCode.T))
            //TakeDamage(10);
    }

    public void TakeDamage(float value, Vector2 damageSourcePosition)
    {
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit!");
            
            PlayerMovement movement = GetComponent<PlayerMovement>();
            
            if (movement != null)
            {
                StartCoroutine(movement.TriggerKnockback(damageSourcePosition));
            }
        }
        currentHealth-=value;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if(currentHealth <=0)
        {
            if(gameObject.CompareTag("Player"))
            {
                Debug.Log("YOU DIED");
                //Destroy(gameObject);
            }
            else
            {
                Debug.Log(gameObject.name + " is dead!");
                Destroy(gameObject);
            }
        }
    }
}
