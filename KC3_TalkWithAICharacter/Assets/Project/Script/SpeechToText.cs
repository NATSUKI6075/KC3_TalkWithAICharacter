using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Audio;

public class MicController : MonoBehaviour
{
    [SerializeField] private PromptMG promptMG;
    [SerializeField] private string apiKey;
    [SerializeField] private Button recordButton;
    [SerializeField] private Image micImage; // 追加
    [SerializeField] private Sprite recordSprite; // 追加
    [SerializeField] private Sprite stopSprite; // 追加

    private AudioClip audioClip;
    private bool isRecording = false;
    private int sampleRate = 44100;
    private string micDevice;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            recordButton.onClick.AddListener(OnRecordButtonPressed);
        }
        else
        {
            Debug.LogError("マイクが見つかりません");
        }
    }

    async void OnRecordButtonPressed()
    {
        if (!isRecording)
        {
            StartRecording();
            micImage.sprite = recordSprite; // 追加
        }
        else
        {
            StopRecording();
            micImage.sprite = stopSprite; // 追加
            await STT(audioClip);
        }
    }

    void StartRecording()
    {
        isRecording = true;
        audioClip = Microphone.Start(micDevice, false, 10, sampleRate);
    }

    void StopRecording()
    {
        isRecording = false;
        Microphone.End(micDevice);
    }

    public async Task STT(AudioClip clip)
    {
        var api = new OpenAIClient(apiKey);
        using var request = new AudioTranscriptionRequest(clip, temperature: 0.1f, language: "ja");
        var result = await api.AudioEndpoint.CreateTranscriptionAsync(request);
        Debug.Log(result);
        await promptMG.SendMessage(result);
    }
}
