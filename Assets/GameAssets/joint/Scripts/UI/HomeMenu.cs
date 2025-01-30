using UnityEngine;

public class HomeMenu : MonoBehaviour
{
    [Header("⚙ Paramètres")]
    [SerializeField] private GameObject parametreMenu; // Glisser le GameObject ParametreMenu ici
    [SerializeField] private GameObject optionButton; // Glisser le GameObject Option ici

    void Start()
    {
        // Vérifie si ParametreMenu est bien assigné
        if (parametreMenu != null)
        {
            parametreMenu.SetActive(false); // Rendre invisible au démarrage
            Debug.Log("[HomeMenu] ParametreMenu désactivé au démarrage.");
        }
        else
        {
            Debug.LogError("[HomeMenu] ⚠ ERREUR: ParametreMenu n'est pas assigné dans l'Inspector !");
        }

        // Vérifie si OptionButton est assigné
        if (optionButton == null)
        {
            Debug.LogError("[HomeMenu] ⚠ ERREUR: OptionButton n'est pas assigné dans l'Inspector !");
        }
    }

    void Update()
    {
        if (optionButton != null && Input.GetMouseButtonDown(0)) // Clic gauche détecté
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == optionButton)
                {
                    Debug.Log("[HomeMenu] Clic détecté sur Option, ouverture de ParametreMenu.");
                    OpenParametreMenu();
                }
            }
        }
    }

    public void OpenParametreMenu()
    {
        if (parametreMenu != null)
        {
            parametreMenu.SetActive(true);
            Debug.Log("[HomeMenu] ParametreMenu est maintenant visible !");
        }
        else
        {
            Debug.LogError("[HomeMenu] ⚠ ERREUR: Impossible d'afficher ParametreMenu, il n'est pas assigné !");
        }
    }
}
