using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegetable : MonoBehaviour
{
    public enum State
    {
        Growing,
        Harvest,
        Rotten,
        Stun
    }
    [SerializeField] private State m_currentState;
     public VegetableType m_type;

    [Header("Growing Parameters")]
    [SerializeField] private float m_growingTime = 15  ;
    [SerializeField] private float m_harvestTime = 15;
    [SerializeField] [ColorUsage(false,true)] private Color m_growingColor = Color.green;
    [SerializeField] [ColorUsage(false, true)] private Color m_harvestColor = Color.red;
    private float m_lifeTimer;


    [Header("Rotten Parameters")]
    [SerializeField] private float m_speed;
    [SerializeField] [ColorUsage(false, true)] private Color m_rottenColor = Color.red;


    private Transform m_target;
    private Cell m_currentCell;
    private MeshRenderer m_renderer;
    private Material m_material;

    private vfxBinder_Parameter m_vfxBinder;

    public State GetState() { return m_currentState; }

    private void Start()
    {
        InitComponent();
    }

    private void InitComponent()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_vfxBinder = GetComponent<vfxBinder_Parameter>(); m_vfxBinder.timeToMoisie = m_harvestTime;
        m_material = m_renderer.material;
        m_material.color = m_growingColor;
    }

    public void InitVegetable(Transform target, Cell cell)
    {
        this.m_target = target;
        this.m_currentCell = cell;
        m_currentCell.currentVegetable = this;
    }

    public void Update()
    {
        GrowingState();
        HarvestState();
        RottenState();
    }

    private void GrowingState()
    {
        if (m_currentState != State.Growing) return;
        if (m_lifeTimer >= m_growingTime)
        {
            m_currentState = State.Harvest;
            m_vfxBinder.activeMoisie = true;
            m_lifeTimer = 0;
            m_material.color = m_harvestColor;
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }

       
    }

    private void HarvestState()
    {
        if (m_currentState != State.Harvest) return;

        if (m_lifeTimer >= m_harvestTime)
        {
            m_currentState = State.Rotten;
            m_currentCell.isEmpty = true;
            m_currentCell.currentVegetable = null;
            m_lifeTimer = 0;
            m_material.color = m_rottenColor;
            gameObject.tag = "Rotten";
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(transform.position.x, 0.5f, transform.position.z), m_lifeTimer / m_harvestTime);
        }
        
    }

    private void RottenState()
    {
        if (m_currentState != State.Rotten) return;

        Vector3 direction = m_target.position - transform.position;
        direction.y = 0;
        transform.position += direction.normalized * m_speed * Time.deltaTime;
    }

    private void StunState()
    {

    }
    
    public void GetHit()
    {
        m_currentState = State.Stun;
        gameObject.tag = "Collectable";
    }
    public void GetCollect()
    { 
        Destroy(gameObject);
    }
}
