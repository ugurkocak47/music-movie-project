using Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Music:IEntity<Guid>
    {
        public string TrackName { get; set; }
        public string? Album { get; set; }
        public string Artist { get; set; }
        public List<Guid> Categories { get; set; }
    }
}
