using AnyoxGames.Character;
using UnityEngine;

namespace AnyoxGames.Audio
{
    public class FoleyMaterial : MonoBehaviour, IFoleyCollider
    {
        [SerializeField] private SoundBank walkingSoundBank;
        [SerializeField] private SoundBank runningSoundBank;
        [SerializeField] private float runSpeedThreshold = 10;
        
        public void PlayFootstepSound(ACharacter character, float masterVolume)
        {
            var isRunning = character.TryGetCharacterBehaviour(out PlayerLocomotionBehaviour locomotionBehaviour) && locomotionBehaviour.Velocity.sqrMagnitude > runSpeedThreshold;
            Sound.PlayAtPosition(isRunning ? runningSoundBank.Next() : walkingSoundBank.Next(), character.transform.position, masterVolume);
        }
    }
}