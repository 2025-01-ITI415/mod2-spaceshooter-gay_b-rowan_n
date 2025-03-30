using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy_0 : Enemy
{
    [Header("Shooting")]
    public GameObject enemy0ProjectilePrefab; // Renamed from enemyProjectilePrefab
    public float enemy0FireRate = 1.5f; // Renamed from fireRate
    private float nextFireTime;
    void Start()
    {
        // Initialize the next fire time
        nextFireTime = Time.time + Random.Range(0.5f, enemy0FireRate);
    }
    public override void Move()
    {
        // Call the base Move method from Enemy
        base.Move();
        // Check if it's time to fire and if this enemy is on screen
        if (Time.time > nextFireTime && bndCheck.isOnScreen)
        {
            FireProjectile();
            nextFireTime = Time.time + enemy0FireRate;
        }
    }
    protected override void FireProjectile()
    {
        if (enemy0ProjectilePrefab == null) return;
        // Create the projectile anchor if it doesn't exist
        if (Weapon.PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            Weapon.PROJECTILE_ANCHOR = go.transform;
        }
        // Instantiate the projectile slightly below the enemy to avoid collision
        Vector3 projectilePosition = transform.position + new Vector3(0, -1, 0);
        GameObject projGO = Instantiate(enemy0ProjectilePrefab, projectilePosition, Quaternion.identity, Weapon.PROJECTILE_ANCHOR);
        ProjectileEnemy proj = projGO.GetComponent<ProjectileEnemy>();
        // Set projectile type and velocity
        proj.type = eWeaponType.blaster;
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(eWeaponType.blaster);
        proj.vel = Vector3.down * def.velocity;
    }
}