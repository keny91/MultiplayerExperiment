using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HealthIndicator : NetworkBehaviour {

    public const int maxHealth = 100;
    [SyncVar (hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    public RectTransform healthbar;


    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    public GameObject m_ExplosionPrefab;


    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth -= amount;

        if(currentHealth <= 0)
        {
            currentHealth = maxHealth;
            Debug.Log("Death");
            RpcRespawn();
        }

        
    }



    // THIS METHOD IS HOOKED; CALLED WHEN THE HOOKED VAR IS CHANGED (CLIENT CALL SERVER -> SERVER SYNCS ALL CLIENTS)
    void OnChangeHealth(int health)
    {
        healthbar.sizeDelta = new Vector2(health * 2, healthbar.sizeDelta.y);
    }

    // CALLED ON THE SERVER AND EXECUTED ON THE CLIENTS
    // THIS  IS A CALL FROM THE SERVER TO CLIENTS (ONLY THE LOCALPLAYER ACTS)
    [ClientRpc] void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            Debug.LogWarning(transform.name + "Respawned from position: " + transform.position);
            transform.position = new Vector3(0, 0, 0);
            transform.Translate(1, 0, 0);
        }
            
    }

}
