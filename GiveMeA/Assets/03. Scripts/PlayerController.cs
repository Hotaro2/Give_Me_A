using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public ObjectPool objectPoolLogic;

    public int reversDir = 1;

    private Rigidbody2D rb;
    private float speed = 10f;
    private float jumpForce = 12f;
    private float moveInput;

    private bool isGrounded;
    public Transform feetPos;
    private float checkRadius = 0.3f;
    public LayerMask whatIsGround;

    private float jumpTimeCounter;
    private float jumpTime = 0.2f;
    private bool isJumping;

    private Animator anim;
    //Dash
    private float dashingVelocity = 50f;
    private float dashingTime = 0.1f;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool canDash = true;
    private float defaultSpeed;

    public Transform FirePos;
    SpriteRenderer rend;

    public bool isAttackOK = true;

    private int curHealth = 10;
    private int maxHealth = 10;

    public Image healthBar;

    private bool godMode = false;

    public void PlayerHit(int dmg)
    {
        if (curHealth <= 0)
            return;
        if (isDashing == true)
            return;
        if (godMode == true)
            return;

        curHealth -= dmg;
        healthBar.fillAmount = curHealth / (float)maxHealth;

        godMode = true;
        StartCoroutine(Pain());
    }

    IEnumerator Pain()
    {
        rend.color = new Color(255 / 255f, 177 / 255f, 177 / 255f); // 연한 빨간색
        Camera.main.transform.DOShakePosition(0.4f, 0.2f); // 카메라 셰이크
        yield return new WaitForSeconds(0.3f);
        rend.color = new Color(255 / 255f, 255 / 255f, 255 / 255f); // 흰색
        Color playerColor = rend.color;
        playerColor.a = 150 / 255f; // 반투명
        rend.color = playerColor;
        yield return new WaitForSeconds(0.5f);
        playerColor.a = 255 / 255f; // 불투명
        rend.color = playerColor;
        yield return new WaitForSeconds(0.5f);
        playerColor.a = 150 / 255f; // 반투명
        rend.color = playerColor;
        yield return new WaitForSeconds(0.5f);
        playerColor.a = 255 / 255f; // 불투명
        rend.color = playerColor;
        yield return new WaitForSeconds(0.5f);
        playerColor.a = 150 / 255f; // 반투명
        rend.color = playerColor;
        yield return new WaitForSeconds(0.5f);
        playerColor.a = 255 / 255f; // 불투명
        rend.color = playerColor;
        godMode = false;
    }

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
        if (isDashing)
            return;
        moveInput = Input.GetAxisRaw("Horizontal") * reversDir;
        rb.velocity = new Vector2(moveInput * defaultSpeed, rb.velocity.y); //움직이기
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        isDashing = false;
    }

    void Update()
    {
        Look();
        Dash();
        Jump();
        Fire();
        DownDash();
    }

    void Look()
    {
        //플레이어 바라보는 방향 바꾸기
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
    void Dash()
    {
        //Dash
        var dashInput = Input.GetKeyDown(KeyCode.Z);
        var inputX = Input.GetAxisRaw("Horizontal") * reversDir;
        var inputY = Input.GetAxisRaw("Vertical") * reversDir;

        if (dashInput && canDash)
        {
            jumpTimeCounter = 0;
            isJumping = false;

            isDashing = true;
            canDash = false;
            dashingDir = new Vector2(inputX, inputY);
            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(-transform.right.x, 0);

            }
            StartCoroutine(StopDashing());
        }


        anim.SetBool("Dash", isDashing);


        if (isDashing)
        {
            rb.velocity = dashingDir.normalized * dashingVelocity;
            return;
        }

        if (IsGrounded())
        {
            canDash = true;
        }
    }
    void DownDash()
    {
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Z))
        {

            anim.SetTrigger("DownDash");

        }


    }
    void Jump()
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
        

        //JumpAttack anim
        if (Input.GetKeyDown(KeyCode.C) && isGrounded == false)
        {
            anim.SetTrigger("JumpAttack");
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            isJumping = false;
        }

        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
    }
    void Fire()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isAttackOK == false)
                return;

            if (isGrounded == true && moveInput == 0)
            {
                anim.SetTrigger("Attack");
            }
            else if (isGrounded == true && moveInput != 0)
            {
                anim.SetTrigger("RunAttack");
            }

            StartCoroutine(DelayFire());
        }
    }
    IEnumerator DelayFire()
    {
        isAttackOK = false;
        for (int i = 0; i < 3; i++)
        {
            //복제한다. //'Bullet'을 'FirePos.transform.position' 위치에 'FirePos.transform.rotation' 회전값으로.       
            GameObject bullet = objectPoolLogic.MakeObj("Player_Bullet");
            bullet.transform.position = FirePos.position;
            bullet.transform.rotation = FirePos.transform.rotation;
            Debug.Log(rend.flipX);
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            bulletRigid.AddForce(-transform.right * 30f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.3f);
        isAttackOK = true;
    }
}