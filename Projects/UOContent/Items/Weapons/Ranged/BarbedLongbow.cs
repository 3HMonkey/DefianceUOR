namespace Server.Items
{
    [Serializable(0)]
    public partial class BarbedLongbow : ElvenCompositeLongbow
    {
        [Constructible]
        public BarbedLongbow() => Attributes.ReflectPhysical = 12;

        public override int LabelNumber => 1073505; // barbed longbow
    }
}
