using UnityEngine;

public class Collitioner : MonoBehaviour
{
    [SerializeField]
    private bool _autoKiller;
    [SerializeField]
    private float _damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBehaviour player = collision.transform.GetComponentInParent<PlayerBehaviour>();
            if (_autoKiller)
            {
                player.GameOver();
            }
            else
            {
                player.GetComponent<ParticlesInvoke>().InvokeParticles();
                player.ChangeLife(_damage);
            }
        }
    }
}
