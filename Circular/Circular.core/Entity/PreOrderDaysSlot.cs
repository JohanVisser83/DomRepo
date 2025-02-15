using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblPreOrderDaySlots")]
public class PreOrderDaysSlot : BaseEntity
{

   public int  ID { get; set; }
    public int PreOrder_ID { get; set; }  
    public string Days { get; set; }

    public string Attribute1 { get; set; }  
    public string Attribute2 { get; set; }  
    public override void ApplyKeys()
    {

    }
}

