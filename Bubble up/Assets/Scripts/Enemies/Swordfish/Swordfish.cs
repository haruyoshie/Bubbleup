using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordfish : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private float minSpeed = 2f, maxSpeed = 8f;

    [SerializeField]
    private float fishSpeed = 2f;

    [SerializeField]
    private bool triggered;

    private bool leftPoint;
    private bool rightPoint;

    private Vector2 leftBoundary;
    private Vector2 rightBoundary;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameManager.Instance.Height += SpeedDifficulty;

        rightPoint = true;

        // Game boundaries
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        leftBoundary = new Vector2(cam.transform.position.x - camWidth, transform.position.y);
        rightBoundary = new Vector2(cam.transform.position.x + camWidth, transform.position.y);

        RandomSpawn();
    }

    void SpeedDifficulty(float h)
    {
        //print("Invoking height" + h);
        AdjustSpeed(h);
    }

    void RandomSpawn()
    {
        if (Random.Range(0, 2) == 0)
        {
            transform.position = new Vector2(leftBoundary.x + 0.6f, leftBoundary.y);
            rightPoint = true;
            leftPoint = false;
        }
        else
        {
            transform.position = new Vector2(rightBoundary.x - 0.6f, rightBoundary.y);
            rightPoint = false;
            leftPoint = true;
        }

    }

    private void AdjustSpeed(float height)
    {
        float normalizedHeight = Mathf.Clamp01(Mathf.InverseLerp(0, GameManager.Instance.MaxHeightDificult, height));
        fishSpeed = Mathf.Lerp(minSpeed, maxSpeed, normalizedHeight);

    }

    void Update()
    {
        if (triggered)
        {
            handleBoundarySwitch();
            MoveSwordFish();
            EnforceBoundary();
        }
    }

    private void handleBoundarySwitch()
    {
        if (Vector2.Distance(transform.position, rightBoundary) < 0.5f && rightPoint)
        {
            rightPoint = false;
            leftPoint = true;
        }
        if (Vector2.Distance(transform.position, leftBoundary) < 0.5f && leftPoint)
        {
            rightPoint = true;
            leftPoint = false;
        }

    }

    private void MoveSwordFish()
    {
        rb.velocity = (rightPoint) ? new Vector2(fishSpeed, 0) : new Vector2(-fishSpeed, 0);
    }

    private void EnforceBoundary()
    {
        // Ensure the fish stays within boundaries
        if (transform.position.x < leftBoundary.x)
        {
            transform.position = new Vector2(leftBoundary.x, transform.position.y);
            rightPoint = true; // Start moving towards the right
            leftPoint = false;
        }
        else if (transform.position.x > rightBoundary.x)
        {
            transform.position = new Vector2(rightBoundary.x, transform.position.y);
            rightPoint = false; // Start moving towards the left
            leftPoint = true;
        }
    }
}


