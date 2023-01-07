using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegetable : MonoBehaviour
{
    public enum State
    {
        Growing,
        Harvest,
        Rotten
    }
    [SerializeField] private State m_currentState;

    [Header("Growing Parameters")]
    [SerializeField] private float m_growingTime = 1  ;
    [SerializeField] private float m_harvestTime = 1;
    private float m_lifeTimer;


    [Header("Rotten Parameters")]
    [SerializeField] private float m_speed;

    private Transform m_target;
    private Cell m_currentCell;

    private State GetState() { return m_currentState; }

    public void Init(Transform target, Cell cell)
    {
        this.m_target = target;
        this.m_currentCell = cell;
    }

    public void Update()
    {
        GrowingState();
        HarwestState();
        RottenState();
    }

    private void GrowingState()
    {
        if (m_currentState != State.Growing) return;
        if (m_lifeTimer >= m_growingTime)
        {
            m_currentState = State.Harvest;
            m_lifeTimer = 0;
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }

    private void HarwestState()
    {
        if (m_currentState != State.Harvest) return;
        if (m_lifeTimer >= m_harvestTime)
        {
            m_currentState = State.Rotten;
            m_currentCell.isEmpty = true;
            m_currentCell.m_currentVegetable = null;
            m_lifeTimer = 0;
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }

    private void RottenState()
    {
        if (m_currentState != State.Rotten) return;

        Vector3 direction = m_target.position - transform.position;
        direction.y = 0;
        transform.position += direction.normalized * m_speed * Time.deltaTime;
    }
}
