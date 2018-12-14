using System.Collections;
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

	public delegate void PlayerDied(bool hasDied, bool isWhite);
    public static event PlayerDied OnPlayerDeath;

	public AudioClip jumpSound;
	public AudioClip clashSound;
	public AudioClip swingSound;
	public AudioClip deathSound;
	public AudioSource soundSrc;


	bool isDead = false;
	bool isFirstPlayer;

	void OnEnable()
	{
		GameManager.OnRestartGame += Restart;
	}
		

	void OnDisable()
	{
		GameManager.OnRestartGame -= Restart;
	}


    [Command]
    private void CmdPlayerConnected()
    {
        if (!isServer)
            return;

		RpcPlayerConnected(isFirstPlayer);
    }

    [ClientRpc]
	private void RpcPlayerConnected(bool isFirstPlayer)
    {
		isDead = false;
		if (isLocalPlayer)
		{
			animator.SetBool("isDead", isDead);
			inventory.NumberOfShuriken = 1;
		}

        if (isFirstPlayer)
        {
			if (isLocalPlayer) {
				GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);	
			} else {
				GetComponent<SpriteRenderer>().material.color = new Color(255, 255, 255);
			}
            this.gameObject.transform.position = whiteSpawnPoint.transform.position;

			if (transform.localScale.x != 1) {
				controller.ShouldFlip();
			}
        }
        else
        {
			if (isLocalPlayer) {
				GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);	
			} else {
				GetComponent<SpriteRenderer>().material.color = new Color(0, 0, 0);
			}
            this.gameObject.transform.position = blackSpawnPoint.transform.position;
			if (transform.localScale.x != -1) {
				controller.ShouldFlip();
			}
        }
    }

    public override void OnStartLocalPlayer()
    {
		isFirstPlayer = isServer;
        CmdPlayerConnected();
    }

    private void Awake()
    {
        blackSpawnPoint = GameObject.FindGameObjectWithTag("BlackSpawn");
        whiteSpawnPoint = GameObject.FindGameObjectWithTag("WhiteSpawn");
    }

	void Start(){
		if (!isFirstPlayer)
		{
			if (isLocalPlayer) {
				GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);	
			} else {
				GetComponent<SpriteRenderer>().material.color = new Color(255, 255, 255);
			}
			this.gameObject.transform.position = whiteSpawnPoint.transform.position;
		}
		else
		{
			if (isLocalPlayer) {
				GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);	
			} else {
				GetComponent<SpriteRenderer>().material.color = new Color(0, 0, 0);
			}
			this.gameObject.transform.position = blackSpawnPoint.transform.position;
		}
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
			if (controller.isGrounded) {
				soundSrc.clip = jumpSound;
				soundSrc.Play ();
			}
        }

        if (Input.GetButtonDown("Fire3") && !swordCollider.enabled)
        {
            StartCoroutine(Attack());
			soundSrc.clip = swingSound;
			soundSrc.Play ();
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
		if (!isLocalPlayer || inventory.NumberOfShuriken <= 0)
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
        NetworkServer.Spawn(projectile);
		RpcShootShuriken ();
    }

	[ClientRpc]
	void RpcShootShuriken()
	{
		if (!isLocalPlayer)
			return;

		inventory.NumberOfShuriken--;
		animator.SetBool("isCasting", false);
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
        animator.SetBool("AirAttacking", false);

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
			soundSrc.clip = deathSound;
			soundSrc.Play ();
        }

        if (OnPlayerDeath!= null)
        {
			OnPlayerDeath(isDead, isServer);
        }
    }

	void Restart(){
		if (isLocalPlayer) 
			CmdRespawn ();
	}

	[Command]
	void CmdRespawn(){
		RpcPlayerConnected (isFirstPlayer);
	}


	public void Knockback(){
		GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-transform.localScale.x * 10, 0), ForceMode2D.Impulse);
		soundSrc.clip = clashSound;
		soundSrc.Play ();
	}

}
