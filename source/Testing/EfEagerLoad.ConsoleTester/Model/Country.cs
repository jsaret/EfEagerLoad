using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EfEagerLoad.Attributes;

namespace EfEagerLoad.ConsoleTester.Model
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        [EagerLoad]
        public virtual IList<Publisher> Publishers { get; set; } = new List<Publisher>();
    }
}
