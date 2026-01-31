using Sirenix.OdinInspector;
using UnityEngine;

namespace Tunax
{
    public class AnimatedManager : MonoBehaviour
    {
        public AnimatedObject[] animatedObjects;
        public float speed = 1;
        public float delay = 0.2f;

        [Button]
        public void PlayAnimation(bool open = false)
        {
            float delay = 0;
            foreach (AnimatedObject animatedObject in animatedObjects)
            {
                animatedObject.Animate(speed,delay,open);
                delay += 0.2f;
            }
        }
    }
}