﻿using Droplet.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Droplet.Models.Entities
{
    public class Recipient
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PESEL { get; set; } = default!;
        public BloodTypeEnum BloodType { get; set; } = default!;

        public ICollection<Transfusion> Transfusions { get; set; } = new List<Transfusion>();
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
