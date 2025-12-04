using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private void Awake()
    {
        instance = this;
    }
    public float dashForce = 10f;       
    public float dashDuration = 0.5f;     
    public float dashCooldown = 0.5f;
    public Animator anim;
    private Rigidbody2D rb;
    private bool isDashing = false;
    private bool canDash = true;
    private Vector3 moveInput;
    private Vector3 lastDirection;
    private PlayerHealthController healthController;
    public List<Weapon> unassignedWeapons, assignedWeapons;
    public List<PassiveItem> unassignedPassives, assignedPassives;
    public Dictionary<PassiveItem, int> passiveLevels;
    private Vector3 firstPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthController = GetComponent<PlayerHealthController>();
        firstPosition = transform.position;
        passiveLevels = new Dictionary<PassiveItem, int>();

        if (UIController.instance != null)
        {
            UIController.instance.StartInitialWeaponRoulette();
        }
    }

    public void SetStartingWeapon(Weapon chosenWeapon)
    {
        if (unassignedWeapons.Contains(chosenWeapon))
        {
            // La lógica de AddWeapon
            AddWeapon(chosenWeapon);
        }
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        } 

        // Movimiento normal
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        if (moveInput != Vector3.zero)
        {
            anim.SetBool("isMoving", true);
            lastDirection = moveInput;
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        Vector3 spriteOrientation = transform.localScale;
        if (moveInput.x > 0)
            spriteOrientation.x = Mathf.Abs(spriteOrientation.x);
        else if (moveInput.x < 0)
            spriteOrientation.x = -Mathf.Abs(spriteOrientation.x);
        transform.localScale = spriteOrientation;

        if (Input.GetButtonDown("Jump") && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate(){
        if (!isDashing) {
            rb.velocity = moveInput * PlayerStats.instance.moveSpeed;
        }
    }
    

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        anim.SetTrigger("roll");

        // Dirección del dash
        Vector3 dashDirection = moveInput;
        if (dashDirection == Vector3.zero)
            dashDirection = lastDirection;

        // Aplicar fuerza del dash
        rb.velocity = dashDirection * dashForce;

        // Esperar duración del dash
        yield return new WaitForSeconds(dashDuration);

        // Terminar dash
        rb.velocity = Vector2.zero;
        isDashing = false;

        // Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Die()
    {
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        foreach (Weapon w in assignedWeapons)
        {
            w.gameObject.SetActive(false);
        }
        
        // Limpiar cofres y pickups al morir
        if (EnemySpawner.instance != null)
        {
            EnemySpawner.instance.CleanUpPickupsAndChests();
        }

        this.enabled = false;
    }

    public void AddWeapon(Weapon weaponToAdd)
    {
        if (!assignedWeapons.Contains(weaponToAdd) && unassignedWeapons.Contains(weaponToAdd))
        {
            unassignedWeapons.Remove(weaponToAdd);
            assignedWeapons.Add(weaponToAdd);

            weaponToAdd.stats = new List<WeaponStats>();
            weaponToAdd.stats.Add(weaponToAdd.baseStats);

            weaponToAdd.gameObject.SetActive(true);
            weaponToAdd.weaponLvl = 0;
            weaponToAdd.statsUpdated = true;

            UIController.instance.UpdateInventoryUI();
        }
    }
    
    public void UpgradePassive(PassiveItem passiveToUpgrade, PassiveStatLevel statsToApply)
    {
        int currentLevelIndex = 0; 

        if (unassignedPassives.Contains(passiveToUpgrade))
        {
            unassignedPassives.Remove(passiveToUpgrade);
            assignedPassives.Add(passiveToUpgrade);
            
            passiveLevels[passiveToUpgrade] = 0; 
            currentLevelIndex = 0;
            
            passiveToUpgrade.InitializeStats();
        }
        else
        {
            passiveLevels[passiveToUpgrade]++;
            currentLevelIndex = passiveLevels[passiveToUpgrade];
        }

        passiveToUpgrade.ApplyLevel(statsToApply);
        
    }

    public void SetWeaponsActive(bool active)
    {
        foreach (Weapon w in assignedWeapons)
        {
            w.gameObject.SetActive(active);
        }
    }

    public void ResetPosition()
    {
        transform.position = firstPosition;
        rb.velocity = Vector2.zero;
        if (anim != null)
        {
            anim.SetBool("isMoving", false);
        }
    }
}
