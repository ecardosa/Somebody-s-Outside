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
    private bool isFlickering = false;

    private bool hasFlickering20 = false;
    private bool hasFlickering10 = false;


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
        HandleInput();

        if (isOn)
        {
            DrainBattery();
            HandleFlickering();
        } else
        {
            //Nomes permet carregar quan esta apagada
            //Paralar si volem fer-ho aixi o no
            RechargeBattery();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(toggleKey) && currentBattery > 0)
        {
            isOn = !isOn;
            if (!isOn)
            {
                spotlight.enabled = false;
                isFlickering = false;
            } else
            {
                spotlight.enabled = true;
            }
        }
    }

    void DrainBattery()
    {
        currentBattery -= Time.deltaTime;
        if (currentBattery <= 0)
        {
            currentBattery = 0;
            isOn = false;
            spotlight.enabled = false;
        }
    }

    void RechargeBattery()
    {
        if (Input.GetKey(rechargeKey))
        {
            float rechargeRate = maxBattery / rechargeTime;
            currentBattery += rechargeRate * Time.deltaTime;
            if (currentBattery > maxBattery)
            {
                currentBattery = maxBattery;
            }
            if (currentBattery > 20f)
            {
                hasFlickering20 = false;
            }
            if (currentBattery > 10f)
            {
                hasFlickering10 = false;
            }
        }
    }

    void HandleFlickering()
    {

    }

    IEnumerator FlickerEffect(int flickerCount)
    {

    }
}
