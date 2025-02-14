using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public class t_Country
    {
        [Key]
        [Column("c_countryid")]
        public int c_countryid { get; set; }

        [Column("c_countryname")]
        [Required]
        [StringLength(50)]
        public string c_countryname { get; set; }

        
    }
}