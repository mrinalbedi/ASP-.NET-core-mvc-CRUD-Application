﻿using System;
using System.Collections.Generic;

namespace MBPatients.Models
{
    public partial class ConcentrationUnit
    {
        public ConcentrationUnit()
        {
            Medication = new HashSet<Medication>();
        }

        public string ConcentrationCode { get; set; }

        public virtual ICollection<Medication> Medication { get; set; }
    }
}
