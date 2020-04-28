using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerCharacteristics : MonoBehaviour
{
    [Header("Player Stats")]
    [Range(1, 10000)] public float maxHealthPlayer = 1000;
    public float currentHealthPlayer;
    [Range(1, 500)] public float maxManaPlayer = 100;
    public float currentManaPlayer;
    [Range(0, 50)] public int damagePlayer = 5;
    [Range(0, 50)] public int manaToRecover = 5;
    public bool isDead = false;

    [Header("User Interface")]
    public Slider hpBar;
    public TextMeshProUGUI hpText;
    public Slider manaBar;
    public TextMeshProUGUI manaText;

    void Start()
    {
        // Initialize values
        // User interface instances
        hpBar = GameObject.Find("HpSlider").GetComponent<Slider>();
        hpText = GameObject.Find("HpText").GetComponent<TextMeshProUGUI>();
        manaBar = GameObject.Find("ManaSlider").GetComponent<Slider>();
        manaText = GameObject.Find("ManaText").GetComponent<TextMeshProUGUI>();

        // Set values
        currentHealthPlayer = maxHealthPlayer;
        currentManaPlayer = 0;
    }

    void Update()
    {
        // Update health value on the UI
        hpBar.value = currentHealthPlayer / maxHealthPlayer;
        hpText.text = " HP : " + currentHealthPlayer.ToString() + " / " + maxHealthPlayer.ToString();

        // Update Mana value on the UI
        manaBar.value = currentManaPlayer / maxManaPlayer;
        manaText.text = " MANA : " + currentManaPlayer.ToString() + " / " + maxManaPlayer.ToString();

        if (currentHealthPlayer <= 0)
            isDead = true;

        if(Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(DecreaseHP(1000));
    }

    private void OnTriggerEnter(Collider other) // ToDo Tempo
    {
        // Detect Enemy attack
        if (other.CompareTag("Enemy"))
        {
            //Debug.Log("Got hit by an enemy ! Health remaing : " + currentHealthPlayer);
            StartCoroutine(DecreaseHP(other.GetComponent<EnemyCharacteristics>().dammageEnemy));
        }
    }

    public IEnumerator DecreaseHP(int dammageTaken)
    {
        for (int i = 0; i < dammageTaken; i++)
        {
            currentHealthPlayer--;
            yield return new WaitForSeconds(0.001f);
        }
    }
}
