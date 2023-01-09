using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedGenerator : MonoBehaviour
{
    [Header("Generator Parameters")]
    [SerializeField] private VegetableType m_vegetableType;
    [SerializeField] private int m_minSeed = 5;
    [SerializeField] private int m_maxSeed = 15;
    [SerializeField] private float m_reloadTime = 10.0f;
    [SerializeField] private Image m_feedbackCooldown;
    [SerializeField] private GameObject m_groupSeed;


    private BoxCollider[] m_collider;
    private float m_reloadTimer = 0.0f;
    private bool m_IsReady = true;


    private void Start()
    {
        InitComponent();
    }
    public void InitComponent()
    {
        m_collider = GetComponents<BoxCollider>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "Player") return;

        GenerateSeed();
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag != "Player") return;

        GenerateSeed();
    }

    public void Update()
    {
        ReloadGenerator();
    }

    private void ReloadGenerator()
    {
        if (m_IsReady) return;

        m_reloadTimer -= Time.deltaTime;

        UpdateReloadFeedback();

        if (m_reloadTimer >0) return;

        m_IsReady = true;
        GlobalSoundManager.PlayOneShot(8, Vector3.zero);
        m_feedbackCooldown.gameObject.SetActive(false);
        for (int i = 0; i < m_collider.Length; i++)
        {
            m_collider[i].enabled = true;
        }

    }

    private void UpdateReloadFeedback()
    {
        float ratio = 1.0f - m_reloadTimer / m_reloadTime;
        m_feedbackCooldown.fillAmount = ratio;

    }
    private void GenerateSeed()
    {
        if (!m_IsReady) return;

        m_IsReady = false;
        m_reloadTimer = m_reloadTime;
        m_feedbackCooldown.gameObject.SetActive(true);
        GameObject goInstantiate = GameObject.Instantiate(m_groupSeed, transform.position + transform.forward * 1.5f, transform.rotation);
        GroupSeed gSeed = goInstantiate.GetComponent<GroupSeed>();
        int count = Random.Range(m_minSeed, m_maxSeed);
        gSeed.SetupGroupSeed(count, m_vegetableType);

        for (int i = 0; i < m_collider.Length; i++)
        {
            if(m_collider[i].isTrigger)
                m_collider[i].enabled = false;

        }
    }
}
