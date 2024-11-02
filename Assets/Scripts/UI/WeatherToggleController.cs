using UnityEngine;
using UnityEngine.UI;

public class WeatherToggleController : MonoBehaviour
{
    public Toggle snowToggle;
    public Toggle rainToggle;
    public GameObject snowPrefab;
    public GameObject rainPrefab;
    public Transform vehicleTransform; // 차량의 Transform을 연결
    public AudioSource rainAudioSource; // 비 소리 AudioSource

    private GameObject currentEffect;

    private void Start()
    {
        snowToggle.onValueChanged.AddListener(delegate { ToggleSnowEffect(snowToggle.isOn); });
        rainToggle.onValueChanged.AddListener(delegate { ToggleRainEffect(rainToggle.isOn); });

        // 초기에는 비 소리를 정지 상태로 설정
        if (rainAudioSource != null)
        {
            rainAudioSource.Stop();
        }
    }

    private void Update()
    {
        // 차량이 이동할 때 현재 활성화된 효과의 위치를 업데이트
        if (currentEffect != null)
        {
            currentEffect.transform.position = vehicleTransform.position;
        }
    }

    private void ToggleSnowEffect(bool isOn)
    {
        if (isOn)
        {
            if (currentEffect != null) Destroy(currentEffect);
            currentEffect = Instantiate(snowPrefab, vehicleTransform.position, Quaternion.identity);
            rainToggle.isOn = false;

            // 비 소리 중지
            if (rainAudioSource != null && rainAudioSource.isPlaying)
            {
                rainAudioSource.Stop();
            }
        }
        else if (currentEffect && currentEffect.name.Contains("Snow"))
        {
            Destroy(currentEffect);
        }
    }

    private void ToggleRainEffect(bool isOn)
    {
        if (isOn)
        {
            if (currentEffect != null) Destroy(currentEffect);
            currentEffect = Instantiate(rainPrefab, vehicleTransform.position, Quaternion.identity);
            snowToggle.isOn = false;

            // 비 소리 재생
            if (rainAudioSource != null && !rainAudioSource.isPlaying)
            {
                rainAudioSource.Play();
            }
        }
        else if (currentEffect && currentEffect.name.Contains("Rain"))
        {
            Destroy(currentEffect);

            // 비 소리 중지
            if (rainAudioSource != null && rainAudioSource.isPlaying)
            {
                rainAudioSource.Stop();
            }
        }
    }
}
