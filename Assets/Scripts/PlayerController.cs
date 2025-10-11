using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float dashDistance = 3f;       
    public float dashDuration = 0.2f;     
    public float dashCooldown = 0.8f;
    public Animator anim;

    private bool isDashing = false;
    private bool canDash = true;
    private Vector3 moveInput;
    private Vector3 lastDirection;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing) return;

        // Movimiento normal
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        transform.position += moveInput * speed * Time.deltaTime;

        if (moveInput != Vector3.zero)
        {
            anim.SetBool("isMoving", true);
            lastDirection = moveInput;
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        // Voltear sprite
        Vector3 spriteOrientation = transform.localScale;
        if (moveInput.x > 0)
            spriteOrientation.x = Mathf.Abs(spriteOrientation.x);
        else if (moveInput.x < 0)
            spriteOrientation.x = -Mathf.Abs(spriteOrientation.x);
        transform.localScale = spriteOrientation;

        // Dash con Space
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        anim.SetTrigger("roll");

        Vector3 dashDirection = (moveInput == Vector3.zero) ? lastDirection : moveInput;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + dashDirection * dashDistance;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
