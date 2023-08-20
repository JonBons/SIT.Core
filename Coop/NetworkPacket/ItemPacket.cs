﻿using Newtonsoft.Json;

namespace SIT.Core.Coop.NetworkPacket
{
    public class ItemPacket : BasePacket
    {
        [JsonProperty(PropertyName = "iid")]
        public string ItemId { get; set; }

        [JsonProperty(PropertyName = "tpl")]
        public string TemplateId { get; set; }

        public ItemPacket(string itemId, string templateId, string method)
        {
            ItemId = itemId;
            TemplateId = templateId;
            Method = method;
        }
    }

    public class ItemPlayerPacket : BasePlayerPacket
    {
        [JsonProperty(PropertyName = "iid")]
        public string ItemId { get; set; }

        [JsonProperty(PropertyName = "tpl")]
        public string TemplateId { get; set; }

        public ItemPlayerPacket(string profileId, string itemId, string templateId, string method)
            : base(profileId, method)
        {
            ItemId = itemId;
            TemplateId = templateId;
            Method = method;
        }
    }
}
