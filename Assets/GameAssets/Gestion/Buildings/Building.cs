using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildings", menuName = "ScriptableObjects/Building")]

public class Building : ScriptableObject
{

    [Header("Info de base")]


    [Tooltip("ID du batiment")]
    public int buildID = 50;
    public bool isPaused = false;
    public string buildDesc;
    public string PolutionEtat;
    public string PopulationEtat;
    public string ElecEtat;
    public string MoneyMake;
    public GameObject buildingObject;
    public int buildClass = 1;

    public bool unlocked = true;

    public void Unlock()
    {
        unlocked = true;
    }
    public void Relock()
    {
        unlocked = false;
    }


    [Header("Sprite")]

    public Sprite buildSprite;



    [Space]
    [Space]
    [Space]
    [Space]
    [Header("Prix de construction")]

    [Tooltip("Prix")]
    public int price = 50;



    [Tooltip("Prix en bois")]
    public int mat_0 = 50;

    [Tooltip("Prix en pierre")]
    public int mat_1 = 50;

    [Tooltip("Prix en fer")]
    public int mat_2 = 50;

    [Range(-1f, 1f)]
    [Tooltip("Contentement du peuple")]
    public float bar_0 = -0.2f;

    [Range(-1f, 1f)]
    [Tooltip("Taxation")]
    public float bar_1 = 0.6f;

    [Range(-1f, 1f)]
    [Tooltip("Contamination")]
    public float bar_2 = 0.6f;





    [Space]
    [Space]
    [Space]
    [Space]
    [Header("Cyclage")]



    [Tooltip("durée Cycle")]
    public float time = 0.6f;

    [Tooltip("Consommation en bois par cycle")]
    public int cons_mat_0 = 50;

    [Tooltip("consommation en pierre par cycle")]
    public int cons_mat_1 = 50;

    [Tooltip("consommation en fer par cycle")]
    public int cons_mat_2 = 50;

    [Range(-1f, 1f)]
    [Tooltip("content par cycle")]
    public float bar_0_cycle = 0.6f;

    [Range(-1f, 1f)]
    [Tooltip("taxes par cycle")]
    public float bar_1_cycle = 0.6f;

    [Range(-1f, 1f)]
    [Tooltip("contamination par cycle")]
    public float bar_2_cycle = 0.6f;


    [Tooltip("prix/revenu par cycle")]
    public int price_cycle = 0;

    [Space]
    [Space]
    [Space]
    [Space]
    [Header("Multiplicateur d'améliorations")]

    [Tooltip("multiplicateur d'améliorations")]
    public float upgradeMult = 1;

    [Tooltip("multiplicateur d'améliorations")]
    public int up_mat_0 = 800;
    public int up_mat_1 = 800;
    public int up_mat_2 = 800;
    public int up_price = 800;


    [Space]
    [Space]
    [Space]
    [Space]



    //Amélioration A = Utilisation d'éléments
    /*
    [Header("Coûts Amélioration 1")]

        [Tooltip("Coût en bois de l'amélioration 1 A")]
        public int upgrade_1a_mat_0= 50;

        [Tooltip("Coût en pierre de l'amélioration 1 A")]
        public int upgrade_1a_mat_1= 50;

        [Tooltip("Coût en fer de l'amélioration 1 A")]
        public int upgrade_1a_mat_2= 50;

        [Tooltip("Coût en fer de l'amélioration 1 A")]
        public int upgrade_1a_price= 50;


    [Header("Nouveaux taux Amélioration 1")]

        [Tooltip("Consommation de bois par cycle amélioration 1 A")]
        public int upgrade_1a_cons_mat_0= 50;

        [Tooltip("Consommation de pierre par cycle amélioration 1 A")]
        public int upgrade_1a_cons_mat_1= 50;

        [Tooltip("Consommation de fer par cycle amélioration 1 A")]
        public int upgrade_1a_cons_mat_2= 50;

        [Range(-1f, 1f)]
        [Tooltip("contentement par cycle amélioration 1 A")]
        public float upgrade_1a_cycle_bar_0= 50;

        [Range(-1f, 1f)]
        [Tooltip("taxation par cycle amélioration 1 A")]
        public float upgrade_1a_cycle_bar_1= 50;

        [Range(-1f, 1f)]
        [Tooltip("contamination par cycle amélioration 1 A")]
        public float upgrade_1a_cycle_bar_2= 50;

        [Tooltip("cout en argent cycle amélioration 1 A")]
        public int upgrade_1a_price_cycle= 50;

    [Space]
    [Space]
    [Header("Coûts taux Amélioration 2")]


        [Tooltip("Coût en bois de l'amélioration 2 A")]
        public int upgrade_2a_mat_0= 50;

        [Tooltip("Coût en pierre de l'amélioration 2 A")]
        public int upgrade_2a_mat_1= 50;

        [Tooltip("Coût en fer de l'amélioration 2 A")]
        public int upgrade_2a_mat_2= 50;

        [Tooltip("Coût en fer de l'amélioration 2 A")]
        public int upgrade_2a_price= 50;

    [Header("Nouveaux taux Amélioration 2")]

        [Tooltip("Consommation de bois par cycle amélioration 2 A")]
        public int upgrade_2a_cons_mat_0= 50;

        [Tooltip("Consommation de pierre par cycle amélioration 2 A")]
        public int upgrade_2a_cons_mat_1= 50;

        [Tooltip("Consommation de fer par cycle amélioration 2 A")]
        public int upgrade_2a_cons_mat_2= 50;

        [Range(-1f, 1f)]
        [Tooltip("contentement par cycle amélioration 2 A")]
        public float upgrade_2a_cycle_bar_0= 50;

        [Range(-1f, 1f)]
        [Tooltip("taxation par cycle amélioration 2 A")]
        public float upgrade_2a_cycle_bar_1= 50;

        [Range(-1f, 1f)]
        [Tooltip("contamination par cycle amélioration 2 A")]
        public float upgrade_2a_cycle_bar_2= 50;

        [Tooltip("cout en argent cycle amélioration 2 A")]
        public int upgrade_2a_price_cycle= 50;

    [Space]
    [Space]
    [Header("Coûts taux Amélioration ")]


        [Tooltip("Coût en bois de l'amélioration 3 A")]
        public int upgrade_3a_mat_0= 50;

        [Tooltip("Coût en pierre de l'amélioration 3 A")]
        public int upgrade_3a_mat_1= 50;

        [Tooltip("Coût en fer de l'amélioration 3 A")]
        public int upgrade_3a_mat_2= 50;

        [Tooltip("Coût en fer de l'amélioration 3 A")]
        public int upgrade_3a_price= 50;

    [Header("Nouveaux taux Amélioration 3")]

        [Tooltip("Consommation de bois par cycle amélioration 3 A")]
        public int upgrade_3a_cons_mat_0= 50;

        [Tooltip("Consommation de pierre par cycle amélioration 3 A")]
        public int upgrade_3a_cons_mat_1= 50;

        [Tooltip("Consommation de fer par cycle amélioration 3 A")]
        public int upgrade_3a_cons_mat_2= 50;

        [Range(-1f, 1f)]
        [Tooltip("contentement par cycle amélioration 3 A")]
        public float upgrade_3a_cycle_bar_0= 50;

        [Range(-1f, 1f)]
        [Tooltip("taxation par cycle amélioration 3 A")]
        public float upgrade_3a_cycle_bar_1= 50;

        [Range(-1f, 1f)]
        [Tooltip("contamination par cycle amélioration 3 A")]
        public float upgrade_3a_cycle_bar_2= 50;

        [Tooltip("cout en argent cycle amélioration 3 A")]
        public int upgrade_3a_price_cycle= 50;





    [Space]
    [Space]
    [Space]
    [Space]
    [Header("Productions d'éléments")]
    [Space]
    [Space]
    [Header("Coûts Amélioration 1")]

    // Amélioration des productions



        [Tooltip("Coût en bois de l'amélioration 1 B")]
        public int upgrade_1b_mat_0= 50;

        [Tooltip("Coût en pierre de l'amélioration 1 B")]
        public int upgrade_1b_mat_1= 50;

        [Tooltip("Coût en fer de l'amélioration 1 B")]
        public int upgrade_1b_mat_2= 50;

        [Tooltip("Coût en fer de l'amélioration 1 B")]
        public int upgrade_1b_price= 50;


    [Header("Nouveaux taux d'amélioration 1")]


        [Range(-1f, 1f)]
        [Tooltip("contentement par cycle amélioration 1 B")]
        public float upgrade_1b_cycle_bar_0= 50;

        [Range(-1f, 1f)]
        [Tooltip("taxation par cycle amélioration 1 B")]
        public float upgrade_1b_cycle_bar_1= 50;

        [Range(-1f, 1f)]
        [Tooltip("contamination par cycle amélioration 1 B")]
        public float upgrade_1b_cycle_bar_2= 50;

        [Tooltip("production en argent cycle amélioration 1 B")]
        public int upgrade_1b_price_cycle= 50;



    [Space]
    [Space]
    [Header("Coûts Amélioration 2")]




        [Tooltip("Coût en bois de l'amélioration 2 B")]
        public int upgrade_2b_mat_0= 50;

        [Tooltip("Coût en pierre de l'amélioration 2 B")]
        public int upgrade_2b_mat_1= 50;

        [Tooltip("Coût en fer de l'amélioration 2 B")]
        public int upgrade_2b_mat_2= 50;

        [Tooltip("Coût en fer de l'amélioration 2 B")]
        public int upgrade_2b_price= 50;

        [Header("Nouveaux taux d'amélioration 2")]

        [Range(-1f, 1f)]
        [Tooltip("contentement par cycle amélioration 2 B")]
        public float upgrade_2b_cycle_bar_0= 50;

        [Range(-1f, 1f)]
        [Tooltip("taxation par cycle amélioration 2 B")]
        public float upgrade_2b_cycle_bar_1= 50;

        [Range(-1f, 1f)]
        [Tooltip("contamination par cycle amélioration 2 B")]
        public float upgrade_2b_cycle_bar_2= 50;

        [Tooltip("production en argent cycle amélioration 2 B")]
        public int upgrade_2b_price_cycle= 50;




    [Space]
    [Space]
    [Header("Coûts Amélioration 3")]


        [Tooltip("Coût en bois de l'amélioration 3 B")]
        public int upgrade_3b_mat_0= 50;

        [Tooltip("Coût en pierre de l'amélioration 3 B")]
        public int upgrade_3b_mat_1= 50;

        [Tooltip("Coût en fer de l'amélioration 3 B")]
        public int upgrade_3b_mat_2= 50;

        [Tooltip("Coût en fer de l'amélioration 3 B")]
        public int upgrade_3b_price= 50;


        [Header("Nouveaux taux d'amélioration 3")]


        [Range(-1f, 1f)]
        [Tooltip("contentement par cycle amélioration 3 B")]
        public float upgrade_3b_cycle_bar_0= 50;

        [Range(-1f, 1f)]
        [Tooltip("taxation par cycle amélioration 3 B")]
        public float upgrade_3b_cycle_bar_1= 50;

        [Range(-1f, 1f)]
        [Tooltip("contamination par cycle amélioration 3 B")]
        public float upgrade_3b_cycle_bar_2= 50;

        [Tooltip("production en argent cycle amélioration 3 B")]
        public int upgrade_3b_price_cycle= 50;






    [Space]
    [Space]
    [Space]
    [Space]
    [Header("Accélération de cycle")]
    [Space]
    [Space]


    [Header("Coûts Amélioration 1")]






        [Tooltip("Coût en bois de l'amélioration 1 C")]
        public int upgrade_1c_mat_0= 50;

        [Tooltip("Coût en pierre de l'amélioration 1 C")]
        public int upgrade_1c_mat_1= 50;

        [Tooltip("Coût en fer de l'amélioration 1 C")]
        public int upgrade_1c_mat_2= 50;

        [Tooltip("Coût en fer de l'amélioration 1 C")]
        public int upgrade_1c_price= 50;

        [Header("Nouvelle vitesse de cycle 1")]

        [Tooltip("Vitesse de cycle 1 C")]
        public float upgrade_1c_time= 50;

    [Space]
    [Space]


    [Header("Coûts Amélioration 2")]

        [Tooltip("Coût en bois de l'amélioration 2 C")]
        public int upgrade_2c_mat_0= 50;

        [Tooltip("Coût en pierre de l'amélioration 2 C")]
        public int upgrade_2c_mat_1= 50;

        [Tooltip("Coût en fer de l'amélioration 2 C")]
        public int upgrade_2c_mat_2= 50;

        [Tooltip("Coût en fer de l'amélioration 2 C")]
        public int upgrade_2c_price= 50;

        [Header("Nouvelle vitesse de cycle 2")]

        [Tooltip("Vitesse de cycle 2 C")]
        public float upgrade_2c_time= 50;

    [Space]
    [Space]


    [Header("Coûts Amélioration 2")]

        [Tooltip("Coût en bois de l'amélioration 3 C")]
        public int upgrade_3c_mat_0= 50;

        [Tooltip("Coût en pierre de l'amélioration 3 C")]
        public int upgrade_3c_mat_1= 50;

        [Tooltip("Coût en fer de l'amélioration 3 C")]
        public int upgrade_3c_mat_2= 50;

        [Tooltip("Coût en fer de l'amélioration 3 C")]
        public int upgrade_3c_price= 50;

        [Header("Nouvelle vitesse de cycle 3")]

        [Tooltip("Vitesse de cycle 3 C")]
        public float upgrade_3c_time= 50;




    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    */
    [Header("Debug")]











    [Tooltip("Couleur pour debug")]
    public string debug = "FF57733";

}
