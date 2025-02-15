namespace Circular.Core.Entity
{
    public  class ActiveOTPDetails : BaseEntity
    {
        public string Name { get; set; }

        public string OTP { get; set; }
        public DateTime CreatedOn { get; set; }

        public string Username { get; set; }


        public override void ApplyKeys()
        {

        }
    }
}
