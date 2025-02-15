using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblMessageSummary")]

public class MessageSummary : BaseEntity
{
    public long SenderId { get; set; }
    public long ReceiverId { get; set; }
    public long FirstMessageId { get; set; }
    public long InitialMessageId { get; set; }
    public long LastMessageId { get; set; }
    public string MessageExchangeCode { get; set; }
    public long? LastReadMessageId { get; set; }
    public bool? IsGroup { get; set; }
    public bool? IsArchived { get; set; }
    public long? IsUnread { get; set; }
    public string? SenderFirstName { get; set; }

    public string? SenderLastName { get; set; }
    public string? MobileNumber { get; set; }

    public int? IsBroadcast { get; set; }

    public string SenderMessage { get; set; }   

    public string SenderProfilePic { get; set; }    
    public override void ApplyKeys()
    {

    }


    
}


