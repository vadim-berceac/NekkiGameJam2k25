using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [field: SerializeField] public FootStepsSettings FootStepsSettings { get; set; }
    
    public void PlayL()
    {
        Play(FootStepsSettings.LFoot);
    }

    public void PlayR()
    {
        Play(FootStepsSettings.RFoot);
    }

    private void Play (Transform t)
    {
        AudioSource.PlayClipAtPoint(FootStepsSettings.FootStepSounds[Random.Range(0, 
            FootStepsSettings.FootStepSounds.Length)], t.position);
    }
}

[System.Serializable]
public struct FootStepsSettings
{
    [field: SerializeField] public Transform LFoot {get;set;}
    [field: SerializeField] public Transform RFoot {get;set;}
    [field: SerializeField] public AudioClip[] FootStepSounds { get; set; }
}
