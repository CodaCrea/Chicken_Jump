using UnityEngine;

public class Missile : MonoBehaviour
{
    [HideInInspector]
    public GameManager gameManager;
    [HideInInspector]
    public PlayerController playerController;
    public Rigidbody2D missileRb;

    private Vector3 _missilePosition;
    private float _speed;
    private bool _isCollisionShield = false;
    private const string tagGround = "Ground", tagEnemy = "Enemy";

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _missilePosition = Vector2.left;
    }

    private void Update()
    {
        Shoot();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Shield") || other.CompareTag("Player") && playerController.isFlashing)
        {
            float bounceForce = 20.0f;
            Vector3 bounceDirection = transform.position - other.transform.position;

            _isCollisionShield = true;
            missileRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
        }
        else if (other.CompareTag("Player") && !playerController.isFlashing)
        {
            playerController.isDead = true;
            gameManager.isReplay = true;
            gameManager.EndGame();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag(tagGround) || other.collider.CompareTag(tagEnemy))
        {
            Destroy(gameObject);
        }
    }

    private void Shoot()
    {
        if (!_isCollisionShield)
        {
            _speed = 10.0f * Time.deltaTime;
            transform.Translate(_missilePosition * _speed);
        }
    }
}
