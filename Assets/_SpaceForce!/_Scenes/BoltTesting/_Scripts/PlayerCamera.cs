using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : BoltSingletonPrefab<PlayerCamera>
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera cam;
    private Transform _camTarget;  
    private Transform _target;
    
    public void SetTarget (BoltEntity entity)
    {
        _target = entity.transform;
        _camTarget = _target.Find("_camTarget");
        transform.parent = _target;
        transform.position = _target.position;
        cam.Follow = _camTarget;
        if (_target.GetComponent<PlatformerMotor>() != null)
            _target.GetComponent<PlatformerMotor>().PlayerCamera = this.GetComponent<Camera>();
    }    

    public void Update()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
 