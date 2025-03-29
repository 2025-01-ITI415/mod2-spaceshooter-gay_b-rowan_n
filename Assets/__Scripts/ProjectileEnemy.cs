using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class ProjectileEnemy : MonoBehaviour
{
    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Dynamic")]
    public Rigidbody rigid;
    [SerializeField]
    private eWeaponType _type;

    // This public property masks the private field _type
    public eWeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Destroy if it goes off the bottom of the screen
        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown))
        {
            Destroy(gameObject);
        }
    }

    public void SetType(eWeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(_type);
        rend.material.color = def.projectileColor;
    }

    public Vector3 vel
    {
        get { return rigid.velocity; }
        set { rigid.velocity = value; }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherGO = other.gameObject;

        // Check if it hit the player
        if (otherGO.CompareTag("Hero"))
        {
            Debug.Log("Projectile hit the Hero!");

            // Get the Hero component
            Hero hero = otherGO.GetComponent<Hero>();
            if (hero != null)
            {
                // Damage the player using the SetShieldLevel method
                hero.SetShieldLevel(hero.shieldLevel - 1);
                Debug.Log("Hero shield level: " + hero.shieldLevel);
            }

            // Destroy the projectile
            Destroy(gameObject);
        }
    }
}