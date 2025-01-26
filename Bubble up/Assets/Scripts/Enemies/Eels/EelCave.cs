using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelCave : MonoBehaviour
{

    [SerializeField]
    private float attackCooldown = 2f;

    [SerializeField]
    private float maxAttackCooldown = 5f, minAttackCooldown = 1f;



    private Transform eelPosition;
    private List<GameObject> eels = new List<GameObject>();

    private float cooldownTimer;

    private Vector2 leftBoundary;
    private Vector2 rightBoundary;

    private int direction;

    private GameObject player;

    void Start()
    {
        //GameManager.Instance.Height += ChangeDifficulty;
        // Game boundaries
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        leftBoundary = new Vector2(cam.transform.position.x - camWidth, transform.position.y);
        rightBoundary = new Vector2(cam.transform.position.x + camWidth, transform.position.y);

        player = GameObject.FindGameObjectWithTag("Player");

        eelPosition = transform.GetChild(0);
        RandomSpawn();

        Transform eelHolder = transform.GetChild(1);

        foreach (Transform eel in eelHolder)
        {
            eels.Add(eel.gameObject);
        }
    }

    void ChangeDifficulty(float height)
    {
        attackCooldown = Mathf.Lerp(maxAttackCooldown, minAttackCooldown, height/GameManager.Instance.MaxHeightDificult);
    }

    void RandomSpawn()
    {
        if (Random.Range(0, 2) == 0)
        {
            transform.position = new Vector2(leftBoundary.x + 0.6f, leftBoundary.y);
            direction = 1;
        }
        else
        {
            transform.position = new Vector2(rightBoundary.x - 0.6f, rightBoundary.y);
            direction = -1;
        }

        transform.localScale = new Vector3(direction * transform.localScale.x, transform.localScale.y, transform.localScale.z);

    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if(distance < 5f)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer > attackCooldown)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        cooldownTimer = 0f;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        eels[FindEel()].transform.position = eelPosition.position;
        eels[FindEel()].GetComponent<EelProjectile>().SetDirection(direction);
    }

    private int FindEel()
    {
        for (int i = 0; i < eels.Count; i++)
        {
            if (!eels[i].activeInHierarchy)
            {
                print("Eel " + i + " is active");
                return i;
            }
        }

        return 0;
    }
}

