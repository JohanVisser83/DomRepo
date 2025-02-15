

namespace Circular.Framework.ShortMessages
{
    public interface IBulkSMS
    {
        string BulkSms(ShortMessageDetails shortMessageDetails, ShortMessageProviderDetails shortMessageProviderDetails);
    }
}
