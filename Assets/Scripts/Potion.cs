using UnityEngine;

public class Potion : MonoBehaviour
{
    [Header("Ustawienia Leczenia")]
    [SerializeField] private int healAmount = 20;

    private bool playerIsClose = false;
    private GameObject playerObject;

    private void Update()
    {
        if (playerIsClose && Input.GetKeyDown(KeyCode.E))
        {
            CollectPotion();
        }
    }

    private void CollectPotion()
    {
        if (playerObject != null)
        {
            Health playerHealth = playerObject.GetComponent<Health>();
            
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount); 
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            playerObject = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            playerObject = null;
        }
    }
}