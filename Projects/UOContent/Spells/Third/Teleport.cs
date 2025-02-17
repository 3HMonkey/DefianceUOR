using Server.Factions;
using Server.Items;
using Server.Misc;
using Server.Regions;
using Server.Spells.Fifth;
using Server.Spells.Fourth;
using Server.Spells.Sixth;

namespace Server.Spells.Third
{
    public class TeleportSpell : MagerySpell, ISpellTargetingPoint3D
    {
        private static readonly SpellInfo _info = new(
            "Teleport",
            "Rel Por",
            215,
            9031,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot
        );

        public TeleportSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Third;

        public void Target(IPoint3D p)
        {
            var orig = p;
            var map = Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            var from = Caster.Location;
            var to = new Point3D(p);

            if (Sigil.ExistsOn(Caster))
            {
                Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (WeightOverloading.IsOverloaded(Caster))
            {
                Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
            }
            else if (!SpellHelper.CheckTravel(Caster, TravelCheckType.TeleportFrom))
            {
            }
            else if (!SpellHelper.CheckTravel(Caster, map, to, TravelCheckType.TeleportTo))
            {
            }
            else if (map?.CanSpawnMobile(p.X, p.Y, p.Z) != true)
            {
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (SpellHelper.CheckMulti(to, map))
            {
                Caster.SendLocalizedMessage(502831); // Cannot teleport to that spot.
            }
            else if (Region.Find(to, map).IsPartOf<HouseRegion>())
            {
                Caster.SendLocalizedMessage(502829); // Cannot teleport to that spot.
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, orig);

                var m = Caster;

                m.Location = to;
                m.ProcessDelta();

                if (m.Player)
                {
                    Effects.SendLocationParticles(
                        EffectItem.Create(from, m.Map, EffectItem.DefaultDuration),
                        0x3728,
                        10,
                        10,
                        2023
                    );
                    Effects.SendLocationParticles(
                        EffectItem.Create(to, m.Map, EffectItem.DefaultDuration),
                        0x3728,
                        10,
                        10,
                        5023
                    );
                }
                else
                {
                    m.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                }

                m.PlaySound(0x1FE);

                var eable = m.GetItemsInRange(0);

                foreach (var item in eable)
                {
                    if (item is ParalyzeFieldSpell.InternalItem || item is PoisonFieldSpell.InternalItem ||
                        item is FireFieldSpell.FireFieldItem)
                    {
                        item.OnMoveOver(m);
                    }
                }

                eable.Free();
            }

            FinishSequence();
        }

        public override bool CheckCast()
        {
            if (Sigil.ExistsOn(Caster))
            {
                Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }

            if (WeightOverloading.IsOverloaded(Caster))
            {
                Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                return false;
            }

            return SpellHelper.CheckTravel(Caster, TravelCheckType.TeleportFrom);
        }

        public override void OnCast()
        {
            Caster.Target = new SpellTargetPoint3D(this, range: Core.ML ? 10 : 12);
        }
    }
}
