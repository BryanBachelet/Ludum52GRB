using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    private float tempsEcoule;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tempsEcoule += Time.deltaTime;
        if(tempsEcoule > 1)
        {
            Destroy(gameObject);
        }
    }
}
