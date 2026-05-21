using AnyoxGames.Character;

namespace AnyoxGames.Audio
{
    public interface IFoleyCollider
    {
        public void PlayFootstepSound(ACharacter character, float masterVolume);
    }
}