using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private Transform m_target;
    private Vector3 m_startPosition;

    public bool isfollowing = false;
    void Start()
    {
        m_startPosition = transform.position - m_target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isfollowing)
        {
            transform.position = m_target.position + m_startPosition;
        }

    }
}
