using Core.Entity;

namespace Entity;

public class MusicCategoryLink:IEntity<Guid>
{
    public Guid MusicId { get; set; }
    public Guid CategoryId { get; set; }
}