using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private GameObject fuelBar;
    [SerializeField] private GameObject fuelManager;
    [SerializeField] private PhysicsMaterial2D normal;
    [SerializeField] private PhysicsMaterial2D flying;
    public bool grounded = true, useJet = false, jetReady = true, onGround = false, sliding = false, opening = true;

    [SerializeField] private float moveSpeed, jetPower, jetFuel = 1.7f, maxFuel = 1.7f, turnSpeed, maxXv, lastY;

    private float flyingTime = 0f, groundedTime = 0f;
    public float fallingTime = 0f;
    public int playerState = 0;
    public bool ending = false;

    private Animator animator;

    public static Player instance;

    private void Awake()
    {
        instance = this;
        opening = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        jetFuel = maxFuel;
        animator = gameObject.GetComponent<Animator>();
        ending = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(rb.velocity.y) < 0.02f) rb.velocity = new Vector2(rb.velocity.x, 0);
        if (grounded) onGround = rb.velocity.y == 0;
        
        //Debug.Log(rb.sharedMaterial.friction);

        if (ending)
        {
            var rx = rb.velocity.x;
            var ry = rb.velocity.y;
            if (Time.timeScale > 0.07f)
                Time.timeScale -= Time.deltaTime / 4f;
            else if (Time.timeScale < 0.07f) Time.timeScale = 0.07f;

            if (rx != 0) rx /= 2f;
            if (ry > 2.5f)
                ry -= Time.deltaTime / 2.5f;

            rb.velocity = new Vector2(rx, ry);
        }

        if (fallingTime <= 0.2f && onGround)
        {
            lastY = transform.position.y;
        }
    }

    private void FixedUpdate()
    {
        if (!opening)
        {
            if (jetReady) useJet = Input.GetKey(KeyCode.Space);
            else useJet = false;

            //Debug.Log(rb.velocity);
            if (grounded)
            {
                rb.sharedMaterial = normal;
                bc.sharedMaterial = normal;

                groundedTime += Time.fixedDeltaTime;
                if (!useJet)
                {
                    if (onGround)
                    {
                        rb.velocity = new Vector2(0, 0);
                        float h = Input.GetAxisRaw("Horizontal");
                        transform.position =
                            new Vector2(transform.position.x + moveSpeed * h * Time.fixedDeltaTime,
                                transform.position.y);
                        if (h > 0)
                        {
                            gameObject.GetComponent<SpriteRenderer>().flipX = false;
                            playerState = 1;
                        }
                        else if (h < 0)
                        {
                            gameObject.GetComponent<SpriteRenderer>().flipX = true;
                            playerState = 1;
                        }
                        else playerState = 0;

                        transform.rotation = Quaternion.Euler(0, 0, 0);
                        rb.freezeRotation = true;
                    }

                    if (Mathf.Abs(rb.velocity.y) < Mathf.Epsilon)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, 0);
                        if (transform.position.y < lastY - 18f && !UISystem.instance.showingText)
                        {
                            Invoke("FallingTextShow", 2f);
                        }
                        onGround = true;
                    }

                }
                else
                {
                    rb.sharedMaterial = flying;
                    bc.sharedMaterial = flying;

                    if (playerState == 1) playerState = 0;
                    rb.freezeRotation = false;
                }

                if (jetFuel < maxFuel && groundedTime >= 0.3f && !useJet && onGround)
                {
                    jetFuel += Time.fixedDeltaTime * 2;
                    jetReady = false;
                }
                else if (jetFuel >= maxFuel && !jetReady)
                {
                    jetFuel = maxFuel;
                    jetReady = true;
                    fuelManager.GetComponent<FuelManager>().FuelOn();
                }
            }
            else onGround = false;


            if (!onGround)
            {
                rb.freezeRotation = false;
                float k = -Input.GetAxisRaw("Horizontal");
                var t = transform.rotation;
                if (k > 0 && t.eulerAngles.z < 360 && t.eulerAngles.z > 270)
                {
                    k *= 1.5f;
                }
                else if (k < 0 && t.eulerAngles.z > 0 && t.eulerAngles.z < 90)
                {
                    k *= 1.5f;
                }

                transform.Rotate(0, 0, t.z + k * turnSpeed * Time.fixedDeltaTime);
                if (!useJet) playerState = 0;
                if (jetReady && jetFuel != 0 && !ending)
                {
                    if (t.z >= Quaternion.Euler(0, 0, 90).z) transform.rotation = Quaternion.Euler(0, 0, 89);
                    else if (t.z <= Quaternion.Euler(0, 0, -90).z) transform.rotation = Quaternion.Euler(0, 0, -89);
                    else if (t.z <= Quaternion.Euler(0, 0, 270).z && t.z >= Quaternion.Euler(0, 0, 180).z)
                        transform.rotation = Quaternion.Euler(0, 0, 271);
                }
                //else if (transform.rotation.z >=)
                //if (transform.rotation.z <= -0.30) transform.rotation = Quaternion.Euler(0, 0, -30);
                //else if (transform.rotation.z >= 0.30) transform.rotation = Quaternion.Euler(0, 0, 45);
                //Debug.Log(transform.rotation.eulerAngles);

            }

            if (useJet && jetReady)
            {
                jetFuel -= Time.fixedDeltaTime;
                if (jetFuel > 0)
                {
                    float yPower = (float)(Math.Cos(transform.rotation.z) * jetPower);
                    if (rb.velocity.y >= 6.0f) yPower = 0f;
                    float xPower = -(float)(Math.Sin(transform.rotation.z) * jetPower);
                    if (Mathf.Abs(rb.velocity.x) >= 6.0f) xPower = 0f;
                    
                    if (xPower * rb.velocity.x < 0)
                        rb.velocity = new Vector2(rb.velocity.x / 2, rb.velocity.y);

                    if (rb.velocity.x >= maxXv && xPower > 0)
                    {
                        xPower = 0;
                    }
                    else if (rb.velocity.x <= -maxXv && xPower < 0)
                    {
                        xPower = 0;
                    }

                    rb.velocity = new Vector2(
                        rb.velocity.x + xPower + (xPower * ((flyingTime * flyingTime + 1f) / 2f + 1.8f)),
                        rb.velocity.y + yPower + (yPower * ((flyingTime * flyingTime + 1f) / 2f + 1.8f))
                    );
                    flyingTime += Time.fixedDeltaTime;
                    if (flyingTime >= 1.5f) flyingTime = 1.5f;
                    //Debug.Log(jetFuel);
                    playerState = 2;
                }
                else playerState = 0;

            }
            else
            {
                flyingTime = 0f;
            }

            if (rb.velocity.y <= -10f) rb.velocity = new Vector2(rb.velocity.x, -10f);

            if ((jetFuel <= 0.1f || !useJet || !jetReady) && (!grounded || rb.velocity.y < -6f) &&
                rb.gravityScale > 1.0f && !ending)
                fallingTime = Mathf.Clamp(fallingTime + Time.fixedDeltaTime, 0f, 3.5f);
            else
                fallingTime = Mathf.Clamp(fallingTime - Time.fixedDeltaTime * 2f, 0f, 2.5f);
            //f(fallingTime > 0.1f) Debug.Log(fallingTime);

            if (fallingTime > 1.5f) Time.timeScale = 0.9f;
            else Time.timeScale = 1f;

            animator.SetInteger("duckState", playerState);

            if (jetFuel == maxFuel && !useJet)
                fuelBar.SetActive(false);
            else
            {
                fuelBar.SetActive(true);
                fuelBar.GetComponent<Image>().fillAmount = jetFuel / maxFuel;
                fuelBar.transform.position
                    = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.55f, 0.1f, 0));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            jetReady = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            grounded = false;
            groundedTime = 0f;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Fuel"))
        {
            jetFuel = maxFuel;
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("LowGravity"))
        {
            rb.gravityScale = 0.5f;
            Time.timeScale = 0.6f;
        }
        else if (other.gameObject.CompareTag("ZeroGravity"))
        {
            rb.gravityScale = 0;
            jetFuel = 0;
            ending = true;
            rb.velocity = new Vector2(rb.velocity.x, 5.88f);
            //Debug.Log(rb.velocity);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LowGravity"))
        {
            rb.gravityScale = 1.5f;
            Time.timeScale = 1;
        }
        else if (other.gameObject.CompareTag("ZeroGravity"))
        {
            rb.gravityScale = 1.5f;
        }
    }

    private void FallingTextShow()
    {
        UISystem.instance.ShowText();
    }

}

