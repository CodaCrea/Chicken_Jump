using UnityEngine;

public class PermanentBoost : MonoBehaviour
{
    public PlayerController playerController;

    private const string TAG_PLAYER = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TAG_PLAYER))
        {
            if (!playerController.isEnd && !playerController.isDead)
            {
                if (playerController.speedLimit < playerController.maxSpeed)
                {
                    if (playerController.speedLimit <= playerController.mediumSpeed)
                    {
                        playerController.speedLimit += playerController.speedPerm;
                    }
                    playerController.speed = playerController.speedLimit;
                }
            }
            Destroy(gameObject);
        }
    }
}
