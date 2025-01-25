using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTile : MonoBehaviour
{
     [System.Serializable]
    public class SpawnableItem
    {
        public string name; // Nombre para identificar el objeto
        public GameObject prefab; // Prefab del objeto a instanciar
    }

    public List<Transform> spawnPoints; // Lista de puntos disponibles para instanciar
    public List<SpawnableItem> spawnableObjects; // Lista de objetos que se pueden instanciar

    private HashSet<Transform> usedPoints = new HashSet<Transform>(); // Puntos ya ocupados

    void Start()
    {
        // Ejemplo: Instanciar 5 objetos al inicio
        SpawnObjects(5);
    }

    public void SpawnObjects(int count)
    {
        int availablePoints = spawnPoints.Count - usedPoints.Count;

        if (availablePoints <= 0)
        {
            Debug.LogWarning("No hay puntos disponibles para instanciar objetos.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = GetRandomAvailablePoint();

            if (spawnPoint != null)
            {
                SpawnableItem itemToSpawn = spawnableObjects[Random.Range(0, spawnableObjects.Count)];
                GameObject objEnvironment = Instantiate(itemToSpawn.prefab, spawnPoint.position, Quaternion.identity);
                objEnvironment.transform.SetParent(spawnPoint);
            }
        }
    }

    private Transform GetRandomAvailablePoint()
    {
        int attempts = 0;

        while (attempts < 100)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            if (!usedPoints.Contains(randomPoint))
            {
                usedPoints.Add(randomPoint); // Marcar el punto como usado
                return randomPoint;
            }

            attempts++;
        }

        Debug.LogWarning("No se encontraron puntos disponibles tras múltiples intentos.");
        return null;
    }

    public void ClearSpawnPoints()
    {
        usedPoints.Clear(); // Libera todos los puntos ocupados
    }

    public void RemoveObjectAt(Transform point)
    {
        if (usedPoints.Contains(point))
        {
            usedPoints.Remove(point); // Marca el punto como libre
        }
    }
}
