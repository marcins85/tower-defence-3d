using NUnit;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap2 : MonoBehaviour
{
    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject dirtTile;

    private short mapSizeX = 10;
    private short mapSizeZ = 10;
    private Vector3 tileSize;

    private short[,] map;

    static public List<Vector2Int> pathPosition;
    static public List<Vector2Int> branchPosition;

    private void Start()
    {
        if (grassTile == null || dirtTile == null)
        {
            Debug.LogError("Nie przypisano prefabów!");
        }

        pathPosition = new List<Vector2Int>();
        branchPosition = new List<Vector2Int>();

        GameObject grassTemp = Instantiate(grassTile);
        Renderer rendGrass = grassTemp.GetComponent<Renderer>();
        tileSize = rendGrass.bounds.size;
        Destroy(grassTemp);

        map = new short[mapSizeX, mapSizeZ];

        GeneratePathWithBranches();
        Generator();

        GameManager.Instance.NotifyMapReady();
    }

    private void Generator()
    {
        for (short x = 0; x < mapSizeX; x++)
        {
            for (short z = 0; z < mapSizeZ; z++)
            {
                Vector3 pos = new Vector3(x * tileSize.x, 0f, z * tileSize.z);
                if (map[x, z] == 1)
                    Instantiate(dirtTile, pos, Quaternion.identity, transform);
                else
                    Instantiate(grassTile, pos, Quaternion.identity, transform);
            }
        }
    }

    private void GeneratePathWithBranches()
    {
        int startEdge = Random.Range(0, 4);
        int endEdge = (startEdge + Random.Range(1, 4)) % 4;

        Vector2Int start = GetRandomPointOnEdge(startEdge);
        Vector2Int end = GetRandomPointOnEdge(endEdge);

        pathPosition = CreatePath(start, end);

        int branches = Random.Range(1, 3);

        for (int i = 0; i < branches; i++)
        {
            Vector2Int branchStart = GetRandomDirtPoint();
            int branchEdge = Random.Range(0, 4);
            while (branchEdge == startEdge || branchEdge == endEdge)
            {
                branchEdge = Random.Range(0, 4);
            }

            Vector2Int branchEnd = GetRandomPointOnEdge(branchEdge);
            branchPosition = CreatePath(branchStart, branchEnd);
        }
    }

    private Vector2Int GetRandomDirtPoint()
    {
        for (int tries = 0; tries < 100; tries++)
        {
            int x = Random.Range(0, mapSizeX);
            int z = Random.Range(0, mapSizeZ);

            if (map[x,z] == 1)
            {
                return new Vector2Int(x, z);
            }
        }

        return new Vector2Int(mapSizeX / 2, mapSizeZ / 2);
    }

    private List<Vector2Int> CreatePath(Vector2Int from, Vector2Int to)
    {
        Vector2Int pos = from;
        map[pos.x, pos.y] = 1;
        List<Vector2Int> path = new List<Vector2Int>();

        path.Add(pos);
        
        while (pos != to)
        {
            bool moveX = (Random.value > 0.5f);

            if (moveX && pos.x != to.x)
                pos.x += (to.x > pos.x) ? 1 : -1;
            else if (pos.y != to.y)
                pos.y += (to.y > pos.y) ? 1 : -1;
            else if (pos.x != to.x)
                pos.x += (to.x > pos.x) ? 1 : -1;

            map[pos.x, pos.y] = 1;

            path.Add(pos);
        }

        return path;
    }

    private Vector2Int GetRandomPointOnEdge(int edge)
    {
        switch(edge)
        {
            case 0:
                return new Vector2Int(0, Random.Range(0, mapSizeZ));
            case 1:
                return new Vector2Int(mapSizeX -1, Random.Range(0, mapSizeZ));
            case 2:
                return new Vector2Int(Random.Range(0, mapSizeX), mapSizeZ - 1);
            case 3:
                return new Vector2Int(Random.Range(0, mapSizeX), 0);
            default:
                return new Vector2Int(0, 0);
        }
    }
}
