using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum VegetableType
{
    Tomato,
    Carrot,
    Radish,
    Special


}

public class VegetableManager : MonoBehaviour
{
    [SerializeField] private GameObject[] vegetableType = new GameObject[0];

    public GameObject GetVegetable(VegetableType type)
    {
        return vegetableType[(int)type];
    }
}
