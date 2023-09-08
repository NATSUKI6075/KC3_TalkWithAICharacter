using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceVox : MonoBehaviour
{
    [SerializeField] private CharacterEmotionMG characterEmotionMG;
    [SerializeField] private AudioSource audioSource;

    private Dictionary<int, AudioClip> audioClips = new Dictionary<int, AudioClip>(); 
    private Dictionary<int, string> emotions = new Dictionary<int, string>(); 
    private int currentKey = 0;
    private int lastKey = -1;

    void Start()
    {
        //StartCoroutine(Speak("こんにちは！みんなもUnityでVOICEVOXを使おう！")); 
    }

    public IEnumerator Speak (string text)
    {
        Debug.Log("IEnumerator Speak ("+text+")");
        int key = currentKey;
        currentKey++;
        // VOICEVOXのREST-APIクライアント
        VoiceVoxApiClient client = new VoiceVoxApiClient();

        // テキストからAudioClipを生成（話者は「8:春日部つむぎ」）
        yield return client.TextToAudioClip(50, text);

        if (client.AudioClip != null)
        {
            // AudioClipをリストに追加
            audioClips.Add(key, client.AudioClip);
            Debug.Log(key);
        }
    }

    private float timeSinceLastPlayed = 0f;
    private float resetEmotionDelay = 2f; // 表情をリセットするまでの時間

    void Update()
    {
        // audioClipsに未再生の音声があり、次に再生すべき音声がある場合
        if(audioClips.Count > 0 && audioClips.ContainsKey(lastKey+1))
        {
            // 音声が現在再生中でない場合
            if(!audioSource.isPlaying)
            {
                // 次の音声をセット
                audioSource.clip = audioClips[lastKey+1];

                // AudioSourceで再生
                audioSource.Play();

                // 最終再生時間をリセット
                timeSinceLastPlayed = 0f;

                // emotionsリストに次に再生する音声と一致するキーがある場合
                if (emotions.ContainsKey(lastKey + 1))
                {
                    // 現在の表情をリセット
                    characterEmotionMG.ResetEmotion();
                    
                    // 新しい表情を設定
                    characterEmotionMG.SetEmotion(emotions[lastKey + 1]);

                    // 既に設定したemotionをemotionsから削除
                    emotions.Remove(lastKey + 1);
                }

                // 既に再生した音声をaudioClipsから削除
                audioClips.Remove(lastKey+1);

                // lastKeyをインクリメント
                lastKey++;
            }
            else
            {
                // 再生中なので経過時間を更新
                timeSinceLastPlayed += Time.deltaTime;
            }
        }
        // 音声の再生が終了している場合
        else if(!audioSource.isPlaying)
        {
            // 経過時間を更新
            timeSinceLastPlayed += Time.deltaTime;
        }

        // AudioSourceが'resetEmotionDelay'秒間再生していない場合、表情をリセット
        if (timeSinceLastPlayed >= resetEmotionDelay)
        {
            characterEmotionMG.ResetEmotion();
            timeSinceLastPlayed = 0f; // 表情をリセットしたら経過時間もリセット
        }
    }

    public void GetEmotion(string emotion)
    {
        emotions.Add(currentKey,emotion);
    }
    
}
