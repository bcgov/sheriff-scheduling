﻿using SS.Api.models.db;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;

namespace SS.Api.Models.DB
{
    [AdaptTo("[name]Dto")]
    public class Location : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? JustinId { get; set; }
        public string JustinCode { get; set; }
        public int? ParentLocationId { get; set; }
        [AdaptIgnore]
        public virtual Region Region { get; set; }
        public int? RegionId { get; set; }
    }
}