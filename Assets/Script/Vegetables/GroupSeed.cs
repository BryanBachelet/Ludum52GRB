using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSeed : MonoBehaviour
{
    public Seed[] allSeed;

    public void SetupGroupSeed(int count, VegetableType type)
    {
        allSeed = new Seed[count];
        for (int i = 0; i < count; i++)
        {
            allSeed[i] = new Seed(type);
        }
    }

    
}
