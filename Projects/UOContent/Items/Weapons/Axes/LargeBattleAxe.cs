namespace Server.Items
{
    [Serializable(0, false)]
    [Flippable(0x13FB, 0x13FA)]
    public partial class LargeBattleAxe : BaseAxe
    {
        [Constructible]
        public LargeBattleAxe() : base(0x13FB) => Weight = 6.0;

        public override WeaponAbility PrimaryAbility => WeaponAbility.WhirlwindAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.BleedAttack;

        public override int AosStrengthReq => 80;
        public override int AosMinDamage => 16;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 29;
        public override float MlSpeed => 3.75f;

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
