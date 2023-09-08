using UnityEngine;
using VRM;

public class VRMLipSyncContextMorphTarget : MonoBehaviour
{
    private OVRLipSyncContextBase lipsyncContext = null;
    public int smoothAmount = 70;
    
    VRMBlendShapeProxy proxy;
    int[] VisemeToBlendTargets = { -1,4,4,5,3,3,3,3,-1,3,2,5,3,6,4 }; //AIUEOへの振り分けテーブル

    [Range(0.0f, 2.0f)]
    public float lipSyncSensitivity = 1.0f;

    void Start ()
    {
        lipsyncContext = GetComponent<OVRLipSyncContextBase>();
        if(lipsyncContext == null)
        {
            Debug.LogError("LipSyncContextMorphTarget.Start Error: " +
                "No OVRLipSyncContext component on this object!");
        }
        else
        {
            lipsyncContext.Smoothing = smoothAmount;
        }
    }

    void LateUpdate ()
    {
        if(lipsyncContext != null)
        {
            OVRLipSync.Frame frame = lipsyncContext.GetCurrentPhonemeFrame();
            if (frame != null)
            {
                UpdateVRMMorph(frame);
               
            }

            if (smoothAmount != lipsyncContext.Smoothing)
            {
                lipsyncContext.Smoothing = smoothAmount;
            }
        }
    }

    private void UpdateVRMMorph(OVRLipSync.Frame frame)
    {
        if (!proxy)
        {
            proxy = FindObjectOfType<VRMBlendShapeProxy>();
        }

        if (!proxy) return;
        for (int i = 1; i < VisemeToBlendTargets.Length; i++)
        {
            if (VisemeToBlendTargets[i] != -1)
            {
                proxy.SetValue((BlendShapePreset)VisemeToBlendTargets[i], Mathf.Min(1.0f, frame.Visemes[i] * lipSyncSensitivity));
            }
        }
    }

    public void SetVRMBlendShapeProxy(VRMBlendShapeProxy _proxy)
    {
        proxy = _proxy;
    }
}
