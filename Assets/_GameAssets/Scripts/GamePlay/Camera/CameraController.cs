using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform ;
    [SerializeField] private Transform _orientationTransform ;
    [SerializeField] private Transform _playerVisualTransform ;
    [SerializeField] private float _rotationSpeed ;

    private void Update()
    {
      Vector3 viewDirection = _playerTransform.position - new Vector3(transform.position.x,_playerTransform.position.y,transform.position.z);

      _orientationTransform.forward = viewDirection.normalized;
      float _horizontalInput = Input.GetAxisRaw("Horizontal");
      float _verticalInput = Input.GetAxisRaw("Vertical");
      
      Vector3 inputDirection = _orientationTransform.forward*_verticalInput + _orientationTransform.right*_horizontalInput;
        if (inputDirection != Vector3.zero)
        {
           _playerVisualTransform.forward = Vector3.Slerp(_playerVisualTransform.forward,inputDirection.normalized,Time.deltaTime*_rotationSpeed); 
        }
      
    }
}
