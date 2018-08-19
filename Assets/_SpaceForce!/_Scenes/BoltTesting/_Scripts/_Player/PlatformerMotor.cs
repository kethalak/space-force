using UnityEngine;

public class PlatformerMotor : PhysicsObject
{
    [Header("Controller")] [Space] 
    [SerializeField] private float _height = 1;
    [SerializeField] private bool _airControl = false; // A mask determining what is ground to the character.
    [SerializeField] private LayerMask _whatIsGround; // Whether or not a player can steer while jumping.
    [SerializeField] private float _jumpForce = 3;
    [SerializeField] private float _gravityModifier = .5f;
    [SerializeField] private float _maxSpeed = 2f;
    [SerializeField] private Jetpack _jetpack;

    public State PlayerState;              // State to be syncd throughout the network
    
    private Transform _groundCheck;    // A position marking where to check if the player is grounded.
    
    private const float GroundedRadius = .05f; // Radius of the overlap circle to determine if grounded

    private bool _facingRight = false;
    
    public Animator Anim;            // Reference to the player's animator component. 
    
    public bool FacingRight
    {
        get
        {
            return _facingRight;
        }
        set
        {
            if (_facingRight == value) return;
            Flip();
            _facingRight = value;
        }
    }
    
    private void Awake()
    {
        PlayerState = new State {Position = transform.localPosition};  
        
        // Setting up references.
        _groundCheck = transform.Find("GroundCheck");
    }
    
    public void SetState (Vector2 position, Vector3 velocity, bool grounded)
    {
        // assign new state
        PlayerState.Position = position;
        PlayerState.Velocity = velocity;
        PlayerState.Grounded = grounded;
        
        // assign local transform
        transform.localPosition = PlayerState.Position;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Anim.SetBool("Ground", PlayerState.Grounded);

        // Set the vertical animation
        Anim.SetFloat("vSpeed", PlayerState.Velocity.y);  
    }
    
    private void Move (Vector2 move)
    {    
        //only control the player if grounded or airControl is turned on
        if (!PlayerState.Grounded && !_airControl) return;
        
        // The Speed animator parameter is set to the absolute value of the horizontal input.
        Anim.SetFloat("Speed", Mathf.Abs(move.x));           
        
        // Move the character
        transform.Translate(transform.InverseTransformDirection(move) * BoltNetwork.frameDeltaTime);
    }

    public State HandleInput(bool up, bool down, bool left, bool right, bool jump, bool shift)
    {
        
        if (_jetpack != null)
        {
            PlayerState.Velocity += transform.TransformDirection(_jetpack.HandleInput(up, down, left, right, jump, shift));;
        }

        if (right && !FacingRight)
            FacingRight = true;

        if (left && FacingRight)
            FacingRight = false;

        CheckIfGrounded();
        
        if (PlayerState.Grounded)
        {
            PlayerState.Velocity = Vector2.zero;

            Run(right, left);

            if (up || jump)
                Jump();
        }
        
        if(!PlayerState.Grounded)
            PlayerState.Velocity += GravityVector * _gravityModifier * BoltNetwork.frameDeltaTime;
        
        Debug.DrawLine(transform.position, transform.position + PlayerState.Velocity, Color.red);
        
        // apply movement
        Move (PlayerState.Velocity);

        DetectTunneling();
        
        // update position
        PlayerState.Position = transform.localPosition;
        
        //done
        return PlayerState;
        
    }

    private void Flip() // Do not call directly. Use FacingRight property.
    {
        // Multiply the player's x local scale by -1.
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;   
    }

    private void Run(bool right, bool left)
    {
        if (right)
            PlayerState.Velocity = transform.right * (1 * _maxSpeed);
        if (left)
            PlayerState.Velocity = transform.right * (-1 * _maxSpeed);      
    }

    private void Jump()
    {
        PlayerState.Velocity += transform.up * _jumpForce;
    }

    private void CheckIfGrounded()
    {
        PlayerState.Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        var colliders = Physics2D.OverlapCircleAll(_groundCheck.position, GroundedRadius, _whatIsGround);
        foreach (var c in colliders)
        {
            if (c.gameObject != gameObject)
                PlayerState.Grounded = true;
        }
    }

    private void DetectTunneling ()
    {
        RaycastHit hit;

        if (!Physics.Raycast(_groundCheck.position, transform.up * -1, out hit, 1, _whatIsGround)) return;
        transform.position = hit.point + (transform.up * _height);
        print("resetting pos");
    }

    private void OnDrawGizmos ()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = PlayerState.Grounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere (_groundCheck.position, .2f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine (_groundCheck.position, _groundCheck.position + new Vector3 (0, (_height / 2f), 0));
    }   
    
    public struct State
    {
        public Vector2 Position;
        public bool Grounded;
        public Vector3 Velocity;
    }
}
