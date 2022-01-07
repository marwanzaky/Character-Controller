using UnityEngine;
using System.Linq;

namespace MarwanZaky
{
    namespace Methods
    {
        public static class AnimatorX
        {
            public static float GetLength(string name, Animator animator)
            {
                var clips = animator.runtimeAnimatorController.animationClips;
                var clipName = clips.FirstOrDefault(el => el.name == name);

                if (clipName) return clipName.length;
                Debug.LogWarning($"The animator animation named {name} was not found");
                return 0;
            }
        }
    }
}