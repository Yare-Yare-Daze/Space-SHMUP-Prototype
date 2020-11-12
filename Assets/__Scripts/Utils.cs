using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    //Возвращает список всех материалов в данном игровом объекте и его дочерних объектах
    static public Material[] GetAllMaterials(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        
        List<Material> mats = new List<Material>();
        foreach (Renderer rend in renderers)
        {
            mats.Add(rend.material);
        }

        return (mats.ToArray());
    }
}
