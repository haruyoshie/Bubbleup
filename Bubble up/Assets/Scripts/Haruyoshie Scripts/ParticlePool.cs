using UnityEngine;
using System.Collections.Generic;

public class ParticlePool : MonoBehaviour
{
    public ParticleSystem particlePrefab; // Prefab del sistema de partículas
    public int poolSize = 5; // Tamaño del pool

    private Queue<ParticleSystem> pool = new Queue<ParticleSystem>();

    void Start()
    {
        // Crear el pool inicial de partículas
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem particle = Instantiate(particlePrefab);
            particle.gameObject.SetActive(false);
            pool.Enqueue(particle);
        }
    }

    public ParticleSystem GetParticle()
    {
        if (pool.Count > 0)
        {
            ParticleSystem particle = pool.Dequeue();
            particle.gameObject.SetActive(true);
            return particle;
        }
        else
        {
            // Crear un nuevo sistema de partículas si el pool está vacío
            ParticleSystem newParticle = Instantiate(particlePrefab);
            return newParticle;
        }
    }

    public void ReturnParticle(ParticleSystem particle)
    {
        particle.Stop();
        particle.gameObject.SetActive(false);
        pool.Enqueue(particle);
    }
}