using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Level up system")]
    public int enemyKilled = 0;
    public float xpTotal = 0;
    public int actualLevel = 1;
    public int xpNextLevelUp = 100;

    [Header("User Interface")]
    public Slider xpBar;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI levelText;

    [Header("Audio")]
    public AudioSource levelUpSound; //todo

    void Start()
    {
        // Initialize values
        // User interface instances
        xpBar = GameObject.Find("XpSlider").GetComponent<Slider>();
        xpText = GameObject.Find("XpText").GetComponent<TextMeshProUGUI>();
        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Update UI
        xpBar.value = xpTotal / xpNextLevelUp;
        xpText.text = " XP : " + xpTotal.ToString() + " / " + xpNextLevelUp.ToString();
        levelText.text = actualLevel.ToString();

        // Check for level up
        if (xpTotal >= xpNextLevelUp)
            LevelUp();
    }

    // Get the infos of the enemy that were killed by the player
    public void NotifyKill(int experienceGiven)
    {
        enemyKilled += 1;   
        StartCoroutine(IncreaseXP(experienceGiven));
    }

    // Increase or decrease a value incrementally to move bar up slowly instead of chunks
    public IEnumerator IncreaseXP(int xpGiven)
    {
        for (int i = 0; i < xpGiven; i++)
        {
            xpTotal++;
            yield return new WaitForSeconds(0.001f);
        }
    }

    private void LevelUp()
    {
        actualLevel++;
        xpTotal = 0;
        xpNextLevelUp = (int)(xpNextLevelUp * 1.25f);
    }
}
