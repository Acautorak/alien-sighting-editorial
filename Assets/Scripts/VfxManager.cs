using UnityEngine;

public class VfxManager : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float vfxTime;
    [Header("References")]
    [SerializeField] private ParticleSystem publishParticleSystem;
    [SerializeField] private ParticleSystem denyParticleSystem;
    private void Start()
    {
        NotificationBuss.Subscribe(EventNames.OnCaseDropped, OnCaseDropped);
        publishParticleSystem.Stop();
    }

    private void OnCaseDropped(object obj)
    {
        PlayPublishEffect();
    }

    private void PlayPublishEffect()
    {
        var emission = publishParticleSystem.emission;
        emission.enabled = true;
        publishParticleSystem.Play();
        Invoke(nameof(StopEmission), vfxTime);
    }

    private void StopEmission()
    {
        var emmision = publishParticleSystem.emission;
        emmision.enabled = false;
    }
}
