using UnityEngine;
using System.Collections;
using System;

namespace MarwanZaky
{
    namespace Audio
    {
        public class AudioManager : MonoBehaviour
        {
            #region Singletone

            public static AudioManager Instance { get; private set; }

            void Awake()
            {
                if (Instance == null)
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    Debug.Log("Multiple AudioManager instances found (Deleted)");
                    Destroy(gameObject);
                }
            }

            #endregion

            [SerializeField] AudioManagerData data;

            void Start()
            {
                foreach (var el in data.Datas)
                    if (el.OnStart)
                        Play(el.Name);
            }

            public (GameObject gameObject, AudioClip audioClip) Play(string name)
            {
                return Play(name, position: Vector3.zero, spatialBlend: 0);
            }

            public (GameObject gameObject, AudioClip audioClip) Play(string name, Vector3 position, float spatialBlend = .8f)
            {
                foreach (var el in data.Datas)
                    if (el.Name == name)
                        return Play(el, position, spatialBlend);

                return (null, null);
            }

            public (GameObject gameObject, AudioClip audioClip) Play(AudioData data, Vector3 position, float spatialBlend, Action onDestroy = null)
            {
                var res = (go: null as GameObject, ac: null as AudioClip);

                var coroutine = PlayIE(data, position, spatialBlend, onDestroy, (go, ac) =>
                 {
                     res.go = go; res.ac = ac;
                 });

                StartCoroutine(coroutine);

                return res;
            }

            IEnumerator PlayIE(AudioData data, Vector3 position, float spatialBlend, Action onDestory, Action<GameObject, AudioClip> result)
            {
                var go = new GameObject(data.Clip.name);
                var audioSource = go.AddComponent<AudioSource>();

                go.transform.position = position;

                audioSource.clip = data.Clip;
                audioSource.volume = data.Volume;
                audioSource.pitch = data.Pitch;
                audioSource.loop = data.Loop;
                audioSource.spatialBlend = spatialBlend;

                audioSource.Play();

                if (!data.Loop)
                    Destroy(go, data.Clip.length);

                result?.Invoke(go, audioSource.clip);

                yield return new WaitForSeconds(data.Clip.length);

                onDestory?.Invoke();
            }
        }
    }
}