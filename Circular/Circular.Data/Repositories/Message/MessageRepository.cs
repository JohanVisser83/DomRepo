using Circular.Core.DTOs;
using Circular.Core.Entity;
using FirebaseAdmin.Messaging;
using Google.Api.Gax;
using Microsoft.Data.SqlClient;
using MimeKit.Cryptography;
using RepoDb;
using System.Data;
using System.Xml.Linq;

namespace Circular.Data.Repositories.Message
{
    public class MessageRepository : DbRepository<SqlConnection>, IMessageRepository
    {

        public MessageRepository(string connectionString) : base(connectionString)
        {

        }

        #region API and Common Functions
        public async Task<Messages> Save(Messages message)
        {
            try
            {
                var newMessage = await ExecuteQueryAsync<Messages>(
               "exec [dbo].[Usp_Messages_SaveMessage]" + " '" + message.FromId + "','" + message.ToId + "',N'"
               + message.Message + "','" + message.MessageTypeId + "','" +
               message.MessageMedia + "','" + message.MessageMediaThumbnail + "','"
                + message.ReferenceId + "','" + message.IsPaid + "';");

                if (newMessage != null)
                    return newMessage.FirstOrDefault<Messages>();
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<bool> ReadMessage(MessageSummary messageSummary)
        {
            MessageSummary _msummary = new MessageSummary();
            _msummary = QueryAsync<MessageSummary?>(e => e.SenderId == messageSummary.SenderId && e.ReceiverId == messageSummary.ReceiverId && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (_msummary == null)
                return false;
            _msummary.LastReadMessageId = messageSummary.LastReadMessageId; ;
            _msummary.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<MessageSummary>(e => new
            {
                e.LastReadMessageId,
                e.ModifiedBy,
                e.ModifiedDate
            });
            var updatedRows = Update<MessageSummary>(entity: _msummary, fields: fields);
            if (updatedRows > 0)
                return true;

            return false;

        }




        public async Task<Poll> SavePoll(PollResults pollResult)
        {
            var count = await InsertAsync<PollResults, int>(pollResult);
            return await GetPoll(pollResult.PollId);
        }

        public async Task<IEnumerable<GetConversation>?> GetChats(long loggedinUserId)
        {
            IEnumerable<GetConversation> messageSummary = await ExecuteQueryAsync<GetConversation>(
                "exec [dbo].[USP_Messages_GetMessageSummary]" + " " + loggedinUserId.ToString() + ";");

            return messageSummary;
        }

        public async Task<MessagesListResponse?> GetConversation(long customerid, long selectedCustomerId, bool IsCommunityPortal = false)
        {
            try
            {

                MessagesListResponse messagesListResponse = null;
                var Conversations = await ExecuteQueryMultipleAsync(
                                        "exec [dbo].[USP_Messages_GetMessages]" + " " + customerid.ToString() + "," + selectedCustomerId.ToString() + ";");

                List<Messages> lstConversation = Conversations.Extract<Messages>().ToList();
                List<Poll> lstPoll = Conversations.Extract<Poll>().ToList();
                List<PollOptions> lstPollOptions = Conversations.Extract<PollOptions>().ToList();
                List<PollResults> lstPollResults = Conversations.Extract<PollResults>().ToList();

                if (lstConversation != null)
                {
                    messagesListResponse = new MessagesListResponse() { Customerid = customerid, SelectedCustomerId = selectedCustomerId };
                    lstConversation.ForEach(m =>
                    {
                        // Fill Poll Object, if exists
                        if (m.MessageTypeId == 10105 && lstPoll != null)
                        {
                            Poll poll = lstPoll.Where<Poll>(p => p.Id == m.ReferenceId).FirstOrDefault();
                            if (poll != null && lstPollOptions != null)
                            {
                                List<PollOptions> options = lstPollOptions.Where<PollOptions>(po => po.PollId == m.ReferenceId).ToList<PollOptions>();
                                poll.Options = options;
                                if (poll.Options != null && lstPollResults != null)
                                {
                                    List<PollResults> pollresult = lstPollResults.Where<PollResults>(pr => pr.PollId == m.ReferenceId).ToList<PollResults>();
                                    if (pollresult != null)
                                    {
                                        m.PollAnswersTotalCount = pollresult.Count;
                                        poll.Options.ForEach(o =>
                                        {
                                            List<PollResults> pollResultoptions = pollresult.Where<PollResults>(pr => pr.PollId == m.ReferenceId && pr.SelectedOptionId == o.Id).ToList<PollResults>();
                                            if (pollResultoptions != null)
                                            {
                                                o.Results = pollResultoptions;
                                                if (pollResultoptions.Count > 0)
                                                {
                                                    o.AnswersCount = pollResultoptions.Count;
                                                    o.AnswersPercentage = Math.Round(((double)o.AnswersCount / (double)m.PollAnswersTotalCount) * 100, 2);
                                                }
                                                else
                                                {
                                                    o.AnswersCount = 0;
                                                    o.AnswersPercentage = 0;
                                                }
                                            }
                                        });
                                    }
                                }
                                m.Poll = poll;
                            }
                        }

                        // group the data in dates
                        int index = -1;
                        if (IsCommunityPortal)
                            index = messagesListResponse.MessageGroups.FindIndex(mg => mg.MessageDate == (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)));
                        else
                            index = messagesListResponse.MessageGroups.FindIndex(mg => mg.MessageDate == m.MessageDate);
                        if (index == -1)
                        {
                            MessagesGroupResponse messagesGroupResponse = new MessagesGroupResponse();
                            if (IsCommunityPortal)
                                messagesGroupResponse.MessageDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                            else
                                messagesGroupResponse.MessageDate = m.MessageDate;

                            messagesGroupResponse.MessageList.Add(m);
                            messagesListResponse.MessageGroups.Add(messagesGroupResponse);
                        }
                        else
                            messagesListResponse.MessageGroups[index].MessageList.Add(m);
                    }
                    );
                }

                if (IsCommunityPortal && messagesListResponse != null)
                {
                    long id = messagesListResponse.MessageGroups[0].MessageList.LastOrDefault().Id;
                    Core.Entity.MessageSummary messageSummary = new Core.Entity.MessageSummary();
                    messageSummary.SenderId = selectedCustomerId;
                    messageSummary.ReceiverId = customerid;
                    messageSummary.LastReadMessageId = id;
                    ReadMessage(messageSummary);
                }

                return messagesListResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Poll> GetPoll(long pollId)
        {
            Poll poll = null;
            long PollAnswersTotalCount = 1;

            var Conversations = await ExecuteQueryMultipleAsync(
                        "exec [dbo].[USP_Messages_GetPoll]" + " " + pollId.ToString() + ";");

            List<Poll> lstPoll = Conversations.Extract<Poll>().ToList();
            List<PollOptions> lstPollOptions = Conversations.Extract<PollOptions>().ToList();
            List<PollResults> lstPollResults = Conversations.Extract<PollResults>().ToList();
            if (lstPoll != null)
            {
                poll = lstPoll.Where<Poll>(p => p.Id == pollId).FirstOrDefault();
                if (poll != null && lstPollOptions != null)
                {
                    List<PollOptions> options = lstPollOptions.Where<PollOptions>(po => po.PollId == pollId).ToList<PollOptions>();
                    poll.Options = options;
                    if (poll.Options != null && lstPollResults != null)
                    {
                        poll.PollResponseCount = lstPollResults.Count;
                        poll.PollOutstandingCount = poll.PollMemberCount - poll.PollResponseCount;
                        poll.PollOutstandingCount = poll.PollOutstandingCount < 0 ? 0 : poll.PollOutstandingCount;
                        List<PollResults> pollresult = lstPollResults.Where<PollResults>(pr => pr.PollId == pollId).ToList<PollResults>();
                        if (pollresult != null)
                        {
                            PollAnswersTotalCount = pollresult.Count;
                            poll.Options.ForEach(o =>
                            {
                                List<PollResults> pollResultoptions = pollresult.Where<PollResults>(pr => pr.PollId == pollId && pr.SelectedOptionId == o.Id).ToList<PollResults>();
                                if (pollResultoptions != null)
                                {
                                    o.Results = pollResultoptions;
                                    o.AnswersCount = pollResultoptions.Count;
                                    o.AnswersPercentage = Math.Round(((double)o.AnswersCount / (double)PollAnswersTotalCount) * 100, 2);
                                }
                            });
                        }
                    }
                }
            }
            return poll;
        }


        public async Task<int> AddArticleComment(ArticleComments item)
        {
            var key = await InsertAsync<ArticleComments, int>(item);
            return key;

        }




        public async Task<IEnumerable<ArticlePostComment>> CommentList(long ArticleId, long CustomerId, long pageNumber, long pageSize)
        {
            var result = await ExecuteQueryAsync<ArticlePostComment>("Exec [dbo].[USP_Article_Comments]" + ArticleId + "," + CustomerId + "," + pageNumber + "," + pageSize);
            foreach (var item in result)
            {
                var replycomment = ExecuteQueryAsync<UserReply>("Exec [dbo].[USP_GetReplyArticle]" + item.Id).Result.ToList();
                item.userReplies = replycomment;
            }
            return result;

        }
        #endregion



        #region Community Portal Specific Functions


        public async Task<List<MessageSummary>> GetAdminChatList(long UserId)
        {

            var query = "exec [dbo].[USP_Messages_GetMessageSummary]" + " " + UserId + ";";
            var result = await ExecuteQueryAsync<MessageSummary>(query);
            return result.ToList<MessageSummary>();


        }
        public async Task<List<ScheduledMessage>> GetScheduledConversations()
        {

            var query = "exec [dbo].[usp_SendScheduledMessages]";
            var result = await ExecuteQueryAsync<ScheduledMessage>(query);
            return result.ToList<ScheduledMessage>();

        }

        public async Task<List<BroadcastMessage>> sendBroadcast()
        {

            var query = "exec [dbo].[usp_SendBroadcastMessages]";
            var result = await ExecuteQueryAsync<BroadcastMessage>(query);
            return result.ToList<BroadcastMessage>();

        }

        public async Task<int> SaveScheduleMessage(MessageSchedule messageSchedule)
        {
            try
            {
                string strcommand = "exec [dbo].[Usp_Messages_ScheduleMessage]" + " '" + messageSchedule.FromId + "','" + messageSchedule.ToId + "','"
                + messageSchedule.Message + "','" + messageSchedule.MessageTypeId + "','" +
               messageSchedule.MessageMedia + "','" + messageSchedule.MessageMediaThumbnail + "','"
                + messageSchedule.ReferenceId + "','" + messageSchedule.CommunityId + "','" + messageSchedule.Schedule.ToString("yyyy-MM-dd HH:mm:ss") + "';";
                var newMessage = await ExecuteQueryAsync(strcommand);

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public async Task<List<UserContactList>> GetUserContactListAsync(long CommunityId, string Search)
        {
            string query = "exec [USP_Messages_SearchContact]" + "" + CommunityId + " ,'" + Search + "';";
            var result = await ExecuteQueryAsync<UserContactList>(query);
            return result.ToList<UserContactList>();
        }


        public async Task<IEnumerable<Groups>?> GetSelectedGroup(long CustomerId, long? CommunityId)
        {
            var customers = QueryAll<CustomerGroups>().Where(x => x.CustomerId == CustomerId && x?.IsActive == true).FirstOrDefault();
            long userid = customers.CustomerId ?? 0;

            var group = QueryAll<Groups>().Where(x => x.CommunityID == CommunityId && userid == CustomerId).ToList();
            return group;
        }


        public async Task<List<Broadcast>> GetAllBroadcastMessage(long communityId)
        {
            var broadcastMessage = QueryAll<Broadcast>().Where(b => b.IsActive == true && b.CommunityId == communityId).ToList();
            return broadcastMessage;
        }



        public async Task<int> SaveBroadcastMessage(Broadcast broadcast)
        {
            try
            {
                var key = await InsertAsync<Broadcast, int>(broadcast);

                return key;
            }
            catch (Exception ex)
            {
                return 0;
            }



        }

        public async Task<List<Broadcast>> GetBroadcastSummary(long Id)
        {

            string query = "exec [USP_Messages_GetBroadcastSummary]" + "" + Id + ";";
            var broadcastMessageSummary = await ExecuteQueryAsync<Broadcast>(query);
            return (List<Broadcast>)broadcastMessageSummary;


        }

        public async Task<bool> ArchivedMessage(long CustomerId, long UserId)
        {

            var query = "exec [USP_Messages_ArchivedMessage]" + "" + CustomerId + " ,'" + UserId + "';";
            var result = await ExecuteQueryAsync<dynamic>(query);
            if (result != null)
            {
                return true;
            }
            return false;


        }

        public async Task<List<MessageSummary>> GetAllArchivedMessages(long CustomerId)
        {

            string query = "exec [USP_Messages_GetAllArchivedMessage]" + "" + CustomerId + ";";
            var result = await ExecuteQueryAsync<MessageSummary>(query);
            return (List<MessageSummary>)result;


        }

        public async Task<IEnumerable<Communities>> GetCommunityName()
        {
            var communityName = QueryAll<Communities>().Where(e => e.IsActive == true).ToList();
            return communityName;
        }



        public async Task<int> SendNewFeeds(NewsFeeds article)
        {
            try
            {
                var key = await InsertAsync<NewsFeeds, int>(article);
                article.ApplyKeys();
                await InsertAllAsync<ArticleMedia>(article.ArticleMedia);
                return key;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public async Task<List<NewsFeeds>> GetAllNewFeeds(long communityId)
        {

            string query = "exec [USP_Messages_GetNewsFeed]" + "" + communityId + ";";
            var newsfeedslist = await ExecuteQueryAsync<NewsFeeds>(query);

            return (List<NewsFeeds>)newsfeedslist;


        }


        public async Task<bool> ArchivedArticle(long Id, long CustomerId)
        {

            var query = "exec [USP_Messages_ArchiveNewsArticle]" + "" + Id + " ,'" + CustomerId + "';";
            var result = await ExecuteQueryAsync<dynamic>(query);
            if (result != null)
            {
                return true;
            }
            return false;

        }

        public async Task<bool> FeatureArticle(long Id, long CustomerId)
        {
            try
            {
                var query = "exec [USP_Messages_FeatureArticle]" + "" + Id + " ,'" + CustomerId + "';";
                var result = await ExecuteQueryAsync<dynamic>(query);
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        public async Task<bool> UnFeatureArticle(long Id, long CustomerId)
        {
            try
            {
                var query = "exec [USP_Messages_UnFeatureArticle]" + "" + Id + " ,'" + CustomerId + "';";
                var result = await ExecuteQueryAsync<dynamic>(query);
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }


        }


        public async Task<List<NewsFeeds>> GetAllArchivedArticle(long CustomerId)
        {

            string query = "exec [USP_Messages_GetArchivedFeeds ]" + "" + CustomerId + ";";
            var result = await ExecuteQueryAsync<NewsFeeds>(query);
            return (List<NewsFeeds>)result;

        }


        public async Task<bool> UnArchivedMessage(long ArchivedUserid, long CustomerId)
        {

            var query = "exec [USP_Messages_UnArchivedMessage]" + "" + ArchivedUserid + " ,'" + CustomerId + "';";
            var result = await ExecuteQueryAsync<dynamic>(query);
            if (result != null)
            {
                return true;
            }
            return false;

        }


        public async Task<bool> UnArchivedArticle(long ArchivedUserid, long CustomerId)
        {

            var query = "exec [USP_Messages_UnArchiveNewsArticle]" + "" + ArchivedUserid + " ,'" + CustomerId + "';";
            var result = await ExecuteQueryAsync<dynamic>(query);
            if (result != null)
            {
                return true;
            }
            return false;

        }

        public async Task<int> SendNewPoll(long UserId, Poll PollItems)
        {

            //split data for options
            try
            {
                List<string> polllist = new List<string>();
                var splitdata = PollItems.Options.FirstOrDefault()?.OptionText.Split(',');
                foreach (var item in splitdata)
                {
                    PollOptions options = new PollOptions();
                    options.OptionText = item;
                    polllist.Add(options.OptionText);
                }
                var xEle = new XElement("Message",
                        from Poll in polllist
                        select new XElement("SendNewPoll",
                               new XElement("OptionText", Poll)));

                var query = "exec [dbo].[USP_Messages_SavePoll]" + " '" + UserId + "','" + PollItems.ReferenceId + "','" + PollItems.PollTitle + "','"
                + PollItems.Question + "','" + xEle.ToString() + "','" + PollItems.IsGroup + "','" + PollItems.CommunityId + "';";


                var result = await ExecuteQueryAsync(query);
                if (result != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }


        public async Task<List<Poll>> GetAllPoll(long communityId)
        {
            string query = "exec [USP_Message_GetAllPollList  ] " + communityId;
            var result = await ExecuteQueryAsync<Poll>(query);
            return (List<Poll>)result;
        }





        public async Task<long> DeletePoll(long? pollid)
        {
            Poll Polls = new Poll();
            Polls.Id = (long)pollid;
            Polls.IsActive = false;
            var fields = Field.Parse<Poll>(x => new
            {
                x.IsActive

            });
            var updaterow = Update<Poll>(entity: Polls, fields: fields);

            var deletePollMessage = ExecuteQueryAsync("Exec [dbo].[USP_Message_DeletePoll] " + pollid);
            return updaterow;
        }


        public async Task<List<PollResults>> GetPollOptionsResult(long Pollid)
        {


            // 

            //string query = "exec [USP_Messages_GetPoll] " + Pollid;
            //var result = await ExecuteQueryAsync<dynamic>(query);
            //return (List<dynamic>)result;

            var optionslistResult = QueryAll<PollResults>().Where(x => x.PollId == Pollid && x.IsActive == true).ToList();
            return (List<PollResults>)optionslistResult;
        }

        public async Task<List<NewsFeeds>> GetArchivedArticleDetails(long Id)
        {
            var articleDetails = QueryAll<NewsFeeds>().Where(a => a.Id == Id && a.IsArchived == true).ToList();
            foreach (var feed in articleDetails)
            {
                feed.ArticleMedia = QueryAsync<ArticleMedia?>(c => c.NewFeedsId == feed.Id && c.IsActive == true).Result.ToList() ?? null;
            }
            return (List<NewsFeeds>)articleDetails;

        }

        public async Task<List<NewsFeeds>> GetActiveArticleDetails(long Id)
        {
            string query = "exec [USP_Messages_ActiveArticleDetails]" + "" + Id + ";";
            var result = await ExecuteQueryAsync<NewsFeeds>(query);
            foreach (var feed in result)
            {
                feed.ArticleMedia = QueryAsync<ArticleMedia?>(c => c.NewFeedsId == feed.Id && c.IsActive == true).Result.ToList() ?? null;
            }
            return (List<NewsFeeds>)result;
            //var activeArticleDetails = QueryAll<NewsFeeds>().Where(a => a.Id == Id && a.IsArchived == false).ToList();
            //return (List<NewsFeeds>)activeArticleDetails;
        }



        public async Task<Communities> GetCurrentUserCommunityName(long id)
        {
            var key = QueryAll<Communities>().Where(c => c.Id == id && c.IsActive == true).FirstOrDefault();
            return key;
        }


        public async Task<List<Communities>> GetCommunitiesAsync()
        {
            return QueryAll<Communities>().Where(e => e.IsActive == true).OrderByDescending(e => e.Id).ToList();
        }

        #endregion








    }
}
