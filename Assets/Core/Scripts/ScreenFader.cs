using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class ScreenFade : MonoBehaviour
{
    public static ScreenFade Instance;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] CinemachineCamera vCam;

    CinemachinePositionComposer transposer;
    Vector3 originalDamping;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (vCam != null)
        {
            transposer = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
        }

        if (transposer != null)
        {
            originalDamping = new Vector3(transposer.Damping.x, transposer.Damping.y, transposer.Damping.z);
        }
    }

    async Task Fade(float targetTransparency)
    {
        float start = canvasGroup.alpha, t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetTransparency, t / fadeDuration);
            await Task.Yield();
        }
        canvasGroup.alpha = targetTransparency;
    }

    public async Task FadeOut()
    {
        await Fade(1);
        SetDamping(Vector3.zero);
    }

    public async Task FadeIn()
    {
        SetDamping(originalDamping);
        await Fade(0);
    }

    void SetDamping(Vector3 d)
    {
        if (transposer == null)
        {
            return;
        }
        transposer.Damping.x = d.x;
        transposer.Damping.y = d.y;
        transposer.Damping.z = d.z;
    }
}