using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsIngestion.Core.Entities
{
    public class CmsEntity
    {
        public string Id { get; set; } = string.Empty;
        public int Version { get; set; }
        public string PayloadJson { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the entity was unpublished by the CMS or disabled by an API Admin.
        /// </summary>
        public bool IsDisabled { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
