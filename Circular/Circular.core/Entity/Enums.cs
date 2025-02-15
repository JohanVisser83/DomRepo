using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.Entity
{
    public enum SignUpStatusCode
    {
        No_Community = 100,
        No_UserType = 101,
        No_BasicDetail = 102,
        No_Password = 103,
        No_Passcode = 104,
        Sign_Up_Completed = 105
    }

    public enum TransactionTypeEnum
    {
        Transfer = 101,
        Withdrawal = 102,
        ETopup = 103,
        Support = 104,
        StoreFront = 105,
        AccountDeletion = 106,
        Subscription = 107,
        Requested = 108,
        ManualTopup = 109,
        Order = 110,
        Event = 111,
        Collection = 112,
        Transport = 113,
        CashOrderSale = 114,
        Refund = 115,
        Fundhub = 116,
        Declined = 117,
        MemberSubscription = 118

    }

    public enum TransactionStatusEnum
    {

        Success = 101,
        Failed = 102,
        Pending = 103

    }

    public enum WithdrawalStatusEnum
    {
        Paid = 101,
        Pending = 102
    }
    public enum MessageTypes
    {
        Payment_Request = 101,
        Text = 102,
        Image = 103,
        Pdf = 10102,
        Video = 10103,
        Poll = 10105,
        Payment_Done = 10106
    }

    public enum AccountOwnerType
    {
        Customer = 101,
        Community = 102,
        Store = 103
    }
    public enum MailType
    {
        planner = 101,
        Setting = 102,
        SickNotes = 103,

        ReportIT = 104,
        ForgotPassword = 105,
        Invoice = 106,

        Transaction = 107,
        Login_OTP = 108,
        Broadcast_Messages = 109,
        Welcome_Message = 110,
        OrderCollected = 111,
        JobApproved = 112,
        AccountSent = 113,

        OrderPlaced = 114,
        JobDecline = 115,
        BusinessApprove = 116,
        BusinessDeclined = 117,
        AccountPaid = 118,
        Explore = 119,
        Event_Email_Paid = 120,
        Event_Email_Free = 121,
        Helpline = 122,
        Login_OTP_Subscription = 123,
        JobPostApplicant = 124,
         Event_Notify = 125,
        Booking_Notification=126,
        Booking_Unregistered=127,
        Support_Request=128,
        Welcome_comm_portal = 10128,
        withdrawal_request = 10129,
        Exeeder_Welcome = 10131,
        StorefrontRecon = 10132,
        Withdrawal_OTP_Email = 10133,
        RequestApproval=10134,
        RequestDeclined=10135,
        Welcome_Subscription_Commportal= 10136,
        Cancle_Subscription_Commportal = 10137

    }

    public enum UserPermissions
    {
        VisitorScanner = 1,
        EventScanner = 2,
        TransportScanner = 5
    }
}