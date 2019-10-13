using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using LionLibrary.SQL;
using System.Threading.Tasks;
using MrConnect.Shared;

namespace WoT.Shared
{
    [Table("characters_work")]
    public class CharacterWork : ICharacterEntity<CharacterWork>
    {
        public static class Ref
        {
            public const string CharacterId = "CharacterId";
            public const string IsWorking = "IsWorking";
            public const string CommittedHours = "CommittedHours";
            public const string WorkFinishesAt = "WorkFinishesAt";
            public const string TotalHours = "TotalHours";
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public uint CharacterId { get; set; }
        [JsonIgnore, IgnoreDataMember]
        public Character Character { get; set; }

        public bool IsWorking { get; set; }
        public byte CommittedHours { get; set; }
        public DateTime WorkFinishesAt { get; set; }

        public uint TotalHours { get; set; }

        private uint _levelsConvertedFromHours = 0;

        private uint _level;
        public uint Level => CalculateLevel().level;

        private uint _xp;
        public uint Xp => CalculateLevel().xp;

        private uint _xpCap;
        public uint XpCap => CalculateLevel().xpCap;

        private uint _rewardConvertedFromLevel = 0;
        private uint _hourlyReward;
        public uint HourlyReward => CalculateHourlyReward();

        private uint CalculateHourlyReward()
        {
            //Nothing changed, ignore hourly reward calculation algorithm
            if (_rewardConvertedFromLevel == Level)
            {
                return _hourlyReward;
            }
            uint workLevel = Level;
            uint transitionalLevel = 1;
            uint rewardIncrement = 15;
            uint reward = 15;
            while (transitionalLevel < workLevel)
            {
                transitionalLevel++;
                reward += rewardIncrement;
                rewardIncrement += 5;
            }

            _rewardConvertedFromLevel = workLevel;
            _hourlyReward = reward;

            return reward;
        }

        private (uint level, uint xp, uint xpCap) CalculateLevel()
        {
            //Nothing changed, ignore level calculation algorithm
            if (_levelsConvertedFromHours == TotalHours)
            {
                return (_level, _xp, _xpCap);
            }

            uint level = 1;
            uint xp = TotalHours;
            uint xpCap = 24;

            for (; xp >= xpCap; xpCap += 24)
            {
                xp -= xpCap;
                level++;
            }

            _level = level;
            _xp = xp;
            _xpCap = xpCap;
            _levelsConvertedFromHours = TotalHours;

            return (_level, _xp, _xpCap);
        }

        public async Task<CharacterWorkFinishedEventArgs> UpdateAsync(DbContext db, DbSet<User> users, DbSet<Character> characters)
        {
            if (DateTime.Now >= WorkFinishesAt)
            {
                Character character = characters.Find(CharacterId);
                User user = users.Find(character.UserId);

                //Add gold to the character
                uint reward = HourlyReward * CommittedHours;
                if (user.IsPremium)
                {
                    reward += reward / 5;
                }
                character.Gold += reward;

                CharacterWorkFinishedEventArgs eventArgs = new CharacterWorkFinishedEventArgs(user, character, this, CommittedHours, reward);

                IsWorking = false;
                TotalHours += CommittedHours;
                CommittedHours = 0;

                await db.UpdateEntityAsync(this);
                await db.UpdateEntityAsync(Character);

                return eventArgs;
            }
            return null;
        }

        public void CancelWork()
        {
            IsWorking = false;
            CommittedHours = 0;
        }

        public SharedDiscordEmbedField AsEmbedField(bool isInline = false) =>
            new SharedDiscordEmbedField
            {
                Name = "⛏ Mining Stats ⛏",
                Value =
                $"```Ini\n" +
                $"[Mining Stats]\n" +
                $"Level = {Level}\n" +
                $"Progress = ( {Xp} / {XpCap} ) |+| {XpCap - Xp} hours left.\n" +
                $"Total = {TotalHours}\n```",
                IsInline = isInline
            };

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CharacterWork>();
        }
    }
}
