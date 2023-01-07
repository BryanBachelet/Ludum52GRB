using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class vfxBinder_Parameter : MonoBehaviour
{
    [SerializeField] private float timeMoisie;
    [SerializeField] public float timeToMoisie = 10;
    public bool activeMoisie = false;

    public float timeProperty;

    private VisualEffect dirt_vfx;
    // Start is called before the first frame update
    void Start()
    {
        dirt_vfx = GetComponentInChildren<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if(activeMoisie && timeMoisie < 1)
        {
            timeMoisie += Time.deltaTime / timeToMoisie;
            dirt_vfx.SetFloat("timeProperty", timeMoisie);
        }
    }
}
