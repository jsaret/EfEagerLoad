using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EfEagerLoad.Attributes;

namespace EfEagerLoad.ConsoleTester.Model
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        [EagerLoad]
        public virtual Publisher Publisher { get; set; }

        [EagerLoad]
        public virtual Category Category { get; set; }

        [EagerLoad]
        //[EagerLoad(maxRootTypeCount: 2, maxTypeCount:2)]
        public virtual Author Author { get; set; }
    }
}
