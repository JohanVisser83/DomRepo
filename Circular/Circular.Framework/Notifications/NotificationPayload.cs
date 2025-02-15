
namespace Circular.Framework.Notifications
{
    public class NotificationPayload
    {
        public string? NotificationPath { get; set; }
        public long? NotificationTypeId { get; set; } = 0;
        public string? NotificationTitle { get; set; } = "";
        public string? NotificationMedia { get; set; } = "";
        public string? NotificationTopic { get; set; }
        public string? NotificationBody { get; set; } = "";
        public long? NotificationReferenceId { get; set; } = 0;
        public long NotificationSenderId { get; set; }
        public long NotificationGroupId { get; set; }
        public decimal? NotificationAmount { get; set; }
        public bool NotificationIsBroadcast { get; set; } = false;
        public Object? AdditionalData { get; set; } = new Object();
        public long NotificationReceiverId { get; set; }
        public long NotificationId { get; set; } = 0;

    }
    public enum NotificationTypes
    {
        Request_Payment = 101,
        Money_Sent = 102,
        Money_Received = 103,
        Topup = 104,
        Withdrawal_Request = 105,
        Message_Sent = 106,
        Message_Received = 107,
        Friend_Request_Sent = 109,
        Friend_Request_Received = 110,
        Friend_Request_Accepted = 111,
        Friend_Request_Rejected = 112,
        Linking_Request_Sent = 113,
        Linking_Request_Received = 114,
        Linking_Request_Accepted = 115,
        Linking_Request_Rejected = 116,
        Collection_Raised = 117,
        Collection_Received = 118,
        Event_Payment_Requested = 119,
        Event_Payment_Received = 120,
        Event_Payment_Sent = 121,
        Collection_Payment_Sent = 122,
        Silent_Alert = 123,
        CommunityBroadcast = 124,
        CheckOut = 126,
        Request_Payment_Checkout = 128,
        New_Event = 129,
        Flexipass = 131,
        Linking_Request_Reported = 132,
        New_Broadcast = 133,
        New_Account = 134,
        Payment_Decline = 135,
        Business_Approved = 136,
        Business_Declined = 137,
        Job_Approved = 138,
        Job_Declined = 139,
        New_Article = 140,
        Poll = 141,
        Request_Approved=142,
        Request_Declined = 143,
        ExpiredSubscription=144,
        ActualExpiredSubscriptions=145
    }
	public enum NotificationTopics
    {
        Circular_user_ReferenceId = 101,
        Circular_all = 102,
        Circular_community_ReferenceId = 103,
        Circular_communityGroups_ReferenceId = 104,
        Circular_linkedUser_ReferenceId = 105,
        Circular_CancelSubscription = 106
    }
}
