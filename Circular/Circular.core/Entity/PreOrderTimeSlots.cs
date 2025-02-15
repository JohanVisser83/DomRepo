using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblPreOrderTimeSlots")]


public class PreOrderTimeSlots : BaseEntity
{
    public long PreOrder_ID { get; set; }

    public string Schedule_Item {get; set; }
    public DateTime Open_Time { get; set; }
    public  DateTime Closed_Time { get; set; }

    

    
    public override void ApplyKeys()
    {

    }

}

