using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;
    public AudioManager audioManager;
    public CameraMovement cameraMovement;
    public GameObject particleShield;
    public Transform groundCheck;
    public Rigidbody2D playerRb;
    public CircleCollider2D colliderShield;
    public CapsuleCollider2D colliderPlayer;
    public SpriteRenderer spritePlayer;
    public Animator playerAnim;
    public LayerMask layerPlayer;
    [HideInInspector]
    public Vector2 directionX;
    [HideInInspector]
    public Vector2 directionY;
    [HideInInspector]
    public float currentSpeed = 5.0f, acceleration = 10.0f, jump = 200.0f;
    [HideInInspector]
    public float speedLimit, speed;
    [HideInInspector]
    public float minSpeed = 5.0f, slowSpeed = 10.0f, speedStart = 15.0f, mediumSpeed = 20.0f, maxSpeed = 25.0f, speedPerm = 2.5f;
    [HideInInspector]
    public bool isGrounded = true;
    [HideInInspector]
    public bool isTouchWall = false;
    [HideInInspector]
    public bool isTouchLimit = false;
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public bool isEnd = false;
    [HideInInspector]
    public bool isOutEnd = false;
    [HideInInspector]
    public bool isFlashing = false;
    [HideInInspector]
    public bool isTakeShield = false;

    private Coroutine _timerCoroutine;
    private int _gravity = 10;
    private float _seconds;
    private float _groundCheckRadius = 0.8f;
    private bool _isVisible = false;
    private const string TAG_BONUS_SHIELD = "Bonus Shield", TAG_MISSILE = "Missile", TAG_OUT_SCREEN = "Out Of Scope", TAG_LIMIT = "Limit", TAG_END = "End";
    private const string TAG_BOOST = "Boost", TAG_BOOST_TEMP = "Boost Temp", TAG_LOST_BOOST_TEMP = "Lost Boost Temp";

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, _groundCheckRadius);
    }
#endif

    private void Start()
    {
        playerRb.constraints = RigidbodyConstraints2D.FreezePositionY;
        particleShield.SetActive(false);
        playerRb.gravityScale = _gravity;
        speedLimit = speedStart;
    }

    private void Update()
    {
        if (gameManager.isStart)
        {
            playerRb.constraints = RigidbodyConstraints2D.None;
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
#if UNITY_ANDROID
            MovePlayerForAndroid();
#elif UNITY_STANDALONE || UNITY_WEBGL
            MovePlayerForWindows();
#endif
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(TAG_MISSILE))
        {
            if (!isTakeShield && !isFlashing)
            {
                isDead = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDead || !isEnd)
        {
            if (other.CompareTag(TAG_LIMIT))
            {
                if (!isFlashing)
                {
                    isFlashing = true;
                    PlayerFlashing();
                }
                else
                {
                    spritePlayer.color = new Color(
                    spritePlayer.color.r,
                    spritePlayer.color.g,
                    spritePlayer.color.b,
                    1.0f
                    );
                }
                isTakeShield = false;
                isTouchLimit = true;
                particleShield.SetActive(false);
                currentSpeed = 5.0f;
                speedLimit = speedStart;
                transform.position = new(transform.position.x + 4.0f, transform.position.y, 0);
            }

            if (other.CompareTag(TAG_BONUS_SHIELD))
            {
                isTakeShield = true;
                audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxTakeShield], audioManager.mixerSounds[audioManager.numberSfxTakeShield], transform.position);
                other.gameObject.SetActive(false);
                particleShield.SetActive(true);
                StartCoroutine(TimerShield());
            }
        }

        if (other.CompareTag(TAG_OUT_SCREEN))
        {
            isDead = true;
            gameManager.EndGame();
            gameObject.SetActive(false);
        }

        if (other.CompareTag(TAG_END))
        {
            isEnd = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(TAG_END))
        {
            isOutEnd = true;
            gameManager.EndGame();
            gameObject.SetActive(false);
        }

        if (other.CompareTag(TAG_BOOST) || other.CompareTag(TAG_BOOST_TEMP))
        {
            audioManager.sounds[audioManager.numberSfxReduction].UnloadAudioData();
            audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxBoost], audioManager.mixerSounds[audioManager.numberSfxReduction], transform.position);
        }

        if (other.CompareTag(TAG_LOST_BOOST_TEMP))
        {
            audioManager.sounds[audioManager.numberSfxBoost].UnloadAudioData();
            audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxReduction], audioManager.mixerSounds[audioManager.numberSfxReduction], transform.position);
        }
    }

#if UNITY_ANDROID
    private void MovePlayerForAndroid()
    {
        if (!isDead)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, _groundCheckRadius, layerPlayer);

            if (isTouchLimit)
            {
                isTouchLimit = false;
            }

            if (speedLimit > maxSpeed)
            {
                speedLimit = maxSpeed;
            }
            else if (speedLimit < minSpeed)
            {
                speedLimit = minSpeed;
            }
            currentSpeed = Mathf.MoveTowards(currentSpeed, speedLimit, acceleration * Time.deltaTime);
            directionX = Vector2.left;
            transform.Translate(currentSpeed * Time.deltaTime * directionX);
            AnimSpeed();

            if (Input.touchCount > 0 && isGrounded && !isEnd)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    float jumpForce = jump * Time.deltaTime;
                    float rotateX = 180.0f;

                    if (playerRb.gravityScale == _gravity)
                    {
                        playerRb.gravityScale = -_gravity;
                        directionY = Vector2.up;
                        transform.Rotate(rotateX, transform.rotation.y, 0);
                    }
                    else
                    {
                        playerRb.gravityScale = _gravity;
                        directionY = Vector2.down;
                        transform.Rotate(-rotateX, transform.rotation.y, 0);
                    }
                    playerRb.AddForce(directionY * jumpForce, ForceMode2D.Impulse);
                }
            }
            else
            {
                playerAnim.SetBool("Run More Slow", false);
                playerAnim.SetBool("Run Slow", false);
                playerAnim.SetBool("Run", false);
                playerAnim.SetBool("Run Speed", false);
                playerAnim.SetBool("Run More Speed", false);
                playerAnim.SetBool("Dead", true);
            }
        }
    }
#elif UNITY_STANDALONE || UNITY_WEBGL                                
    private void MovePlayerForWindows()
    {
        if (!isDead)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, _groundCheckRadius, layerPlayer);

            if (isTouchLimit)
            {
                isTouchLimit = false;
            }

            if (speedLimit > maxSpeed)
            {
                speedLimit = maxSpeed;
            }
            else if (speedLimit < minSpeed)
            {
                speedLimit = minSpeed;
            }
            currentSpeed = Mathf.MoveTowards(currentSpeed, speedLimit, acceleration * Time.deltaTime);
            directionX = Vector2.left;
            transform.Translate(currentSpeed * Time.deltaTime * directionX);
            AnimSpeed();
            StartCoroutine(Jump());
        }
        else
        {
            playerAnim.SetBool("Run More Slow", false);
            playerAnim.SetBool("Run Slow", false);
            playerAnim.SetBool("Run", false);
            playerAnim.SetBool("Run Speed", false);
            playerAnim.SetBool("Run More Speed", false);
            playerAnim.SetBool("Dead", true);
        }

        IEnumerator Jump()
        {
            float jumpForce = jump * Time.deltaTime;
            float rotateX = 180.0f;


            while (Input.GetKeyDown(KeyCode.Mouse0) && isGrounded && gameManager.isStart && !isEnd)
            {
                audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxJump], audioManager.mixerSounds[audioManager.numberSfxJump], transform.position);
                if (playerRb.gravityScale == _gravity)
                {
                    playerRb.gravityScale = -_gravity;
                    directionY = Vector2.up;
                    transform.Rotate(rotateX, transform.rotation.y, 0);
                }
                else
                {
                    playerRb.gravityScale = _gravity;
                    directionY = Vector2.down;
                    transform.Rotate(-rotateX, transform.rotation.y, 0);
                }
                playerRb.AddForce(directionY * jumpForce, ForceMode2D.Impulse);
                yield return null;
            }
        }
    }
#endif

    private void AnimSpeed()
    {
        switch (speedLimit > 0)
        {
            case true when speedLimit == minSpeed:
                playerAnim.SetBool("Run Slow", false);
                playerAnim.SetBool("Run More Speed", false);
                playerAnim.SetBool("Run More Slow", true);
                break;
            case true when speedLimit == slowSpeed:
                playerAnim.SetBool("Run More Slow", false);
                playerAnim.SetBool("Run", false);
                playerAnim.SetBool("Run Slow", true);

                break;
            case true when speedLimit == speedStart:
                playerAnim.SetBool("Run Slow", false);
                playerAnim.SetBool("Run Speed", false);
                playerAnim.SetBool("Run More Speed", false);
                playerAnim.SetBool("Run", true);
                break;
            case true when speedLimit == mediumSpeed:
                playerAnim.SetBool("Run", false);
                playerAnim.SetBool("Run More Speed", false);
                playerAnim.SetBool("Run Speed", true);
                break;
            case true when speedLimit == maxSpeed:
                playerAnim.SetBool("Run", false);
                playerAnim.SetBool("Run Speed", false);
                playerAnim.SetBool("Run More Speed", true);
                break;
            default:
                return;
        }
    }

    private void PlayerFlashing()
    {
        StartCoroutine(Flashing());
        StartCoroutine(TimerFlashing());

        IEnumerator Flashing()
        {
            float flashingSpeed = 0.1f;

            while (isFlashing)
            {
                _isVisible = !_isVisible;
                float alpha = _isVisible ? 1.0f : 0.0f;
                spritePlayer.color = new Color(
                    spritePlayer.color.r,
                    spritePlayer.color.g,
                    spritePlayer.color.b,
                    alpha
                    );
                yield return new WaitForSeconds(flashingSpeed);
            }
        }

        IEnumerator TimerFlashing()
        {
            _seconds = 3.0f;
            yield return new WaitForSeconds(_seconds);
            isFlashing = false;
            spritePlayer.color = new Color(
                    spritePlayer.color.r,
                    spritePlayer.color.g,
                    spritePlayer.color.b,
                    1.0f
                    );
        }
    }

    private IEnumerator TimerShield()
    {
        if (isTakeShield)
        {
            _seconds = 6.5f;
            yield return new WaitForSeconds(_seconds);
            isTakeShield = false;
            particleShield.SetActive(false);
        }
    }

    // Arrêt et relance de la coroutine du boost temporaire
    public void CoroutineBoostTemp()
    {
        if (!isDead && !isEnd)
        {
            _seconds = 3.0f;
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
            }
            _timerCoroutine = StartCoroutine(TimerBoostTemp(_seconds));
        }

        // Temps de fin du boost et du ralentissement
        IEnumerator TimerBoostTemp(float sec)
        {
            yield return new WaitForSeconds(sec);
            if (speedLimit < maxSpeed)
            {
                speedLimit = speed;
            }
            else
            {
                speedLimit = mediumSpeed;
            }
        }
    }
}
