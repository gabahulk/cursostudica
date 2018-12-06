using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenBehavior : MonoBehaviour {
    public bool isGrounded = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;
            isGrounded = true;
        }
    }
}
