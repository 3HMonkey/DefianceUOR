using Server;
using Server.Mobiles;
using System;


namespace Scripts.Mythik.Systems.Achievements
{
   
    class AchieveData
    {
        //public int ID { get; set; }
        public int Progress { get; set; }
        public DateTime? CompletedOn { get; set; }

        public AchieveData()
        {

        }
        public AchieveData(IGenericReader reader)
        {
            Deserialize(reader);
        }
        public void Serialize(IGenericWriter writer)
        {
            writer.Write(1); // version
            writer.Write(Progress);
            writer.Write(CompletedOn.ToString());

        }
        public void Deserialize(IGenericReader reader)
        {
            int version = reader.ReadInt();
            Progress = reader.ReadInt();
            CompletedOn = reader.ReadDateTime();
        }

    }
}