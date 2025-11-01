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
    //public Weapon activeWeapon;
    public List<Weapon> unassignedWeapons, assignedWeapons;
    public List<PassiveItem> unassignedPassives, assignedPassives;
    public Dictionary<PassiveItem, int> passiveLevels;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthController = GetComponent<PlayerHealthController>();

        passiveLevels = new Dictionary<PassiveItem, int>();

        foreach (Weapon w in assignedWeapons)
        {
            w.GenerateLevelPath();
        }
        foreach (Weapon w in unassignedWeapons)
        {
            w.GenerateLevelPath();
        }
        if (unassignedWeapons.Count > 0)
        {
            int randomWeaponIndex = Random.Range(0, unassignedWeapons.Count);
            AddWeapon(unassignedWeapons[randomWeaponIndex]);
        }
        foreach (PassiveItem p in unassignedPassives)
        {
            p.GenerateLevelPath();
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
        this.enabled = false;
    }

    public void AddWeapon(Weapon weaponToAdd)
    {
        if (unassignedWeapons.Contains(weaponToAdd))
        {
            assignedWeapons.Add(weaponToAdd);
            weaponToAdd.gameObject.SetActive(true);
            unassignedWeapons.Remove(weaponToAdd);
            weaponToAdd.weaponLvl = 0;
            weaponToAdd.statsUpdated = true;

            UIController.instance.UpdateInventoryUI();
        }
    }
    
    public void UpgradePassive(PassiveItem passiveToUpgrade)
    {
        int currentLevel = 0; 

        if (unassignedPassives.Contains(passiveToUpgrade))
        {
            unassignedPassives.Remove(passiveToUpgrade);
            assignedPassives.Add(passiveToUpgrade);
            
            passiveLevels[passiveToUpgrade] = 0; 
            currentLevel = 0;
        }
        else
        {
            passiveLevels[passiveToUpgrade]++;
            currentLevel = passiveLevels[passiveToUpgrade];
        }

        PlayerStats.instance.ApplyStatsForPassive(passiveToUpgrade, currentLevel);
        UIController.instance.UpdateInventoryUI();
    }
}
