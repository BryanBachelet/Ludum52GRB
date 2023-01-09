using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    [Header("Life Parameter")] 
    [SerializeField] private int m_maxLife;
    [SerializeField] private int m_currentLife;

    [Header("Life Feedback")]
    [SerializeField] private Image m_lifeFeedbackImg;


    public void ChangeLife(int entry)
    {
        m_currentLife += entry;
        m_currentLife = Mathf.Clamp(m_currentLife, 0, m_maxLife);
        m_lifeFeedbackImg.fillAmount = ((float)m_currentLife / (float)m_maxLife);
    }

}
