using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PlayerController playerController;
    public Transform[] wayPoints;
    public CapsuleCollider2D enemyCollider;

    private Transform _target;
    private float _speed = 5.0f;
    private int _destPoint = 0;
    private const string TAG_PLAYER = "Player";

    private void Start()
    {
        _target = wayPoints[0];
    }

    private void Update()
    {
        MoveEnemy();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag(TAG_PLAYER) && playerController.isTakeShield)
        {
            enemyCollider.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(TAG_PLAYER))
        {
            enemyCollider.isTrigger = false;
        }
    }

    private void MoveEnemy()
    {
        float closeTargetDistance = 0.3f;
        Vector2 directionX = _target.position - transform.position;

        transform.Translate(_speed * Time.deltaTime * directionX.normalized, Space.World);

        if (Vector3.Distance(transform.position, _target.position) < closeTargetDistance)
        {
            _destPoint = (_destPoint + 1) % wayPoints.Length;
            _target = wayPoints[_destPoint];
        }
    }
}
