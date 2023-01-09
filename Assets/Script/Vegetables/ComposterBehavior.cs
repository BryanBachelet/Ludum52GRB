using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComposterBehavior : MonoBehaviour
{
    [Header("Composter Parametres")]
    [SerializeField] private int m_vegetableCount;
    
    [SerializeField] private int m_rottenVegetableMax;
    [SerializeField] private int m_rottenVegetableMin;

    [SerializeField] private VegetableType m_type;
    [SerializeField] private GameObject m_groupSeed;

    private int m_seedCondition;

    private void Start()
    {
        m_seedCondition = Random.Range(m_rottenVegetableMin, m_rottenVegetableMax);
    }

    public void AddVegetable(int number)
    {
        m_vegetableCount += number;

        while(m_vegetableCount > m_seedCondition)
        {
            CreateSeed();
            m_vegetableCount -= m_seedCondition;
            m_seedCondition = Random.Range(m_rottenVegetableMin, m_rottenVegetableMax);
        }
    }

    private void CreateSeed()
    {
        GameObject goInstantiate = GameObject.Instantiate(m_groupSeed, transform.position + transform.right * 1.5f, transform.rotation);
        GroupSeed gSeed = goInstantiate.GetComponent<GroupSeed>();
        gSeed.SetupGroupSeed(1, m_type);
    }
}
