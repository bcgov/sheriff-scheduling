﻿using System.ComponentModel.DataAnnotations;
using Mapster;

namespace db.models
{
    [AdaptTo("[name]Dto")]
    public class Region : BaseEntity
    {
        [Key]
        public int Id {get;set;}
        public string Code { get; set; }
        public string Name { get; set; }
    }
}