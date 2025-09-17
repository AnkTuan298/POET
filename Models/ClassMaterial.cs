using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POET.Models
{
    public class ClassMaterial
    {
        [Key]
        public int MaterialId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        [ForeignKey("Class")]
        public int ClassId { get; set; }

        public virtual Class Class { get; set; }
    }
}