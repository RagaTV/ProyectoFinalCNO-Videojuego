using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveInput = new Vector3(0f, 0f, 0f);
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
        transform.position+=moveInput*speed*Time.deltaTime;

        if(moveInput != Vector3.zero){
            anim.SetBool("isMoving", true);
        } else {
            anim.SetBool("isMoving", false);
        }

        Vector3 currentScale = transform.localScale;

        if (moveInput.x > 0)
        {
            currentScale.x = Mathf.Abs(currentScale.x); 
        }
        else if (moveInput.x < 0)
        {
            currentScale.x = -Mathf.Abs(currentScale.x);
        }
        transform.localScale = currentScale;

    }
}
