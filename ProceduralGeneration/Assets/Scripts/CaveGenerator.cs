using UnityEngine;
using System.Collections.Generic;

public class CaveGenerator : MonoBehaviour
{
    public int width = 50;
    public int height = 20;
    public int depth = 50;
    public float fillProbability = 0.45f;
    public int smoothIterations = 5;
    public GameObject wallPrefab;

    private int[,,] grid;
    private List<List<Vector3Int>> regions = new List<List<Vector3Int>>();

    void Start()
    {
        grid = InitializeGrid();
        for (int i = 0; i < smoothIterations; i++)
        {
            grid = ApplyCellularAutomata(grid);
        }

        DetectRegions();
        ConnectRegions();

        DrawGrid();
    }

    int[,,] InitializeGrid()
    {
        int[,,] newGrid = new int[width, height, depth];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (IsBorderCell(x, y, z))
                    {
                        newGrid[x, y, z] = 1; // Always make borders walls
                    }
                    else
                    {
                        newGrid[x, y, z] = Random.Range(0f, 1f) < fillProbability ? 1 : 0;
                    }
                }
            }
        }
        return newGrid;
    }

    int[,,] ApplyCellularAutomata(int[,,] oldGrid)
    {
        int[,,] newGrid = new int[width, height, depth];

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                for (int z = 1; z < depth - 1; z++)
                {
                    int wallCount = CountWallNeighbors(oldGrid, x, y, z);
                    newGrid[x, y, z] = wallCount >= 14 ? 1 : 0;
                }
            }
        }

        return newGrid;
    }

    int CountWallNeighbors(int[,,] grid, int x, int y, int z)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (i == 0 && j == 0 && k == 0) continue;
                    if (grid[x + i, y + j, z + k] == 1) count++;
                }
            }
        }
        return count;
    }

    void DetectRegions()
    {
        bool[,,] visited = new bool[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (grid[x, y, z] == 0 && !visited[x, y, z])
                    {
                        List<Vector3Int> newRegion = GetRegion(x, y, z, visited);
                        regions.Add(newRegion);
                    }
                }
            }
        }
    }

    List<Vector3Int> GetRegion(int startX, int startY, int startZ, bool[,,] visited)
    {
        List<Vector3Int> region = new List<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(new Vector3Int(startX, startY, startZ));
        visited[startX, startY, startZ] = true;

        while (queue.Count > 0)
        {
            Vector3Int cell = queue.Dequeue();
            region.Add(cell);

            foreach (Vector3Int neighbor in GetNeighbors(cell))
            {
                if (!visited[neighbor.x, neighbor.y, neighbor.z] && grid[neighbor.x, neighbor.y, neighbor.z] == 0)
                {
                    visited[neighbor.x, neighbor.y, neighbor.z] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return region;
    }

    List<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (Mathf.Abs(i) + Mathf.Abs(j) + Mathf.Abs(k) == 1) // 6-connected neighbors
                    {
                        int nx = cell.x + i;
                        int ny = cell.y + j;
                        int nz = cell.z + k;

                        if (nx >= 0 && nx < width && ny >= 0 && ny < height && nz >= 0 && nz < depth)
                        {
                            neighbors.Add(new Vector3Int(nx, ny, nz));
                        }
                    }
                }
            }
        }

        return neighbors;
    }

    void ConnectRegions()
    {
        if (regions.Count < 2) return;

        List<Vector3Int> mainRegion = regions[0];
        foreach (List<Vector3Int> region in regions)
        {
            if (region.Count > mainRegion.Count)
            {
                mainRegion = region;
            }
        }

        foreach (List<Vector3Int> region in regions)
        {
            if (region == mainRegion) continue;

            Vector3Int start = mainRegion[Random.Range(0, mainRegion.Count)];
            Vector3Int end = region[Random.Range(0, region.Count)];

            CarveTunnel(start, end);
        }
    }

    void CarveTunnel(Vector3Int start, Vector3Int end)
    {
        Vector3Int current = start;

        while (current != end)
        {
            grid[current.x, current.y, current.z] = 0;

            int dx = Mathf.Abs(end.x - current.x);
            int dy = Mathf.Abs(end.y - current.y);
            int dz = Mathf.Abs(end.z - current.z);

            if (dx >= dy && dx >= dz) current.x += (end.x > current.x) ? 1 : -1;
            else if (dy >= dx && dy >= dz) current.y += (end.y > current.y) ? 1 : -1;
            else current.z += (end.z > current.z) ? 1 : -1;
        }
    }

    void DrawGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (grid[x, y, z] == 1)
                    {
                        Instantiate(wallPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
                    }
                }
            }
        }
    }

    bool IsBorderCell(int x, int y, int z)
    {
        return (x == 0 || x == width - 1 || y == 0 || y == height - 1 || z == 0 || z == depth - 1);
    }
}
