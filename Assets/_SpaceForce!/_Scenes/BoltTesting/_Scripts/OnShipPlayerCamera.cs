using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnShipPlayerCamera : BoltSingletonPrefab<OnShipPlayerCamera>
{
    private Transform _target;
    
    public void SetTarget (BoltEntity entity)
    {
        _target = entity.transform;
        transform.parent = _target;
        transform.position = new Vector3(_target.transform.position.x, _target.transform.position.y, -10f);
    }

    public void Update()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
 