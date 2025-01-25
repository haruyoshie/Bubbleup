using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTiles : MonoBehaviour
{
    public GameObject[] groundPrefabs; // Prefabs de las piezas del terreno
    public float spawnDistance = 20f; // Distancia entre cada pieza del terreno
    public int initialGroundCount = 5; // Cantidad inicial de piezas de terreno
    public Transform player; // Referencia al jugador
    public float despawnDistance = 10f; // Distancia para eliminar piezas antiguas

    private float nextSpawnPosition = 0f; // Posición donde se generará la próxima pieza
    private Queue<GameObject> spawnedGrounds = new Queue<GameObject>(); // Cola para manejar las piezas del terreno

    void Start()
    {
        // Generar las piezas iniciales del terreno
        for (int i = 0; i < initialGroundCount; i++)
        {
            SpawnGround();
        }
    }

    void Update()
    {
        // Generar nuevas piezas del terreno si el jugador se acerca al borde
        if (player.position.y + spawnDistance > nextSpawnPosition)
        {
            SpawnGround();
        }

        // Destruir piezas antiguas para optimizar
        if (spawnedGrounds.Count > 0 && player.position.y - spawnedGrounds.Peek().transform.position.y > despawnDistance)
        {
            DestroyOldGround();
        }
    }

    void SpawnGround()
    {
        // Elegir un prefab aleatorio de la lista
        GameObject groundPrefab = groundPrefabs[Random.Range(0, groundPrefabs.Length)];

        // Instanciar la pieza del terreno en el eje Y
        GameObject newGround = Instantiate(groundPrefab, new Vector3(0, nextSpawnPosition, 0), Quaternion.identity);

        // Añadir la pieza a la cola
        spawnedGrounds.Enqueue(newGround);

        // Actualizar la posición para el siguiente spawn
        nextSpawnPosition += spawnDistance;
    }

    void DestroyOldGround()
    {
        // Eliminar la pieza más antigua del terreno
        GameObject oldGround = spawnedGrounds.Dequeue();
        Destroy(oldGround);
    }
}

