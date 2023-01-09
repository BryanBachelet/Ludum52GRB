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
    [SerializeField] private float m_growingTime = 15;
    [SerializeField] private float m_harvestTime = 15;
    [SerializeField] [ColorUsage(false,true)] private Color m_growingColor = Color.green;
    [SerializeField] [ColorUsage(false, true)] private Color m_harvestColor = Color.red;
    private float m_lifeTimer;
    private float m_totalLifetime;
    private float m_totalTimeSpent;


    [Header("Rotten Parameters")]
    [SerializeField] private float m_speed;
    [SerializeField] [ColorUsage(false, true)] private Color m_rottenColor = Color.red;


    private Transform m_target;
    private Cell m_currentCell;
    private MeshRenderer m_renderer;
    private Material m_material;

    private vfxBinder_Parameter m_vfxBinder;
    public GameObject[] vegetable_GrowingState = new GameObject[3];
    public Material materialColorOverTime;
    public Color[] materialState;
    public MeshRenderer[] tomatoMesh;
    public SkinnedMeshRenderer[] raishtoMesh;
    public Color currentColor; 
    public State GetState() { return m_currentState; }

    private void Start()
    {

        m_totalLifetime = m_growingTime + m_harvestTime;
        InitComponent();
    }

    private void InitComponent()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_vfxBinder = GetComponent<vfxBinder_Parameter>(); m_vfxBinder.timeToMoisie = m_harvestTime;
        materialColorOverTime = new Material(Shader.Find("HDRP/Lit"));
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
        if (!vegetable_GrowingState[0].gameObject.activeInHierarchy)
        {
            vegetable_GrowingState[0].gameObject.SetActive(true);
            vegetable_GrowingState[1].gameObject.SetActive(false);
            vegetable_GrowingState[2].gameObject.SetActive(false);
            tomatoMesh = gameObject.transform.GetChild(1).GetComponentsInChildren<MeshRenderer>();
            if (tomatoMesh.Length < 1)
            {
                raishtoMesh = gameObject.transform.GetChild(1).GetComponentsInChildren<SkinnedMeshRenderer>();
            }
        }
        currentColor = Color.Lerp(materialState[0], materialState[1], m_totalTimeSpent / m_growingTime);
        materialColorOverTime.color = currentColor;
        for (int i = 0; i < tomatoMesh.Length; i++)
        {
            tomatoMesh[i].materials[1].color = currentColor;
        }
        for (int i = 0; i < raishtoMesh.Length; i++)
        {
            raishtoMesh[i].materials[1].color = currentColor;
        }

        if (m_lifeTimer >= m_growingTime)
        {

            m_currentState = State.Harvest;

            m_vfxBinder.activeMoisie = true;
            m_lifeTimer = 0;
            m_material.color = m_harvestColor;
        }
        else
        {
            m_totalTimeSpent += Time.deltaTime;
            m_lifeTimer += Time.deltaTime;
        }

       
    }

    private void HarvestState()
    {
        if (m_currentState != State.Harvest) return;

        if (!vegetable_GrowingState[1].gameObject.activeInHierarchy)
        {
            vegetable_GrowingState[0].gameObject.SetActive(false);
            vegetable_GrowingState[1].gameObject.SetActive(true);
            vegetable_GrowingState[2].gameObject.SetActive(false);
            tomatoMesh = gameObject.transform.GetChild(2).GetComponentsInChildren<MeshRenderer>();
            if(tomatoMesh.Length < 1)
            {
                raishtoMesh = gameObject.transform.GetChild(2).GetComponentsInChildren<SkinnedMeshRenderer>();
            }
        }
        currentColor = Color.Lerp(materialState[1], materialState[2], m_totalTimeSpent / m_totalLifetime);
        materialColorOverTime.color = currentColor;
        for (int i = 0; i < tomatoMesh.Length; i++)
        {
            tomatoMesh[i].materials[1].color = currentColor;
        }
        for(int i = 0; i < raishtoMesh.Length; i++)
        {
            raishtoMesh[i].materials[1].color = currentColor;
        }
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
            m_totalTimeSpent += Time.deltaTime;
            m_lifeTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(transform.position.x, 0.5f, transform.position.z), m_lifeTimer / m_harvestTime);
        }
        
    }

    private void RottenState()
    {
        if (m_currentState != State.Rotten) return;
        if (!vegetable_GrowingState[2].gameObject.activeInHierarchy)
        {
            vegetable_GrowingState[0].gameObject.SetActive(false);
            vegetable_GrowingState[1].gameObject.SetActive(false);
            vegetable_GrowingState[2].gameObject.SetActive(true);
            tomatoMesh = gameObject.transform.GetChild(3).GetComponentsInChildren<MeshRenderer>();
            if (tomatoMesh.Length < 1)
            {
                raishtoMesh = gameObject.transform.GetChild(3).GetComponentsInChildren<SkinnedMeshRenderer>();
            }
            for (int i = 0; i < tomatoMesh.Length; i++)
            {
                tomatoMesh[i].materials[1].color = currentColor;
            }
            for (int i = 0; i < raishtoMesh.Length; i++)
            {
                raishtoMesh[i].materials[1].color = currentColor;
            }
        }

        Vector3 direction = m_target.position - transform.position;
        direction.y = 0;
        transform.position += direction.normalized * m_speed * Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerCollision(other);
        
    }

    private void PlayerCollision(Collider other)
    {
        if (m_currentState != State.Rotten || other.tag != "Player") return;

        PlayerLife playerLife = other.GetComponent<PlayerLife>();
        playerLife.ChangeLife(-1);
        GetCollect();
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
