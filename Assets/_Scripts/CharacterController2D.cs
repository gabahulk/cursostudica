using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
	[SerializeField] private bool m_AirControl = false;
	[SerializeField] private LayerMask m_WhatIsGround;
	[SerializeField] private Transform m_GroundCheck;
	[SerializeField] private Transform m_CeilingCheck;

	const float k_GroundedRadius = .2f; 
	public bool isGrounded;
	public bool isFalling; 
	const float k_CeilingRadius = .2f;
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;
	private Vector3 velocity = Vector3.zero;

    public PlayerActions actions;

    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}


	private void FixedUpdate()
	{
		isGrounded = false;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				isGrounded = true;
		}

        isFalling = m_Rigidbody2D.velocity.y < -0.01;
	}


	public void Move(float move, bool jump)
	{
		//only control the player if grounded or airControl is turned on
		if (isGrounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// smoothing 
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

			// input is moving the player right and the player is facing left.
			if (move > 0 && !m_FacingRight)
			{
                ShouldFlip();
			}
			//input is moving the player left and the player is facing right
			else if (move < 0 && m_FacingRight)
			{
                ShouldFlip();
			}
		}
		// If the player should jump...
		if (isGrounded && jump)
		{
			// Add a vertical force to the player.
			isGrounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}

    public void ShouldFlip() {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        m_FacingRight = !m_FacingRight;
        actions.FlipPlayerEventHandler(theScale);
    }

    public void Flip(Vector3 scale)
    {
        transform.localScale = scale;
    }
}
