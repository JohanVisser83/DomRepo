using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCommunityPaymentGateways")]


    public class Gateways :BaseEntity
    {
       
        public long CommunityId { get; set; }
        public string GatewayName { get; set; }
        public string? APIKey { get; set; }
        public string? SecretKey { get; set; }
        public string? CallBackUrl { get; set; }
        public string? SuccessUrl { get; set; }
        public string? FailureUrl { get; set; }
        public string? PendingUrl { get; set; }
       

        public override void ApplyKeys()
        {
            throw new NotImplementedException();
        }
    
}
