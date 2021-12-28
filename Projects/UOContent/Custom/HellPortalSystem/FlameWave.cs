using System;
using Server.Items;
using Server.Mobiles;

namespace Server.HellPortalSystem;

public class FlameWave : Timer
{
    private readonly Item m_From;
    private Point3D m_StartingLocation;
    private readonly Map m_Map;
    private int m_Count;
    private Point3D m_Point;

    public FlameWave(Item from)
        : base(TimeSpan.FromMilliseconds(300.0), TimeSpan.FromMilliseconds(300.0))
    {
        m_From = from;
        m_StartingLocation = from.Location;
        m_Map = from.Map;
        m_Count = 0;
        m_Point = new Point3D();
        SetupDamage(from);
    }

    protected override void OnTick()
    {
        if (m_From == null || m_From.Deleted)
        {
            Stop();
            return;
        }

        double dist = 0.0;

        for (int i = -m_Count; i < m_Count + 1; i++)
        {
            for (int j = -m_Count; j < m_Count + 1; j++)
            {
                m_Point.X = m_StartingLocation.X + i;
                m_Point.Y = m_StartingLocation.Y + j;
                m_Point.Z = m_Map.GetAverageZ(m_Point.X, m_Point.Y);
                dist = GetDist(m_StartingLocation, m_Point);
                if (dist < ((double)m_Count + 0.1) && dist > ((double)m_Count - 3.1))
                {
                    Effects.SendLocationParticles(
                        EffectItem.Create(m_Point, m_Map, EffectItem.DefaultDuration),
                        0x3709,
                        10,
                        30,
                        5052
                    );
                }
            }
        }

        m_Count += 3;

        if (m_Count > 15)
            Stop();
    }

    private void SetupDamage(Item from)
    {
        foreach (Mobile m in from.GetMobilesInRange(12))
        {
            if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned) || m.Player)
            {
                //Timer.DelayCall(TimeSpan.FromMilliseconds(300 * (GetDist(m_StartingLocation, m.Location) / 3)), new TimerStateCallback<object>(Hurt), m);
                Timer.DelayCall(
                    TimeSpan.FromMilliseconds(300 * (GetDist(m_StartingLocation, m.Location) / 3)),
                    () => Hurt(m)
                );
            }
        }
    }

    public void Hurt(object o)
    {
        Mobile m = o as Mobile;

        if (m_From == null || m == null || m.Deleted)
            return;

        int toDamage = Utility.RandomMinMax(10, 90);
        m.Damage(toDamage);

        m.SendMessage("You are being burnt alive by the seering heat!");
    }

    private double GetDist(Point3D start, Point3D end)
    {
        int xdiff = start.X - end.X;
        int ydiff = start.Y - end.Y;
        return Math.Sqrt((xdiff * xdiff) + (ydiff * ydiff));
    }
}
