﻿using Newtonsoft.Json;

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
                    return JsonConvert.DeserializeObject<PauseMatchPhoMsg>(serializedMessage);
                case 3:
                    return JsonConvert.DeserializeObject<ContinueMatchPhoMsg>(serializedMessage);
                case 4:
                    return JsonConvert.DeserializeObject<FinishMatchPhoMsg>(serializedMessage);
                case 5:
                    return JsonConvert.DeserializeObject<CloseMatchPhoMsg>(serializedMessage);
                case 6:
                    return JsonConvert.DeserializeObject<PrepareMatchStartPhoMsg>(serializedMessage);
                case 7:
                    return JsonConvert.DeserializeObject<DefinedMatchRulesPhoMsg>(serializedMessage);
                case 8:
                    return JsonConvert.DeserializeObject<TriggerPointInteractedPhoMsg>(serializedMessage);
                case 9:
                    return JsonConvert.DeserializeObject<HuntedHitByBulletPhoMsg>(serializedMessage);
                case 10:
                    return JsonConvert.DeserializeObject<HuntedKilledPhoMsg>(serializedMessage);

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
                case PauseMatchPhoMsg casted:
                    return 2;
                case ContinueMatchPhoMsg casted:
                    return 3;
                case FinishMatchPhoMsg casted:
                    return 4;
                case CloseMatchPhoMsg casted:
                    return 5;
                case PrepareMatchStartPhoMsg casted:
                    return 6;
                case DefinedMatchRulesPhoMsg casted:
                    return 7;
                case TriggerPointInteractedPhoMsg casted:
                    return 8;
                case HuntedHitByBulletPhoMsg casted:
                    return 9;
                case HuntedKilledPhoMsg casted:
                    return 10;

                default:
                    throw new System.NotImplementedException($"PhotonMessageFactory is missing implementation for deserializing mesasges of type { msg.GetType().Name }");
            }
        }
    }
}