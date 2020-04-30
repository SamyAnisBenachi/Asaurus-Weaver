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

    public bool canLaunchSpecialAttack = false;
    public bool launchSpecialAttack = false;

    public Collider colToCheck;
    public int actualDamageMultiplier;
    int basedamageMultiplier = 1;
    int boostedDamageMultiplier = 3;
    int finalBoostedDamageMultiplier = 6;
    float auraSwordDuration = 6f;

    public AudioSource audioHit;
    public bool touchedEnemy = false;

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
        attackHitbox[0] = GameObject.Find("ColliderSword").GetComponent<Collider>();
        attackHitbox[1] = GameObject.Find("ColliderSpecialSword").GetComponent<Collider>();

        // Set values
        speedForwardOnStart = speedForward;
        speedBackwardOnStart = speedBackward;
        actualDamageMultiplier = basedamageMultiplier;

        // Temp
        foreach (GameObject go in cubeTests)
            go.SetActive(false);

        // Set the collider to check on start
        colToCheck = attackHitbox[0];
    }

    private void Update()
    {
        // Cheat to remove
        if (Input.GetKeyDown(KeyCode.Space))
            playerCharacteristics.currentManaPlayer = 100;

        // Detect player inputs and send them to the animator
        UpdatePlayerInputs();

        // Walking
        UpdateWalking();

        // Rotation
        UpdateRotation();

        // Update the animator State
        UpdateAnimatorCurrentState();

        // Detect if the special attack should be used
        CheckForSpecialAttack();

        // Update the attacks multipliers
        UpdateAttackMultipler();

        // Update the player speed
        UpdateSpeed();

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

        // Update specialAttack value
        animatorPlayer.SetBool("launchSpecialAttack", launchSpecialAttack);
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
    private void DetectMouseInputs()
    {
        // Right click mouse input to get the special sword
        if (Input.GetMouseButton(1) && canLaunchSpecialAttack)
            launchSpecialAttack = true;

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

            // Consume player mana when shielding, not consumed if auraSword activated
            if(!launchSpecialAttack)
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

    private void UpdateAttackMultipler()
    {
        // Action when using final attack
        if (currentState.Contains("Attack04") && launchSpecialAttack)
        {
            // Shake camera
            Invoke("CameraShake", 0.4f);

            // Update damage multiplier
            actualDamageMultiplier = finalBoostedDamageMultiplier;
        }
        else if (launchSpecialAttack)
            actualDamageMultiplier = boostedDamageMultiplier;
        else
            actualDamageMultiplier = basedamageMultiplier;
    }

    private void UpdateSpeed() 
    {
        if (currentState.Contains("Attack"))
        {
            slowPlayerSpeed(true, 0.5f);

            if (actualDamageMultiplier == finalBoostedDamageMultiplier)
                slowPlayerSpeed(true, 0f);
        }
        else
            slowPlayerSpeed(false, 1f);
    }

    private void slowPlayerSpeed(bool shouldSlow, float multiplier)
    {
        if (shouldSlow)
        {
            // Reduce speed when the player is attacking
            speedForward = speedForwardOnStart * multiplier;
            speedBackward = speedBackwardOnStart * multiplier;
        }
        else
        {
            // Set the normal speed when the player is not attacking
            speedForward = speedForwardOnStart;
            speedBackward = speedBackwardOnStart;
        }
    }

    private void CheckForSpecialAttack()
    {
        // Special attack activation
        if (playerCharacteristics.currentManaPlayer == 100)
            canLaunchSpecialAttack = true;

        // Action when using final attack
        if (launchSpecialAttack && canLaunchSpecialAttack)
        {
            canLaunchSpecialAttack = false;
            StartCoroutine(playerCharacteristics.DecreaseMana(100));

            // Appear sword aura
            swordAura.SetActive(true);
            colToCheck = attackHitbox[1];

            // Stop attack after timer
            //StartCoroutine(stopSpecialAttack(auraSwordDuration));
        }

        if(launchSpecialAttack && playerCharacteristics.currentManaPlayer == 0)
            StartCoroutine(stopSpecialAttack(auraSwordDuration)); //mettre une fonction normal todo
    }

    private IEnumerator stopSpecialAttack(float timer) 
    {
        yield return new WaitForSeconds(0f);
        swordAura.SetActive(false);
        launchSpecialAttack = false;
        colToCheck = attackHitbox[0];
    }

    private void CheckDamageAndManaRecover()
    {
        for (int i = 0; i < canAttackRecover.Length; i++)
        {
            if (recordedState != currentState)
            {
                if ((currentState.Contains("Attack0" + (i + 1).ToString()) && recordedState.Contains("Attack") && canAttackRecover[i])
                    || ((!recordedState.Contains("Attack") || recordedState.Contains("Attack04")) && i == 0))
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
        if (currentState.Contains("Attack04") && launchSpecialAttack)
            Invoke("LaunchAttack", 0.35f);
        else
            Invoke("LaunchAttack", 0.05f);

        // Recover mana
        //Invoke("RecoverMana", 0.05f);

        // Visual display to remove
        cubeTests[index].SetActive(true);
        StartCoroutine(closeCube(index));
    }

    private void LaunchAttack()
    {
        // Detect ennemies in range of attack
        Collider[] colliders = Physics.OverlapBox(colToCheck.bounds.center, colToCheck.bounds.extents, colToCheck.transform.rotation, LayerMask.GetMask("Enemy"));

        foreach (Collider collider in colliders)
        {
            // Recover Mana
            if (!touchedEnemy)
                RecoverMana();

            // With this variable to false, the player won't recover mana for each enemy touched, and same for the hitAttackSound
            touchedEnemy = true;

            // Deal damage
            collider.GetComponent<EnemyCharacteristics>().TakeDamage(playerCharacteristics.damagePlayer * actualDamageMultiplier);

            if (launchSpecialAttack)
                collider.GetComponent<Animator>().SetTrigger("getHit");
        }
    }

    private void RecoverMana()
    {
        if (playerCharacteristics.currentManaPlayer < playerCharacteristics.maxManaPlayer && !launchSpecialAttack)
            if (playerCharacteristics.currentManaPlayer + playerCharacteristics.manaToRecover <= playerCharacteristics.maxManaPlayer)
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
