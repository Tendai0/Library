﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryData.Models
{
    public class BranchHours
    {
        [Key]
        public int BranchId { get; set; }
        public LibraryBranch LibraryBranch { get; set; }
        [Range(0,6)]
        public int DayOfWeek { get; set; }
        [Range(0,23)]
        public int OpenTime { get; set; }
        [Range (0,23)]
        public int CloseTime { get; set; }

    }
}
