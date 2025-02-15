using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Services.Message
{
    public interface IMessageService
    {
        #region API and Common Functions
        public Task<Messages> Save(Core.Entity.Messages message);
        public Task<bool> ReadMessage(MessageSummary messageSummary);

        public Task<Poll> SavePoll(PollResults pollResult);

        public  Task<IEnumerable<GetConversation>?> GetChats(long loggedinUserId);

        public Task<MessagesListResponse?> GetConversation(long customerid, long selectedCustomerId);

        Task<Poll> GetPoll(long Pollid);
        Task<int> AddArticleComment(ArticleComments item);

     
        Task<IEnumerable<ArticlePostComment>?> CommentList(long ArticleId, long CustomerId, long pageNumber, long pageSize);

        #endregion


        #region Community Portal Specific Functions

        public Task<List<MessageSummary>> GetAdminChatList(long UserId);
        public Task<int> SaveScheduleMessage(MessageSchedule schedulemessages);

        public Task<List<UserContactList>> GetUserContactListAsync(long communityId,string Search);

        public Task<IEnumerable<Groups>?> GetSelectedGroup(long CustomerId, long? CommunityId);

        public Task<int> SaveBroadcastMessage(Broadcast broadcast);
        Task<List<Broadcast>> GetAllBroadcastMessage(long communityId);
        public Task<MessagesListResponse?> GetAdminConversation(long customerid, long? selectedCustomerId);
        Task<List<Broadcast>> GetBroadcastSummary(long id);
        Task<bool> ArchivedMessage(long CustomerId, long userId);
        Task<List<MessageSummary>> GetAllArchivedMessages(long CustomerId);
        Task<IEnumerable<Communities>> GetCommunityName();
        public Task<int> SendNewFeeds(NewsFeeds article);
        Task<List<NewsFeeds>> GetAllNewFeeds(long communityId);
        Task<bool> ArchivedArticle(long id, long CustomerId);
        
        Task<bool> FeatureArticle(long id, long CustomerId);

        Task<bool> UnFeatureArticle(long id, long CustomerId); 
        Task<bool> UnArchivedMessage(long ArchivedUserid, long userId);
        public Task<List<NewsFeeds>> GetAllArchivedArticle(long CustomerId);
        
       Task<bool> UnArchivedArticle(long ArchivedUserid, long userId);

        Task<int> SendNewPoll(long UserId, Poll pollItems);

        Task<List<Poll>> GetAllPoll(long communityId);
        
        Task<long> DeletePoll(long? id);

        Task<List<PollResults>> GetPollOptionsResult(long Pollid);
        Task<List<NewsFeeds>> GetArchivedArticleDetails(long id);
        Task<List<NewsFeeds>> GetActiveArticleDetails(long id);
        
        Task<Communities> GetCurrentUserCommunityName(long communityId);
        //Task<long> UpdateReadMessage(long customerId, long id);

        Task<List<ScheduledMessage?>> GetScheduledConversations();
        Task<List<BroadcastMessage?>> sendBroadcast();
        Task<List<Communities>> GetCommunitiesAsync();
       



        #endregion



    }
}
