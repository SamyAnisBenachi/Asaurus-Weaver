using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerController : MonoBehaviour
{
    [Header("Character instances")]
    public Animator animatorPlayer;
    public PlayerCharacteristics playerCharacteristics;

    [Header("Movement values")]
    [SerializeField] [Range(5f, 50f)] float speedForward = 15f;
    [SerializeField] [Range(5f, 15f)] float speedBackward = 5f;
    [SerializeField] [Range(50f, 100f)] float speedRotation = 100f;
    float speedForwardOnStart;
    float speedBackwardOnStart;
    float verticalInput;
    float horizontalInput;

    [Header("Weapon aura")]
    public GameObject shieldAura;
    public GameObject swordAura;

    [Header("Attack combo and hit gestion")]
    public bool[] canAttackRecover = new bool[] { true, true, true, true };
    public Collider[] attackHitbox; // = new Collider[] { new Collider() }; //To initialise
    public string currentState;
    public string recordedState;

    // Temp
    public GameObject[] cubeTests;
    public bool didFirstAttack = false;

    void Start()
    {
        // Initialize values
        // Player instances (own)
        animatorPlayer = this.GetComponent<Animator>();
        playerCharacteristics = this.GetComponent<PlayerCharacteristics>();

        // Weapon aura instances
        shieldAura = GameObject.Find("ShieldAura");
        swordAura = GameObject.Find("SwordAura");
        shieldAura.SetActive(false);
        swordAura.SetActive(false);

        // Weapon collider
        attackHitbox[0] = GameObject.Find("ColliderWeapon").GetComponent<Collider>();

        // Set values
        speedForwardOnStart = speedForward;
        speedBackwardOnStart = speedBackward;

        // Temp
        foreach (GameObject go in cubeTests)
            go.SetActive(false);

    }

    private void Update()
    {
        // Detect player inputs and send them to the animator
        UpdatePlayerInputs();

        // Walking
        UpdateWalking();

        // Rotation
        UpdateRotation();

        // Animator State
        UpdateAnimatorCurrentState();

        // Detect if the special attack should be used
        CheckForSpecialAttack();

        // Detect left click mouse input to attack
        DetectMouseInputs();

        // Reset attack bools when the player stop attacking
        ResetAttackBools();
    }

    private void UpdatePlayerInputs()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        // Send player inputs to the animator
        animatorPlayer.SetFloat("vertical", verticalInput);
        animatorPlayer.SetFloat("horizontal", horizontalInput);
    }

    private void UpdateWalking()
    {
        if (verticalInput > 0f)
            this.transform.position += this.transform.forward * speedForward * verticalInput * Time.deltaTime;
        else
            this.transform.position += this.transform.forward * speedBackward * verticalInput * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        this.transform.Rotate(this.transform.up, speedRotation * horizontalInput * Time.deltaTime);
    }

    private void UpdateAnimatorCurrentState()
    {
        currentState = (this.animatorPlayer.GetCurrentAnimatorClipInfo(0))[0].clip.name;
    }

    private void ResetAttackBools()
    {
        if ((!currentState.Contains("Attack")))
        {
            // Reset all attacks
            for (int i = 0; i < canAttackRecover.Length; i++)
                canAttackRecover[i] = true;
        }
    }

    private void CheckForSpecialAttack()
    {
        // Action when using final attack
        if (animatorPlayer.GetCurrentAnimatorStateInfo(0).IsName("Attack04"))
        {
            // Appear sword aura
            swordAura.SetActive(true);

            // Reduce speed when the player is attacking
            speedForward = 0;
            speedBackward = 0;
            Invoke("CameraShake", 0.4f);

            //Need to appear only when mana == 100 for 1 total combo for exemple - ToDo
        }
        else
        {
            // Set the normal speed when the player is not attacking
            speedForward = speedForwardOnStart;
            speedBackward = speedBackwardOnStart;

            // Disappear shield aura
            swordAura.SetActive(false);
        }
    }

    private void DetectMouseInputs()
    {
        // Right click mouse input to defend
        if (Input.GetMouseButton(0))
        {
            // Play attack animation
            animatorPlayer.SetTrigger("attack");

            // Check the booleans values to see if an attack should deal dammage and recover mana
            CheckDamageAndManaRecover();
        }

        // Left click mouse input to defend
        if (Input.GetMouseButton(1) && playerCharacteristics.currentManaPlayer > 0)
        {
            // Play defend animation
            animatorPlayer.SetBool("isDefending", true);

            // Appear shield aura
            shieldAura.SetActive(true);

            // Consume player mana when shielding
            playerCharacteristics.currentManaPlayer--;
        }
        else
        {
            // Stop defend animation
            animatorPlayer.SetBool("isDefending", false);

            // Disappear shield aura
            shieldAura.SetActive(false);
        }
    }

    private void CheckDamageAndManaRecover()
    {
        for (int i = 0; i < canAttackRecover.Length; i++)
        {
            if (recordedState != currentState)
            {
                if ((animatorPlayer.GetCurrentAnimatorStateInfo(0).IsName("Attack0" + (i + 1).ToString())
                    && recordedState.Contains("Attack") && canAttackRecover[i])
                    || ((!recordedState.Contains("Attack") || recordedState == "Attack04") && i == 0))
                {
                    CompleteAttack(i);
                }
            }
            else
            {
                // Need to consider the case when the player is doing the same thing, so recordedState == currentState - ToDo
            }
        }
        recordedState = currentState;
    }

    private void CompleteAttack(int index)
    {
        // Reset all attacks
        for (int i = 0; i < canAttackRecover.Length; i++)
            canAttackRecover[i] = true;

        // Block the attack that has just be done to not spam click
        canAttackRecover[index] = false;

        // Deal attack damage
        if (currentState == "Attack04")
            Invoke("LaunchAttack", 0.35f);
        else
            Invoke("LaunchAttack", 0.05f);

        // Recover mana
        Invoke("RecoverMana", 0.05f);

        // Visual display to remove
        cubeTests[index].SetActive(true);
        StartCoroutine(closeCube(index));
    }

    private void LaunchAttack()
    {
        // Get the collider to check
        Collider colToCheck = attackHitbox[0];

        // Detect ennemies in range of attack
        Collider[] colliders = Physics.OverlapBox(colToCheck.bounds.center, colToCheck.bounds.extents, colToCheck.transform.rotation, LayerMask.GetMask("Enemy"));

        foreach (Collider collider in colliders)
        {
            collider.GetComponent<EnemyCharacteristics>().TakeDamage(playerCharacteristics.damagePlayer);
        }
    }

    private void RecoverMana()
    {
        if (playerCharacteristics.currentManaPlayer < playerCharacteristics.maxManaPlayer)
            if (playerCharacteristics.maxManaPlayer + playerCharacteristics.manaToRecover > playerCharacteristics.maxManaPlayer)
                playerCharacteristics.currentManaPlayer += playerCharacteristics.manaToRecover;
            else
                playerCharacteristics.currentManaPlayer = playerCharacteristics.maxManaPlayer;

        // Need to consider the fact that it should only regen when it its an enemy - ToDo
    }
  
    private void CameraShake()
    {
        CameraShaker.Instance.ShakeOnce(10f, 0.1f, 0.1f, 0.1f);
    }

    // Temp // Need to replace with combo text
    IEnumerator closeCube(int i)
    {
        yield return new WaitForSeconds(0.2f);
        cubeTests[i].SetActive(false);
    }
}
