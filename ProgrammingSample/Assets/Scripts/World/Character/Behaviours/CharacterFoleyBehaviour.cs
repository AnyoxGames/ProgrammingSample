using AnyoxGames.Audio;
using UnityEngine;

public class CharacterFoleyBehaviour : ACharacterBehaviour
{
    [SerializeField] private float footstepDistance;
    [SerializeField] private float volumeMultiplier = 1;

    private IFoleyCollider foleyCollider;
    
    private float distanceSinceLastFootstep;
    private Vector3 lastPosition;

    private CharacterLocomotionBehaviour locomotionBehaviour;

    public override void Initialize()
    {
        character.TryGetCharacterBehaviour(out locomotionBehaviour);
    }

    private void Update()
    {
        if (locomotionBehaviour && !locomotionBehaviour.IsGrounded)
        {
            distanceSinceLastFootstep = 0;
            return;
        }
        
        var position = transform.position;
        distanceSinceLastFootstep += Vector3.Distance(lastPosition, position);
        lastPosition = position;
        
        if (distanceSinceLastFootstep > footstepDistance)
        {
            distanceSinceLastFootstep = 0;
            foleyCollider?.PlayFootstepSound(character, volumeMultiplier);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.TryGetComponent(out IFoleyCollider newFoleyCollider) && hit.normal.y > 0.6)
        {
            foleyCollider = newFoleyCollider;
        }
    }
}