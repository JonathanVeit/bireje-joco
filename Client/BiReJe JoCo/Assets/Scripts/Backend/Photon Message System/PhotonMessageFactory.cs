using Newtonsoft.Json;

namespace BiReJeJoCo.Backend
{
    public class PhotonMessageFactory 
    {
        public byte SerializeMessage<TMessage>(TMessage message, out string serializedMessage)
            where TMessage : PhotonMessage
        {
            serializedMessage = JsonConvert.SerializeObject(message);
            return GetMessageCode(message);
        }

        public PhotonMessage DeserializeMessage(string serializedMessage, byte code)
        {
            switch (code)
            {
                case 1:
                    return JsonConvert.DeserializeObject<StartGamePhoMsg>(serializedMessage);
                case 2:
                    return JsonConvert.DeserializeObject<PauseGamePhoMsg>(serializedMessage);
                case 3:
                    return JsonConvert.DeserializeObject<ContinueGamePhoMsg>(serializedMessage);

                default:
                    throw new System.NotImplementedException($"PhotonMessageFactory is missing implementation for deserializing messages of code { code }");
            }
        }

        public byte GetMessageCode(PhotonMessage msg)
        {
            switch (msg)
            {
                case StartGamePhoMsg casted:
                    return 1;
                case PauseGamePhoMsg casted:
                    return 2;
                case ContinueGamePhoMsg casted:
                    return 3;

                default:
                    throw new System.NotImplementedException($"PhotonMessageFactory is missing implementation for deserializing mesasges of type { msg.GetType().Name }");
            }
        }
    }
}