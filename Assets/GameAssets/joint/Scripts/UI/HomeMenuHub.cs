using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HomeMenuHub : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;

    public Color normalTextColor = Color.black;
    public Color hoverTextColor = Color.blue;
    public Color specialHoverTextColor = Color.red; // Couleur spéciale pour le bouton 6
    public Color outlineColor = Color.white;

    public GameObject canvas; // Référence au Canvas principal
    public GameObject canvasCredit; // Référence au Canvas de crédit
    public Camera mainCamera; // Référence à la caméra principale
    public Transform targetPoint; // Point cible pour la caméra

    public float scrollDuration = 2f; // Durée du défilement en secondes

    private Vector3 initialCameraPosition; // Position initiale de la caméra
    private Coroutine scrollCoroutine; // Référence à la coroutine de scrolling
    private bool isScrolling = false; // Indique si l'animation est en cours

    void Start()
    {
        // Stocker la position initiale de la caméra
        initialCameraPosition = mainCamera.transform.position;

        button1.onClick.AddListener(Button1Clicked);
        button2.onClick.AddListener(Button2Clicked);
        button3.onClick.AddListener(Button3Clicked);
        button4.onClick.AddListener(Button4Clicked);
        button5.onClick.AddListener(Button5Clicked);
        button6.onClick.AddListener(Button6Clicked);

        AddHoverEffects(button1, hoverTextColor);
        AddHoverEffects(button2, hoverTextColor);
        AddHoverEffects(button3, hoverTextColor);
        AddHoverEffects(button4, hoverTextColor);
        AddHoverEffects(button5, hoverTextColor);
        AddHoverEffects(button6, specialHoverTextColor); // Utilisation de la couleur rouge pour le bouton 6
    }

    void Update()
    {
        // Vérifier si la touche Échap est pressée
        if (Input.GetKeyDown(KeyCode.Escape) && canvasCredit.activeSelf)
        {
            // Arrêter l'animation de scrolling si elle est en cours
            if (isScrolling && scrollCoroutine != null)
            {
                StopCoroutine(scrollCoroutine);
                isScrolling = false;
            }

            // Revenir instantanément au Canvas principal et repositionner la caméra
            mainCamera.transform.position = initialCameraPosition;
            canvasCredit.SetActive(false);
            canvas.SetActive(true);
            Debug.Log("Returned to main canvas via Escape key.");
        }
    }

    void Button1Clicked() { Debug.Log("Button 1 clicked!"); }
    void Button2Clicked() { Debug.Log("Button 2 clicked!"); }
    void Button3Clicked() { Debug.Log("Button 3 clicked!"); }
    void Button4Clicked() { Debug.Log("Button 4 clicked!"); }

    void Button5Clicked()
    {
        if (canvas != null && canvasCredit != null)
        {
            canvas.SetActive(false);
            canvasCredit.SetActive(true);
            scrollCoroutine = StartCoroutine(HandleCreditSequence());
        }
    }

    void Button6Clicked() { Application.Quit(); }

    IEnumerator HandleCreditSequence()
    {
        isScrolling = true; // Indiquer que l'animation commence

        // Attendre 2 secondes avant de commencer l'animation
        yield return new WaitForSeconds(2f);

        // Faire défiler la caméra jusqu'au point cible
        Vector3 targetPosition = new Vector3(targetPoint.position.x, targetPoint.position.y, mainCamera.transform.position.z);

        float elapsedTime = 0f;

        while (elapsedTime < scrollDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(initialCameraPosition, targetPosition, elapsedTime / scrollDuration);
            elapsedTime += Time.deltaTime;
            yield return null;

            // Si Échap est pressée pendant l'animation, sortir immédiatement
            if (!isScrolling) yield break;
        }

        // Assurer que la caméra atteint la position finale
        mainCamera.transform.position = targetPosition;

        // Attendre 2 secondes à la position cible
        yield return new WaitForSeconds(2f);

        // Revenir instantanément à la position initiale
        mainCamera.transform.position = initialCameraPosition;

        // Masquer le Canvas de crédit et afficher le Canvas principal
        canvasCredit.SetActive(false);
        canvas.SetActive(true);

        isScrolling = false; // Animation terminée
    }

    void AddHoverEffects(Button button, Color hoverColor)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((eventData) =>
        {
            buttonText.color = hoverColor;

            buttonText.fontMaterial.SetFloat("_OutlineWidth", 0.2f);
            buttonText.fontMaterial.SetColor("_OutlineColor", outlineColor);
        });
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((eventData) =>
        {
            buttonText.color = normalTextColor;

            buttonText.fontMaterial.SetFloat("_OutlineWidth", 0f);
        });
        trigger.triggers.Add(pointerExit);
    }
}
