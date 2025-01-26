using System.Collections.Generic;
using UnityEngine;

public class ManagerTile : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableItem
    {
        public string _name;
        public GameObject _prefab;
    }

    [SerializeField] private bool _dontSpawnNothing = false;
    [SerializeField] private List<SpawnableItem> _spawnableEnemies;
    [SerializeField] private List<SpawnableItem> _spawnableHeals;
    [SerializeField] private List<SpawnableItem> _spawnableWalls;
    [SerializeField] private int _minEnemies = 1;
    [SerializeField] private int _maxEnemies = 3;
    [SerializeField] private int _minHeals = 1;
    [SerializeField] private int _maxHeals = 2;
    [SerializeField] private int _wallDensity = 5;

    private Vector2 _screenBounds;
    private List<Vector2> _spawnPositions = new List<Vector2>();
    private List<Vector2> _wallSpawnPositions = new List<Vector2>();
    private HashSet<Vector2> _usedPositions = new HashSet<Vector2>();
    private HashSet<Vector2> _usedWallPositions = new HashSet<Vector2>();
    private float _currentHeight;

    public bool DontSpawnNothing { get => _dontSpawnNothing; set => _dontSpawnNothing = value; }

    private void Start()
    {
        CalculateScreenBounds();
        CalculateSpawnPositions();
        SpawnWalls();
        if (!_dontSpawnNothing) SpawnObjects();
        GameManager.Instance.Height += ChangeCurrentHeight;
    }

    private void CalculateScreenBounds()
    {
        Camera mainCamera = Camera.main;
        _screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }

    private void ChangeCurrentHeight(float height)
    {
        _currentHeight = height;
    }

    private void CalculateSpawnPositions()
    {
        float leftBound = -_screenBounds.x;
        float rightBound = _screenBounds.x+1;

        // Generar posiciones para enemigos y curas
        for (float y = -5f; y <= 5f; y += 1f)
        {
            _spawnPositions.Add(new Vector2(leftBound, y));
            _spawnPositions.Add(new Vector2(rightBound, y));
        }

        // Generar posiciones para paredes
        float verticalRange = _screenBounds.y * 2;
        float gap = verticalRange / _wallDensity;
        float startY = -_screenBounds.y;

        for (int i = 0; i < _wallDensity; i++)
        {
            float y = startY + i * gap;
            _wallSpawnPositions.Add(new Vector2(leftBound, y));
            _wallSpawnPositions.Add(new Vector2(rightBound, y));
        }
    }

    private void SpawnWalls()
    {
        int leftOrderCount = 0;
        int rightOrderCount = 0;

        foreach (Vector2 position in _wallSpawnPositions)
        {
            if (Random.value > 0.5f) // 50% de probabilidad de spawnear
            {
                SpawnableItem wallToSpawn = _spawnableWalls[Random.Range(0, _spawnableWalls.Count)];
                GameObject spawnedWall = Instantiate(wallToSpawn._prefab, position, Quaternion.identity);
                spawnedWall.transform.SetParent(transform, false);

                // Ajustar orden de renderizado según lado
                if (position.x < 0) // Izquierda
                {
                    if (leftOrderCount > 0)
                    {
                        AdjustOrderInLayer(spawnedWall, leftOrderCount);
                    }
                    leftOrderCount++;
                }
                else // Derecha
                {
                    if (rightOrderCount > 0)
                    {
                        AdjustOrderInLayer(spawnedWall, rightOrderCount);
                    }
                    rightOrderCount++;
                }

                _usedWallPositions.Add(position);
            }
        }
    }

    private void SpawnObjects()
    {
        float maxHeight = GameManager.Instance.MaxHeightDificult;
        float heightRatio = Mathf.Clamp01(_currentHeight / maxHeight);

        int enemyCount = Mathf.RoundToInt(Mathf.Lerp(_minEnemies, _maxEnemies, heightRatio));
        int healCount = Mathf.RoundToInt(Mathf.Lerp(_maxHeals, _minHeals, heightRatio));

        SpawnItems(_spawnableEnemies, enemyCount);
        SpawnItems(_spawnableHeals, healCount);
    }

    private void SpawnItems(List<SpawnableItem> items, int count)
    {
        if (items.Count == 0) return;

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPosition = GetRandomSpawnPosition();

            if (spawnPosition == Vector2.zero) continue;

            SpawnableItem itemToSpawn = items[Random.Range(0, items.Count)];
            GameObject spawnedObject = Instantiate(itemToSpawn._prefab, spawnPosition, Quaternion.identity);
            spawnedObject.transform.SetParent(transform, false);
            ResolveOverlap(spawnedObject, spawnPosition);
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        int attempts = 0;

        while (attempts < 100)
        {
            Vector2 position = _spawnPositions[Random.Range(0, _spawnPositions.Count)];

            if (!_usedPositions.Contains(position))
            {
                _usedPositions.Add(position);
                return position;
            }

            attempts++;
        }

        return Vector2.zero;
    }

    private void AdjustOrderInLayer(GameObject wall, int orderIncrement)
    {
        foreach (SpriteRenderer renderer in wall.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.sortingOrder += orderIncrement;
        }
    }

    private void ResolveOverlap(GameObject spawnedObject, Vector2 position)
    {
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(position, 1f);

        foreach (Collider2D collider in overlaps)
        {
            if (collider.gameObject != spawnedObject)
            {
                foreach (SpriteRenderer renderer in spawnedObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.sortingOrder += 2;
                }
            }
        }
    }

    public void ClearUsedPositions()
    {
        _usedPositions.Clear();
        _usedWallPositions.Clear();
    }
}
