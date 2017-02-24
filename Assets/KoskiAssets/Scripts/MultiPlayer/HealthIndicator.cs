using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HealthIndicator : NetworkBehaviour {

    public const float maxHealth = 100;
    [SyncVar (hook = "OnChangeHealth")]
    public float currentHealth = maxHealth;

   // public RectTransform healthbar;
    public Slider m_Slider;
    public Image m_FillImage;

    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    //public GameObject m_ExplosionPrefab;


    public void TakeDamage(float amount)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth -= amount;

        if(currentHealth <= 0)
        {
            
            Debug.Log("Death");
            RpcRespawn();
        }

        
    }


    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        m_Slider.value = currentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, currentHealth / maxHealth);
    }




    // THIS METHOD IS HOOKED; CALLED WHEN THE HOOKED VAR IS CHANGED (CLIENT CALL SERVER -> SERVER SYNCS ALL CLIENTS)
    void OnChangeHealth(float health)
    {
        SetHealthUI();
        //healthbar.sizeDelta = new Vector2(health * 2, healthbar.sizeDelta.y);
    }

    // CALLED ON THE SERVER AND EXECUTED ON THE CLIENTS
    // THIS  IS A CALL FROM THE SERVER TO CLIENTS (ONLY THE LOCALPLAYER ACTS)
    [ClientRpc] void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            Debug.LogWarning(transform.name + "Respawned from position: " + transform.position);
            currentHealth = maxHealth;
            SetHealthUI();
            transform.position = new Vector3(0, 0, 0);
            transform.Translate(1, 0, 0);
        }
            
    }

}
