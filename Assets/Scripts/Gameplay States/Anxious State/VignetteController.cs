using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
public class VignetteController : MonoBehaviour
{
    public Volume volume;
    Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        if (volume.profile.TryGet(out Vignette vignette))
            this.vignette = vignette;

        float intensity = 0;

        DOVirtual.Float(0, 0.4f, 1, x =>
        {
            vignette.intensity.Override(x);
            intensity = x;
        })
        .OnComplete(() =>
        {
            Pulse(intensity, intensity + 0.1f, 1f);
        });
    }

    void Pulse(float minPulse, float maxPulse, float speed)
    {
        DOVirtual.Float(minPulse, maxPulse, speed, x =>
        {
            vignette.intensity.Override(x);
        })
            .SetLoops(-1, LoopType.Yoyo);
    }
}
