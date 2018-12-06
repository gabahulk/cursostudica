using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBehavior : MonoBehaviour {
	public Transform target;

	void OnTriggerEnter2D(Collider2D col){
		col.gameObject.transform.position = new Vector2(target.transform.position.x, col.gameObject.transform.position.y);
	}
}
