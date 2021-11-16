using UnityEngine;

public enum PitchType { VeryLow, Low, Normal, High, VeryHigh }

[System.Serializable]
public struct AudioData {
    [SerializeField] string name;
    [SerializeField] bool onStart;
    [SerializeField] AudioClip clip;
    [SerializeField, Range(0f, 1f)] float volume;
    [SerializeField] PitchType pitchType;
    [SerializeField] bool loop;

    public string Name => name;

    public bool OnStart => onStart;

    public AudioClip Clip => clip;

    public float Volume => volume;

    public float Pitch {
        get {
            switch (pitchType) {
                case PitchType.VeryLow: return .5f;
                case PitchType.Low: return .5f;
                case PitchType.Normal: return 1f;
                case PitchType.High: return 2f;
                case PitchType.VeryHigh: return 3f;
                default: return 0f;
            }
        }
    }

    public bool Loop => loop;
}