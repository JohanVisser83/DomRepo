

namespace Circular.Core.DTOs
{
    public  class PreOrderTimeSlotsDTO
    {
        public long PreOrder_ID { get; set; }

        public string Schedule_Item { get; set; }
        public DateTime Open_Time { get; set; }
        public DateTime Closed_Time { get; set; }

        public string Days { get; set; }
    }
}
