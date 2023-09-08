using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using OpenAI;
using OpenAI.Chat;

public class ChatGPT : MonoBehaviour
{
    [SerializeField] private VoiceVox voiceVox;

    [SerializeField] private string apiKey;

    public async Task<string> SendMessage(List<Message> messages)
    {
        Debug.Log("ChatGPT.SendMessage");
        string reply = await Streaming(messages);
        if (string.IsNullOrEmpty(reply))
        {
            Debug.LogError("返答なし");
        }

        return reply;
    }

    async Task<string> Streaming(List<Message> messages)
    {
        var api = new OpenAIClient(apiKey);

        var chatRequest = new ChatRequest(messages, "gpt-3.5-turbo" );

        string reply = "", allReply = "";
        HashSet<char> specialChars = new HashSet<char> { '、', '。', '！', '？', '\n', '♪', '.', ',', '!', '?' };
        string emotion = "";

        await api.ChatEndpoint.StreamCompletionAsync(chatRequest, result =>
        {
            foreach (var choice in result.Choices.Where(choice => !string.IsNullOrWhiteSpace(choice.Delta?.Content)))
            {
                reply += choice.Delta.Content;
                allReply += choice.Delta.Content;

                if (reply.Length > 0)
                {
                    // Check if there is text enclosed in brackets
                    Regex regex = new Regex(@"\[(.*?)\]");
                    Match match = regex.Match(reply);
                    if (match.Success)
                    {
                        emotion = match.Groups[1].Value;  // Extract the content inside brackets
                        Emotion(emotion);
                        emotion = "";
                        reply = regex.Replace(reply, ""); // Remove the matched part from reply
                    }
                    
                    if (reply.Length > 0)
                    {
                        char lastChar = reply[reply.Length - 1];
                        if (specialChars.Contains(lastChar))
                        {
                            int index = reply.LastIndexOf(lastChar);
                            string dividedReply = reply.Substring(0, index + 1);
                            StartCoroutine(voiceVox.Speak(dividedReply));
                            reply = reply.Substring(index + 1);
                        }
                    }

                }
            }
        });

        //分割の条件に合わず文章が残っていた場合の処理
        if (reply.Length > 0)
        {
            StartCoroutine(voiceVox.Speak(reply));
            reply = "";
        }

        await Task.Delay(3000);

        Debug.Log(allReply);

        return allReply;
    }

    void Emotion(string emotion)
    {
        voiceVox.GetEmotion(emotion);
    }

}
