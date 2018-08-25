using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedProjectileWeapon : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _projectileFx;

    public bool FireOneShot()
    {
        foreach (var fx in _projectileFx)
        {
            var main = fx.main;
            var gunRotation = transform.localRotation.eulerAngles;
            var angle = gunRotation.z * Mathf.Deg2Rad;
            main.startRotationZ = -angle;
            fx.Emit(1);
        }

        return true;
    }
}
