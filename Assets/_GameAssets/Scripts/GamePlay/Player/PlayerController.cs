using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientationTransform ;
    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private KeyCode _movementKey;
    [Header("Jump Settings")]
    [SerializeField]private KeyCode _jumpKey;
    [SerializeField]private float _jumpForce;
    [SerializeField]private bool _canJump;
    [SerializeField]private float _jumpCoolDown;
    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;
    private bool isSliding=false;
  private Rigidbody _playerRigidBody;
  private float _horizontalInput , _verticalInput ; 
  private Vector3 _movementDirection;
  
    private void Awake()
    {
        _playerRigidBody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        setInputs();
        limitPlayerSpeed();
    }
    private void FixedUpdate()
    {
        setPlayerMovement();
    }
    private void setInputs()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(_slideKey))
        {
            isSliding=true;
            Debug.Log("Player is sliding");
        }
        else if (Input.GetKeyDown(_movementKey))
        {
            isSliding=false;
            Debug.Log("Player is moving");
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
        if (isSliding)
        {
            _playerRigidBody.AddForce(_movementDirection.normalized*_movementSpeed*_slideMultiplier,ForceMode.Force);
        }else
        {
            _playerRigidBody.AddForce(_movementDirection.normalized*_movementSpeed,ForceMode.Force);
        }
        
    }
    private void setPlayerJumping()
    {
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
    private void limitPlayerSpeed()
    {
        Vector3 _flatVelocity = new Vector3(_playerRigidBody.linearVelocity.x,0f,_playerRigidBody.linearVelocity.z);
        if (_flatVelocity.magnitude>_movementSpeed)
        {
            Vector3 limitedVelocity = _flatVelocity.normalized*_movementSpeed;
            _playerRigidBody.linearVelocity = new Vector3(_playerRigidBody.linearVelocity.x,_playerRigidBody.linearVelocity.y,_playerRigidBody.linearVelocity.z);
        }
    }
}

