using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.Message;
using Circular.Framework.Middleware.Emailer;
using Circular.Framework.Notifications;
using Circular.Services.Notifications;
using Circular.Services.Email;
using FirebaseAdmin.Messaging;
using static Azure.Core.HttpHeader;
using Org.BouncyCastle.Tls;
using MailKit.BounceMail;
using ZXing;

namespace Circular.Services.Message
{
   

    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _MessageRepository;
        private readonly INotificationService _notificationService;
        IMailService _mailService;


        public MessageService(IMessageRepository MessageRepository, INotificationService notificationService, IMailService mailService)
        {
            _MessageRepository = MessageRepository;
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        #region API and Common Functions

        public async Task<Messages> Save(Core.Entity.Messages messages)
        {


            messages.FillDefaultValues();
            Messages _messages = await _MessageRepository.Save(messages);



            if (_messages != null)
            {
                string senderName = _messages.SenderName;
             
                    if (messages.IsCommunityPortalMessage == 1)
                        senderName = messages.CommunityName;

                _notificationService.Notify(Framework.Notifications.NotificationTypes.Message_Received,
                    NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", messages.ToId.ToString()),
                senderName, "You received a new message from " + senderName, _messages.Id, 0, false, "",
                   messages.FromId, messages.CommunityId ?? 0, "", messages.ToId);
            }
            return _messages;
        }
        public async Task<bool> ReadMessage(MessageSummary messageSummary)
        {
            return await _MessageRepository.ReadMessage(messageSummary);
        }

        public async Task<Poll> SavePoll(Core.Entity.PollResults pollResult)
        {
            pollResult.FillDefaultValues();
            return await _MessageRepository.SavePoll(pollResult);
        }

        public async Task<Poll> GetPoll(long Pollid)
        {
            return await _MessageRepository.GetPoll(Pollid);
        }



        public async Task<IEnumerable<ArticlePostComment>?> CommentList(long ArticleId, long CustomerId, long pageNumber, long pageSize)
        {
            return await _MessageRepository.CommentList(ArticleId, CustomerId, pageNumber,pageSize);
        }

        public async Task<int> SaveScheduleMessage(MessageSchedule messageSchedule)
        {
            messageSchedule.FillDefaultValues();
            return await _MessageRepository.SaveScheduleMessage(messageSchedule);
        }

        public async Task<IEnumerable<GetConversation>?> GetChats(long loggedinUserId)
        {
            return await _MessageRepository.GetChats(loggedinUserId);
        }

        public async Task<MessagesListResponse?> GetConversation(long customerid, long selectedCustomerId)
        {
            return await _MessageRepository.GetConversation(customerid, selectedCustomerId,false);
        }

        #endregion

        #region Community Portal Specific Functions

        public async Task<List<MessageSummary>> GetAdminChatList(long UserId)
        {
            return await _MessageRepository.GetAdminChatList(UserId);

        }

        public async Task<List<UserContactList>> GetUserContactListAsync(long communityId, string Search)
        {
            return await _MessageRepository.GetUserContactListAsync(communityId, Search);
        }

        public async Task<IEnumerable<Groups>?> GetSelectedGroup(long CustomerId, long? CommunityId)
        {
            return await _MessageRepository.GetSelectedGroup(CustomerId, CommunityId);
        }

        public async Task<int> SaveBroadcastMessage(Broadcast broadcast)
        {
            broadcast.FillDefaultValues();
            int result = await _MessageRepository.SaveBroadcastMessage(broadcast);
            return result;
        }
        public async Task<int> AddArticleComment(ArticleComments item)
        {
            item.FillDefaultValues();
            return await _MessageRepository.AddArticleComment(item);
        }
        public async Task<int> SendNewFeeds(NewsFeeds article)
        {
           
                article.FillDefaultValues();
                int result = await _MessageRepository.SendNewFeeds(article);
                if (result > 0)
                {
                    string msg = "";
                    string topic = "";
                    long receiverId = 0;
                    decimal feeds = 0;



                    if ((article.IsGroup ?? 0) == 0)
                    {
                        topic = NotificationTopics.Circular_community_ReferenceId.ToString().Replace("ReferenceId", article.CommunityId.ToString());
                        msg = article.Title + " article has been posted in your community.";
                        receiverId = 0;
                    }

                    else
                    {

                        topic = NotificationTopics.Circular_communityGroups_ReferenceId.ToString().Replace("ReferenceId", article.ReferenceId.ToString());
                        msg = article.Title + " article has been posted in your community group.";
                        receiverId = article.ReferenceId ?? 0;

                    }
                    _notificationService.Notify(NotificationTypes.New_Article, topic, article.CommunityName, msg
                   , result, feeds, false, "", article.CreatedBy, article.CommunityId ?? 0, "", receiverId);
                }


                return result;



            

        }

        public async Task<List<Broadcast>> GetAllBroadcastMessage(long communityId)
        {
            return await _MessageRepository.GetAllBroadcastMessage(communityId);
        }

        public async Task<MessagesListResponse?> GetAdminConversation(long customerid, long? selectedCustomerId)
        {
            return await _MessageRepository.GetConversation(customerid, selectedCustomerId??0,true);
        }

        public async Task<List<ScheduledMessage?>> GetScheduledConversations()
        {
            List<ScheduledMessage?> result =  await _MessageRepository.GetScheduledConversations();
            foreach (ScheduledMessage _messages in result)
            {
                if (_messages != null)
                {
                    string senderName = _messages.CommunityName;
                    _notificationService.Notify(Circular.Framework.Notifications.NotificationTypes.Message_Received,
                    NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", _messages.ToId.ToString()),
                    senderName, "You received a new message from " + senderName, _messages.InsertedId, 0, false, "",
                    _messages.FromId, _messages.CommunityId ?? 0, "", _messages.ToId);
                }
            }
            return result;
        }
        public async Task<List<BroadcastMessage?>> sendBroadcast()
        {
            try
            {
                List<BroadcastMessage?> result = await _MessageRepository.sendBroadcast();
                string senderName = "";
                if (result != null)
                {
                    foreach (BroadcastMessage broadcast in result)
                    {
                        if (broadcast != null)
                        {
                            senderName = broadcast.CommunityName;

                            string msg = "";
                            string topic = "";
                            long receiverId = 0;
                            decimal broad = 0;

                            if (broadcast.ToId <= 0)
                            {
                                _notificationService.Notify(Circular.Framework.Notifications.NotificationTypes.New_Broadcast,
                                NotificationTopics.Circular_community_ReferenceId.ToString().Replace("ReferenceId", broadcast.CommunityId.ToString()),
                                senderName, "You received a new broadcast message from " + senderName, 0, 0, false, "",
                                broadcast.FromId, broadcast.CommunityId ?? 0, "", broadcast.CommunityId??0);
                            }
                            else
                            {
                                _notificationService.Notify(Circular.Framework.Notifications.NotificationTypes.New_Broadcast,
                                NotificationTopics.Circular_communityGroups_ReferenceId.ToString().Replace("ReferenceId", broadcast.ReferenceId.ToString()),
                                senderName, "You received a new broadcast message from " + senderName, 0, 0, false, "",
                                broadcast.FromId, broadcast.CommunityId ?? 0, "", broadcast.ReferenceId??0);
                            }
                        }

                        MailRequest mailRequest = new MailRequest();
                        mailRequest.FromUserId = broadcast.FromId;
                        mailRequest.ReferenceId = broadcast.ReferenceId;
                        mailRequest.To = broadcast.SenderEmail;
                        MailSettings mailSettings = _mailService.EmailParameter(MailType.Broadcast_Messages, ref mailRequest);
                        string body = mailRequest.Body;
                        string[] PlaceHolders = { "$senderfullname", "$deliveredcount", "$Overalldeliveryrate", "$Undeliveredmessages" };
                        string[] Values = {broadcast.SenderFullName,broadcast.TotalReceiverCount.ToString(),broadcast.Overalldeliveryrate.ToString(),broadcast.undeliveredmessages.ToString() };
                        if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                        {
                            for (int index = 0; index < PlaceHolders.Length; index++)
                                body = body.Replace(PlaceHolders[index], Values[index]);
                        }
                        mailRequest.Body = body;
                        await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);

                    }



				}
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<List<Broadcast>> GetBroadcastSummary(long id)
        {
            return await _MessageRepository.GetBroadcastSummary(id);
        }

        public async Task<bool> ArchivedMessage(long CustomerId, long UserId)
        {
            return await _MessageRepository.ArchivedMessage(CustomerId, UserId);
        }

        public async Task<List<MessageSummary>> GetAllArchivedMessages(long CustomerId)
        {
            return await _MessageRepository.GetAllArchivedMessages(CustomerId);
        }

        public async Task<IEnumerable<Communities>> GetCommunityName()
        {
              return await _MessageRepository.GetCommunityName();
        }

        public async Task<List<NewsFeeds>> GetAllNewFeeds(long communityId)
        {
            return await _MessageRepository.GetAllNewFeeds(communityId);
        }

        public async Task<bool> ArchivedArticle(long id, long CustomerId)
        {
            return await _MessageRepository.ArchivedArticle(id, CustomerId);
        }
        

        public async Task<bool> FeatureArticle(long id, long CustomerId)
        {
            return await _MessageRepository.FeatureArticle(id, CustomerId);
        }

        public async Task<bool> UnFeatureArticle(long id, long CustomerId)
        {
            return await _MessageRepository.UnFeatureArticle(id, CustomerId);
        }

        public async Task<List<NewsFeeds>> GetAllArchivedArticle(long CustomerId)
        {
            return await _MessageRepository.GetAllArchivedArticle(CustomerId);
        }

        public async Task<bool> UnArchivedMessage(long ArchivedUserid, long CustomerId)
        {
            return await _MessageRepository.UnArchivedMessage(ArchivedUserid, CustomerId);
        }


        public async Task<bool> UnArchivedArticle(long ArchivedUserid, long CustomerId)
        {
            return await _MessageRepository.UnArchivedArticle(ArchivedUserid, CustomerId);
        }


        public async Task<int> SendNewPoll(long Userid, Poll pollItems)
        {
            int result = await _MessageRepository.SendNewPoll(Userid, pollItems);

            if (result > 0)
            {
                string msg = "";
                string topic = "";
                long receiverId = 0;
                if ((pollItems.IsGroup ?? 0) == 0)
                {
                    topic = NotificationTopics.Circular_community_ReferenceId.ToString().Replace("ReferenceId", pollItems.CommunityId.ToString());
                    msg = pollItems.PollTitle + " Poll is just added in your community.";
                    receiverId = 0;
                }
                else
                {
                    topic = NotificationTopics.Circular_communityGroups_ReferenceId.ToString().Replace("ReferenceId", pollItems.ReferenceId.ToString());
                    msg = pollItems.PollTitle + " Poll just added in your community group.";
                    receiverId = pollItems.ReferenceId ?? 0;
                }
                _notificationService.Notify(NotificationTypes.Poll,topic,pollItems.CommunityName,msg
               , result,0, false, "", pollItems.CreatedBy, pollItems.CommunityId ?? 0, "", receiverId);
            }
            return result;
        }


        public async Task<List<Poll>> GetAllPoll(long communityId)
        {
            return await _MessageRepository.GetAllPoll(communityId);
        }

        

        public async Task<long> DeletePoll(long? pollid)
        {
            return await _MessageRepository.DeletePoll(pollid);
        }

        public async Task<List<PollResults>> GetPollOptionsResult(long pollid)
        {
            return await _MessageRepository.GetPollOptionsResult(pollid);
        }


        public async Task<List<NewsFeeds>> GetArchivedArticleDetails(long Id)
        {
            return await _MessageRepository.GetArchivedArticleDetails(Id);
        }

        

        public async Task<List<NewsFeeds>> GetActiveArticleDetails(long Id)
        {
            return await _MessageRepository.GetActiveArticleDetails(Id);
        }


       

        public async Task<Communities> GetCurrentUserCommunityName(long Id)
        {
            return await _MessageRepository.GetCurrentUserCommunityName(Id);
        }

        public async Task<List<Communities>> GetCommunitiesAsync()
        {
            return await _MessageRepository.GetCommunitiesAsync();
        }




        #endregion








    }
}
