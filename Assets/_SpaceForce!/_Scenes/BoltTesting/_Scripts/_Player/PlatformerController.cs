using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEditor;
using UnityEngine;

public class PlatformerController : Bolt.EntityEventListener<IPlatformerPlayerState>
{
    private bool _up;
    private bool _down;
    private bool _left;
    private bool _right;
    private bool _jump;
    private bool _shift;
    
    private PlatformerMotor _motor;
    
    private void Awake()
    {
        _motor = GetComponent<PlatformerMotor>();    
    }
    
    private void Update()
    {
        PollKeys(true);
        
        if (!entity.isOwner) return;
        state.FacingRight = _motor.FacingRight;
    }
    
    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        state.SetAnimator(_motor.Anim);
        
        if (entity.isOwner)
            state.FacingRight = _motor.FacingRight;

        state.AddCallback("FacingRight", FacingChanged);
    }

    private void FacingChanged() 
    {
        _motor.FacingRight = state.FacingRight;
    }
    
    private void PollKeys(bool mouse)
    {
        _up  = Input.GetAxis("Vertical") > 0.01f;
        _down = Input.GetAxis("Vertical") < -0.01f;
        _right = Input.GetAxis("Horizontal") > 0.01f;
        _left = Input.GetAxis("Horizontal") < -0.01f;
        _jump = Input.GetKeyDown(KeyCode.Space);
        _shift = Input.GetKey(KeyCode.LeftShift);
        if (mouse)
        {
        }
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
            var state = _motor.HandleInput(cmd.Input.Up, cmd.Input.Down, cmd.Input.Left, cmd.Input.Right, cmd.Input.Jump, cmd.Input.Shift);

            // copy the motor state to the commands result (this gets sent back to the client)
            cmd.Result.Position = state.Position;
            cmd.Result.Grounded = state.Grounded;
            cmd.Result.Velocity = state.Velocity;
        }
        
        //Set state properties to match owners 
        if (!cmd.IsFirstExecution) return;
        state.Ground = _motor.Anim.GetBool("Ground");
        state.vSpeed = _motor.Anim.GetFloat("vSpeed");
        state.Speed = _motor.Anim.GetFloat("Speed");
    }
}
