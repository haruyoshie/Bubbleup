using System.Collections;
using UnityEngine;

public class JellyfishController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float centerMoveChance = 0.5f;
    [SerializeField] private float orbitRadius = 2f;
    [SerializeField] private float orbitSpeed = 1f;
    [SerializeField] private float upwardSpeed = 2f;

    private Rigidbody2D rb;

    private bool isMoving = false;
    private bool isOrbiting = false;
    private bool isMovingUp = false;

    private float timeToMove;
    private float orbitAngle = 0f;

    private Vector2 leftBoundary;
    private Vector2 rightBoundary;
    private Vector2 targetPosition;
    private Vector2 centerPosition;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        leftBoundary = new Vector2(cam.transform.position.x - camWidth, transform.position.y);
        rightBoundary = new Vector2(cam.transform.position.x + camWidth, transform.position.y);
        centerPosition = new Vector2((leftBoundary.x + rightBoundary.x) / 2, transform.position.y);

        RandomSpawn();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
        else if (isMovingUp)
        {
            MoveUpwards();
        }
        else if (isOrbiting)
        {
            OrbitAroundCenter();
        }
    }

    private void RandomSpawn()
    {
        int randomNumber = Random.Range(0, 2);

        rb.position = (randomNumber == 0) ? new Vector2(leftBoundary.x + 0.5f, leftBoundary.y) : new Vector2(rightBoundary.x - 0.5f, rightBoundary.y);

        float randomX = Random.Range(leftBoundary.x + 0.5f, rightBoundary.x - 0.5f);
        targetPosition = new Vector2(randomX, transform.position.y);

        if (Random.value < centerMoveChance)
        {
            // Move to the center
            targetPosition = new Vector2((leftBoundary.x + rightBoundary.x) / 2, transform.position.y);
        }
        else
        {
            float randomPos = Random.Range(leftBoundary.x + 0.5f, rightBoundary.x - 0.5f);
            targetPosition = new Vector2(randomPos, transform.position.y);
        }
        isMoving = true;
    }

    private void MoveToTarget()
    {
        timeToMove += Time.deltaTime;
        Vector2 direction = (targetPosition - rb.position).normalized;

        rb.velocity = direction * moveSpeed;

        if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
        {
            rb.velocity = Vector2.zero; // Stop movement
            isMoving = false;

            // If we are at the center, start the upward movement and orbiting
            if (targetPosition == centerPosition)
            {
                StartCoroutine(MoveUpAndOrbit());
            }
        }
    }

    private IEnumerator MoveUpAndOrbit()
    {
        // Wait for a moment before moving upward (you can adjust the wait time if needed)
        yield return new WaitForSeconds(0.5f); // Wait for half a second

        // Start moving the jellyfish upward
        isMovingUp = true;

        // Wait until the jellyfish moves upward to the desired position
        yield return new WaitUntil(() => transform.position.y >= centerPosition.y + orbitRadius);

        Vector2 offset = transform.position - (Vector3)centerPosition; // Get the vector from center to the jellyfish
        orbitAngle = Mathf.Atan2(offset.y, offset.x);

        print("Orbit Angle" + orbitAngle);

        // Once it reaches the upward position, start orbiting around the center
        isMovingUp = false;
        isOrbiting = true;
    }


    private void MoveUpwards()
    {
        // Apply velocity to move upward (using Rigidbody2D)
        rb.velocity = new Vector2(0f, upwardSpeed);

        // Check if the jellyfish has reached the desired upward position
        if (transform.position.y >= centerPosition.y + orbitRadius)
        {
            rb.velocity = Vector2.zero; // Stop upward movement
            isMovingUp = false;
        }
    }

    private void OrbitAroundCenter()
    {
        // Orbit around the center
        orbitAngle += orbitSpeed * Time.deltaTime; // Increase the angle to simulate rotation

        // Calculate the new position using sine and cosine for circular motion
        float x = centerPosition.x + orbitRadius * Mathf.Cos(orbitAngle);
        float y = centerPosition.y + orbitRadius * Mathf.Sin(orbitAngle);

        // Update the position of the jellyfish
        rb.MovePosition(new Vector2(x, y));
    }


    [ContextMenu("Reset Jellyfish")]
    public void ResetJellyfish()
    {
        isMoving = false;
        isOrbiting = false;
        orbitAngle = 0f;

        // Restart the movement process
        RandomSpawn();
    }
}

