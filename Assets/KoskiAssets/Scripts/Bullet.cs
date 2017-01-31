using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    void OnCollisionEnter(Collision coll)
    {
        GameObject hit = coll.gameObject;
        HealthIndicator health = hit.transform.GetComponentInParent<HealthIndicator>();
        // HealthIndicator health = hit.GetComponent<HealthIndicator>();
        // Collision with any object
        //Debug.Log("HIT :", hit);

        if (health != null)
        {
            health.TakeDamage(10);
            
        }
        Destroy(gameObject);

    }

}
