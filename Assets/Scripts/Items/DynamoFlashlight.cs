using System.Collections;
using UnityEngine;

public class DynamoFlashlight : MonoBehaviour
{
    /**Configuració de la bateria*/
    //Duració en segons de la bateria(2 minuts)
    public float maxBattery = 120f;
    //Temps necessari de recarrega del 0 al 100%
    public float rechargeTime = 5f;

    /**Controls i referencies*/
    public Light spotlight;
    public KeyCode toggleKey = KeyCode.F;
    public KeyCode rechargeKey = KeyCode.R;//Mantenint apretat

    private float currentBattery;
    private bool isOn = false;
    private bool wantsToBeOn = false;
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
        if (Input.GetKey(rechargeKey))
        {
            RechargeBattery();
            if (isOn)
            {
                TurnOffLight();
            }
        } else
        {
            if(wantsToBeOn && currentBattery > 0 && !isOn)
            {
                TurnOnLight();
            }
            HandleInput();
            if (isOn)
            {
                DrainBattery();
                HandleFlickering();
            }
        } 

    }

    void HandleInput()
    {
        if (Input.GetKeyDown(toggleKey))
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
