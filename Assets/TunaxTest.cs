using UnityEngine;
using DG.Tweening;

public class TunaxTest : MonoBehaviour
{
    public Transform moveObject;
    public Transform target;

    private void Start()
    {
        Invoke("JumpToTarget", 2f);
    }

 


    public void JumpToTarget(){
        moveObject.DOJump(target.position, 3, 0, 2).SetEase(Ease.InOutSine);
    }
}
