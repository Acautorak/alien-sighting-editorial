using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    [Header("Properties")]
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeMagnitude;

    private Vector3 originalPosition;

    [Header("References")]
    [SerializeField] private Camera targetCamera;

    private void Start()
    {
        if(targetCamera != null)
        {
            originalPosition = targetCamera.transform.position;
        }
    }

    private void ShakeCameraPublish()
    {
        if(targetCamera == null) return;

        LeanTween.moveLocal(targetCamera.gameObject, originalPosition + Random.insideUnitSphere * shakeMagnitude, shakeDuration)
        .setEase(LeanTweenType.easeShake)
        .setLoopPingPong(1)
        .setOnComplete(()=> 
        {
            targetCamera.transform.localPosition = originalPosition;
        });
    }

    public void SetTargetCamera(Camera newCamera)
    {
        targetCamera = newCamera;
        if(targetCamera != null)
        {
            originalPosition = targetCamera.transform.localPosition;
        }
    }
}
