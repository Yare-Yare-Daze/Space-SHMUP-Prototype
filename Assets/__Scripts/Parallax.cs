using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Set in Inspector")] 
    public GameObject poi;
    public GameObject[] panels;
    public float scrollSpeed = -30f;
    public float motionMult = 0.25f;

    private float panelHeight;
    private float depth;

    private void Start()
    {
        panelHeight = panels[0].transform.localScale.y;
        depth = panels[0].transform.localScale.z;
        
        panels[0].transform.position = new Vector3(0,0,depth);
        panels[1].transform.position = new Vector3(0, panelHeight, depth);
    }

    private void Update()
    {
        float transformY, transformX = 0;
        transformY = Time.time * scrollSpeed % panelHeight + (panelHeight * 0.5f);

        if (poi != null)
        {
            transformX = poi.transform.position.x * motionMult;
        }
        
        panels[0].transform.position = new Vector3(transformX, transformY, depth);
        if (transformY >= 0)
        {
            panels[1].transform.position = new Vector3(transformX, transformY - panelHeight, depth);
        }
        else
        {
            panels[1].transform.position = new Vector3(transformX, transformY + panelHeight, depth);
        }
    }
}
