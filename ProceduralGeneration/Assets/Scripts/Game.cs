using UnityEditor;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int gridWidth = 20;
    public int gridHeight = 20;
    public float cellSize = 0.5f;
    public GameObject cellPrefab;

    private Cell[,] grid;
    private float updateInterval = 2f;
    private float timer;

    void Start()
    {
        InitializeGrid();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > updateInterval)
        {
            UpdateGrid();
            timer = 0;
        }
    }

    void InitializeGrid()
    {
        grid = new Cell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject cellObj = Instantiate(cellPrefab, new Vector3(x * cellSize, y * cellSize, 0), Quaternion.identity);
                cellObj.transform.parent = transform;
                grid[x,y] = cellObj.GetComponent<Cell>();
                grid[x,y].isAlive = Random.value > 0.5f;
                grid[x,y].UpdateColor();
            }
        }
    }
    public void UpdateGrid()
    {
        bool[,] nextState = new bool[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0;y < gridHeight; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x,y);
                bool isAlive = grid[x,y].isAlive;

                if(isAlive && (aliveNeighbors < 2 || aliveNeighbors > 3))
                {
                    nextState[x,y] = false;
                }
                else if (!isAlive && aliveNeighbors == 3)
                {
                    nextState[x,y] = true;
                }
                else 
                {
                    nextState[x,y] = isAlive;
                }
            }
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x,y].isAlive = nextState[x,y];
                grid[x,y].UpdateColor();
            }
        }
    }

    int CountAliveNeighbors(int x, int y)
    {
        int count = 0;
        int[] dx ={-1,0,1,-1,1,-1,0,1};
        int[] dy ={-1,-1,-1,0,0,1,1,1};
        
        for(int i = 0; i<8; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];
        
            if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight && grid[nx,ny].isAlive)
            {
                count++;
            }
        }
        return count;
    }
}
