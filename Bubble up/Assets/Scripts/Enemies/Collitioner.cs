using UnityEngine;

public class Collitioner : MonoBehaviour
{
    [SerializeField]
    private bool _disableOnCollition = false;

    [SerializeField]
    private GameObject _parent;

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

            if (_disableOnCollition)
            {
                _parent.SetActive(false);
                AudioManager.Instance.PlayClic();
            }
        }
    }
}
