using HotelApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Services
{
    public interface IPropertySelector<T>
    {
        T PropertySwitch(T entity, params object[] parameters);
    }
}
