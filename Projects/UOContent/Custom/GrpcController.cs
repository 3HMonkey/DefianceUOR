using GrpcGateway;

namespace Server
{
    public class GrpcController
    {

        public static void Initialize()
        {


            EventSink.CharacterCreated += EventSink_CharacterCreated;
            EventSink.WorldSaveDone += EventSink_WorldSaveDone;



        }

        private static void EventSink_CharacterCreated(CharacterCreatedEventArgs obj)
        {
            SendService.SendCharacterCreated(obj.Name);
        }

        private static void EventSink_WorldSaveDone(double obj)
        {
            SendService.SendWorldSave(obj);
        }


    }
}
