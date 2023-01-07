using System;
using System.Collections.Generic;
using UnityEngine;

public class ThrowVegetable : MonoBehaviour
{
    [HideInInspector] public Vector3 directon;
    [SerializeField] private float m_speed;
    [SerializeField] private string m_vegetableTag;


    private void Update()
    {
        transform.position += directon.normalized * m_speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == m_vegetableTag)
        {
            Vegetable vegeCurrent = other.GetComponent<Vegetable>();
            vegeCurrent.GetCollect();
            Destroy(gameObject);
        }
    }

}
