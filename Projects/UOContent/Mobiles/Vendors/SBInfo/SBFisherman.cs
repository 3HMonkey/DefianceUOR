using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBFisherman : SBInfo
    {
        public override IShopSellInfo SellInfo { get; } = new InternalSellInfo();

        public override List<GenericBuyInfo> BuyInfo { get; } = new InternalBuyInfo();

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(RawFishSteak), 3, 20, 0x97A, 0));
                // TODO: Add( new GenericBuyInfo( typeof( SmallFish ), 3, 20, 0xDD6, 0 ) );
                // TODO: Add( new GenericBuyInfo( typeof( SmallFish ), 3, 20, 0xDD7, 0 ) );
                Add(new GenericBuyInfo(typeof(Fish), 6, 80, 0x9CC, 0));
                Add(new GenericBuyInfo(typeof(Fish), 6, 80, 0x9CD, 0));
                Add(new GenericBuyInfo(typeof(Fish), 6, 80, 0x9CE, 0));
                Add(new GenericBuyInfo(typeof(Fish), 6, 80, 0x9CF, 0));
                Add(new GenericBuyInfo(typeof(FishingPole), 15, 20, 0xDC0, 0));

                //Add(new GenericBuyInfo(typeof(AquariumFishNet), 250, 20, 0xDC8, 0x240));
                //Add(new GenericBuyInfo(typeof(AquariumFood), 62, 20, 0xEFC, 0));
                //Add(new GenericBuyInfo(typeof(FishBowl), 6312, 20, 0x241C, 0x482));
                //Add(new GenericBuyInfo(typeof(VacationWafer), 67, 20, 0x971, 0));
                //Add(new GenericBuyInfo(typeof(AquariumNorthDeed), 250002, 20, 0x14F0, 0));
                //Add(new GenericBuyInfo(typeof(AquariumEastDeed), 250002, 20, 0x14F0, 0));
                //Add(new GenericBuyInfo(typeof(NewAquariumBook), 15, 20, 0xFF2, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(RawFishSteak), 1);
                Add(typeof(Fish), 1);
                // TODO: Add( typeof( SmallFish ), 1 );
                Add(typeof(FishingPole), 7);
            }
        }
    }
}
