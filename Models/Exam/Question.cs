﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PO_implementacja_StudiaPodyplomowe.Models
{
    public class Question
    {
        private int IdQuestion { get; set; }
        public string Content { get; set; }
        public int Points { get; set; }
        public string Answer { get; set; }
    }
}