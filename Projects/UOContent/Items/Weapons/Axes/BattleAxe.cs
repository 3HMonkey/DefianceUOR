namespace Server.Items
{
    [Serializable(0, false)]
    [Flippable(0xF47, 0xF48)]
    public partial class BattleAxe : BaseAxe
    {
        [Constructible]
        public BattleAxe() : base(0xF47)
        {
            Weight = 4.0;
            Layer = Layer.TwoHanded;
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.BleedAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ConcussionBlow;

        public override int AosStrengthReq => 35;
        public override int AosMinDamage => 15;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 31;
        public override float MlSpeed => 3.50f;

        public override int OldStrengthReq => 40;
        public override int OldMinDamage => 6;
        public override int OldMaxDamage => 38;
        public override int OldSpeed => 30;

        public override int Dice_Num => 2;
        public override int Dice_Sides => 17;
        public override int Dice_Offset => 4;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;
    }
}
