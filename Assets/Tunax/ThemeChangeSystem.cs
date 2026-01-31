using Sirenix.OdinInspector;
using UnityEngine;

namespace Tunax
{
    public class ThemeChangeSystem : MonoBehaviour
    {
        public AnimatedManager[] managers;

        [Button]
        public void PlayAnimation(bool openClose)
        {
            foreach (var animatedManager in managers)
            {
                animatedManager.PlayAnimation(openClose);
            }
        }
    }
}
