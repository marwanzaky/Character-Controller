using UnityEngine;
using MarwanZaky.Audio;

namespace MarwanZaky
{
    public class Destroy : MonoBehaviour
    {
        [SerializeField] GameObject fractured;
        [SerializeField] string audioClip;

        public void Invoke()
        {
            Instantiate(fractured, transform.position, transform.rotation);
            AudioManager.Instance.Play(audioClip, transform.position);
            Destroy(gameObject);
        }
    }
}