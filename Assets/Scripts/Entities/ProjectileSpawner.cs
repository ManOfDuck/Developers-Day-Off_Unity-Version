using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : SimulatedScript
{
    public enum Direction
    {
        Up,
        Down,
        Backward,
        Forward
    }

    [SerializeField] private float _timeBetweenShots;
    public float TimeBetweenShots { get => _timeBetweenShots; set => _timeBetweenShots = value; }

    [SerializeField] private Direction _launchDirection;
    public Direction LaunchDirection { get => _launchDirection; set => _launchDirection = value; }

    [SerializeField] private float _launchSpeed;
    public float LaunchSpeed { get => _launchSpeed; set => _launchSpeed = value; }

    [SerializeField] private GameObject _projectilePrefab;
    public GameObject ProjectilePrefab { get => _projectilePrefab; set => _projectilePrefab = value; }

    private BoxCollider2D _collider;
    private BoxCollider2D Collider => TryAssignReference(ref _collider);

    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer SpriteRenderer => TryAssignReference(ref _spriteRenderer);

    private Rigidbody2D _body;
    private Rigidbody2D Body => TryAssignReference(ref _body);

    private Vector2 LaunchVector
    {
        get
        {
            Vector2 vector;
            switch (LaunchDirection)
            {
                case Direction.Up:
                    vector = Vector2.up;
                    break;
                case Direction.Down:
                    vector = Vector2.down;
                    break;
                case Direction.Backward:
                    vector = Vector2.left;
                    break;
                default:
                case Direction.Forward:
                    vector = Vector2.right;
                    break;
            }

            vector *= this.transform.right;

            if (SpriteRenderer != null && !SpriteRenderer.flipX)
            {
                vector *= -1;
            }

            return vector;
        }
    }

    protected override string DefaultVisualComponentName => "ProjectileSpawner";


    public override SimulatedComponent Copy(SimulatedObject destination)
    {
        ProjectileSpawner copy = destination.gameObject.AddComponent<ProjectileSpawner>();

        copy.TimeBetweenShots = this.TimeBetweenShots;
        copy.LaunchDirection = this.LaunchDirection;
        copy.LaunchSpeed = this.LaunchSpeed;
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

            yield return new WaitForSeconds(TimeBetweenShots);

            Fire();
        }
    }

    private void Fire()
    {
        GameObject projectile = Instantiate(ProjectilePrefab);

        float launchOffset = CalculateOffset(projectile);

        projectile.transform.position = this.transform.position + (Vector3) (LaunchVector * launchOffset);

        if (projectile.TryGetComponent<Rigidbody2D>(out var projectileBody))
        {
            projectileBody.velocity = LaunchVector * LaunchSpeed;
            if (ValidateReferences(Body))
            {
                projectileBody.velocity += Vector3.Dot(LaunchVector, Body.velocity) * LaunchVector;
            }
        }
    }

    private float CalculateOffset(GameObject projectile)
    {
        float launchOffset = 0;

        if (Collider != null && projectile.TryGetComponent<Collider2D>(out var ProjectileCollider))
        {
            float xOffset = Collider.bounds.size.x / 2 + ProjectileCollider.bounds.size.x / 2;
            float yOffset = Collider.bounds.size.y / 2 + ProjectileCollider.bounds.size.y / 2;


            switch (LaunchDirection)
            {
                case Direction.Up:
                case Direction.Down:
                    launchOffset = yOffset;
                    break;
                case Direction.Backward:
                case Direction.Forward:
                    launchOffset = xOffset;
                    break;
            }

            launchOffset += 0.05f;
        }

        return launchOffset;
    }
}
 