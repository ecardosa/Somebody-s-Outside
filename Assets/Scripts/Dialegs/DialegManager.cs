using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections;

public class DialegManager : MonoBehaviour
{
    [Header("Arxiu JSON d'Ink")]
    public TextAsset inkAsset;

    [Header("Interfície d'Usuari")]
    public TextMeshProUGUI textDialeg;
    public float tempsVisible = 4f;
    public float velocitatLletres = 0.04f; // Nou: temps de pausa entre cada lletra

    private Story story;

    void Start()
    {
        if (inkAsset != null)
        {
            story = new Story(inkAsset.text);
        }

        if (textDialeg != null) textDialeg.text = "";
    }

    public void ReproduirAvisBateria(int percentatge)
    {
        Debug.Log("AVÍS REBUT AL DIALEG MANAGER: " + percentatge + "%"); // <-- Afegeix això

        story.variablesState["bateria"] = percentatge;
        // ... resta del codi
        story.ChoosePathString("avis_bateria");

        string textMostrar = "";
        while (story.canContinue)
        {
            textMostrar += story.Continue();
        }

        if (!string.IsNullOrWhiteSpace(textMostrar))
        {
            StopAllCoroutines();
            StartCoroutine(MostrarTextAUI(textMostrar.Trim()));
        }
    }

    IEnumerator MostrarTextAUI(string text)
    {
        textDialeg.text = ""; // Ens assegurem que el calaix de text estigui buit

        // Bucle que escriu lletra a lletra
        foreach (char lletra in text.ToCharArray())
        {
            textDialeg.text += lletra;
            yield return new WaitForSeconds(velocitatLletres);
        }

        // Un cop ha acabat d'escriure tota la frase, espera els segons configurats
        yield return new WaitForSeconds(tempsVisible);
        textDialeg.text = "";
    }
}