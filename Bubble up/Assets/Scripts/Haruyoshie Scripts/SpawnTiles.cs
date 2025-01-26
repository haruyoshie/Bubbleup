using System.Collections.Generic;
using UnityEngine;

public class SpawnTiles : MonoBehaviour
{
    public List<GameObject> _groundPrefabs;
    public float _spawnDistance = 20f;
    public int _initialGroundCount = 5;
    public Transform _player;
    public float _despawnDistance = 10f;

    private float _nextSpawnPosition = -13.7f;
    private Queue<GameObject> _spawnedGrounds = new Queue<GameObject>();
    private float _height;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Height += OnHeightChanged;
        }

        for (int i = 0; i < _initialGroundCount; i++)
        {
            SpawnGround(i >= 2);
        }
    }

    void Update()
    {
        if (_player.position.y + _spawnDistance > _nextSpawnPosition)
        {
            SpawnGround(true);
        }

        if (_spawnedGrounds.Count > 0 && _player.position.y - _spawnedGrounds.Peek().transform.position.y > _despawnDistance)
        {
            DestroyOldGround();
        }
    }

    private void OnHeightChanged(float height)
    {
        _height = height;
    }

    void SpawnGround(bool spawn)
    {
        GameObject groundPrefab = SelectPrefabByHeight();
        GameObject newGround = Instantiate(groundPrefab, new Vector3(0, _nextSpawnPosition, 0), Quaternion.identity);
        newGround.GetComponent<ManagerTile>().DontSpawnNothing = !spawn;
        _spawnedGrounds.Enqueue(newGround);
        _nextSpawnPosition += _spawnDistance;
    }

    void DestroyOldGround()
    {
        GameObject oldGround = _spawnedGrounds.Dequeue();
        Destroy(oldGround);
    }

    GameObject SelectPrefabByHeight()
    {
        float zones = GameManager.Instance.MaxHeightDificult/3;
        if (_height < zones)
        {
            return _groundPrefabs[0];
        }
        else if (_height >= zones && _height < zones * 2)
        {
            return _groundPrefabs[1];
        }
        else
        {
            return _groundPrefabs[2];
        }
    }
}
