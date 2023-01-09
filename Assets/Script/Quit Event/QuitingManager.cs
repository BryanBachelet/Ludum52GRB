using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuitingManager : MonoBehaviour
{
    [Header("Events Parameters")]
    //[SerializeField] private float m_timeBetweenEvent = 10f;
    [SerializeField] private UnityEvent[] m_quitEvent;
    [SerializeField][TextArea] private string[] m_eventText;
    [SerializeField] private UIDialogue m_dialogue;
   


    private int m_currentEvent = 0;

    public GameObject carrotSeed;
    public GameObject radishSeed;
    public CameraBehavior camScriptBehavior;
    public GameObject limiteTerrain2;
    public void ListenToEvent(int index, UnityAction call)
    {
        m_quitEvent[0].AddListener(call);
    }

    public void LaunchEvent()
    {
        if (m_currentEvent == m_quitEvent.Length) return;

        m_quitEvent[m_currentEvent].Invoke();
        m_currentEvent++;
    }

    public void WriteText()
    {
       StartCoroutine( m_dialogue.WriteText(m_eventText[m_currentEvent]));
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SpawnCarrotObject()
    {
        carrotSeed.SetActive(true);
    }

    public void SpawnRadishObject()
    {
        radishSeed.SetActive(true);
    }

    public void UnlockCamera()
    {
        camScriptBehavior.isfollowing = true;
        limiteTerrain2.SetActive(false);
    }
    public void IncreaseSpeed()
    {

    }
    public void BiggerThrowableObject()
    {

    }
    public void ColorChanging()
    {

    }

}
