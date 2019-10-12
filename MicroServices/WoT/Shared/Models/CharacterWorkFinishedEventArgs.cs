using System;

namespace WoT.Shared
{
    public class CharacterWorkFinishedEventArgs : EventArgs
    {
        public User User { get; }
        public Character Character { get; }
        public ICharacterEntity<CharacterWork> Entity { get; }
        public uint Hours { get; }
        public uint Reward { get; }

        public CharacterWorkFinishedEventArgs(
            User user,
            Character character,
            ICharacterEntity<CharacterWork> entity,
            uint hours,
            uint reward)
        {
            User = user;
            Character = character;
            Entity = entity;
            Hours = hours;
            Reward = reward;
        }

        public override string ToString()
        {
            return $"Character [{Character.Name}] just finished their work! +{Reward} Gold for {Hours} hours of work.";
        }
    }
}
