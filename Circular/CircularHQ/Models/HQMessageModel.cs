using Circular.Core.Entity;
using CircularWeb.Business;

namespace CircularHQ.Models
{
    public class HQMessageModel : BaseModel
    {

        public HQMessageModel()
        {
            this.lstMessages = new List<MessageSummary>();
            this.userContactLists = new List<UserContactList>();
            this.lstbroadcastMessage = new List<Broadcast>();
            this.lstMessageSummary = new MessagesListResponse();
            this.broadcastSummaryDetails = new  List<Broadcast>();
            this.lstArchivedMessage = new List<MessageSummary>();
            this.lstArchivedMessageSummary = new MessagesListResponse();
            this.lstNewsFeeds = new List<NewsFeeds>();
            this.lstNewFeedsDetails = new NewsFeeds();
            this.lstPolllist = new List<Poll>();
            this.lstPollResult = new Poll();
            this.lstPollResults = new List<PollResults>();
            this.lstPollOptionsResult = new List<PollResults>();
            this.lstCustomerBusinessIndex = new List<CustomerBusinessIndex>();
            this.lstJobPostingList = new List<Jobs>();
            this.lstActiveNewsFeedDetails = new NewsFeeds();
            this.lstSponsorInformation = new SponsorInformation();
            this.Communities = new List<Communities>();
            this.Customers = new List<Customers>();
        }


        public IEnumerable<MessageSummary> lstMessages { get; set; }

        public IEnumerable<UserContactList> userContactLists { get; set; }

        public IEnumerable<Broadcast> lstbroadcastMessage { get; set; }

        public MessagesListResponse lstMessageSummary { get; set; }

        public IEnumerable<Broadcast> broadcastSummaryDetails { get; set; }

        public IEnumerable<MessageSummary> lstArchivedMessage { get; set; } 

        public MessagesListResponse lstArchivedMessageSummary { get; set; }

        public string CommunityName { get; set; }

        public IEnumerable<NewsFeeds> lstNewsFeeds { get; set;}

        public IEnumerable<NewsFeeds> ArchivedArticle { get; set; }

        public IEnumerable<Poll> lstPolllist { get; set; }  

        public Poll lstPollResult { get; set; }

        public IEnumerable<PollResults> lstPollResults { get; set; }

        public IEnumerable<PollResults> lstPollOptionsResult { get; set; }
        public IEnumerable<CustomerBusinessIndex> lstCustomerBusinessIndex { get; set;}
        public IEnumerable<Jobs> lstJobPostingList { get; set; }

        public NewsFeeds lstNewFeedsDetails { get; set; }

        public NewsFeeds lstActiveNewsFeedDetails { get; set; }

        public SponsorInformation lstSponsorInformation { get; set; }

        public IEnumerable<Communities> Communities { get; set; }
        public IEnumerable<Customers> Customers { get; set; }


    }



}
