using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : TraversePath
{
    protected override string DefaultVisualComponentName => "Map Controller";

    private readonly float inputThreshold = 0.15f;

    private BoxCollider2D _collider;
    private SpriteRenderer _renderer;
    private Animator _animator;

    public BoxCollider2D Collider => TryAssignReference(ref _collider);
    public SpriteRenderer Renderer => TryAssignReference(ref _renderer);
    public Animator Animator => TryAssignReference(ref _animator);

    private GameManager gameManager;
    private InputManager inputManager;

    private IEnumerator moveCoroutine;
    private List<Vector2> currentPath;

    public override SimulatedComponent Copy(SimulatedObject destination)
    {
        MapController copy = destination.gameObject.AddComponent<MapController>();

        copy.Speed = this.Speed;

        return copy;
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        gameManager = GameManager.Instance;
        inputManager = InputManager.Instance;
        inputManager.OnInteract.AddListener(Interact);
    }

    private void Interact()
    {
        if (!ValidateReferences(Body) || PathManager.Instance == null) return;

        LevelTile tile = PathManager.Instance.GetLevelTileAtPoint(Body.position);
        if (tile)
        {
            gameManager.StartLevel(tile.LevelName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PathManager.Instance == null) return;

        UpdateAnimator();

        UpdateSprite();

        if (inputManager.MoveInput == Vector2.zero) return;

        Vector2 movementDirection = GetMovementDirection(inputManager.MoveInput);

        if (!ValidateReferences(Body)) return;

        if (PathManager.Instance.GetTileAtPoint(Body.position) == null)
        {
            Debug.Log("get outta here");
            StopCoroutine(moveCoroutine);
            Moving = false;
            return;
        }

        // If we move the opposite direction, stop moving so we can turn around
        if (inputManager.MoveInput == -Body.velocity.normalized)
        {
            StopCoroutine(moveCoroutine);
            int startingPoint = currentPath.Count - currentPoint;
            currentPath.Reverse();

            bool loop = currentPath[^1] == currentPath[0] && !PathManager.Instance.IsPositionStoppingPoint(currentPath[0]);
            moveCoroutine = Move(currentPath, startingPoint, loop);
            StartCoroutine(moveCoroutine);
        }

        if (!Moving)
        {
            List<Vector2> path = PathManager.Instance.GetPathFromPoint(Body.position, movementDirection);

            if (path is null) return;

            if (Collider != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    path[i] += (Collider.bounds.size / 3) * Vector2.up;
                }
            }

            currentPath = path;
            bool loop = currentPath[^1] == currentPath[0] && !PathManager.Instance.IsPositionStoppingPoint(currentPath[0]);
            moveCoroutine = Move(currentPath, 0, loop);
            StartCoroutine(moveCoroutine);
        }
    }

    private void UpdateAnimator()
    {
        if (ValidateReferences(Animator))
        {
            Animator.SetFloat("Speed", Body.velocity.magnitude);
        }
    }

    private void UpdateSprite()
    {
        if (Renderer == null) return;
        if (inputManager.MoveInput.x > 0 || (ValidateReferences(Body) && Body.velocity.x > 0))
        {
            Renderer.flipX = true;
        }
        if (inputManager.MoveInput.x < 0 || (ValidateReferences(Body) && Body.velocity.x < 0))
        {
            Renderer.flipX = false;
        }
    }

    // TODO: this sucks ass
    private Vector2 GetMovementDirection(Vector2 inputDirection)
    {
        if (inputDirection.y > inputThreshold)
        {
            return Vector2.up;
        }
        if (inputDirection.x > inputThreshold)
        {
            return Vector2.right;
        }
        if (-inputDirection.y > inputThreshold)
        {
            return Vector2.down;
        }
        if (-inputDirection.x > inputThreshold)
        {
            return Vector2.left;
        }

        return Vector2Int.zero;
    }
}
