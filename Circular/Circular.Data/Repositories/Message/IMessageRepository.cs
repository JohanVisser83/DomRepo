using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Data.Repositories.Message
{
    public interface IMessageRepository
    {

        #region API and Common Functions

        Task<Messages> Save(Core.Entity.Messages messages);
        Task<bool> ReadMessage(MessageSummary messageSummary);

        Task<Poll> SavePoll(Core.Entity.PollResults pollResult);
        Task<Poll> GetPoll(long pollid);

          Task<IEnumerable<GetConversation>?> GetChats(long loggedinUserId);

        Task<MessagesListResponse?> GetConversation(long customerid, long selectedCustomerId, bool IsCommunityPortal);

        #endregion

        #region Community Portal Specific Functions
        Task<List<MessageSummary>> GetAdminChatList(long UserId);
        Task<int> AddArticleComment(ArticleComments item);

        Task<int> SaveScheduleMessage(MessageSchedule messageSchedule);
        Task<List<UserContactList>> GetUserContactListAsync(long CommunityId, string Search);
        
        Task<int> SaveBroadcastMessage(Broadcast broadcast);

        Task<IEnumerable<Groups>?> GetSelectedGroup(long CustomerId, long? CommunityId);
        Task<List<Broadcast>> GetAllBroadcastMessage(long communityId);

        Task<List<Broadcast>> GetBroadcastSummary(long id);
        Task<bool> ArchivedMessage(long CustomerId, long userId);
        Task<List<MessageSummary>> GetAllArchivedMessages(long CustomerId);
        Task<IEnumerable<Communities>> GetCommunityName();
        Task<int> SendNewFeeds(NewsFeeds article);
        Task<List<NewsFeeds>> GetAllNewFeeds(long customerId);

        Task<bool> ArchivedArticle(long Id, long CustomerId);

        Task<IEnumerable<ArticlePostComment>?> CommentList(long ArticleId, long CustomerId, long pageNumber, long pageSize);

        Task<bool> FeatureArticle(long Id, long CustomerId);

        Task<bool> UnFeatureArticle(long id, long customerId);
        Task<List<NewsFeeds>> GetAllArchivedArticle(long Customerid);

        Task<bool> UnArchivedMessage (long ArchivedUserid, long CustomerId);
        Task<bool> UnArchivedArticle(long ArchivedUserid, long CustomerId);
        Task<int> SendNewPoll(long userid, Poll pollItems);
        Task<List<Poll>> GetAllPoll(long communityId);
        
        Task<long> DeletePoll(long? pollid);
        Task<List<PollResults>> GetPollOptionsResult(long pollid);
        Task<List<NewsFeeds>> GetArchivedArticleDetails(long id);
        Task<List<NewsFeeds>> GetActiveArticleDetails(long id);
        
        Task<Communities> GetCurrentUserCommunityName(long id);

        Task<List<ScheduledMessage?>> GetScheduledConversations();
        Task<List<BroadcastMessage?>> sendBroadcast();
        Task<List<Communities>> GetCommunitiesAsync();




        #endregion
    }
}
