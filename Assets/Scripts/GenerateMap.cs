using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject dirtTile;

    private short mapSizeX = 10;
    private short mapSizeZ = 10;
    private Vector3 tileSize;

    private int[,] map; // 0 = grass, 1 = dirt

    private void Start()
    {
        if (grassTile == null || dirtTile == null)
        {
            Debug.LogError("Nie przypisano prefabów!");
        }

        GameObject grassTemp = Instantiate(grassTile);
        Renderer rendGrass = grassTemp.GetComponent<Renderer>();
        tileSize = rendGrass.bounds.size;
        Destroy(grassTemp);

        map = new int[mapSizeX, mapSizeZ];
        GeneratePathWithBranches();
        Generator();
    }

    private void GeneratePathWithBranches()
    {
        // Wybierz losowe krawędzie startu i końca (lewa, prawa, góra, dół)
        int startEdge = Random.Range(0, 4);
        int endEdge = (startEdge + Random.Range(1, 4)) % 4; // inna krawędź

        Vector2Int start = GetRandomPointOnEdge(startEdge);
        Vector2Int end = GetRandomPointOnEdge(endEdge);

        // Główna ścieżka
        CreatePath(start, end);

        // Dodaj 1-2 rozwidlenia z losowych punktów ścieżki do innych krawędzi
        int branches = Random.Range(1, 3);
        for (int i = 0; i < branches; i++)
        {
            Vector2Int branchStart = GetRandomDirtPoint();
            int branchEdge = Random.Range(0, 4);
            // Unikaj rozwidlenia do tej samej krawędzi co główna ścieżka
            while (branchEdge == startEdge || branchEdge == endEdge)
                branchEdge = Random.Range(0, 4);

            Vector2Int branchEnd = GetRandomPointOnEdge(branchEdge);
            CreatePath(branchStart, branchEnd);
        }
    }

    private Vector2Int GetRandomPointOnEdge(int edge)
    {
        switch (edge)
        {
            case 0: // lewa
                return new Vector2Int(0, Random.Range(0, mapSizeZ));
            case 1: // prawa
                return new Vector2Int(mapSizeX - 1, Random.Range(0, mapSizeZ));
            case 2: // góra
                return new Vector2Int(Random.Range(0, mapSizeX), mapSizeZ - 1);
            case 3: // dół
                return new Vector2Int(Random.Range(0, mapSizeX), 0);
            default:
                return new Vector2Int(0, 0);
        }
    }

    private Vector2Int GetRandomDirtPoint()
    {
        // Szukaj losowego punktu na istniejącej ścieżce
        for (int tries = 0; tries < 100; tries++)
        {
            int x = Random.Range(0, mapSizeX);
            int z = Random.Range(0, mapSizeZ);
            if (map[x, z] == 1)
                return new Vector2Int(x, z);
        }
        return new Vector2Int(mapSizeX / 2, mapSizeZ / 2);
    }

    private void CreatePath(Vector2Int from, Vector2Int to)
    {
        Vector2Int pos = from;
        map[pos.x, pos.y] = 1;

        while (pos != to)
        {
            // Losowo wybierz kierunek (najpierw X lub Z)
            bool moveX = (Random.value > 0.5f);

            if (moveX && pos.x != to.x)
                pos.x += (to.x > pos.x) ? 1 : -1;
            else if (pos.y != to.y)
                pos.y += (to.y > pos.y) ? 1 : -1;
            else if (pos.x != to.x)
                pos.x += (to.x > pos.x) ? 1 : -1;

            map[pos.x, pos.y] = 1;
        }
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
}
