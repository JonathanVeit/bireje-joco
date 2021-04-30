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
                    return JsonConvert.DeserializeObject<StartMatchPhoMsg>(serializedMessage);
                case 2:
                    return JsonConvert.DeserializeObject<PausePausePhoMsg>(serializedMessage);
                case 3:
                    return JsonConvert.DeserializeObject<ContinueMatchPhoMsg>(serializedMessage);
                case 4:
                    return JsonConvert.DeserializeObject<EndMatchPhoMsg>(serializedMessage);
                case 5:
                    return JsonConvert.DeserializeObject<QuitMatchPhoMsg>(serializedMessage);
                case 6:
                    return JsonConvert.DeserializeObject<PrepareMatchStartPhoMsg>(serializedMessage);
                case 7:
                    return JsonConvert.DeserializeObject<DefineMatchRulesPhoMsg>(serializedMessage);

                default:
                    throw new System.NotImplementedException($"PhotonMessageFactory is missing implementation for deserializing messages of code { code }");
            }
        }

        public byte GetMessageCode(PhotonMessage msg)
        {
            switch (msg)
            {
                case StartMatchPhoMsg casted:
                    return 1;
                case PausePausePhoMsg casted:
                    return 2;
                case ContinueMatchPhoMsg casted:
                    return 3;
                case EndMatchPhoMsg casted:
                    return 4;
                case QuitMatchPhoMsg casted:
                    return 5;
                case PrepareMatchStartPhoMsg casted:
                    return 6;
                case DefineMatchRulesPhoMsg casted:
                    return 7;

                default:
                    throw new System.NotImplementedException($"PhotonMessageFactory is missing implementation for deserializing mesasges of type { msg.GetType().Name }");
            }
        }
    }
}