using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Repositories.Models
{
    [Table("t_state")]
    public class t_State
    {
        [Key]
        [Column("c_stateid")]
        public int c_stateid { get; set; }

        [Column("c_statename")]
        [Required]
        [StringLength(50)]
        public string c_statename { get; set; }

        [Column("c_countryid")]
        public int c_countryid { get; set; }

    }
}