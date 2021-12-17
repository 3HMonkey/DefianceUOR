using Server;
using Server.Items;

public class OrganizePouch : BaseContainer
{
    [Constructible]
    public OrganizePouch() : base(0xE79)
    {
        Weight = 1.0;
        Name = "Organization Pouch";
    }

    public OrganizePouch(Serial serial)
        : base(serial)
    {
    }

    public override void Serialize(IGenericWriter writer)
    {
        base.Serialize(writer);

        writer.Write(0); // version
    }

    public override void Deserialize(IGenericReader reader)
    {
        base.Deserialize(reader);

        int version = reader.ReadInt();
    }
}
