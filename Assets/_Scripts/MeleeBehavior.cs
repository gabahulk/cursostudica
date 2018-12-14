using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehavior : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.CompareTag ("Player")) {
			col.gameObject.GetComponent<PlayerActions> ().Die ();
		} else if (col.gameObject.CompareTag ("Projectile")){
			float x = -col.gameObject.GetComponent<Rigidbody2D> ().velocity.x;
			float y = -Random.Range(-col.gameObject.GetComponent<Rigidbody2D> ().velocity.y,col.gameObject.GetComponent<Rigidbody2D> ().velocity.y);
			var reflectionVelocity = new Vector2 (x, y);
			col.gameObject.GetComponent<Rigidbody2D> ().velocity = reflectionVelocity;
		} else if (col.gameObject.CompareTag ("Weapon")){
			col.GetComponentInParent<PlayerActions> ().Knockback ();
		}
	}
}
