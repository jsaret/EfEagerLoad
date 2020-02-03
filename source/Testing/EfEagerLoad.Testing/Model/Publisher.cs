﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EfEagerLoad.Attributes;

namespace EfEagerLoad.Testing.Model
{
    public class Publisher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        [EagerLoad(onlyIfOnRoot:true)]
        public virtual IList<Book> Books { get; set; } = new List<Book>();
    }
}