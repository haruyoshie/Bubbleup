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
    private float _height; // Altura actual

    void Start()
    {
        // Suscribirse al evento de altura
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Height += OnHeightChanged;
        }

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

    private void OnHeightChanged(float height)
    {
        _height = height; // Actualizar la altura actual
        //Debug.Log($"Altura actual: {_height}");
    }

    void SpawnGround()
    {
        // Elegir un prefab según la altura actual
        GameObject groundPrefab = SelectPrefabByHeight(_height);

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

    GameObject SelectPrefabByHeight(float height)
    {
        // Seleccionar prefab según rangos de altura
        if (height < 50)
        {
            return groundPrefabs[0]; // Prefab para alturas bajas
        }
        else if (height < 100)
        {
            return groundPrefabs[1]; // Prefab para alturas medias
        }
        else
        {
            return groundPrefabs[2]; // Prefab para alturas altas
        }
    }
}
