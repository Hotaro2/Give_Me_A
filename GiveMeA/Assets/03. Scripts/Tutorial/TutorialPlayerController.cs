using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerController : MonoBehaviour
{
    public ObjectPool objectPoolLogic;

    public Rigidbody2D rb;
    private float speed = 10f;
    private float jumpForce = 15f;
    private float moveInput;

    private bool isGrounded;
    public Transform feetPos;
    private float checkRadius = 0.3f;
    public LayerMask whatIsGround;

    private float jumpTimeCounter;
    private float jumpTime = 0.2f;
    private bool isJumping;

    public Animator anim;
    //Dash
    private bool isDashing;
    private float defaultSpeed;

    public Transform FirePos;
    SpriteRenderer rend;

    public int controllLevel = 0;
    public bool breakMove = false;
    public bool useDash = false;



    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.flipX = false;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        defaultSpeed = speed;
    }

    void FixedUpdate()
    {
        if (controllLevel == 0)
            return;
        if (breakMove)
            return;


        if (isDashing)
            return;

        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * defaultSpeed, rb.velocity.y); //움직이기
    }


    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        Look();
        Jump();
    }

    private void Look()
    {
        //플레이어 바라보는 방향 바꾸기
        if (controllLevel == 0)
            return;
        if (breakMove)
            return;

        if (moveInput == 0)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }

        if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    private void Jump()
    {
        //점프
        if (isGrounded == true && Input.GetKeyDown(KeyCode.X))
        {
            anim.SetTrigger("takeOf");
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if (isGrounded == true)
        {
            anim.SetBool("isJumping", false);
        }
        else
        {
            anim.SetBool("isJumping", true);
        }

        if (Input.GetKey(KeyCode.X) && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            isJumping = false;
        }
    }


}

