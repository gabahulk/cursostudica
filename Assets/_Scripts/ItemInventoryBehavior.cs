using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventoryBehavior : MonoBehaviour {
    int _numberOfShuriken = 1;

    public int NumberOfShuriken
    {
        get
        {
            return _numberOfShuriken;
        }

        set
        {
            _numberOfShuriken = value;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile") 
            && collision.gameObject.GetComponent<ShurikenBehavior>() != null)
        {
            if (collision.gameObject.GetComponent<ShurikenBehavior>().isGrounded
            || collision.gameObject.GetComponent<ShurikenBehavior>().isStuck)
            {
                NumberOfShuriken++;
                Destroy(collision.gameObject);
            }
        }
    }
}
