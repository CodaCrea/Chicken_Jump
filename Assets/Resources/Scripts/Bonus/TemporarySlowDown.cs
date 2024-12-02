using UnityEngine;

public class TemporarySlowDown : MonoBehaviour
{
    public PlayerController playerController;

    private const string TAG_PLAYER = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TAG_PLAYER))
        {
            if (!playerController.isEnd && !playerController.isDead)
            {
                playerController.speed = playerController.speedLimit;
                if (playerController.speed >= playerController.maxSpeed)
                {
                    playerController.speedLimit -= playerController.slowSpeed;
                }
                else
                {
                    playerController.speedLimit -= playerController.speedPerm;
                }
            }
            Destroy(gameObject);
            playerController.CoroutineBoostTemp();
        }
    }
}
