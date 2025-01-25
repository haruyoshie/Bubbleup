using UnityEngine;

public class Collitioner : MonoBehaviour
{
    [SerializeField]
    private float _damage = 10f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBehaviour player = collision.transform.GetComponentInParent<PlayerBehaviour>();
            print("Dealing damage");
            player.ChangeLife(-_damage);
        }
    }
}
