using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Anima2D;
using Bolt;
using Boo.Lang;
using UnityEditor;
using UnityEngine;

public class PlatformerController : Bolt.EntityEventListener<IPlatformerPlayerState>
{
    [SerializeField] private RangedProjectileWeapon _weapon;
    [SerializeField] private SpriteMeshInstance[] _renderers;
    
    private bool _up;
    private bool _down;
    private bool _left;
    private bool _right;
    private bool _jump;
    private bool _shift;
    private bool _fire;
    
    private Vector2 _mousePos;
    private bool _facingRight;
    private bool _jetpackUp;
    private bool _jetpackDown;
    private bool _jetpackLeft;
    private bool _jetpackRight;
    
    private Jetpack _jetpack;
    private PlatformerMotor _motor;
    private PlayerAiming _aiming;
    
    private void Awake()
    {
        _motor = GetComponent<PlatformerMotor>();
        _aiming = GetComponent<PlayerAiming>();
        _jetpack = GetComponent<Jetpack>();
    }

    private void Update()
    {
        PollKeys(true);
    }

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        state.SetAnimator(GetComponentInChildren<Animator>());
        state.AddCallback ("FacingRight", FacingChanged);
        state.AddCallback("WeaponRotation", WeaponRotationChanged);
        state.AddCallback("JetpackFlameState[]", OnJetpackFlameStateChanged);
        state.AddCallback("Color", ColorChanged);
        state.OnFire += OnFire;
        if (entity.isOwner)
            state.Color = Random.ColorHSV(0f, 1, .5f, 1, .1f, 1);
    }

    private void OnFire()
    {
        if (entity.hasControl || entity.isOwner) return;
        _weapon.FireOneShot();
        _fire = false;
    }

    private void ColorChanged()
    {

        foreach (var r in _renderers)
        {
            r.color = state.Color;
        }
    }
    
    private void FacingChanged()
    {
        Flip(state.FacingRight);
    }

    private void WeaponRotationChanged()
    {
        _aiming.Weapon.localRotation = Quaternion.Euler(state.WeaponRotation);
    }

    private void OnJetpackFlameStateChanged(Bolt.IState state, string path, Bolt.ArrayIndices indices)
    {
        if (entity.hasControl || entity.isOwner) return;
        
        var up = this.state.JetpackFlameState[0] == 1;
        var down = this.state.JetpackFlameState[1]  == 1;
        var left = this.state.JetpackFlameState[2] == 1;
        var right = this.state.JetpackFlameState[3] == 1;
        
        _jetpack.SetJetpackAnimations(up, down, left, right);
    }
    
    private void PollKeys(bool mouse)
    {
        _up  = Input.GetAxis("Vertical") > 0.01f;
        _down = Input.GetAxis("Vertical") < -0.01f;
        _right = Input.GetAxis("Horizontal") > 0.01f;
        _left = Input.GetAxis("Horizontal") < -0.01f;
        _jump = Input.GetKeyDown(KeyCode.Space);
        _shift = Input.GetKey(KeyCode.LeftShift);
        _fire = Input.GetKeyDown(KeyCode.Mouse0);
        if (!mouse) return;
        _mousePos = Input.mousePosition;
        _mousePos.x = Mathf.Clamp(_mousePos.x, -1024, 1024);
        _mousePos.y = Mathf.Clamp(_mousePos.y, -1024, 1024);
    }
    
    public override void SimulateController()
    {
        var input = PlatformerPlayerCommand.Create();

        input.Left = _left;
        input.Right = _right;
        input.Up = _up;
        input.Down = _down;
        input.Jump = _jump;
        input.Shift = _shift;
        input.MousePos = _mousePos;
        input.FacingRight = CheckDirection(_motor.PlayerCamera, input.MousePos);
        input.WeaponRotation = _aiming.Aim(_motor.PlayerCamera, input.MousePos);
        input.Fire = _fire;
        input.JetpackUp = _jetpack.FlameInput(input.Up, input.Shift);
        input.JetpackDown = _jetpack.FlameInput(input.Down, input.Shift);
        input.JetpackLeft = _jetpack.FlameInput(input.Left, input.Shift);
        input.JetpackRight = _jetpack.FlameInput(input.Right, input.Shift);
        entity.QueueInput(input);
    }
    
    public override void ExecuteCommand(Command command, bool resetState)
    {
        var cmd = (PlatformerPlayerCommand)command;

        if (resetState)
        {
            // we got a correction from the server, reset (this only runs on the client)          
            _motor.SetState(cmd.Result.Position, cmd.Result.Velocity, cmd.Result.Grounded);
        }
        else
        {
            var newState = _motor.HandleInput(cmd.Input.Up, cmd.Input.Down, cmd.Input.Left, cmd.Input.Right, cmd.Input.Jump, cmd.Input.Shift, cmd.Input.MousePos);
            
            // copy the motor state to the commands result (this gets sent back to the client)
            cmd.Result.Position = newState.Position;
            cmd.Result.Grounded = newState.Grounded;
            cmd.Result.Velocity = newState.Velocity;
            
            state.WeaponRotation = cmd.Input.WeaponRotation;
            
            SyncJetpackState(cmd);

            if (!cmd.IsFirstExecution) return;
            AnimatePlayer(newState);
            state.FacingRight = cmd.Input.FacingRight;

            if (!cmd.Input.Fire) return;
            FireWeapon(cmd);
        }
    }

    private void SyncJetpackState(PlatformerPlayerCommand cmd)
    {
        state.JetpackFlameState[0] = cmd.Input.JetpackUp ? 1 : 0;
        state.JetpackFlameState[1] = cmd.Input.JetpackDown ? 1 : 0;
        state.JetpackFlameState[2] = cmd.Input.JetpackLeft ? 1 : 0;
        state.JetpackFlameState[3] = cmd.Input.JetpackRight ? 1 : 0;      
    }
    private void AnimatePlayer (PlatformerMotor.State newState)
    {
        state.Ground = newState.Grounded;
        state.Speed = newState.Speed;
        state.Jump = newState.Jump;
    }

    private void FireWeapon(PlatformerPlayerCommand cmd)
    {
        state.Fire();
        _weapon.FireOneShot();
        _fire = false;
        // if we are the owner and the active weapon is a hitscan weapon, do logic  
    }
    
    private bool CheckDirection(Camera playerCamera, Vector2 mousePos)
    {
        Vector2 screenPos = playerCamera.WorldToScreenPoint(transform.position);

        if (mousePos.x < screenPos.x && _facingRight)
        {
            Flip(false);
            return false;
        }
        else if (mousePos.x > screenPos.x && !_facingRight)
        {
            Flip(true);
            return true;
        }

        return _facingRight;
    }
    
    private void Flip(bool facingRight)
    {
        if (_facingRight == facingRight) return;
        
        _facingRight = facingRight;
        // Multiply the player's x local scale by -1.
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;   
    }
}
