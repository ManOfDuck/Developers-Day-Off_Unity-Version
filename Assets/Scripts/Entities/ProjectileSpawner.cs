using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : SimulatedScript
{
    [SerializeField] private float _timeBetweenShots;
    public float TimeBetweenShots { get => _timeBetweenShots; set => _timeBetweenShots = value; }

    [SerializeField] private Vector2 _launchDirection;
    public Vector2 LaunchDirection { get => _launchDirection; set => _launchDirection = value; }

    [SerializeField] private GameObject _projectilePrefab;
    public GameObject ProjectilePrefab { get => _projectilePrefab; set => _projectilePrefab = value; }

    protected override string DefaultVisualComponentName => "ProjectileSpawner";


    public override SimulatedComponent Copy(SimulatedObject destination)
    {
        ProjectileSpawner copy = destination.gameObject.AddComponent<ProjectileSpawner>();

        copy.TimeBetweenShots = this.TimeBetweenShots;
        copy.LaunchDirection = this.LaunchDirection;
        copy.ProjectilePrefab = this.ProjectilePrefab;

        return copy;
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();

        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            while (!DoCoroutines)
            {
                yield return null;
            }
            GameObject gameObject = Instantiate(ProjectilePrefab);
            gameObject.transform.SetParent(this.transform);
            gameObject.transform.position = this.transform.position;

            if (gameObject.TryGetComponent<Rigidbody2D>(out var body))
            {
                body.velocity = LaunchDirection;
            }

            yield return new WaitForSeconds(TimeBetweenShots);
        }
    }
}
 