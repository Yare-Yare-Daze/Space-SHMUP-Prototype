using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Part
{
    public string name; // Имя этой части
    public float health; // Степень стойкости этой части
    public string[] protectedBy; // Другие части, защищающие эту

    [HideInInspector] public GameObject go;
    [HideInInspector] public Material mat;
}

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")] 
    public Part[] parts;
    
    private Vector3 p0, p1;
    private float timeStart;
    private float duration = 4;

    private void Start()
    {
        p0 = p1 = pos;
        InitMovement();

        Transform t;
        foreach (Part part in parts)
        {
            t = transform.Find(part.name);
            if (t != null)
            {
                part.go = t.gameObject;
                part.mat = part.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement()
    {
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }

    private Part FindPart(string n)
    {
        foreach (Part part in parts)
        {
            if (part.name == n)
            {
                return part;
            }
        }

        return null;
    }
    
    private Part FindPart(GameObject go)
    {
        foreach (Part part in parts)
        {
            if (part.go == go)
            {
                return part;
            }
        }

        return null;
    }

    private bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }

    private bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }

    private bool Destroyed(Part part)
    {
        if (part == null)
        {
            return true;
        }

        return (part.health <= 0);
    }

    private void ShowLocalizeDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part partHit = FindPart(goHit);
                if (partHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    partHit = FindPart(goHit);
                }

                if (partHit.protectedBy != null)
                {
                    foreach (string s in partHit.protectedBy)
                    {
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }

                partHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowLocalizeDamage(partHit.mat);
                if (partHit.health <= 0)
                {
                    partHit.go.SetActive(false);
                }

                bool allDestroyed = true;
                foreach (Part part in parts)
                {
                    if (!Destroyed(part))
                    {
                        allDestroyed = false;
                        break;
                    }
                }

                if (allDestroyed)
                {
                    Main.S.ShipDestroed(this);
                    Destroy(this.gameObject);
                }

                Destroy(other);
                break;
            
        }
    }
}
