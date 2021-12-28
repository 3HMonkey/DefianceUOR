using Server.Targeting;

namespace Server.HellPortalSystem;

public class HellPortalTarget : Target
{
    private readonly HellPortalAltar _hellPortalAltar;

    public HellPortalTarget(HellPortalAltar hellPortalAltar) : base(10, false, TargetFlags.None)
    {
        _hellPortalAltar = hellPortalAltar;
    }

    protected override void OnTarget(Mobile from, object target)
    {
        Item itm = (Item)target;

        if (target is Item)
        {
            if (itm.IsChildOf(from.Backpack))
            {
                if (_hellPortalAltar.CurrentTokenLoad + itm.Amount <= _hellPortalAltar.MaxTokensInState)
                {
                    _hellPortalAltar.CurrentTokenLoad += itm.Amount;
                    itm.Delete();
                    // Effect
                    Point3D location = _hellPortalAltar.Location;
                    location.Z += 10;
                    Effects.SendLocationEffect(location, _hellPortalAltar.Map, 14170, 100, 10, 0, 0);
                    Effects.PlaySound(location, _hellPortalAltar.Map, 0x32E);
                }
                else
                {
                    from.SendMessage("The maximum ressource amount has been reached.");
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001);
            }
        }
        else
        {
            from.SendMessage("This is not a sacrificial ressource.");
        }
    }
}
