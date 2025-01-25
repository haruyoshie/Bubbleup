using System.Collections;
using UnityEngine;

public class ParticlesInvoke : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour _player;

    [SerializeField]
    private ParticlePool _particlePool;

    void Start()
    {
        _player.JumpState += InvokeParticles;
    }

    private void InvokeParticles(bool obj)
    {
        if (obj) return;

        ParticleSystem dust = _particlePool.GetParticle();
        dust.transform.position = transform.position;
        dust.Play();
        StartCoroutine(ReturnToPool(dust));
    }

    private IEnumerator ReturnToPool(ParticleSystem dust)
    {
        yield return new WaitForSeconds(dust.main.duration); 
        _particlePool.ReturnParticle(dust); 
    }
}
