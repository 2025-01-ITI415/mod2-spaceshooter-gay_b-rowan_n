using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject enemyProjectilePrefab;
    public float fireRate = 1.5f; // Seconds between shots

    [Header("Dynamic")]
    public eWeaponType type = eWeaponType.blaster;
    private float nextShotTime;

    void Start()
    {
        // Set initial shot time with some randomness
        nextShotTime = Time.time + Random.Range(0.1f, fireRate);
    }

    void Update()
    {
        // Check if it's time to fire
        if (Time.time > nextShotTime)
        {
            FireProjectile();
            nextShotTime = Time.time + fireRate;
        }
    }

    void FireProjectile()
    {
        // Create the projectile anchor if it doesn't exist
        if (Weapon.PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            Weapon.PROJECTILE_ANCHOR = go.transform;
        }

        // Get weapon definition from Main
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(type);

        // Instantiate the projectile
        GameObject projGO = Instantiate<GameObject>(enemyProjectilePrefab, Weapon.PROJECTILE_ANCHOR);

        // Position it slightly below the enemy to avoid self-collision
        Vector3 position = transform.position;
        position.y -= 0.5f; // Offset to avoid collision with the enemy
        projGO.transform.position = position;

        // Configure projectile
        ProjectileEnemy proj = projGO.GetComponent<ProjectileEnemy>();
        proj.type = type;

        // Set velocity from weapon definition
        proj.vel = Vector3.down * def.velocity;
    }
}