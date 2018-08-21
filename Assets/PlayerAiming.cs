using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    public Transform Weapon;
    
    public Vector3 Aim(Camera playerCamera, Vector2 mousePos)
    {
        if (playerCamera == null)
            return Vector3.zero;

        var mouseX = mousePos.x;
        var mouseY = mousePos.y;
        float screenX = Screen.width;
        float screenY = Screen.height;
 
        if (mouseX < 0 || mouseX > screenX || mouseY < 0 || mouseY > screenY)
            return Vector3.zero;
        
        Vector2 targetPos = playerCamera.WorldToScreenPoint(Weapon.position);
        
        mousePos.x = targetPos.x - mousePos.x;
        mousePos.y = targetPos.y - mousePos.y;

        var angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        Weapon.localRotation = Quaternion.Euler(transform.root.localScale.x < 0 ? new Vector3(0, 0, angle) : new Vector3(0, 0, 180-angle));
        return Weapon.localRotation.eulerAngles;
    }

}
