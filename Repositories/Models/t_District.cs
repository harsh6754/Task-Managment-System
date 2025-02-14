using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    [Table("t_district")]
    public class t_District
    {
        [Key]
        [Column("c_districtid")]
        public int c_districtid { get; set; }

        [Column("c_districtname")]
        [Required]
        [StringLength(50)]
        public string c_districtname { get; set; }

        [Column("c_stateid")]
        public int c_stateid { get; set; }
    }
}