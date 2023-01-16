using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OB_Net_Core_Tutorial.BLL.DTO
{
    public class CarDTO
    {
        public string Name { get; set; }
        public Guid TypeId { get; set; }
    }
}