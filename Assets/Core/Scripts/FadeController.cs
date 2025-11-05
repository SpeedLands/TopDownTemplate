using UnityEngine;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance; // Singleton

    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject.transform.parent.gameObject); // Hacemos que el Canvas no se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject.transform.parent.gameObject);
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            // CanvasGroup es más eficiente para controlar el alfa de toda una UI
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }
    }

    // Coroutine para hacer el fade out (oscurecer la pantalla)
    public IEnumerator FadeOut(float duration = 1f)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / duration);
            yield return null;
        }
        canvasGroup.alpha = 1; // Asegurarse de que termina completamente opaco
    }

    // Coroutine para hacer el fade in (aclarar la pantalla)
    public IEnumerator FadeIn(float duration = 1f)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / duration);
            yield return null;
        }
        canvasGroup.alpha = 0; // Asegurarse de que termina completamente transparente
    }
}