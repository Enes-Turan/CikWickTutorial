using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action OnPlayerJumped;
    [Header("References")]
    [SerializeField] private Transform _orientationTransform ;
    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private KeyCode _movementKey;
    [Header("Jump Settings")]
    [SerializeField]private KeyCode _jumpKey;
    [SerializeField]private float _jumpForce;
    [SerializeField]private bool _canJump;
    [SerializeField]private float _airMultiplier;      
    [SerializeField]private float _airDrag;
    [SerializeField]private float _slideDrag;
    [SerializeField]private float _groundDrag;
    [SerializeField]private float _jumpCoolDown;
    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;
    private bool isSliding=false;
    private StateController _stateController;
    private Rigidbody _playerRigidBody;
    private float _horizontalInput , _verticalInput ; 
    private Vector3 _movementDirection;
  
    private void Awake()
    {
        _stateController = GetComponent<StateController>();
        _playerRigidBody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        setInputs();
        setStates();
        limitPlayerSpeed();
    }
    private void FixedUpdate()
    {
        setPlayerMovement();
    }
    private void setStates()
    {
        var movementDirection =GetMovementDirection();
        var _isGrounded = isGrounded();
        var currentState = _stateController.GetCurrentState();
        var newState = currentState switch
        {
            _ when movementDirection ==Vector3.zero && _isGrounded && !isSliding => PlayerState.Idle,
            _ when movementDirection !=Vector3.zero && _isGrounded && !isSliding => PlayerState.Move,
            _ when movementDirection !=Vector3.zero && _isGrounded && isSliding => PlayerState.Slide,
            _ when movementDirection ==Vector3.zero && _isGrounded && isSliding => PlayerState.SlideIdle,
            _ when _canJump && !_isGrounded=>PlayerState.Jump,
            _ =>currentState
        };
        if (newState!=currentState)
        {
            _stateController.ChangeState(newState);
        }
       
    }
    private void setInputs()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(_slideKey))
        {
            isSliding=true;
        }
        else if (Input.GetKeyDown(_movementKey))
        {
            isSliding=false;
        }
        else if (Input.GetKeyDown(_jumpKey)&&_canJump&& isGrounded())
        { 
            _canJump=false;
            setPlayerJumping();
            Invoke(nameof(resetJumping),_jumpCoolDown);
        }
    }
    private void setPlayerMovement()
    {
        
        _movementDirection = _orientationTransform.forward*_verticalInput + _orientationTransform.right*_horizontalInput;
        float forceMultiplier = _stateController.GetCurrentState() switch
        {
          PlayerState.Move=>1f,
          PlayerState.Slide=>_slideMultiplier,
          PlayerState.Jump=>_airMultiplier,
           _ =>1f  
        };
        _playerRigidBody.AddForce(_movementDirection.normalized*_movementSpeed*forceMultiplier,ForceMode.Force);
        
    }
    private void setPlayerJumping()
    {
        OnPlayerJumped?.Invoke();
        _playerRigidBody.linearVelocity = new Vector3(_playerRigidBody.linearVelocity.x,0f,_playerRigidBody.linearVelocity.z);
        _playerRigidBody.AddForce(transform.up*_jumpForce,ForceMode.Impulse);
        _canJump=false;
    }
    private void resetJumping()
    {
        _canJump=true;
    }
    private bool isGrounded()
    {
        return Physics.Raycast(transform.position,Vector3.down,_playerHeight*0.5f+0.2f,_groundLayer);
    }
    private void setDrag()
    {
        _playerRigidBody.linearDamping= _stateController.GetCurrentState() switch
        {
          PlayerState.Move =>_groundDrag,
          PlayerState.Slide=>_slideDrag,
          PlayerState.Jump=>_airDrag,
          _ =>_playerRigidBody.linearDamping  
        };
    }
    private void limitPlayerSpeed()
    {
        Vector3 _flatVelocity = new Vector3(_playerRigidBody.linearVelocity.x,0f,_playerRigidBody.linearVelocity.z);
        if (_flatVelocity.magnitude>_movementSpeed)
        {
            Vector3 limitedVelocity = _flatVelocity.normalized*_movementSpeed;
            _playerRigidBody.linearVelocity = new Vector3(_playerRigidBody.linearVelocity.x,_playerRigidBody.linearVelocity.y,_playerRigidBody.linearVelocity.z);
        }
    }
    private Vector3 GetMovementDirection()
    {
        return _movementDirection.normalized;
    }
}

