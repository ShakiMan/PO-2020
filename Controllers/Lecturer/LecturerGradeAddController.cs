﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PO_implementacja_StudiaPodyplomowe.Controllers.Lecturer
{
    public class LecturerGradeAddController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}