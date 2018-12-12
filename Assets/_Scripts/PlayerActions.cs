﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerActions : NetworkBehaviour {

    public CharacterController2D controller;
    [SerializeField]
    private Animator animator;

    public float runSpeed = 40f;

    [SyncVar]
    float horizontalMove = 0f;
    bool jump = false;

    [SerializeField]
    ItemInventoryBehavior inventory;

    [SerializeField]
    Collider2D swordCollider;
    [SerializeField]
    GameObject shurikenPrefab;
    [SerializeField]
    GameObject shurikenLaunchPosition;
    [SerializeField]
    Vector2 projectileForce;

    [SerializeField]
    AnimationClip airAttackClip;

    [SerializeField]
    AnimationClip attackClip;

    GameObject whiteSpawnPoint;
    GameObject blackSpawnPoint;

    public delegate void PlayerDied(bool hasDied);
    public static event PlayerDied OnPlayerDeath;

    bool isDead = false;


    [Command]
    private void CmdPlayerConnected()
    {
        if (!isServer)
            return;
        if (isLocalPlayer)
        {
            RpcPlayerConnected(true);
        }
        else
        {
            RpcPlayerConnected(false);
        }
    }

    [ClientRpc]
    private void RpcPlayerConnected(bool isFirstPlayer)
    {
        print("dasdaadasd");
        if (isFirstPlayer)
        {
            print("AAAAAAAAAAAAAAAAAAAAAAAA");
            GetComponent<SpriteRenderer>().material.color = new Color(255, 255, 255);
            this.gameObject.transform.position = whiteSpawnPoint.transform.position;
        }
        else
        {
            print("BBBBBBBBBBBBBBBBBB");
            GetComponent<SpriteRenderer>().material.color = new Color(0, 0, 0);
            this.gameObject.transform.position = blackSpawnPoint.transform.position;
            controller.ShouldFlip();
        }
    }

    public override void OnStartLocalPlayer()
    {
        CmdPlayerConnected();
    }

    private void Awake()
    {
        blackSpawnPoint = GameObject.FindGameObjectWithTag("BlackSpawn");
        whiteSpawnPoint = GameObject.FindGameObjectWithTag("WhiteSpawn");
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || controller == null || !isLocalPlayer)
        {
            return;
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButtonDown("Fire3") && !swordCollider.enabled)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetButtonDown("Fire2") && inventory.NumberOfShuriken > 0)
        {
            animator.SetTrigger("Cast");
            animator.SetBool("isCasting", true);
        }

    }

    public void FlipPlayerEventHandler(Vector3 scale) {
        if (isLocalPlayer)
        {
            CmdFlipPlayer(scale);
        }
    }

    [ClientRpc]
    void RpcFlipPlayer(Vector3 scale) {
        controller.Flip(scale);
    }

    void LauchProjectile() {
        if (!isLocalPlayer)
            return;
        CmdShootShuriken();
    }

    [Command]
    void CmdShootShuriken()
    {
        if (!isServer)
            return;

        var projectile = Instantiate(shurikenPrefab, shurikenLaunchPosition.transform.position, Quaternion.identity) as GameObject;
        var directionedForce = projectileForce;
        directionedForce.x = directionedForce.x * transform.localScale.x;
        projectile.GetComponent<Rigidbody2D>().velocity = directionedForce;
        inventory.NumberOfShuriken--;
        animator.SetBool("isCasting", false);
        NetworkServer.Spawn(projectile);
    }

    IEnumerator Attack()
    {
        float hitDuration;
        if (controller.isGrounded)
        {
            animator.SetTrigger("Attack");
            hitDuration = attackClip.length;
        }
        else {
            animator.SetBool("AirAttacking",true);
            hitDuration = airAttackClip.length;
        }

		CmdStartAttack ();
        yield return new WaitForSeconds(hitDuration);
		CmdStopAttack();
        animator.SetBool("AirAttacking", swordCollider.enabled);

    }

	[Command]
	void CmdStartAttack()
	{
		if (!isServer)
			return;
		RpcStartAttack();
	}

	[ClientRpc]
	void RpcStartAttack() {
		swordCollider.enabled = true;
	}

	[Command]
	void CmdStopAttack()
	{
		if (!isServer)
			return;
		RpcStopAttack();
	}

	[ClientRpc]
	void RpcStopAttack() {
		swordCollider.enabled = false;
	}

	[Command]
	void CmdMoveCharacter()
	{
		if (!isServer)
			return;
		RpcMoveCharacter();
	}

    [ClientRpc]
    void RpcMoveCharacter() {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
    }

    [Command]
    void CmdFlipPlayer(Vector3 scale)
    {
        if (!isServer)
            return;
        RpcFlipPlayer(scale);
    }


    void FixedUpdate()
    {
        if (isDead || controller == null || !isLocalPlayer)
        {
            return;
        }

        // Move our character
        CmdMoveCharacter();

        //Animate our character
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        animator.SetBool("isGrounded", controller.isGrounded);
        animator.SetBool("isFalling", controller.isFalling);

        jump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile") && !collision.gameObject.GetComponent<ShurikenBehavior>().isGrounded)
        {
            Die();
        }
    }

    public void Die()
    {
		if (isLocalPlayer && !isDead) {
            isDead = true;
            CmdPlayerDied();
		}
    }

    [Command]
    void CmdPlayerDied()
    {
        if (!isServer)
            return;
        RpcPlayerDied();
    }

    [ClientRpc]
    void RpcPlayerDied()
    {
		if (isLocalPlayer)
        {
			animator.SetBool("isDead", isDead);
        }

        if (OnPlayerDeath!= null)
        {
            OnPlayerDeath(isDead);
        }
    }
}
