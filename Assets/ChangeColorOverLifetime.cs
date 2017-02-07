using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorOverLifetime : MonoBehaviour {

    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var col = ps.colorOverLifetime;
        col.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

        col.color = grad;
    }
}
