using Core.Entity;

namespace Entity;

public class MovieCategoryLink:IEntity<Guid>
{
    public Guid MovieId { get; set; }
    public Guid CategoryId { get; set; }
}