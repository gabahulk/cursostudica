using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {

    public CharacterController2D controller;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    int numberOfProjectiles = 1;
    [SerializeField]
    Collider2D swordCollider;
    [SerializeField]
    float hitDuration = .5f;
    [SerializeField]
    GameObject shurikenPrefab;
    [SerializeField]
    GameObject shurikenLaunchPosition;
    [SerializeField]
    Vector2 projectileForce;

    // Update is called once per frame
    void Update()
    {

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        if (Input.GetButtonDown("Fire3") && !swordCollider.enabled)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetButtonDown("Fire2") && numberOfProjectiles > 0)
        {
            LauchProjectile();
        }

    }

    void LauchProjectile() {
        var projectile = Instantiate(shurikenPrefab, shurikenLaunchPosition.transform.position, Quaternion.identity) as GameObject;
        var directionedForce = projectileForce;
        directionedForce.x = directionedForce.x * transform.localScale.x;
        projectile.GetComponent<Rigidbody2D>().velocity = directionedForce;
        numberOfProjectiles--;
    }

    IEnumerator Attack()
    {
        swordCollider.enabled = true;
        yield return new WaitForSeconds(hitDuration);
        swordCollider.enabled = false;
    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {

            if (collision.gameObject.GetComponent<ShurikenBehavior>().isGrounded)
            {
                numberOfProjectiles++;
                Destroy(collision.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
