using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EfEagerLoad.Attributes;

namespace EfEagerLoad.ConsoleTester.Model
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        //[EagerLoad(maxRootTypeCount: 1, maxTypeCount: 2)]
        [EagerLoad(maxTypeCount: 2)]
        public virtual IList<Book> Books { get; set; } = new List<Book>();
    }
}