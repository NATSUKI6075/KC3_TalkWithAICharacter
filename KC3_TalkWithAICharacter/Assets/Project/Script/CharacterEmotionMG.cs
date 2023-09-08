using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class CharacterEmotionMG : MonoBehaviour
{
    List<string> emotions = new List<string>();
    VRMBlendShapeProxy vrmBlendShapeProxy = null;
    private BlendShapePreset currentPreset = BlendShapePreset.Unknown;

    // Start is called before the first frame update
    void Start()
    {
        vrmBlendShapeProxy = GetComponent<VRMBlendShapeProxy>();
        SetVRMBlendShapeProxy(vrmBlendShapeProxy);
    }

    public List<string> GetEmotions()
    {
        return emotions;
    }

    public void SetVRMBlendShapeProxy(VRMBlendShapeProxy newVRMBlendShapeProxy)
    {
        vrmBlendShapeProxy = newVRMBlendShapeProxy;

        var blendShapeClips = vrmBlendShapeProxy.BlendShapeAvatar.Clips;
     
        foreach (var clip in blendShapeClips)
        {
            if (!clip.name.StartsWith("Look") && clip.name != "A" && clip.name != "I" && clip.name != "U" && clip.name != "E" && clip.name != "O")
            {
                emotions.Add(clip.name.Replace("BlendShape.",""));
            }
        }

        foreach (var name in emotions)
        {
           // Debug.Log(name);
        }
    }

    public void SetEmotion(string emotion)
    {
        if (vrmBlendShapeProxy != null)
        {
            foreach(string tmp in emotions)
            {
                if (tmp == emotion)
                {
                    var preset = Enum.TryParse<BlendShapePreset>(emotion, true, out var result)
                        ? result
                        : BlendShapePreset.Unknown;

                    //ResetEmotion();

                    vrmBlendShapeProxy.SetValue(preset, 1f);
                    Debug.Log(emotion + "の表情をする。");

                    currentPreset = preset;
                }
            }
        }
        else
        {
            Debug.Log("vrmBlendShapeProxy = null");
        }
    }

    public void ResetEmotion()
    {
        if (currentPreset != BlendShapePreset.Unknown && vrmBlendShapeProxy != null)
        {
            vrmBlendShapeProxy.SetValue(currentPreset, 0f);
        }
    }
}
