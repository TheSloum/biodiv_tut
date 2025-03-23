using System.Collections;
using UnityEngine;

public class LetterSequence : MonoBehaviour
{
    private string targetSequence = "CROUPI";
    private string currentInput = "";
    public GameObject prefab;
    public float shakeDuration = 0.5f;
    public float shakeStrength = 1f;
    private Vector3 originalCameraPosition;
    public Canvas canvas;

    private bool isWaitingForCooldown = false;

    private void Start()
    {
        originalCameraPosition = Camera.main.transform.position;
    }

    void Update()
    {
        if (isWaitingForCooldown) return;

        if (Input.anyKeyDown)
        {
            string keyPressed = Input.inputString.ToUpper();
            if (!string.IsNullOrEmpty(keyPressed) && keyPressed.Length == 1)
            {
                currentInput += keyPressed;
            }
        }

        if (currentInput.Length > targetSequence.Length)
        {
            currentInput = currentInput.Substring(currentInput.Length - targetSequence.Length);
        }

        if (currentInput == targetSequence)
        {
            StartCoroutine(ShakeCamera());
            StartCoroutine(HandlePrefabAndCooldown());
            currentInput = "";
        }
    }

    private IEnumerator ShakeCamera()
    {
        if (Camera.main == null) yield break;

        float elapsed = 0f;
        Vector3 shakePosition = originalCameraPosition;

        while (elapsed < shakeDuration)
        {
            float xShake = Random.Range(-shakeStrength, shakeStrength);
            float yShake = Random.Range(-shakeStrength, shakeStrength);
            Camera.main.transform.position = shakePosition + new Vector3(xShake, yShake, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.position = originalCameraPosition;
    }


    private IEnumerator HandlePrefabAndCooldown()
    {
        isWaitingForCooldown = true;

        GameObject spawnedPrefab = Instantiate(prefab, canvas.transform);
        RectTransform rectTransform = spawnedPrefab.GetComponent<RectTransform>();

        rectTransform.localPosition = new Vector3(-4000f, Random.Range(-300f, 300f), 0f);
        rectTransform.localScale = new Vector3(700f, 700f, 1f);

        float moveTime = 0f;
        Vector3 startPosition = rectTransform.localPosition;
        Vector3 endPosition = startPosition + new Vector3(10000f, 0f, 0f);

        while (moveTime < 10f)
        {
            rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, moveTime / 10f);
            moveTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = endPosition;
        yield return new WaitForSeconds(10f);
        Destroy(spawnedPrefab);

        isWaitingForCooldown = false;
    }
}
