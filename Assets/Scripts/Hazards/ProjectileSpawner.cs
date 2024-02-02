using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : SimulatedScript
{
    [SerializeField] float timeBetweenShots;
    [SerializeField] Vector2 launchDirection;
    [SerializeField] GameObject projectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            GameObject gameObject = Instantiate(projectilePrefab);
            gameObject.transform.SetParent(this.transform);
            gameObject.transform.position = this.transform.position;

            if (gameObject.TryGetComponent<Rigidbody2D>(out var body))
            {
                body.velocity = launchDirection;
                Debug.Log("go");
            }

            yield return new WaitForSeconds(timeBetweenShots);
        }
    }
}
