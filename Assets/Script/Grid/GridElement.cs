using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridElement : MonoBehaviour
{
    [SerializeField] private int m_xSize= 1;
    [SerializeField] private int m_zSize= 1;
    [SerializeField][HideInInspector] private GridManager m_gridManager;

    public void SetGridManager(GridManager gridManager)
    {
        m_gridManager = gridManager;
    }
    // Start is called before the first frame update
   public void InitObstacle()
    {
        Vector2 cellSize = m_gridManager.GetCellSize();
        Vector3 startPoint = transform.position + new Vector3((m_xSize - 1)* -cellSize.x,0, (m_zSize - 1)* -cellSize.y)/2;
        Vector3 positionCell = startPoint;
        for (int i = 0; i < (m_xSize); i++)
        {
            
            for (int j = 0; j < (m_zSize); j++)
            {
                positionCell = startPoint + new Vector3(i * cellSize.x, 0, j * cellSize.y);
                Cell closestCell = m_gridManager.ClosestCells(positionCell);
                closestCell.isEmpty = false;
            }
        }
    }


}
