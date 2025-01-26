using UnityEngine;

public class JellyfishController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float moveDuration = 2f;
    private Rigidbody2D rb;

    private bool isMovingToCenter = true;
    private float timeToMove;
    private bool isRotating = false;

    private Vector2 leftBoundary;
    private Vector2 rightBoundary;
    private Vector2 targetPosition;


    private bool isMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        leftBoundary = new Vector2(cam.transform.position.x - camWidth, transform.position.y);
        rightBoundary = new Vector2(cam.transform.position.x + camWidth, transform.position.y);

        RandomSpawn();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    private void RandomSpawn()
    {
        int randomNumber = Random.Range(0, 2);

        rb.position = (randomNumber == 0) ? new Vector2(leftBoundary.x + 0.5f, leftBoundary.y) : new Vector2(rightBoundary.x - 0.5f, rightBoundary.y);

        float randomX = Random.Range(leftBoundary.x + 0.5f, rightBoundary.x - 0.5f);
        targetPosition = new Vector2(randomX, transform.position.y);

        isMoving = true;

    }

    private void MoveToTarget()
    {
        timeToMove += Time.deltaTime;
        Vector2 direction = (targetPosition - rb.position).normalized;

        rb.velocity = direction * moveSpeed;

        if (Vector2.Distance(rb.position, targetPosition) < 0.1f || timeToMove > moveDuration)
        {
            rb.velocity = Vector2.zero;
            isMoving = false;
        }
    }
}

