
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Manager Data", menuName = "Scriptable Objects/Audio Manager Data", order = 20)]
public class AudioManagerData : ScriptableObject {
    [SerializeField] AudioData[] datas;

    public AudioData[] Datas {
        get {
            return datas;
        }
    }
}