using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerHaru : MonoBehaviour
{
    public Rigidbody2D player;
    public ParticlePool particlePool;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Detecta la tecla de espacio
        {
//            Debug.Log("click");
            player.AddForce(new Vector2(0, 5), ForceMode2D.Impulse); // Aplica la fuerza como impulso
            // Obtener un sistema de partículas del pool
            ParticleSystem dust = particlePool.GetParticle();
            dust.transform.position = player.transform.position;
            dust.Play();

            // Devolverlo al pool después de que termine
            StartCoroutine(ReturnToPool(dust));
        }
    }

    private IEnumerator ReturnToPool(ParticleSystem dust)
    {
        yield return new WaitForSeconds(dust.main.duration); // Espera a que termine
        particlePool.ReturnParticle(dust); // Devuelve el sistema al pool
    }
    
}
