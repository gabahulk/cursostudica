using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenBehavior : MonoBehaviour {
    public bool isGrounded = false;
    public bool isStuck = false;
    [SerializeField]
    Transform spriteTransform;

    public float rotationSpeed;

    private void Update()
    {
        if (!isStuck)
        {
            spriteTransform.Rotate(0,0, -rotationSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.gameObject.CompareTag ("Ground") || collision.gameObject.CompareTag ("Player")) {
			gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;
			isStuck = true;
			if (collision.gameObject.CompareTag ("Ground")) {
				isGrounded = true;
			} else {
				transform.parent = collision.transform;
			}
		}
    }
}
