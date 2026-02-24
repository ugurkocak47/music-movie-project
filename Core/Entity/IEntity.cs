using System.ComponentModel.DataAnnotations;

namespace Core.Entity
{
    public class IEntity<TId>
    {
        [Key]
        public TId Id { get; set; }
        public DateTime CreatedDate { get; set; } 
        public DateTime? UpdatedDate { get; set; } 
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}