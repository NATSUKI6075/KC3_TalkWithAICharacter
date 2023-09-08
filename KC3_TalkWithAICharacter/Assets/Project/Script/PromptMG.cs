using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using OpenAI.Chat;
using System.Linq;
using System.Threading.Tasks;


public class PromptMG : MonoBehaviour
{
    [SerializeField] private ChatGPT chatGPT;
    [SerializeField] private CharacterSetting characterSetting;

    List<Message> messages;

    string systemPrompt;

    // Start is called before the first frame update
    async void Start()
    { 
        ResetPrompt();

        //await SendMessage("いろんな表情して");
    }

    public void PromptAdd(string role,string text)
    {
        if(role == "System")
        {
            messages.Add(new Message(Role.System, text));
        }

        else if(role == "User")
        {
            messages.Add(new Message(Role.User, text));
        }

        else if(role == "Assistant")
        {
            messages.Add(new Message(Role.Assistant, text));
        }

        else
        {
            Debug.Log("role間違い");
        }
    }

    public void ResetPrompt()
    {
        systemPrompt = characterSetting.GetCharacterSetting();   
        Debug.Log("キャラ設定:" + systemPrompt);
        messages = new List<Message>
        {
            new Message(Role.System, systemPrompt),
        };
    }
    public async Task SendMessage(string comment)
    {
        ResetPrompt();

        //Remindを取得
        PromptAdd("System",characterSetting.GetRemind());

        // 新しいメッセージを追加
        PromptAdd("User", comment);

        int j = 0;
        // リストの要素を表示
        foreach (Message message in messages)
        {
            Debug.Log(j+"個目");j++;
            Debug.Log("Role: " + message.Role + ", Text: " + message.Content);
        }

        //AIからの返答を取得
        string reply = await chatGPT.SendMessage(messages);

    }


}
