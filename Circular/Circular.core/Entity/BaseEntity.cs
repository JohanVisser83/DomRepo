using RepoDb.Attributes;

namespace Circular.Core.Entity
{
   
    public abstract class BaseEntity
    {
       
        [Identity]
        public long Id { get; set; }
        public Guid GUID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public long CreatedBy { get; set; }
        public long ModifiedBy { get; set; }

        public DateTime CreatedDateOnly { 
            get 
            {
                return this.CreatedDate.Date;
            } 
        }

        public DateTime ModifiedDateOnly
        {
            get
            {   
                return this.ModifiedDate.Date;
            }
        }


        public abstract void ApplyKeys();
        public void FillDefaultValues()
        {
            IsActive = (IsActive == null || IsActive == false) ? true : IsActive;
            CreatedBy = (CreatedBy == null || CreatedBy == 0) ? 101 : CreatedBy;
            ModifiedBy = (ModifiedBy == null || ModifiedBy == 0) ? 101 : ModifiedBy;
            CreatedDate = (CreatedDate == null || CreatedDate == DateTime.MinValue) ? DateTime.Now : CreatedDate;
            ModifiedDate = (ModifiedDate == null || ModifiedDate == DateTime.MinValue) ? DateTime.Now : ModifiedDate;
            GUID = (GUID == null || GUID == (new Guid("00000000-0000-0000-0000-000000000000"))) ? Guid.NewGuid() : GUID;
        }
        public BaseEntity FillDefaultValues(String Default = "")
        {
            IsActive = (IsActive == null || IsActive == false) ? true : IsActive;
            CreatedBy = (CreatedBy == null || CreatedBy == 0) ? 101 : CreatedBy;
            ModifiedBy = (ModifiedBy == null || ModifiedBy == 0) ? 101 : ModifiedBy;
            CreatedDate = (CreatedDate == null || CreatedDate == DateTime.MinValue) ? DateTime.Now : CreatedDate;
            ModifiedDate = (ModifiedDate == null || ModifiedDate == DateTime.MinValue) ? DateTime.Now : ModifiedDate;
            GUID = (GUID == null || GUID == (new Guid("00000000-0000-0000-0000-000000000000"))) ? Guid.NewGuid() : GUID;

            return this;
        }
        public void UpdateModifiedByAndDateTime()
        {
            ModifiedBy = (ModifiedBy == null || ModifiedBy == 0) ? 101 : ModifiedBy;
            ModifiedDate = DateTime.Now;
        }
    }


}
