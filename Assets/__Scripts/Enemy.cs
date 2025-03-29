using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour
{
    [Header("General Settings")]
    public float speed = 20f;   // Movement speed in meters per second
    public float health = 10;   // Damage needed to destroy this enemy
    public int score = 100;     // Points earned for destroying this enemy
    public float powerUpDropChance = 1f;

    [Header("Shooting")]
    public bool canShoot = false;
    public GameObject enemyProjectilePrefab;
    public float fireRate = 1.5f; // Seconds between shots
    private float nextShotTime;

    protected BoundsCheck bndCheck;
    protected bool calledShipDestroyed = false;

    // Property to get and set enemy position
    public Vector3 pos
    {
        get => transform.position;
        set => transform.position = value;
    }

    protected virtual void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();

        // Initialize shooting timer with some randomness
        nextShotTime = Time.time + Random.Range(0.5f, fireRate);
    }

    protected virtual void Update()
    {
        Move();

        // Check if the enemy can shoot and if it's time to fire
        if (canShoot && Time.time > nextShotTime && bndCheck.isOnScreen)
        {
            FireProjectile();
            nextShotTime = Time.time + fireRate;
        }

        // Destroy the enemy if it moves off the bottom of the screen
        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown))
        {
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        // Default movement: move downward
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    protected virtual void FireProjectile()
    {
        if (enemyProjectilePrefab == null) return;

        // Ensure projectile anchor exists
        if (Weapon.PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            Weapon.PROJECTILE_ANCHOR = go.transform;
        }

        // Get weapon definition
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(eWeaponType.blaster);

        // Instantiate the projectile
        GameObject projGO = Instantiate(enemyProjectilePrefab, Weapon.PROJECTILE_ANCHOR);

        // Position it slightly below the enemy to avoid self-collision
        Vector3 position = transform.position;
        position.y -= 0.5f;
        projGO.transform.position = position;

        // Configure projectile
        ProjectileEnemy proj = projGO.GetComponent<ProjectileEnemy>();
        proj.type = eWeaponType.blaster;

        // Set velocity from weapon definition
        proj.vel = Vector3.down * def.velocity;
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;

        // Check for collisions with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            // Only damage this Enemy if it’s on screen
            if (bndCheck.isOnScreen)
            {
                // Get the damage amount from the weapon dictionary
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if (health <= 0 && !calledShipDestroyed)
                {
                    calledShipDestroyed = true;
                    Main.SHIP_DESTROYED(this);
                    Destroy(gameObject);
                }
            }
            // Destroy the projectile regardless
            Destroy(otherGO);
        }
        else
        {
            Debug.Log("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
