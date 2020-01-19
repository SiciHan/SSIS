﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Team8ADProjectSSIS.Models
{
    public class Category
    {
        [Key]
        public int IdCategory { get; set; }
        public string Label { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}