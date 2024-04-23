using System.Collections;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : CharacterController
{
    [Header("Health")]
    [SerializeField] private int _maxHealth;
    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }

    [SerializeField] private float _playerDamageCooldown;
    public float PlayerDamageCooldown { get => _playerDamageCooldown; set => _playerDamageCooldown = value; }

    [SerializeField] private UnityEngine.Rendering.Volume _damageVolume;
    public Volume DamageVolume { get => _damageVolume; set => _damageVolume = value; }


    [Header("Walljumping")]
    [SerializeField] private float _horizontalWalljumpForce;
    public float HorizontalWalljumpForce { get => _horizontalWalljumpForce; set => _horizontalWalljumpForce = value; }

    [SerializeField] private float _verticalWalljumpForce;
    public float VerticalWalljumpForce { get => _verticalWalljumpForce; set => _verticalWalljumpForce = value; }

    [SerializeField] private int _walljumpCount;
    public int WalljumpCount { get => _walljumpCount; set => _walljumpCount = value; }

    private int health;
    private bool damagable = true;

    private InputManager inputManager;

    public IEnumerator DamageCoroutineObject { get; private set; }


    public override SimulatedComponent Copy(SimulatedObject destination)
    {
        PlayerController copy = base.Copy(destination) as PlayerController;

        copy.MaxHealth = this.MaxHealth;
        copy.PlayerDamageCooldown = this.PlayerDamageCooldown;
        copy.DamageVolume = this.DamageVolume;

        copy.HorizontalWalljumpForce = this.HorizontalWalljumpForce;
        copy.VerticalWalljumpForce = this.VerticalWalljumpForce;
        copy.WalljumpCount = this.WalljumpCount;

        return copy;
    }

    // Update is called once per frame
    override protected void Start()
    {
        base.Start();
        inputManager = InputManager.Instance;
        health = MaxHealth;

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
        MaxHealth += healthToAdd;
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
        while (i < PlayerDamageCooldown)
        {
            i += Time.deltaTime;
            //This is a nice juice thing, if we want a volume to appear on damage (like red at the sides of the screen)

            if (DamageVolume != null)
                DamageVolume.weight = (PlayerDamageCooldown - i) / PlayerDamageCooldown;
            yield return null;
        }

        damagable = true;
    }

    public void Die()
    {
        PlayerSpawn.Respawn();
        health = MaxHealth;
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.performed)
            gameManager.TogglePause();
    }
}
