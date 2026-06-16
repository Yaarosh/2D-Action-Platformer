using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHealth;
    /*private*/ public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;    
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
