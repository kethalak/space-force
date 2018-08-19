using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicsObject : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float _mass = 1;
    [SerializeField] private bool _kinematic;
    [SerializeField] private bool _attracts = false;

    private float Mass => _mass;
    private bool IsKinematic => _kinematic;
    private bool Attracts => _attracts;
    
    protected Vector3 GravityVector { get; private set; } = new Vector2(0,0f);
    
    private const float GravityScale = 6.674f;
    
    protected virtual void FixedUpdate()
    {
        var physicsObjects = FindObjectsOfType<PhysicsObject>();
        
        if(!IsKinematic)
            RotateTowardsGravity();
        
        if (!Attracts)
            return;
        
        foreach (var obj in physicsObjects)
        {
            if (obj == this)
                continue;

            if (obj.IsKinematic)
                continue;
            
            Attract(obj);
        }     
    }

    private void Attract(PhysicsObject obj)
    {
        var direction = transform.position - obj.transform.position;
        var distance = direction.magnitude;
        var forceMagnitude = GravityScale * (Mass * obj.Mass) / (distance * distance);
        var force = direction.normalized * forceMagnitude;
        
        obj.GravityVector = (obj.GravityVector + force) / 2;
    }
    
    private void RotateTowardsGravity()
    {
          var pos = transform.position;
      
          var dir = (pos + GravityVector) - new Vector3(transform.position.x, transform.position.y, 0);
        
          var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
          angle += 90;
        
          transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }  

}
