using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CmsIngestion.Application.DTOs
{
    public class CmsEventDto
    {
        public string Type { get; set; } = string.Empty; // "publish", "delete", "unPublish"
        public string Id { get; set; } = string.Empty;
        public JsonObject? Payload { get; set; }
        public int Version { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
