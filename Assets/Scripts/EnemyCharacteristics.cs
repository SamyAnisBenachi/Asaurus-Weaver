using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyCharacteristics : MonoBehaviour
{
    [Header("Enemy Stats")]
    [Range(0, 200)] public float maxHealthEnemy = 100;
    [SerializeField] float currentHealthEnemy;
    [Range(0, 50)] public int dammageEnemy = 15;
    [Range(0, 1000)] public int xpGiven = 50;
    public bool isDead = false;

    [Header("User Interface")]
    public Slider sliderHealthEnemy;

    [Header("Enemy animator")]
    public Animator animatorEnemy;

    [Header("Player detection")]
    public GameObject player;
    public Animator animatorPlayer;

    void Start()
    {
        // Initialize values
        player = GameObject.Find("Player");
        sliderHealthEnemy = this.GetComponentInChildren<Slider>();
        currentHealthEnemy = maxHealthEnemy;
        animatorEnemy = this.GetComponent<Animator>();
        animatorPlayer = GameObject.Find("Player").GetComponent<Animator>();
    }

    void Update()
    {
        if (!isDead)
        {
            // Update Health Bar value
            sliderHealthEnemy.value = currentHealthEnemy / maxHealthEnemy;

            // Check for death
            if (currentHealthEnemy <= 0)
                Die();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealthEnemy -= damage;
    }

    public void Die()
    {
        isDead = true;

        // Set dead animation
        animatorEnemy.SetTrigger("isDead");

        // Disable health bar
        sliderHealthEnemy.gameObject.SetActive(false);

        // Disable Slime colider
        if (this.name.Contains("Slime"))
            this.GetComponent<SphereCollider>().isTrigger = true;

        // Increase the score of the player
        player.GetComponent<GameManager>().NotifyKill(xpGiven);

        // Disable the scripts
        this.GetComponent<EnemyBehavior>().enabled = false;
        this.enabled = false;
    }
}
