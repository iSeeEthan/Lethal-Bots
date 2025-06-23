using LethalBots.Enums;
using System;

namespace LethalBots.SaveAdapter
{
    /// <summary>
    /// Represents the date serializable, to be saved on disk, necessay for LethalBot (not much obviously)
    /// </summary>
    [Serializable]
    public class SaveFile
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IdentitySaveFile[] IdentitiesSaveFiles;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    [Serializable]
    public class IdentitySaveFile
    {
        public int IdIdentity;
        public int SuitID;
        public int Hp;
        public int Status;
        public int XP;
        public int Level;

        public override string ToString()
        {
            return $"IdIdentity: {IdIdentity}, suitID {SuitID}, Hp {Hp}, XP {XP}, Level {Level}, Status {Status} {(EnumStatusIdentity)Status}";
        }
    }
}
