using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    public Character currentCharacter;
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;
    public bool isRecovering;

    private void Update()
    {
        if(healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime;
        }

        if (isRecovering)
        {
            float percentage = currentCharacter.currentPower / currentCharacter.maxPower;
            powerImage.fillAmount = percentage;

            if (percentage >= 1)
            {
                isRecovering = false;
                return;
            }
        }
    }
    //调整血量百分比
    public void OnHealthChange(float persentage)
    {
        healthImage.fillAmount = persentage;
    }

    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;
    }
}
