﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public class QRAttendanceScannerDTO
    {
        public string AttendanceScannerImage { get; set; }
        public long CommunityId { get; set; }
    }
}
