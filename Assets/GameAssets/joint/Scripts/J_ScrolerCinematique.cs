using System.Collections;
using UnityEngine;

public class J_CameraCinematique : MonoBehaviour
{
    public Transform cameraTransform; // Référence à la caméra
    public float startX = -10f; // Position de départ de la caméra
    public float targetX = 10f; // Position cible
    public float moveDuration = 3f; // Durée du déplacement

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        startPosition = new Vector3(startX, cameraTransform.position.y, cameraTransform.position.z);
        targetPosition = new Vector3(targetX, cameraTransform.position.y, cameraTransform.position.z);

        // 📌 Si cinématique est false, placer directement la caméra à la position cible
        if (!Materials.instance.cinematique)
        {
            cameraTransform.position = targetPosition;
        }
        else
        {
            cameraTransform.position = startPosition;
        }
    }

    void Update()
    {
        if (Materials.instance.cinematique && !isMoving)
        {
            StartCoroutine(MoveCamera());
        }

        // Vérifie la touche selon la plateforme
        if ((Application.platform == RuntimePlatform.WebGLPlayer && Input.GetKeyDown(KeyCode.P)) ||
            (Application.platform != RuntimePlatform.WebGLPlayer && Input.GetKeyDown(KeyCode.Escape)))
        {
            SkipToTarget();
        }
    }

    IEnumerator MoveCamera()
    {
        isMoving = true;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t); // Accélération/décélération fluide
            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);

            elapsedTime += Time.fixedDeltaTime; // Plus stable que Time.deltaTime
            yield return new WaitForFixedUpdate(); // Évite les à-coups
        }

        cameraTransform.position = targetPosition;
        isMoving = false;
        Materials.instance.cinematique = false; // Désactiver la cinématique après l'animation
    }

    void SkipToTarget()
    {
        if (isMoving)
        {
            StopAllCoroutines();
            cameraTransform.position = targetPosition;
            isMoving = false;
            Materials.instance.cinematique = false; // Désactiver immédiatement la cinématique
        }
    }
}
