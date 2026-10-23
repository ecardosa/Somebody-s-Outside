using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class DynamoFlashlight : MonoBehaviour
{
    /**Configuració de la bateria*/
    //Duració en segons de la bateria(2 minuts)
    public float timeToEmpty = 120f;
    //Temps necessari de recarrega del 0 al 100%
    public float timeToRecharge = 5f;
    /**Intensitat maxima de la llanterna*/
    public float maxIntensity = 1000f;

    /**Controls i referencies*/
    public Light spotlight;
    public KeyCode toggleKey = KeyCode.F;
    public KeyCode rechargeKey = KeyCode.R;//Mantenint apretat

    public TextMeshProUGUI batteryText;

    /**Animació llanterna*/
    private Quaternion initialCrankRotation;
    private float currentCrankAngle = 0f;
    private bool isFinishingCrank = false;
    private float crankVelocity = 0f;
    private float targetCrankAngle = 0f;
    public Transform crankTransform;
    public float crankSpeed = 360f;
    public Vector3 rotationAxis = new Vector3(-1,0,0);

    private float currentBatteryPercentage = 100f;
    private bool isOn = false;
    private bool wantsToBeOn = false;
    private bool isRecharging = false;
    private bool isFlickering = false;

    private bool hasFlickered15 = false;
    private bool hasFlickered10 = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentBatteryPercentage = 100f;
        if (spotlight != null)
        {
            spotlight.enabled = false;
            spotlight.intensity = maxIntensity;
        }

        if(crankTransform != null)
        {
            initialCrankRotation = crankTransform.localRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(rechargeKey))
        {
            if (!isRecharging)
            {
                isRecharging = true;
                isFinishingCrank = false;
                if (isOn) TurnOffLight();
            }
            else
            {
                StopRecharging();
            }
        }

        if (isRecharging)
        {
            RechargeBattery();
            AnimateCrank();

            if (spotlight != null)
            {
                spotlight.enabled = true;
                spotlight.intensity = maxIntensity * (currentBatteryPercentage / 100f);
            }

            if (currentBatteryPercentage >= 100f)
            {
                StopRecharging();
            }
        }
        else
        {
            if (isFinishingCrank)
            {
                FinishCrankRotation();
            }

            if (wantsToBeOn && currentBatteryPercentage > 0 && !isOn)
            {
                TurnOnLight();
            }
            else if (!wantsToBeOn && spotlight != null && spotlight.enabled && !isFinishingCrank)
            {
                TurnOffLight();
            }

            HandleInput();

            if (isOn)
            {
                DrainBattery();
                HandleFlickering();

                if (!isFlickering && spotlight != null)
                {
                    spotlight.intensity = maxIntensity * (currentBatteryPercentage / 100f);
                }
            }
        }

        UpdateUIText();
    }

    void UpdateUIText()
    {
        if (batteryText != null)
        {
            int displayPercentage = Mathf.RoundToInt(currentBatteryPercentage);
            batteryText.text = displayPercentage + "%";
        }
    }

    void StopRecharging()
    {
        isRecharging = false;
        if (crankTransform != null)
        {
            isFinishingCrank = true;
            crankVelocity = crankSpeed;
            if(crankSpeed > 0)
            {
                targetCrankAngle = Mathf.Ceil(currentCrankAngle / 360f) * 360;
            } else
            {
                targetCrankAngle = Mathf.Floor(currentCrankAngle / 360f) * 360f;
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(toggleKey) && !isRecharging)
        {
            wantsToBeOn = !wantsToBeOn; 

            if (wantsToBeOn && currentBatteryPercentage > 0)
            {
                TurnOnLight();
            }
            else
            {
                TurnOffLight();
            }
        }
    }

    void TurnOnLight()
    {
        isOn = true;
        if (spotlight != null)
        {
            spotlight.enabled = true;
        }
    }

    void TurnOffLight()
    {
        isOn = false;
        isFlickering = false;
        if (spotlight != null)
        {
            spotlight.enabled = false;
        }
    }

    void DrainBattery()
    {
        float drainRate = 100f / timeToEmpty;
        currentBatteryPercentage -= drainRate * Time.deltaTime;
        if (currentBatteryPercentage <= 0)
        {
            currentBatteryPercentage = 0;
            TurnOffLight();
        }
    }

    void RechargeBattery()
    {
        float rechargeRate = 100f / timeToRecharge;
        currentBatteryPercentage += rechargeRate * Time.deltaTime;
        if(currentBatteryPercentage > 100f)
        {
            currentBatteryPercentage = 100f;
        }
        if (currentBatteryPercentage > 15f)
        {
            hasFlickered15 = false;
        }
        if (currentBatteryPercentage > 10f)
        {
            hasFlickered10 = false;
        }
    }

    void AnimateCrank()
    {
        if(crankTransform != null)
        {
            currentCrankAngle += crankSpeed * Time.deltaTime;
            crankTransform.localRotation = initialCrankRotation * Quaternion.Euler(rotationAxis * currentCrankAngle);
        }
    }

    void FinishCrankRotation()
    {
        if(crankTransform != null)
        {
            currentCrankAngle = Mathf.SmoothDamp(currentCrankAngle, targetCrankAngle, ref crankVelocity, 0.15f, Mathf.Abs(crankSpeed), Time.deltaTime);          
            if(Mathf.Abs(targetCrankAngle - currentCrankAngle) < 0.5f)
            {
                currentCrankAngle = targetCrankAngle;
            }
            crankTransform.localRotation = initialCrankRotation * Quaternion.Euler(rotationAxis * currentCrankAngle);
            if(Mathf.Approximately(currentCrankAngle, targetCrankAngle))
            {
                isFinishingCrank = false;
                currentCrankAngle = 0f;
                crankTransform.localRotation = initialCrankRotation;
            }
        }
    }

    void HandleFlickering()
    {
        if (isFlickering)
        {
            return;
        }
        if(currentBatteryPercentage <= 15 && currentBatteryPercentage > 10f && !hasFlickered15)
        {
            StartCoroutine(FlickerEffect(1));
            hasFlickered15 = true;
        } else if (currentBatteryPercentage <=10f && currentBatteryPercentage > 5f && !hasFlickered10) {
            StartCoroutine(FlickerEffect(3));
            hasFlickered10= true;
        } else if (currentBatteryPercentage <= 5f && currentBatteryPercentage > 0f)
        {
            spotlight.enabled = Mathf.PerlinNoise(Time.time * 15f, 0f) > 0.5f;
        }
    }

    IEnumerator FlickerEffect(int flickerCount)
    {
        isFlickering = true;

        for (int i = 0; i < flickerCount; i++)
        {
            spotlight.enabled = false;
            yield return new WaitForSeconds(0.15f);
            spotlight.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
        isFlickering = false;
    }
}
