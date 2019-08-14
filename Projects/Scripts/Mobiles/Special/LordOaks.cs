using System;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
  public class LordOaks : BaseChampion
  {
    private BaseCreature m_Queen;
    private bool m_SpawnedQueen;

    [Constructible]
    public LordOaks() : base(AIType.AI_Mage, FightMode.Evil)
    {
      Body = 175;
      SetStr(403, 850);
      SetDex(101, 150);
      SetInt(503, 800);

      SetHits(3000);
      SetStam(202, 400);

      SetDamage(21, 33);

      SetDamageType(ResistanceType.Physical, 75);
      SetDamageType(ResistanceType.Fire, 25);

      SetResistance(ResistanceType.Physical, 85, 90);
      SetResistance(ResistanceType.Fire, 60, 70);
      SetResistance(ResistanceType.Cold, 60, 70);
      SetResistance(ResistanceType.Poison, 80, 90);
      SetResistance(ResistanceType.Energy, 80, 90);

      SetSkill(SkillName.Anatomy, 75.1, 100.0);
      SetSkill(SkillName.EvalInt, 120.1, 130.0);
      SetSkill(SkillName.Magery, 120.0);
      SetSkill(SkillName.Meditation, 120.1, 130.0);
      SetSkill(SkillName.MagicResist, 100.5, 150.0);
      SetSkill(SkillName.Tactics, 100.0);
      SetSkill(SkillName.Wrestling, 100.0);

      Fame = 22500;
      Karma = 22500;

      VirtualArmor = 100;
    }

    public LordOaks(Serial serial) : base(serial)
    {
    }

    public override ChampionSkullType SkullType => ChampionSkullType.Enlightenment;

    public override Type[] UniqueList => new[] { typeof(OrcChieftainHelm) };

    public override Type[] SharedList => new[]
    {
      typeof(RoyalGuardSurvivalKnife),
      typeof(DjinnisRing),
      typeof(LieutenantOfTheBritannianRoyalGuard),
      typeof(SamaritanRobe),
      typeof(DetectiveBoots),
      typeof(TheMostKnowledgePerson)
    };

    public override Type[] DecorativeList => new[]
    {
      typeof(WaterTile),
      typeof(WindSpirit),
      typeof(Pier),
      typeof(DirtPatch)
    };

    public override MonsterStatuetteType[] StatueTypes => new MonsterStatuetteType[] { };

    public override string DefaultName => "Lord Oaks";

    public override bool AutoDispel => true;
    public override bool CanFly => true;
    public override bool BardImmune => !Core.SE;
    public override bool Unprovokable => Core.SE;
    public override bool Uncalmable => Core.SE;
    public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;

    public override Poison PoisonImmune => Poison.Deadly;

    public override void GenerateLoot()
    {
      AddLoot(LootPack.UltraRich, 5);
    }

    public void SpawnPixies(Mobile target)
    {
      Map map = Map;

      if (map == null)
        return;

      Say(1042154); // You shall never defeat me as long as I have my queen!

      int newPixies = Utility.RandomMinMax(3, 6);

      for (int i = 0; i < newPixies; ++i)
      {
        Pixie pixie = new Pixie { Team = Team, FightMode = FightMode.Closest };

        pixie.MoveToWorld(map.GetRandomNearbyLocation(Location), map);
        pixie.Combatant = target;
      }
    }

    public override int GetAngerSound()
    {
      return 0x2F8;
    }

    public override int GetIdleSound()
    {
      return 0x2F8;
    }

    public override int GetAttackSound()
    {
      return Utility.Random(0x2F5, 2);
    }

    public override int GetHurtSound()
    {
      return 0x2F9;
    }

    public override int GetDeathSound()
    {
      return 0x2F7;
    }

    public void CheckQueen()
    {
      if (Map == null)
        return;

      if (!m_SpawnedQueen)
      {
        Say(1042153); // Come forth my queen!

        m_Queen = new Silvani { Team = Team };
        m_Queen.MoveToWorld(Location, Map);

        m_SpawnedQueen = true;
      }
      else if (m_Queen?.Deleted != false)
        m_Queen = null;
    }

    public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
    {
      CheckQueen();

      if (m_Queen != null)
      {
        scalar *= 0.1;

        if (0.1 >= Utility.RandomDouble())
          SpawnPixies(caster);
      }
    }

    public override void OnGaveMeleeAttack(Mobile defender)
    {
      base.OnGaveMeleeAttack(defender);

      defender.Damage(Utility.Random(20, 10), this);
      defender.Stam -= Utility.Random(20, 10);
      defender.Mana -= Utility.Random(20, 10);
    }

    public override void OnGotMeleeAttack(Mobile attacker)
    {
      base.OnGotMeleeAttack(attacker);

      CheckQueen();

      if (m_Queen != null && 0.1 >= Utility.RandomDouble())
        SpawnPixies(attacker);

      attacker.Damage(Utility.Random(20, 10), this);
      attacker.Stam -= Utility.Random(20, 10);
      attacker.Mana -= Utility.Random(20, 10);
    }

    public override void Serialize(GenericWriter writer)
    {
      base.Serialize(writer);

      writer.Write(0); // version

      writer.Write(m_Queen);
      writer.Write(m_SpawnedQueen);
    }

    public override void Deserialize(GenericReader reader)
    {
      base.Deserialize(reader);

      int version = reader.ReadInt();

      switch (version)
      {
        case 0:
        {
          m_Queen = reader.ReadMobile<BaseCreature>();
          m_SpawnedQueen = reader.ReadBool();

          break;
        }
      }
    }
  }
}