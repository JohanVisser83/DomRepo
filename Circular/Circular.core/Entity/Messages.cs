using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblMessages")]
public class Messages : BaseEntity
{
    public Messages()
    {

    }
    public long FromId { get; set; }
    public long ToId { get; set; }
    public string? MessageExchangeCode { get; set; }
    public long MessageTypeId { get; set; }
    public bool IsNewMessage { get; set; }
    public string? Message { get; set; }
    public string? MessageMedia { get; set; }
    public string? MessageMediaThumbnail { get; set; }
    public long? ReferenceId { get; set; }
    public bool? IsPaid { get; set; }

    public string? SenderFirstName { get; set; }
    public string? SenderName { get; set; }
    public string? RecieverName { get; set; }
    public string? SenderLastName { get; set; }
    public string? SenderProfilePic { get; set; }

    public string? ReceverFirstName { get; set; }
    public string? ReceiverLastName { get; set; }
    public string? ReceiverProfilePic { get; set; }

    public DateTime? MessageDate { get; set; }

    public decimal Amount { get; set; }
    public long? TransactionStatusId { get; set; }
    public string? TransactionTitle { get; set; }
    public string? TransactionDesc { get; set; }

    private bool _ispollAnswered;
    public bool IsPollAnswered
    {
        get
        {
            _ispollAnswered = false;
            if (Poll != null && Poll.Options != null)
            {
                Poll.Options.ForEach(o =>
                {
                    if (o.Results != null)
                    {
                        o.Results.ForEach(r =>
                        {
                            if (r != null && r.UserId == ToId)
                            {
                                _ispollAnswered = true;
                                return;
                            }
                        });
                    }
                    else
                        _ispollAnswered = false;
                });
            }
            else
                _ispollAnswered = false;
            return _ispollAnswered;
        }
    }
    public long PollAnswersTotalCount { get; set; }

    public bool? IsBroadcast { get; set; }

    public long? CommunityId { get; set; }

    public string? MobileNumber { get; set; }
    public Poll Poll { get; set; }

    public int IsCommunityPortalMessage { get; set; } = 0;
    public string? CommunityName { get; set; } = "";

    public override void ApplyKeys()
    {

    }
}

public class MessagesListResponse
{
    public MessagesListResponse()
    {
        this.MessageGroups = new List<MessagesGroupResponse>();
    }

    public long? Customerid { get; set; }
    public long? SelectedCustomerId { get; set; }

    // Added Required field for Community protal
    public string? MessageExchangeCode { get; set; }
    public long? MessageTypeId { get; set; }
    public bool IsNewMessage { get; set; }
    public string? Message { get; set; }
    public string? MessageMedia { get; set; }
    public string? MessageMediaThumbnail { get; set; }
    public long? ReferenceId { get; set; }
    public bool? IsPaid { get; set; }

    public string? SenderFirstName { get; set; }
    public string? SenderLastName { get; set; }
    public string? SenderProfilePic { get; set; }

    public string? ReceverFirstName { get; set; }
    public string? ReceiverLastName { get; set; }
    public string? ReceiverProfilePic { get; set; }

    public DateTime? MessageDate { get; set; }

    public decimal Amount { get; set; }
    public long? TransactionStatusId { get; set; }
    public string? TransactionTitle { get; set; }
    public string? TransactionDesc { get; set; }

    public List<MessagesGroupResponse> MessageGroups { get; set; }

}
public class MessagesGroupResponse
{
    public MessagesGroupResponse()
    {
        this.MessageList = new List<Messages>();
    }
    public DateTime? MessageDate { get; set; }
    public List<Messages>? MessageList { get; set; }
}


public class ScheduledMessage
{
    public ScheduledMessage()
    {

    }
    public long Id { get; set; }
    public long InsertedId { get; set; }

    public long FromId { get; set; }
    public long ToId { get; set; }
    public long MessageTypeId { get; set; }
    public string? Message { get; set; }
    public string? MessageMedia { get; set; }
    public string? MessageMediaThumbnail { get; set; }
    public long? ReferenceId { get; set; }
  
    public long? CommunityId { get; set; }
    public string? CommunityName { get; set; } = "";





}

public class BroadcastMessage
{
    public BroadcastMessage()
    {

    }
    public long Id { get; set; }
    public long InsertedId { get; set; }

    public long FromId { get; set; }
    public long ToId { get; set; }
    public long MessageTypeId { get; set; }
    public string? Message { get; set; }
    public string? MessageMedia { get; set; }
    public string? MessageMediaThumbnail { get; set; }
    public long? ReferenceId { get; set; }

    public long? CommunityId { get; set; }
    public string? CommunityName { get; set; } = "";

	public long TotalReceiverCount { get; set; }

	public string? SenderFullName { get; set; } = "";

	public string? SenderEmail { get; set; } = "";

	public int Overalldeliveryrate { get; set; }
	public int undeliveredmessages { get; set; }



}
