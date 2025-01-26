using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelProjectile : MonoBehaviour
{

    private float force  = 2f;

    [SerializeField]
    private float direction;

    [SerializeField]
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private Rigidbody2D rb;

    private float lifeTime;

    private bool hit;

    private GameObject player;


    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = player.transform.position - transform.position;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
    }

    private void Update()
    {
        if (hit) return;

        lifeTime += Time.deltaTime;

        if (lifeTime > 5) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hit = true;
            boxCollider2D.enabled = false;
            gameObject.SetActive(false);
        }
    }

    public void SetDirection(float direction)
    {
        lifeTime = 0;
        hit = false;
        boxCollider2D.enabled = true;
        gameObject.SetActive(true);
    }
}
