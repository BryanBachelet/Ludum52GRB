using System;
using System.Collections.Generic;
using UnityEngine;

public class ThrowVegetable : MonoBehaviour
{
    [HideInInspector] public Vector3 directon;
    [SerializeField] private float m_speed;
    [SerializeField] private string m_vegetableTag;
    [SerializeField] private float m_lifeTime;


    private void Update()
    {
        transform.position += directon.normalized * m_speed * Time.deltaTime;
        m_lifeTime += Time.deltaTime;
        if (m_lifeTime <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == m_vegetableTag)
        {
            Vegetable vegeCurrent = other.GetComponent<Vegetable>();
            vegeCurrent.GetHit();
            Destroy(gameObject);
        }
    }

}
