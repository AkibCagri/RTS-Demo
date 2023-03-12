using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    int currentHealth;
    float fillStep;
    [SerializeField] Image healthBar;
    [SerializeField] float showTime = 3;
    [SerializeField] GameObject healthCanvas;
    Coroutine turnOffCoroutine;

      void Start()
    {
        fillStep = 1f / (float)maxHealth;
        currentHealth = maxHealth;
    }

    // This function decrease health.
    public bool GetDamage(int damage)
    {
        bool isDead = false;
        currentHealth -= damage;
        SetHealtBar();
        if (currentHealth <= 0)
        {
            GetComponent<IDestroyable>().DestroyMe();
            isDead = true;
        }
        return isDead;
    }

    // This function set health bar according to health.
    void SetHealtBar()
    {
        healthCanvas.SetActive(true);
        float fillAmount = currentHealth * fillStep;
        healthBar.fillAmount = fillAmount;

        if (turnOffCoroutine != null)
            StopCoroutine(turnOffCoroutine);
        turnOffCoroutine = StartCoroutine(TurnOffEnum());
    }

    // This coroutine waits for show time before turning off the healt canvas.
    IEnumerator TurnOffEnum()
    {
        yield return new WaitForSeconds(showTime);
        healthCanvas.SetActive(false);
    }
}
