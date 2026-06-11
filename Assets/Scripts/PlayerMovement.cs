using NUnit.Framework;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Vector3 baseScale;
    private Animator anim;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float originalGravity;
    private float horizontalInput;
    private bool isAttacking = false;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask enemyLayer;

    private void Awake()
    {
        // Get base scale from unity
        baseScale = transform.localScale;
        // Grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalGravity = body.gravityScale;
    }

    private void Update()
    {

        if(isAttacking)
        {
            if(isGrounded())
                body.linearVelocity = new Vector2(0, body.linearVelocity.y);
            return;
        }

        if(Input.GetKeyDown(KeyCode.Q) && !isAttacking && !onWall() && wallJumpCooldown > 0.2f)
        {
            Attack();
        }


        horizontalInput = Input.GetAxis("Horizontal");

        //Flipping player when moving left and right
        if(horizontalInput > 0.01f)
            transform.localScale = baseScale;
        else if(horizontalInput < -0.01f)
            transform.localScale = new Vector3(-baseScale.x, baseScale.y, baseScale.z);

        // set animator parameters
        anim.SetBool("run", horizontalInput !=0);
        anim.SetBool("grounded", isGrounded());

        

            //Wall jump logic
        if (wallJumpCooldown > 0.2f)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            bool pressingAgainstWall = (horizontalInput > 0.01f && transform.localScale.x > 0) || 
                                    (horizontalInput < -0.01f && transform.localScale.x < 0);

            if (onWall() && !isGrounded() && pressingAgainstWall)
            {
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = originalGravity;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }
    private void Jump()
    {
        if(isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
        else if(onWall() && !isGrounded())
        {
            if(horizontalInput == 0)
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);           
            }
            else
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            
            wallJumpCooldown = 0;
            
        }

    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
    private void Attack()
    {
        isAttacking = true;
        anim.SetTrigger("attack");
    }
    private void PerformHit()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Health>().TakeDamage(10);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider !=null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x,0), 0.1f, wallLayer);
        return raycastHit.collider !=null;
    }

    private void OnDrawGizmosSelected()
    {   
        // Sprawdzamy, czy w ogóle przypisaliśmy punkt ataku, żeby Unity nie sypało błędami
        if (attackPoint == null) return;

        // Ustawiamy kolor naszej wizualizacji (np. czerwony)
        Gizmos.color = Color.red;

        // Rysujemy druciane koło w pozycji punktu ataku o wielkości naszego zasięgu
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
