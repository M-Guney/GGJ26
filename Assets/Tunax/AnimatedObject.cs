using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tunax
{
    public class AnimatedObject : MonoBehaviour
    {
        public Transform myTransform;
        public Transform firstTransform;
        public Transform targetTransform;
        private Tween moveTween;
        private Tween rotateTween;
        private Tween scaleTween;
        
        [Button]
        public void Animate(float speed,float delay,bool goToTarget=true)
        {
            var transformTarget = goToTarget ? targetTransform : firstTransform;
            AllTweenKill();
            moveTween = myTransform.DOMove(transformTarget.position, speed).SetDelay(delay);
            rotateTween= myTransform.DORotate(transformTarget.eulerAngles, speed).SetDelay(delay);
            scaleTween = myTransform.DOScale(transformTarget.localScale, speed).SetDelay(delay);
        }

        private void AllTweenKill()
        {
            moveTween?.Kill();
            rotateTween?.Kill();
            scaleTween?.Kill();
        }
    }
}