using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicsObject : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float _mass = 1;
    [SerializeField] private bool _kinematic;
    [SerializeField] private bool _attracts = false;
    [SerializeField] private bool _autoMass;
    
    private float Mass => _mass;
    private bool IsKinematic => _kinematic;
    private bool Attracts => _attracts;
    private List<PhysicsObject> _physicsObjects;
    protected Vector3 GravityVector { get; private set; } = new Vector2(0,0f);
    
    private const float GravityScale = 6.674f;

    protected virtual void FixedUpdate()
    {
        if (_autoMass)
            _mass = transform.localScale.magnitude * 100;
        
        if(!IsKinematic) 
            RotateTowardsGravity();
        
        if (!Attracts)
            return;

        if (PlanetManager.Instance.PhysicsObjects == null) return;
        
        foreach (var obj in PlanetManager.Instance.PhysicsObjects)
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
