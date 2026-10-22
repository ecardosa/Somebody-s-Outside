using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class DynamoFlashlight : MonoBehaviour
{
    /**Configuració de la bateria*/
    //Duració en segons de la bateria(2 minuts)
    public float maxBattery = 120f;
    //Temps necessari de recarrega del 0 al 100%
    public float rechargeTime = 5f;
    
    /**Intensitat maxima de la llanterna*/
    public float maxIntensity = 1000f;

    /**Controls i referencies*/
    public Light spotlight;
    public KeyCode toggleKey = KeyCode.F;
    public KeyCode rechargeKey = KeyCode.R;//Mantenint apretat

    public TextMeshProUGUI batteryText;

    /**Animació llanterna*/
    public Transform crankTransform;
    public float crankSpeed = 360f;
    public Vector3 rotationAxis = new Vector3(-1,0,0);

    private float currentBattery;
    private bool isOn = false;
    private bool wantsToBeOn = false;
    private bool isRecharging = false;
    private bool isFlickering = false;

    private bool hasFlickered20 = false;
    private bool hasFlickered10 = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentBattery = maxBattery;
        if (spotlight != null)
        {
            spotlight.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(rechargeKey))
        {
            isRecharging = !isRecharging;
            if (isRecharging && isOn)
            {
                TurnOffLight();
            }
        }
        if (isRecharging)
        {
            RechargeBattery();
            AnimateCrank();

            if (spotlight != null)
            {
                spotlight.enabled = true;
                spotlight.intensity = maxIntensity * (currentBattery / maxBattery);
            }
            if (currentBattery >= maxBattery)
            {
                isRecharging = false;
            }
        }
        else
        {
            if (wantsToBeOn && currentBattery > 0 && !isOn)
            {
                TurnOnLight();
            }
            else if (!wantsToBeOn && spotlight != null && spotlight.enabled)
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
                    spotlight.intensity = maxIntensity * (currentBattery / maxBattery);
                }
            }
        }
        UpdateUIText();
    }

    void UpdateUIText()
    {
        if (batteryText != null)
        {
            int percentage = Mathf.RoundToInt((currentBattery / maxBattery) * 100f);
            batteryText.text = percentage + "%";
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(toggleKey) && !isRecharging)
        {
            wantsToBeOn = !wantsToBeOn; 

            if (wantsToBeOn && currentBattery > 0)
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
        currentBattery -= Time.deltaTime;
        if (currentBattery <= 0)
        {
            currentBattery = 0;
            TurnOffLight();
        }
    }

    void RechargeBattery()
    {
        float rechargeRate = maxBattery / rechargeTime;
        currentBattery += rechargeRate * Time.deltaTime;
        if (currentBattery > maxBattery)
        {
            currentBattery = maxBattery;
        }
        if (currentBattery > 20f)
        {
            hasFlickered20 = false;
        }
        if (currentBattery > 10f)
        {
            hasFlickered10 = false;
        }
    }

    void AnimateCrank()
    {
        if(crankTransform != null)
        {
            crankTransform.Rotate(rotationAxis * crankSpeed * Time.deltaTime);
        }
    }

    void HandleFlickering()
    {
        if (isFlickering)
        {
            return;
        }
        if(currentBattery <= 20 && currentBattery > 10f && !hasFlickered20)
        {
            StartCoroutine(FlickerEffect(1));
            hasFlickered20 = true;
        } else if (currentBattery <=10f && currentBattery > 5f && !hasFlickered10) {
            StartCoroutine(FlickerEffect(3));
            hasFlickered10= true;
        } else if (currentBattery <= 5f && currentBattery > 0f)
        {
            spotlight.enabled = Mathf.PerlinNoise(Time.time * 15f, 0f) > 0.5f;
        } else if (currentBattery > 5f)
        {
            spotlight.enabled = true;
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
