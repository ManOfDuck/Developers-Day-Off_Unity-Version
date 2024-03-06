using System.Collections;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : CharacterController
{
    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private float playerDamageCooldown;
    [SerializeField] private UnityEngine.Rendering.Volume damageVolume;

    [Header("Walljumping")]
    [SerializeField] private float horizontalWalljumpForce;
    [SerializeField] private float verticalWalljumpForce;
    [SerializeField] private int walljumpCount;

    int health;
    bool damagable = true;

    private InputManager inputManager;

    public IEnumerator DamageCoroutineObject { get; private set; }

    // Update is called once per frame
    override protected void Start()
    {
        base.Start();
        inputManager = InputManager.Instance;
        health = maxHealth;

        inputManager.OnJumpReleased.AddListener(CancelJump);
    }

    override protected void FixedUpdate()
    {
        base.FixedUpdate();

        Move(inputManager.moveInput);

        if (inputManager.jumpBufferActive)
        {
            bool jumpSucceeded = TryJump();
            if (jumpSucceeded)
            {
                inputManager.ConsumeJumpInput();
            }
        }
        else
        {
            
        }
    }

    public void GoRMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            gameManager.ResetScene();
        }
    }

    public void Heal(int healthToAdd)
    {
        maxHealth += healthToAdd;
    }

    public void Hurt(int damage)
    {
        if (damagable)
        {
            DamageCoroutineObject = DoDamageCooldown();
            StartCoroutine(DamageCoroutineObject);

            health -= damage;

            if (health <= 0)
            {
                Die();
            }
        }
    }

    protected virtual IEnumerator DoDamageCooldown()
    {
        damagable = false;

        float i = 0;
        while (i < playerDamageCooldown)
        {
            i += Time.deltaTime;
            //This is a nice juice thing, if we want a volume to appear on damage (like red at the sides of the screen)

            if (damageVolume != null)
                damageVolume.weight = (playerDamageCooldown - i) / playerDamageCooldown;
            yield return null;
        }

        damagable = true;
    }

    public void Die()
    {
        PlayerSpawn.Respawn();
        health = maxHealth;
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.performed)
            gameManager.TogglePause();
    }
}
