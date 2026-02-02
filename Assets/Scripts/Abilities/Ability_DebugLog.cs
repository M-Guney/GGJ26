using UnityEngine;

namespace GGJ26.Abilities
{
    [CreateAssetMenu(fileName = "NewDebugAbility", menuName = "GGJ26/Abilities/Debug Log")]
    public class Ability_DebugLog : MaskAbility
    {
        public string message = "Ability Activated!";

        public override void Activate(GameObject user)
        {
            Debug.Log($"[Ability] {user.name} used {name}: {message}");
        }
    }
}
