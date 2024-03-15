

using Newtonsoft.Json;

namespace GoldSprite.MyUdpperAPI
{
    public abstract class ResponsePacket : Packet
    {
        public byte RepCode { get; set; } = IStatus.SEND_SUCCESS;
        [JsonIgnore]
        public bool IsSend => IStatus.IsSend(RepCode);
        [JsonIgnore]
        public bool IsSuccess => IStatus.IsSuccess(RepCode);
        public string Reason { get; set; } = "";
        [JsonIgnore]
        public string ReasonMsg => $"{GetType().Name}包{(IsSend ? "发送" : "响应")}{(IsSuccess ? "成功" : "失败")}{(Reason == "" ? "" : ", reson: " + Reason)}.";
    }
}