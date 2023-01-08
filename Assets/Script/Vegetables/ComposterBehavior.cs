using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComposterBehavior : MonoBehaviour
{
    [SerializeField] private int m_vegetableCount;

    public void AddVegetable(int number)
    {
        m_vegetableCount = number;
    }
}
