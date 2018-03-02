using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Auth.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public string Id { get; set; }       
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
