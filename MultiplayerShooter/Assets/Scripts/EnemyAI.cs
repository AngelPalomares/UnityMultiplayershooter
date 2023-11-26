using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float shootingRange = 10f;
    [SerializeField] private float shootingDistance = 5f; // Distance threshold to start shooting
    [SerializeField] private float minDistanceToPlayer = 2f; // Minimum distance to keep from the player
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootingInterval = 1f;
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private float bulletSpeed;

    private bool playerNearby = false;
    private bool isShooting = false;
    private float shootingTimer = 0f;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set for EnemyAI script!");
            return;
        }

        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f; // Ignore vertical difference for horizontal movement
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= shootingRange)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (distanceToPlayer <= shootingDistance)
            {
                playerNearby = true;
            }
            else
            {
                playerNearby = false;
            }
        }
        else
        {
            playerNearby = false;
        }

        if (playerNearby)
        {
            // Calculate the maximum distance to move towards the player
            float maxMoveDistance = Mathf.Max(distanceToPlayer - minDistanceToPlayer, 0f);

            // Move towards the player, limited by the maximum move distance
            transform.Translate(directionToPlayer.normalized * Mathf.Min(maxMoveDistance, moveSpeed * Time.deltaTime));
        }

        if (distanceToPlayer <= shootingDistance && !isShooting)
        {
            // Start shooting at the player
            isShooting = true;
            shootingTimer = 0f;
        }

        if (isShooting)
        {
            // Shoot at the player
            shootingTimer += Time.deltaTime;
            if (shootingTimer >= shootingInterval)
            {
                shootingTimer = 0f;
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

        // Calculate the direction towards the player
        Vector3 directionToPlayer = player.position - bulletSpawnPoint.position;

        // Apply a force to the bullet to move it towards the player
        bulletRigidbody.AddForce(directionToPlayer.normalized * bulletSpeed, ForceMode.VelocityChange);

        Destroy(bullet, bulletLifetime);
    }
}
