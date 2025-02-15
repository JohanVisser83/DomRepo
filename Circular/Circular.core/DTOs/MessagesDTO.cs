using Microsoft.AspNetCore.Http;
namespace Circular.Core.DTOs
{
    public class MessagesDTO : BaseEntityDTO
    {
        public MessagesDTO()
        {

        }
        public long FromId { get; set; }
        public long ToId { get; set; }
        public string MessageExchangeCode { get; set; }
        public long MessageTypeId { get; set; }
        public bool IsNewMessage { get; set; }

        public string? Message { get; set; }
        public string? MessageMedia { get; set; }
        public IFormFile? Mediafile { get; set; }
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
                        if(o.Results != null)
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
            set { _ispollAnswered = value; }
        }
        public long PollAnswersTotalCount { get; set; }
        public PollDTO  Poll { get; set; }
        public bool? IsBroadcast { get; set; }

       

    }

    public class ReadMessageRequest
    {
        public long SelectedUserId { get; set; }
        public long LastReadMessageId { get; set; }

        public long ReceiverId { get; set; }
    }
    public class GetMessagesRequest
    {
        public long SelectedCustomerId { get; set; }
        public long Customerid { get; set; }
    }
    public class SaveMessagesRequest : BaseEntityDTO
    {
        public long FromId { get; set; }
        public long ToId { get; set; }
        public long MessageTypeId { get; set; }
        public string? Message { get; set; }
        public string? MessageMedia { get; set; }
        public string? MessageMediaThumbnail { get; set; }
        public long? ReferenceId { get; set; }

    }
    public class GetConversation 
    {
        public long SenderId { get; set; }
        public string? MobileNumber { get; set; }
        public string SenderMessage { get; set; }
        public DateTime? MessageDateAndTime { get; set; }
        public string? CommunityName { get; set; }
        public bool IsUnread { get; set; }
        public string? SenderFirstName { get; set; }
        public string? SenderLastName { get; set; }
        public string? SenderProfilePic { get; set; }
        public bool IsBroadcast { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? LastMessageId { get; set; }
        public long? LastReadMessageId { get; set; }

    }

    public class MessagesListResponseDTO
    {
        public MessagesListResponseDTO()
        {
            this.MessageGroups = new List<MessagesGroupResponseDTO>();
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






        public List<MessagesGroupResponseDTO> MessageGroups { get; set; }

    }
    public class MessagesGroupResponseDTO
    {
        public DateTime? MessageDate { get; set; }
        public List<MessagesDTO>? MessageList { get; set; }
    }




}
