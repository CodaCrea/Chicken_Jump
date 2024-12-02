using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public PlayerController playerController;

    private Vector3 _targetStartPosition;
    private float _playerPositionX = 4.0f;
    private float _camPositionZ = 10.0f;


    private void Awake()
    {
        _targetStartPosition = new(playerController.transform.position.x + _playerPositionX, transform.position.y, -_camPositionZ);
    }

    private void Start()
    {
        transform.position = _targetStartPosition;
    }

    private void Update()
    {
        MoveCam();
    }

    private void MoveCam()
    {
        if (!playerController.isDead && !playerController.isEnd)
        {
            float currentSpeed = playerController.currentSpeed;

            transform.Translate(currentSpeed * Time.deltaTime * Vector2.right);
        }
    }
}
