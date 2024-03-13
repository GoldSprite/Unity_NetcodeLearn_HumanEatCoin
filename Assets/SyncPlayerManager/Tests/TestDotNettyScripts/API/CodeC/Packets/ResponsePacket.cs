namespace GoldSprite.TestDotNetty_API
{
    public abstract class ResponsePacket : Packet
    {
        public byte RepCode { get; set; } = IStatus.SEND_SUCCESS;
        public bool IsSend => IStatus.IsSend(RepCode);
        public bool IsSuccess => IStatus.IsSuccess(RepCode);
        public string Reson { get; set; } = "";
        public string ResonMsg => $"{GetType().Name}包{(IsSend ? "发送" : "响应")}{(IsSuccess ? "成功" : "失败")}{(Reson == "" ? "" : ", reson: " + Reson)}.";
    }
}