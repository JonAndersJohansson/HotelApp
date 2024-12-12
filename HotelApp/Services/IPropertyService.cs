using HotelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Services
{
    public interface IPropertyService
    {
        void PropertySwitch(Room room, bool isNew);
    }
}
