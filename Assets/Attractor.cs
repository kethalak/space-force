using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    private const float G = 667.4f;

    public Rigidbody2D rb;
    public bool IsKinematic;
    
    private void FixedUpdate()
    {
        var attractors = FindObjectsOfType<Attractor>();
        foreach (var a in attractors)
        {
            if (a.IsKinematic)
                continue;
            
            if (a == this)
                continue;
            
            Attract(a);
        }
    }

    private void Attract(Attractor a)
    {
        var rbToAttract = a.rb;
        var direction = rb.position - rbToAttract.position;
        var distance = direction.magnitude;

        var forceMagnitude = G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
        var force = direction.normalized * forceMagnitude;
        
        rbToAttract.AddForce(force);
        
        Debug.DrawLine(a.rb.position, a.rb.position + force, Color.green);
    }
}
