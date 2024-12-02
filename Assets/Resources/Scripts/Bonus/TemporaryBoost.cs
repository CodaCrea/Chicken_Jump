using UnityEngine;

public class TemporaryBoost : MonoBehaviour
{
    public PlayerController playerController;

    private const string TAG_PLAYER = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TAG_PLAYER))
        {
            if (!playerController.isEnd && !playerController.isDead)
            {
                playerController.speedLimit = playerController.maxSpeed;
            }
            Destroy(gameObject);
            playerController.CoroutineBoostTemp();
        }
    }
}
