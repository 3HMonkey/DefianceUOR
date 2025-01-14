using System;
using Server.HuePickers;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class PlayerVendorBuyGump : Gump
    {
        private readonly PlayerVendor m_Vendor;
        private readonly VendorItem m_VI;

        public PlayerVendorBuyGump(PlayerVendor vendor, VendorItem vi) : base(100, 200)
        {
            m_Vendor = vendor;
            m_VI = vi;

            AddBackground(100, 10, 300, 150, 5054);

            AddHtmlLocalized(125, 20, 250, 24, 1019070); // You have agreed to purchase:

            if (!string.IsNullOrEmpty(vi.Description))
            {
                AddLabel(125, 45, 0, vi.Description);
            }
            else
            {
                AddHtmlLocalized(125, 45, 250, 24, 1019072); // an item without a description
            }

            AddHtmlLocalized(125, 70, 250, 24, 1019071); // for the amount of:
            AddLabel(125, 95, 0, vi.Price.ToString());

            AddButton(250, 130, 4005, 4007, 0);
            AddHtmlLocalized(282, 130, 100, 24, 1011012); // CANCEL

            AddButton(120, 130, 4005, 4007, 1);
            AddHtmlLocalized(152, 130, 100, 24, 1011036); // OKAY
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            var from = state.Mobile;

            if (!m_Vendor.CanInteractWith(from, false))
            {
                return;
            }

            if (m_Vendor.IsOwner(from))
            {
                m_Vendor.SayTo(from, 503212); // You own this shop, just take what you want.
                return;
            }

            if (info.ButtonID == 1)
            {
                m_Vendor.Say(from.Name);

                if (!m_VI.Valid || !m_VI.Item.IsChildOf(m_Vendor.Backpack))
                {
                    m_Vendor.SayTo(from, 503216); // You can't buy that.
                    return;
                }

                var totalGold = 0;

                if (from.Backpack != null)
                {
                    totalGold += from.Backpack.GetAmount(typeof(Gold));
                }

                totalGold += Banker.GetBalance(from);

                if (totalGold < m_VI.Price)
                {
                    m_Vendor.SayTo(from, 503205); // You cannot afford this item.
                }
                else if (!from.PlaceInBackpack(m_VI.Item))
                {
                    m_Vendor.SayTo(from, 503204); // You do not have room in your backpack for this.
                }
                else
                {
                    var leftPrice = m_VI.Price;

                    if (from.Backpack != null)
                    {
                        leftPrice -= from.Backpack.ConsumeUpTo(typeof(Gold), leftPrice);
                    }

                    if (leftPrice > 0)
                    {
                        Banker.Withdraw(from, leftPrice);
                    }

                    m_Vendor.HoldGold += m_VI.Price;

                    from.SendLocalizedMessage(503201); // You take the item.
                }
            }
            else
            {
                from.SendLocalizedMessage(503207); // Cancelled purchase.
            }
        }
    }

    public class PlayerVendorOwnerGump : Gump
    {
        private readonly PlayerVendor m_Vendor;

        public PlayerVendorOwnerGump(PlayerVendor vendor) : base(50, 200)
        {
            m_Vendor = vendor;

            var perDay = m_Vendor.ChargePerDay;

            AddPage(0);
            AddBackground(25, 10, 530, 140, 5054);

            AddHtmlLocalized(425, 25, 120, 20, 1019068); // See goods
            AddButton(390, 25, 4005, 4007, 1);
            AddHtmlLocalized(425, 48, 120, 20, 1019069); // Customize
            AddButton(390, 48, 4005, 4007, 2);
            AddHtmlLocalized(425, 72, 120, 20, 1011012); // CANCEL
            AddButton(390, 71, 4005, 4007, 0);

            AddHtmlLocalized(40, 72, 260, 20, 1038321); // Gold held for you:
            AddLabel(300, 72, 0, m_Vendor.HoldGold.ToString());
            AddHtmlLocalized(40, 96, 260, 20, 1038322); // Gold held in my account:
            AddLabel(300, 96, 0, m_Vendor.BankAccount.ToString());

            // AddHtmlLocalized( 40, 120, 260, 20, 1038324, false, false ); // My charge per day is:
            // Localization has changed, we must use a string here
            AddHtml(40, 120, 260, 20, "My charge per day is:");
            AddLabel(300, 120, 0, perDay.ToString());

            var days = (m_Vendor.HoldGold + m_Vendor.BankAccount) / (double)perDay;

            AddHtmlLocalized(40, 25, 260, 20, 1038318); // Amount of days I can work:
            AddLabel(300, 25, 0, ((int)days).ToString());
            AddHtmlLocalized(40, 48, 260, 20, 1038319); // Earth days:
            AddLabel(300, 48, 0, ((int)(days / 12.0)).ToString());
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            var from = state.Mobile;

            if (!m_Vendor.CanInteractWith(from, true))
            {
                return;
            }

            switch (info.ButtonID)
            {
                case 1:
                    {
                        m_Vendor.OpenBackpack(from);

                        break;
                    }
                case 2:
                    {
                        from.SendGump(new PlayerVendorCustomizeGump(m_Vendor, from));

                        break;
                    }
            }
        }
    }

    public class NewPlayerVendorOwnerGump : Gump
    {
        private readonly PlayerVendor m_Vendor;

        public NewPlayerVendorOwnerGump(PlayerVendor vendor) : base(50, 200)
        {
            m_Vendor = vendor;

            var perRealWorldDay = vendor.ChargePerRealWorldDay;
            var goldHeld = vendor.HoldGold;

            AddBackground(25, 10, 530, 180, 0x13BE);

            AddImageTiled(35, 20, 510, 160, 0xA40);
            AddAlphaRegion(35, 20, 510, 160);

            AddImage(10, 0, 0x28DC);
            AddImage(537, 175, 0x28DC);
            AddImage(10, 175, 0x28DC);
            AddImage(537, 0, 0x28DC);

            if (goldHeld < perRealWorldDay)
            {
                var goldNeeded = perRealWorldDay - goldHeld;

                AddHtmlLocalized(40, 35, 260, 20, 1038320, 0x7FFF); // Gold needed for 1 day of vendor salary:
                AddLabel(300, 35, 0x1F, goldNeeded.ToString());
            }
            else
            {
                var days = goldHeld / perRealWorldDay;

                AddHtmlLocalized(40, 35, 260, 20, 1038318, 0x7FFF); // # of days Vendor salary is paid for:
                AddLabel(300, 35, 0x480, days.ToString());
            }

            AddHtmlLocalized(40, 58, 260, 20, 1038324, 0x7FFF); // My charge per real world day is:
            AddLabel(300, 58, 0x480, perRealWorldDay.ToString());

            AddHtmlLocalized(40, 82, 260, 20, 1038322, 0x7FFF); // Gold held in my account:
            AddLabel(300, 82, 0x480, goldHeld.ToString());

            AddHtmlLocalized(40, 108, 260, 20, 1062509, 0x7FFF); // Shop Name:
            AddLabel(140, 106, 0x66D, vendor.ShopName);

            if (vendor is RentedVendor rentedVendor)
            {
                rentedVendor.ComputeRentalExpireDelay(out var days, out var hours);

                AddLabel(
                    38,
                    132,
                    0x480,
                    $"Location rental will expire in {days} day{(days != 1 ? "s" : "")} and {hours} hour{(hours != 1 ? "s" : "")}."
                );
            }

            AddButton(390, 24, 0x15E1, 0x15E5, 1);
            AddHtmlLocalized(408, 21, 120, 20, 1019068, 0x7FFF); // See goods

            AddButton(390, 44, 0x15E1, 0x15E5, 2);
            AddHtmlLocalized(408, 41, 120, 20, 1019069, 0x7FFF); // Customize

            AddButton(390, 64, 0x15E1, 0x15E5, 3);
            AddHtmlLocalized(408, 61, 120, 20, 1062434, 0x7FFF); // Rename Shop

            AddButton(390, 84, 0x15E1, 0x15E5, 4);
            AddHtmlLocalized(408, 81, 120, 20, 3006217, 0x7FFF); // Rename Vendor

            AddButton(390, 104, 0x15E1, 0x15E5, 5);
            AddHtmlLocalized(408, 101, 120, 20, 3006123, 0x7FFF); // Open Paperdoll

            AddButton(390, 124, 0x15E1, 0x15E5, 6);
            AddLabel(408, 121, 0x480, "Collect Gold");

            AddButton(390, 144, 0x15E1, 0x15E5, 7);
            AddLabel(408, 141, 0x480, "Dismiss Vendor");

            AddButton(390, 162, 0x15E1, 0x15E5, 0);
            AddHtmlLocalized(408, 161, 120, 20, 1011012, 0x7FFF); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var from = sender.Mobile;

            if (info.ButtonID == 1 || info.ButtonID == 2) // See goods or Customize
            {
                m_Vendor.CheckTeleport(from);
            }

            if (!m_Vendor.CanInteractWith(from, true))
            {
                return;
            }

            switch (info.ButtonID)
            {
                case 1: // See goods
                    {
                        m_Vendor.OpenBackpack(from);

                        break;
                    }
                case 2: // Customize
                    {
                        from.SendGump(new NewPlayerVendorCustomizeGump(m_Vendor));

                        break;
                    }
                case 3: // Rename Shop
                    {
                        m_Vendor.RenameShop(from);

                        break;
                    }
                case 4: // Rename Vendor
                    {
                        m_Vendor.Rename(from);

                        break;
                    }
                case 5: // Open Paperdoll
                    {
                        m_Vendor.DisplayPaperdollTo(from);

                        break;
                    }
                case 6: // Collect Gold
                    {
                        m_Vendor.CollectGold(from);

                        break;
                    }
                case 7: // Dismiss Vendor
                    {
                        m_Vendor.Dismiss(from);

                        break;
                    }
            }
        }
    }

    public class PlayerVendorCustomizeGump : Gump
    {
        private static readonly CustomCategory[] Categories =
        {
            new(
                Layer.InnerTorso,
                1011357,
                true,
                new[]
                {
                    // Upper Torso
                    new CustomItem(typeof(Shirt), 1011359, 5399),
                    new CustomItem(typeof(FancyShirt), 1011360, 7933),
                    new CustomItem(typeof(PlainDress), 1011363, 7937),
                    new CustomItem(typeof(FancyDress), 1011364, 7935),
                    new CustomItem(typeof(Robe), 1011365, 7939)
                }
            ),

            new(
                Layer.MiddleTorso,
                1011371,
                true,
                new[]
                {
                    // Over chest
                    new CustomItem(typeof(Doublet), 1011358, 8059),
                    new CustomItem(typeof(Tunic), 1011361, 8097),
                    new CustomItem(typeof(JesterSuit), 1011366, 8095),
                    new CustomItem(typeof(BodySash), 1011372, 5441),
                    new CustomItem(typeof(Surcoat), 1011362, 8189),
                    new CustomItem(typeof(HalfApron), 1011373, 5435),
                    new CustomItem(typeof(FullApron), 1011374, 5437)
                }
            ),

            new(
                Layer.Shoes,
                1011388,
                true,
                new[]
                {
                    // Footwear
                    new CustomItem(typeof(Sandals), 1011389, 5901),
                    new CustomItem(typeof(Shoes), 1011390, 5904),
                    new CustomItem(typeof(Boots), 1011391, 5899),
                    new CustomItem(typeof(ThighBoots), 1011392, 5906)
                }
            ),

            new(
                Layer.Helm,
                1011375,
                true,
                new[]
                {
                    // Hats
                    new CustomItem(typeof(SkullCap), 1011376, 5444),
                    new CustomItem(typeof(Bandana), 1011377, 5440),
                    new CustomItem(typeof(FloppyHat), 1011378, 5907),
                    new CustomItem(typeof(WideBrimHat), 1011379, 5908),
                    new CustomItem(typeof(Cap), 1011380, 5909),
                    new CustomItem(typeof(TallStrawHat), 1011382, 5910)
                }
            ),

            new(
                Layer.Helm,
                1015319,
                true,
                new[]
                {
                    // More Hats
                    new CustomItem(typeof(StrawHat), 1011382, 5911),
                    new CustomItem(typeof(WizardsHat), 1011383, 5912),
                    new CustomItem(typeof(Bonnet), 1011384, 5913),
                    new CustomItem(typeof(FeatheredHat), 1011385, 5914),
                    new CustomItem(typeof(TricorneHat), 1011386, 5915),
                    new CustomItem(typeof(JesterHat), 1011387, 5916)
                }
            ),

            new(
                Layer.Pants,
                1011367,
                true,
                new[]
                {
                    // Lower Torso
                    new CustomItem(typeof(LongPants), 1011368, 5433),
                    new CustomItem(typeof(Kilt), 1011369, 5431),
                    new CustomItem(typeof(Skirt), 1011370, 5398)
                }
            ),

            new(
                Layer.Cloak,
                1011393,
                true,
                new[]
                {
                    // Back
                    new CustomItem(typeof(Cloak), 1011394, 5397)
                }
            ),

            new(
                Layer.Hair,
                1011395,
                true,
                new[]
                {
                    // Hair
                    new CustomItem(0x203B, 1011052),
                    new CustomItem(0x203C, 1011053),
                    new CustomItem(0x203D, 1011054),
                    new CustomItem(0x2044, 1011055),
                    new CustomItem(0x2045, 1011047),
                    new CustomItem(0x204A, 1011050),
                    new CustomItem(0x2047, 1011396),
                    new CustomItem(0x2048, 1011048),
                    new CustomItem(0x2049, 1011049)
                }
            ),

            new(
                Layer.FacialHair,
                1015320,
                true,
                new[]
                {
                    // Facial Hair
                    new CustomItem(0x2041, 1011062),
                    new CustomItem(0x203F, 1011060),
                    new CustomItem(0x204B, 1015321, true),
                    new CustomItem(0x203E, 1011061),
                    new CustomItem(0x204C, 1015322, true),
                    new CustomItem(0x2040, 1015323),
                    new CustomItem(0x204D, 1011401)
                }
            ),

            new(
                Layer.FirstValid,
                1011397,
                false,
                new[]
                {
                    // Held items
                    new CustomItem(typeof(FishingPole), 1011406, 3520),
                    new CustomItem(typeof(Pickaxe), 1011407, 3717),
                    new CustomItem(typeof(Pitchfork), 1011408, 3720),
                    new CustomItem(typeof(Cleaver), 1015324, 3778),
                    new CustomItem(typeof(Mace), 1011409, 3933),
                    new CustomItem(typeof(Torch), 1011410, 3940),
                    new CustomItem(typeof(Hammer), 1011411, 4020),
                    new CustomItem(typeof(Longsword), 1011412, 3936),
                    new CustomItem(typeof(GnarledStaff), 1011413, 5113)
                }
            ),

            new(
                Layer.FirstValid,
                1015325,
                false,
                new[]
                {
                    // More held items
                    new CustomItem(typeof(Crossbow), 1011414, 3920),
                    new CustomItem(typeof(WarMace), 1011415, 5126),
                    new CustomItem(typeof(TwoHandedAxe), 1011416, 5186),
                    new CustomItem(typeof(Spear), 1011417, 3939),
                    new CustomItem(typeof(Katana), 1011418, 5118),
                    new CustomItem(typeof(Spellbook), 1011419, 3834)
                }
            )
        };

        private readonly Mobile m_Vendor;

        public PlayerVendorCustomizeGump(Mobile v, Mobile from) : base(30, 40)
        {
            m_Vendor = v;
            int x, y;

            from.CloseGump<PlayerVendorCustomizeGump>();

            AddPage(0);
            AddBackground(0, 0, 585, 393, 5054);
            AddBackground(195, 36, 387, 275, 3000);
            AddHtmlLocalized(10, 10, 565, 18, 1011356);  // <center>VENDOR CUSTOMIZATION MENU</center>
            AddHtmlLocalized(60, 355, 150, 18, 1011036); // OKAY
            AddButton(25, 355, 4005, 4007, 1);
            AddHtmlLocalized(320, 355, 150, 18, 1011012); // CANCEL
            AddButton(285, 355, 4005, 4007, 0);

            y = 35;
            for (var i = 0; i < Categories.Length; i++)
            {
                var cat = Categories[i];
                AddHtmlLocalized(5, y, 150, 25, cat.LocNumber, true);
                AddButton(155, y, 4005, 4007, 0, GumpButtonType.Page, i + 1);
                y += 25;
            }

            for (var i = 0; i < Categories.Length; i++)
            {
                var cat = Categories[i];
                AddPage(i + 1);

                for (var c = 0; c < cat.Entries.Length; c++)
                {
                    var entry = cat.Entries[c];
                    x = 198 + c % 3 * 129;
                    y = 38 + c / 3 * 67;

                    AddHtmlLocalized(x, y, 100, entry.LongText ? 36 : 18, entry.LocNumber);

                    if (entry.ArtNumber != 0)
                    {
                        AddItem(x + 20, y + 25, entry.ArtNumber);
                    }

                    AddRadio(x, y + (entry.LongText ? 40 : 20), 210, 211, false, (c << 8) + i);
                }

                if (cat.CanDye)
                {
                    AddHtmlLocalized(327, 239, 100, 18, 1011402); // Color
                    AddRadio(327, 259, 210, 211, false, 100 + i);
                }

                AddHtmlLocalized(456, 239, 100, 18, 1011403); // Remove
                AddRadio(456, 259, 210, 211, false, 200 + i);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Vendor.Deleted)
            {
                return;
            }

            var from = state.Mobile;

            if (m_Vendor is PlayerVendor vendor && !vendor.CanInteractWith(from, true))
            {
                return;
            }

            if (m_Vendor is PlayerBarkeeper barkeeper && !barkeeper.IsOwner(from))
            {
                return;
            }

            if (info.ButtonID == 0)
            {
                if (m_Vendor is PlayerVendor) // do nothing for barkeeps
                {
                    m_Vendor.Direction = m_Vendor.GetDirectionTo(from);
                    m_Vendor.Animate(32, 5, 1, true, false, 0);         // bow
                    m_Vendor.SayTo(from, 1043310 + Utility.Random(12)); // a little random speech
                }
            }
            else if (info.ButtonID == 1 && info.Switches.Length > 0)
            {
                var cnum = info.Switches[0];
                var cat = cnum % 256;
                var ent = cnum >> 8;

                if (cat < Categories.Length && cat >= 0)
                {
                    if (ent < Categories[cat].Entries.Length && ent >= 0)
                    {
                        var item = m_Vendor.FindItemOnLayer(Categories[cat].Layer);

                        item?.Delete();

                        var items = m_Vendor.Items;

                        for (var i = 0; item == null && i < items.Count; ++i)
                        {
                            var checkitem = items[i];
                            var type = checkitem.GetType();

                            for (var j = 0; item == null && j < Categories[cat].Entries.Length; ++j)
                            {
                                if (type == Categories[cat].Entries[j].Type)
                                {
                                    item = checkitem;
                                }
                            }
                        }

                        item?.Delete();

                        if (Categories[cat].Layer == Layer.FacialHair)
                        {
                            if (m_Vendor.Female)
                            {
                                from.SendLocalizedMessage(1010639); // You cannot place facial hair on a woman!
                            }
                            else
                            {
                                var hue = m_Vendor.FacialHairHue;

                                m_Vendor.FacialHairItemID = 0;
                                m_Vendor.ProcessDelta(); // invalidate item ID for clients

                                m_Vendor.FacialHairItemID = Categories[cat].Entries[ent].ItemID;
                                m_Vendor.FacialHairHue = hue;
                            }
                        }
                        else if (Categories[cat].Layer == Layer.Hair)
                        {
                            var hue = m_Vendor.HairHue;

                            m_Vendor.HairItemID = 0;
                            m_Vendor.ProcessDelta(); // invalidate item ID for clients

                            m_Vendor.HairItemID = Categories[cat].Entries[ent].ItemID;
                            m_Vendor.HairHue = hue;
                        }
                        else
                        {
                            item = Categories[cat].Entries[ent].Create();

                            if (item != null)
                            {
                                item.Layer = Categories[cat].Layer;

                                if (!m_Vendor.EquipItem(item))
                                {
                                    item.Delete();
                                }
                            }
                        }

                        from.SendGump(new PlayerVendorCustomizeGump(m_Vendor, from));
                    }
                }
                else
                {
                    cat -= 100;

                    if (cat < 100)
                    {
                        if (cat < Categories.Length && cat >= 0)
                        {
                            var category = Categories[cat];

                            if (category.Layer == Layer.Hair)
                            {
                                new PVHairHuePicker(false, m_Vendor, from).SendTo(state);
                            }
                            else if (category.Layer == Layer.FacialHair)
                            {
                                new PVHairHuePicker(true, m_Vendor, from).SendTo(state);
                            }
                            else
                            {
                                Item item = null;

                                var items = m_Vendor.Items;

                                for (var i = 0; item == null && i < items.Count; ++i)
                                {
                                    var checkitem = items[i];
                                    var type = checkitem.GetType();

                                    for (var j = 0; item == null && j < category.Entries.Length; ++j)
                                    {
                                        if (type == category.Entries[j].Type)
                                        {
                                            item = checkitem;
                                        }
                                    }
                                }

                                if (item != null)
                                {
                                    new PVHuePicker(item, m_Vendor, from).SendTo(state);
                                }
                            }
                        }
                    }
                    else
                    {
                        cat -= 100;

                        if (cat < Categories.Length)
                        {
                            var category = Categories[cat];

                            if (category.Layer == Layer.Hair)
                            {
                                m_Vendor.HairItemID = 0;
                            }
                            else if (category.Layer == Layer.FacialHair)
                            {
                                m_Vendor.FacialHairItemID = 0;
                            }
                            else
                            {
                                Item item = null;

                                var items = m_Vendor.Items;

                                for (var i = 0; item == null && i < items.Count; ++i)
                                {
                                    var checkitem = items[i];
                                    var type = checkitem.GetType();

                                    for (var j = 0; item == null && j < category.Entries.Length; ++j)
                                    {
                                        if (type == category.Entries[j].Type)
                                        {
                                            item = checkitem;
                                        }
                                    }
                                }

                                item?.Delete();
                            }

                            from.SendGump(new PlayerVendorCustomizeGump(m_Vendor, from));
                        }
                    }
                }
            }
        }

        private class CustomItem
        {
            public CustomItem(int itemID, int loc, bool longText = false) : this(null, itemID, loc, 0, longText)
            {
            }

            public CustomItem(Type type, int loc, int art = 0) : this(type, 0, loc, art)
            {
            }

            public CustomItem(Type type, int itemID = 0, int loc = 0, int art = 0, bool longText = false)
            {
                Type = type;
                ItemID = itemID;
                LocNumber = loc;
                ArtNumber = art;
                LongText = longText;
            }

            public Type Type { get; }

            public int ItemID { get; }

            public int LocNumber { get; }

            public int ArtNumber { get; }

            public bool LongText { get; }

            public Item Create()
            {
                if (Type == null)
                {
                    return null;
                }

                Item i = null;

                try
                {
                    var ctor = Type.GetConstructor(Array.Empty<Type>());
                    if (ctor != null)
                    {
                        i = ctor.Invoke(null) as Item;
                    }
                }
                catch
                {
                    // ignored
                }

                return i;
            }
        }

        private class CustomCategory
        {
            public CustomCategory(Layer layer, int loc, bool canDye, CustomItem[] items)
            {
                Entries = items;
                CanDye = canDye;
                Layer = layer;
                LocNumber = loc;
            }

            public bool CanDye { get; }

            public CustomItem[] Entries { get; }

            public Layer Layer { get; }

            public int LocNumber { get; }
        }

        private class PVHuePicker : HuePicker
        {
            private readonly Item m_Item;
            private readonly Mobile m_Mob;
            private readonly Mobile m_Vendor;

            public PVHuePicker(Item item, Mobile v, Mobile from) : base(item.ItemID)
            {
                m_Item = item;
                m_Vendor = v;
                m_Mob = from;
            }

            public override void OnResponse(int hue)
            {
                if (m_Item.Deleted)
                {
                    return;
                }

                if (m_Vendor is PlayerVendor vendor && !vendor.CanInteractWith(m_Mob, true))
                {
                    return;
                }

                if (m_Vendor is PlayerBarkeeper barkeeper && !barkeeper.IsOwner(m_Mob))
                {
                    return;
                }

                m_Item.Hue = hue;
                m_Mob.SendGump(new PlayerVendorCustomizeGump(m_Vendor, m_Mob));
            }
        }

        private class PVHairHuePicker : HuePicker
        {
            private readonly bool m_FacialHair;
            private readonly Mobile m_Mob;
            private readonly Mobile m_Vendor;

            public PVHairHuePicker(bool facialHair, Mobile v, Mobile from) : base(0xFAB)
            {
                m_FacialHair = facialHair;
                m_Vendor = v;
                m_Mob = from;
            }

            public override void OnResponse(int hue)
            {
                if (m_Vendor.Deleted)
                {
                    return;
                }

                if (m_Vendor is PlayerVendor vendor && !vendor.CanInteractWith(m_Mob, true))
                {
                    return;
                }

                if (m_Vendor is PlayerBarkeeper barkeeper && !barkeeper.IsOwner(m_Mob))
                {
                    return;
                }

                if (m_FacialHair)
                {
                    m_Vendor.FacialHairHue = hue;
                }
                else
                {
                    m_Vendor.HairHue = hue;
                }

                m_Mob.SendGump(new PlayerVendorCustomizeGump(m_Vendor, m_Mob));
            }
        }
    }

    public class NewPlayerVendorCustomizeGump : Gump
    {
        private static readonly HairOrBeard[] m_HairStyles =
        {
            new(0x203B, 1011052), // Short
            new(0x203C, 1011053), // Long
            new(0x203D, 1011054), // Ponytail
            new(0x2044, 1011055), // Mohawk
            new(0x2045, 1011047), // Pageboy
            new(0x204A, 1011050), // Topknot
            new(0x2047, 1011396), // Curly
            new(0x2048, 1011048), // Receding
            new(0x2049, 1011049)  // 2-tails
        };

        private static readonly HairOrBeard[] m_BeardStyles =
        {
            new(0x2041, 1011062), // Mustache
            new(0x203F, 1011060), // Short beard
            new(0x204B, 1015321), // Short Beard & Moustache
            new(0x203E, 1011061), // Long beard
            new(0x204C, 1015322), // Long Beard & Moustache
            new(0x2040, 1015323), // Goatee
            new(0x204D, 1011401)  // Vandyke
        };

        private readonly PlayerVendor m_Vendor;

        public NewPlayerVendorCustomizeGump(PlayerVendor vendor) : base(50, 50)
        {
            m_Vendor = vendor;

            AddBackground(0, 0, 370, 370, 0x13BE);

            AddImageTiled(10, 10, 350, 20, 0xA40);
            AddImageTiled(10, 40, 350, 20, 0xA40);
            AddImageTiled(10, 70, 350, 260, 0xA40);
            AddImageTiled(10, 340, 350, 20, 0xA40);

            AddAlphaRegion(10, 10, 350, 350);

            AddHtmlLocalized(10, 12, 350, 18, 1011356, 0x7FFF); // <center>VENDOR CUSTOMIZATION MENU</center>

            AddHtmlLocalized(10, 42, 150, 18, 1062459, 0x421F); // <CENTER>HAIR</CENTER>

            for (var i = 0; i < m_HairStyles.Length; i++)
            {
                var hair = m_HairStyles[i];

                AddButton(10, 70 + i * 20, 0xFA5, 0xFA7, 0x100 | i);
                AddHtmlLocalized(45, 72 + i * 20, 110, 18, hair.Name, 0x7FFF);
            }

            AddButton(10, 70 + m_HairStyles.Length * 20, 0xFB1, 0xFB3, 2);
            AddHtmlLocalized(45, 72 + m_HairStyles.Length * 20, 110, 18, 1011403, 0x7FFF); // Remove

            AddButton(10, 70 + (m_HairStyles.Length + 1) * 20, 0xFA5, 0xFA7, 3);
            AddHtmlLocalized(45, 72 + (m_HairStyles.Length + 1) * 20, 110, 18, 1011402, 0x7FFF); // Color

            if (vendor.Female)
            {
                AddButton(160, 290, 0xFA5, 0xFA7, 1);
                AddHtmlLocalized(195, 292, 160, 18, 1015327, 0x7FFF); // Male

                AddHtmlLocalized(195, 312, 160, 18, 1015328, 0x421F); // Female
            }
            else
            {
                AddHtmlLocalized(160, 42, 210, 18, 1062460, 0x421F); // <CENTER>BEARD</CENTER>

                for (var i = 0; i < m_BeardStyles.Length; i++)
                {
                    var beard = m_BeardStyles[i];

                    AddButton(160, 70 + i * 20, 0xFA5, 0xFA7, 0x200 | i);
                    AddHtmlLocalized(195, 72 + i * 20, 160, 18, beard.Name, 0x7FFF);
                }

                AddButton(160, 70 + m_BeardStyles.Length * 20, 0xFB1, 0xFB3, 4);
                AddHtmlLocalized(195, 72 + m_BeardStyles.Length * 20, 160, 18, 1011403, 0x7FFF); // Remove

                AddButton(160, 70 + (m_BeardStyles.Length + 1) * 20, 0xFA5, 0xFA7, 5);
                AddHtmlLocalized(195, 72 + (m_BeardStyles.Length + 1) * 20, 160, 18, 1011402, 0x7FFF); // Color

                AddHtmlLocalized(195, 292, 160, 18, 1015327, 0x421F); // Male

                AddButton(160, 310, 0xFA5, 0xFA7, 1);
                AddHtmlLocalized(195, 312, 160, 18, 1015328, 0x7FFF); // Female
            }

            AddButton(10, 340, 0xFA5, 0xFA7, 0);
            AddHtmlLocalized(45, 342, 305, 18, 1060675, 0x7FFF); // CLOSE
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var from = sender.Mobile;

            if (!m_Vendor.CanInteractWith(from, true))
            {
                return;
            }

            switch (info.ButtonID)
            {
                case 0: // CLOSE
                    {
                        m_Vendor.Direction = m_Vendor.GetDirectionTo(from);
                        m_Vendor.Animate(32, 5, 1, true, false, 0);         // bow
                        m_Vendor.SayTo(from, 1043310 + Utility.Random(12)); // a little random speech

                        break;
                    }
                case 1: // Female/Male
                    {
                        if (m_Vendor.Female)
                        {
                            m_Vendor.Body = 400;
                            m_Vendor.Female = false;
                        }
                        else
                        {
                            m_Vendor.Body = 401;
                            m_Vendor.Female = true;

                            m_Vendor.FacialHairItemID = 0;
                        }

                        from.SendGump(new NewPlayerVendorCustomizeGump(m_Vendor));

                        break;
                    }
                case 2: // Remove hair
                    {
                        m_Vendor.HairItemID = 0;

                        from.SendGump(new NewPlayerVendorCustomizeGump(m_Vendor));

                        break;
                    }
                case 3: // Color hair
                    {
                        if (m_Vendor.HairItemID > 0)
                        {
                            new PVHuePicker(m_Vendor, false, from).SendTo(from.NetState);
                        }
                        else
                        {
                            from.SendGump(new NewPlayerVendorCustomizeGump(m_Vendor));
                        }

                        break;
                    }
                case 4: // Remove beard
                    {
                        m_Vendor.FacialHairItemID = 0;

                        from.SendGump(new NewPlayerVendorCustomizeGump(m_Vendor));

                        break;
                    }
                case 5: // Color beard
                    {
                        if (m_Vendor.FacialHairItemID > 0)
                        {
                            new PVHuePicker(m_Vendor, true, from).SendTo(from.NetState);
                        }
                        else
                        {
                            from.SendGump(new NewPlayerVendorCustomizeGump(m_Vendor));
                        }

                        break;
                    }
                default:
                    {
                        int hairhue;

                        if ((info.ButtonID & 0x100) != 0) // Hair style selected
                        {
                            var index = info.ButtonID & 0xFF;

                            if (index >= m_HairStyles.Length)
                            {
                                return;
                            }

                            var hairStyle = m_HairStyles[index];

                            hairhue = m_Vendor.HairHue;

                            m_Vendor.HairItemID = 0;
                            m_Vendor.ProcessDelta();

                            m_Vendor.HairItemID = hairStyle.ItemID;

                            m_Vendor.HairHue = hairhue;

                            from.SendGump(new NewPlayerVendorCustomizeGump(m_Vendor));
                        }
                        else if ((info.ButtonID & 0x200) != 0) // Beard style selected
                        {
                            if (m_Vendor.Female)
                            {
                                return;
                            }

                            var index = info.ButtonID & 0xFF;

                            if (index >= m_BeardStyles.Length)
                            {
                                return;
                            }

                            var beardStyle = m_BeardStyles[index];

                            hairhue = m_Vendor.FacialHairHue;

                            m_Vendor.FacialHairItemID = 0;
                            m_Vendor.ProcessDelta();

                            m_Vendor.FacialHairItemID = beardStyle.ItemID;

                            m_Vendor.FacialHairHue = hairhue;

                            from.SendGump(new NewPlayerVendorCustomizeGump(m_Vendor));
                        }

                        break;
                    }
            }
        }

        private class HairOrBeard
        {
            public HairOrBeard(int itemID, int name)
            {
                ItemID = itemID;
                Name = name;
            }

            public int ItemID { get; }

            public int Name { get; }
        }

        private class PVHuePicker : HuePicker
        {
            private readonly bool m_FacialHair;
            private readonly Mobile m_From;
            private readonly PlayerVendor m_Vendor;

            public PVHuePicker(PlayerVendor vendor, bool facialHair, Mobile from) : base(0xFAB)
            {
                m_Vendor = vendor;
                m_FacialHair = facialHair;
                m_From = from;
            }

            public override void OnResponse(int hue)
            {
                if (!m_Vendor.CanInteractWith(m_From, true))
                {
                    return;
                }

                if (m_FacialHair)
                {
                    m_Vendor.FacialHairHue = hue;
                }
                else
                {
                    m_Vendor.HairHue = hue;
                }

                m_From.SendGump(new NewPlayerVendorCustomizeGump(m_Vendor));
            }
        }
    }
}
