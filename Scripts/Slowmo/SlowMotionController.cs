using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SlowMotionController : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0.01f, 1f)]
    public float slowTimeScale = 0.15f;
    public float normalTimeScale = 1f;
    public float transitionSpeed = 6f;

    [Header("Input Detection")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Camera Effects")]
    public Camera fpsCam;
    public float slowFOV = 55f;
    public float normalFOV = 70f;

    [Header("Audio Effects")]
    public AudioSource[] affectedAudioSources;

    [Header("URP Motion Blur")]
    public Volume postProcessVolume;
    public float slowMotionBlurIntensity = 0.6f;
    public float normalMotionBlurIntensity = 0f;

    [Header("URP Vignette")]
    [Range(0f, 1f)] public float normalVignetteIntensity = 0.2f;
    [Range(0f, 1f)] public float slowVignetteIntensity = 0.4f;
    [Range(0f, 1f)] public float normalVignetteSmoothness = 0.5f;
    [Range(0f, 1f)] public float slowVignetteSmoothness = 0.9f;

    [Header("Debug")]
    public bool showDebug;

    private float targetTimeScale;
    private MotionBlur motionBlur;
    private Vignette vignette;

    // 🔑 NEW
    private bool gameplayStarted = false;

    void Start()
    {
        targetTimeScale = normalTimeScale;
        Time.timeScale = normalTimeScale;
        Time.fixedDeltaTime = 0.02f;

        if (fpsCam)
            fpsCam.fieldOfView = normalFOV;

        if (postProcessVolume)
        {
            postProcessVolume.profile.TryGet(out motionBlur);
            postProcessVolume.profile.TryGet(out vignette);

            if (motionBlur)
                motionBlur.intensity.value = normalMotionBlurIntensity;

            if (vignette)
            {
                vignette.intensity.value = normalVignetteIntensity;
                vignette.smoothness.value = normalVignetteSmoothness;
            }
        }
    }

    void Update()
    {
        HandleTimeScaleByInput();
        HandleCameraFOV();
        HandleAudioPitch();
        HandleMotionBlur();
        HandleVignette();
    }

    // -------------------- TIME --------------------
    void HandleTimeScaleByInput()
    {
        bool movementInput =
            Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f ||
            Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f ||
            Input.GetKey(jumpKey);

        // 🔥 ACTIVATE GAMEPLAY ON FIRST INPUT
        if (!gameplayStarted && movementInput)
        {
            gameplayStarted = true;
        }

        // ⛔ Before gameplay starts → ALWAYS normal time
        if (!gameplayStarted)
        {
            targetTimeScale = normalTimeScale;
            Time.timeScale = Mathf.Lerp(
                Time.timeScale,
                targetTimeScale,
                Time.unscaledDeltaTime * transitionSpeed
            );
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            return;
        }

        // ✅ Normal slow-mo logic
        targetTimeScale = movementInput ? normalTimeScale : slowTimeScale;

        Time.timeScale = Mathf.Lerp(
            Time.timeScale,
            targetTimeScale,
            Time.unscaledDeltaTime * transitionSpeed
        );

        Time.timeScale = Mathf.Clamp(Time.timeScale, slowTimeScale, normalTimeScale);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (showDebug)
            Debug.Log($"TimeScale: {Time.timeScale:0.00}");
    }

    // -------------------- CAMERA --------------------
    void HandleCameraFOV()
    {
        if (!fpsCam) return;

        float targetFOV =
            targetTimeScale == slowTimeScale ? slowFOV : normalFOV;

        fpsCam.fieldOfView = Mathf.Lerp(
            fpsCam.fieldOfView,
            targetFOV,
            Time.unscaledDeltaTime * transitionSpeed
        );
    }

    // -------------------- AUDIO --------------------
    void HandleAudioPitch()
    {
        if (affectedAudioSources == null) return;

        float targetPitch = Time.timeScale < 0.5f ? 0.6f : 1f;

        foreach (var src in affectedAudioSources)
        {
            if (!src) continue;

            src.pitch = Mathf.Lerp(
                src.pitch,
                targetPitch,
                Time.unscaledDeltaTime * transitionSpeed
            );
        }
    }

    // -------------------- MOTION BLUR --------------------
    void HandleMotionBlur()
    {
        if (!motionBlur) return;

        float targetBlur =
            targetTimeScale == slowTimeScale
            ? slowMotionBlurIntensity
            : normalMotionBlurIntensity;

        motionBlur.intensity.value = Mathf.Lerp(
            motionBlur.intensity.value,
            targetBlur,
            Time.unscaledDeltaTime * transitionSpeed
        );
    }

    // -------------------- VIGNETTE --------------------
    void HandleVignette()
    {
        if (!vignette) return;

        float targetIntensity =
            targetTimeScale == slowTimeScale
            ? slowVignetteIntensity
            : normalVignetteIntensity;

        float targetSmoothness =
            targetTimeScale == slowTimeScale
            ? slowVignetteSmoothness
            : normalVignetteSmoothness;

        vignette.intensity.value = Mathf.Lerp(
            vignette.intensity.value,
            targetIntensity,
            Time.unscaledDeltaTime * transitionSpeed
        );

        vignette.smoothness.value = Mathf.Lerp(
            vignette.smoothness.value,
            targetSmoothness,
            Time.unscaledDeltaTime * transitionSpeed
        );
    }
}
