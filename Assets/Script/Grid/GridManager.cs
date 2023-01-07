using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SerializeField]
public class Cell
{
    public Cell(Vector3 pos, int index)
    {
        this.index = index;
        position = pos;
    }
    public Vector3 position = Vector3.zero;
    public bool isEmpty = true ;
    public int index =0;
    public Vegetable currentVegetable;
}

public class GridManager : MonoBehaviour
{
    [Header("Grid Parameter")]
    [SerializeField] private Vector2 m_cellSize = new Vector2(0.5f, 0.5f);
    [SerializeField] private int m_gridSize = 100;
    [SerializeField] [Range(0f, 1.0f)] private float m_debugSizePoint = 0.3f;
    [SerializeField] private bool m_showGrid = false;
    [SerializeField][HideInInspector] private Cell[] m_cellsPosition = new Cell[0];
    private Vector3 m_startPoint;



    // Start is called before the first frame update
    void Awake()
    {
        CalculateGrid();
    }

    private void CalculateGrid()
    {
        m_cellsPosition = new Cell[m_gridSize*m_gridSize];
        m_startPoint = new Vector3(m_gridSize / 2 * -m_cellSize.x, 0, m_gridSize / 2 * -m_cellSize.y);
        for (int i = 0; i < m_gridSize; i++)
        {
            for (int j = 0; j < m_gridSize; j++)
            {
                m_cellsPosition[j + i * m_gridSize] = new Cell(m_startPoint + new Vector3(m_cellSize.x * i, 0, m_cellSize.y * j), j + i * m_gridSize);
                m_cellsPosition[j + i * m_gridSize].isEmpty = true;
            }
        }
    }
    
    public Cell ClosestCells(Vector3 position)
    {
        float xf = Mathf.Clamp((Mathf.Abs(m_startPoint.x) + position.x), 0, Mathf.Abs(m_startPoint.x * 2.0f));
        int x = Mathf.RoundToInt((xf / m_cellSize.x));
        float zf = Mathf.Clamp((Mathf.Abs(m_startPoint.z)  + position.z), 0, Mathf.Abs(m_startPoint.z*2.0f));
        int z = Mathf.RoundToInt((zf / m_cellSize.y));
        int index = (z + x * m_gridSize);
        return m_cellsPosition[index];
    }

    public bool IsEmpty(int cellID)
    {
        return m_cellsPosition[cellID].isEmpty;
    }

    private void OnDrawGizmos()
    {
        if (m_showGrid)
        {
            Vector3 startPoint = new Vector3(m_gridSize / 2 * -m_cellSize.x, 0, m_gridSize / 2 * -m_cellSize.y);
            for (int i = 0; i < m_gridSize; i++)
            {
                for (int j = 0; j < m_gridSize; j++)
                {
                    Vector3 center = startPoint + new Vector3(m_cellSize.x * i, 0, m_cellSize.y * j);
                    Gizmos.DrawSphere(center, m_debugSizePoint);
                }

            }
        }
    }

    public void DrawClosestCellPosition(Vector3 position)
    {
        Cell cell = ClosestCells(position);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(cell.position, m_debugSizePoint*2);
    }
}
