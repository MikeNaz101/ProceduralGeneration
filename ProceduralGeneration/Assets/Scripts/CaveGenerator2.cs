using UnityEngine;
using System.Collections.Generic;

public class CaveGenerator2 : MonoBehaviour
{
    public int width = 50;
    public int height = 20;
    public int depth = 50;
    public float fillProbability = 0.45f;
    public int smoothIterations = 5;

    private int[,,] grid;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    void Start()
    {
        grid = InitializeGrid();
        for (int i = 0; i < smoothIterations; i++)
        {
            grid = ApplyCellularAutomata(grid);
        }

        GenerateMesh();
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

    bool IsBorderCell(int x, int y, int z)
    {
        return (x == 0 || x == width - 1 || y == 0 || y == height - 1 || z == 0 || z == depth - 1);
    }

    void GenerateMesh()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = new Material(Shader.Find("Standard"));

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (grid[x, y, z] == 1) // Only add mesh for walls
                    {
                        AddCubeMesh(vertices, triangles, new Vector3(x, y, z));
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    void AddCubeMesh(List<Vector3> vertices, List<int> triangles, Vector3 position)
    {
        int vertexIndex = vertices.Count;

        // Add vertices for a cube at the given position
        vertices.Add(position + new Vector3(0, 0, 0));
        vertices.Add(position + new Vector3(1, 0, 0));
        vertices.Add(position + new Vector3(1, 1, 0));
        vertices.Add(position + new Vector3(0, 1, 0));
        vertices.Add(position + new Vector3(0, 0, 1));
        vertices.Add(position + new Vector3(1, 0, 1));
        vertices.Add(position + new Vector3(1, 1, 1));
        vertices.Add(position + new Vector3(0, 1, 1));

        // Add triangles for each face of the cube
        int[] faceTriangles = {
            0, 2, 1, 0, 3, 2,  // Front face
            4, 5, 6, 4, 6, 7,  // Back face
            0, 1, 5, 0, 5, 4,  // Bottom face
            2, 3, 7, 2, 7, 6,  // Top face
            0, 4, 7, 0, 7, 3,  // Left face
            1, 2, 6, 1, 6, 5   // Right face
        };

        foreach (int index in faceTriangles)
        {
            triangles.Add(vertexIndex + index);
        }
    }
}
