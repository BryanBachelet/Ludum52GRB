using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogue : MonoBehaviour
{
    [SerializeField] private Text m_text;
    [SerializeField] private float m_timeForLetter = 0.3f;

    private float m_timerForLetter;
    private int m_currentLetter;

    private void Awake()
    {
        m_text = GetComponent<Text>();
    }

    public IEnumerator WriteText(string text)
    {
        m_text.text = null;
        m_currentLetter = 0;
        while (m_text.text != text)
        {

            m_timerForLetter += Time.deltaTime;
            if (m_timerForLetter > m_timeForLetter)
            {
                m_text.text += text[m_currentLetter];
                m_currentLetter++;
                m_timerForLetter = 0;
            }
            yield return Time.deltaTime;
        }
        yield return null;

    }
}
