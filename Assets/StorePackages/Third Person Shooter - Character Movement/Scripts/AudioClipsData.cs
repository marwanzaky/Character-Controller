
using UnityEngine;

namespace Packtool
{
    [CreateAssetMenu(fileName = "New Audio Clips Data", menuName = "Scriptable Objects/Audio Clips Data", order = 20)]
    public class AudioClipsData : ScriptableObject
    {
        public float volume = 1f;
        public float pitch = 1f;

        public AudioClip[] clips;

        public (AudioClip audioClip, float length) RandomClip()
        {
            var res = clips[Random.Range(0, clips.Length - 1)];
            return (res, res.length / pitch);
        }
    }
}