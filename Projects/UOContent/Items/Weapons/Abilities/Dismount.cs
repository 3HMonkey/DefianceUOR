using System;
using Server.Mobiles;
using Server.Spells.Ninjitsu;

namespace Server.Items
{
    /// <summary>
    ///     Perfect for the foot-soldier, the Dismount special attack can unseat a mounted opponent.
    ///     The fighter using this ability must be on his own two feet and not in the saddle of a steed
    ///     (with one exception: players may use a lance to dismount other players while mounted).
    ///     If it works, the target will be knocked off his own mount and will take some extra damage from the fall!
    /// </summary>
    public class Dismount : WeaponAbility
    {
        public static readonly TimeSpan RemountDelay = TimeSpan.FromSeconds(10.0);

        public override int BaseMana => 20;

        public override bool Validate(Mobile from)
        {
            if (!base.Validate(from))
            {
                return false;
            }

            if (from.Mounted && from.Weapon is not Lance)
            {
                from.SendLocalizedMessage(1061283); // You cannot perform that attack while mounted!
                return false;
            }

            return true;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
            {
                return;
            }

            if (defender is ChaosDragoon || defender is ChaosDragoonElite)
            {
                return;
            }

            // TODO: Should there be a message here?
            if (attacker.Mounted && (attacker.Weapon is not Lance || defender.Weapon is not Lance))
            {
                return;
            }

            ClearCurrentAbility(attacker);

            var mount = defender.Mount;

            if (mount == null && !AnimalForm.UnderTransformation(defender))
            {
                attacker.SendLocalizedMessage(1060848); // This attack only works on mounted targets
                return;
            }

            if (Core.ML && attacker is LesserHiryu && Utility.RandomDouble() <= 0.8)
            {
                return; // Lesser Hiryu have an 80% chance of missing this attack
            }

            attacker.SendLocalizedMessage(1060082); // The force of your attack has dislodged them from their mount!

            if (attacker.Mounted)
            {
                defender.SendLocalizedMessage(1062315); // You fall off your mount!
            }
            else
            {
                defender.SendLocalizedMessage(1060083); // You fall off of your mount and take damage!
            }

            defender.PlaySound(0x140);
            defender.FixedParticles(0x3728, 10, 15, 9955, EffectLayer.Waist);

            if (defender is PlayerMobile mobile)
            {
                if (AnimalForm.UnderTransformation(mobile))
                {
                    mobile.SendLocalizedMessage(1114066, attacker.Name); // ~1_NAME~ knocked you out of animal form!
                }
                else if (mobile.Mounted)
                {
                    mobile.SendLocalizedMessage(1040023); // You have been knocked off of your mount!
                }

                mobile.SetMountBlock(BlockMountType.Dazed, TimeSpan.FromSeconds(10), true);
            }
            else
            {
                defender.Mount.Rider = null;
            }

            if (attacker is PlayerMobile playerMobile)
            {
                playerMobile.SetMountBlock(BlockMountType.DismountRecovery, RemountDelay, true);
            }
            else if (Core.ML && attacker is BaseCreature { ControlMaster: PlayerMobile pm })
            {
                pm.SetMountBlock(BlockMountType.DismountRecovery, RemountDelay, false);
            }

            if (!attacker.Mounted)
            {
                AOS.Damage(defender, attacker, Utility.RandomMinMax(15, 25), 100, 0, 0, 0, 0);
            }
        }
    }
}
