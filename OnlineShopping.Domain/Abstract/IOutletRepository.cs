using OnlineShopping.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Domain.Abstract
{
    public interface IOutletRepository
    {
        IQueryable<Outlet> Outlets { get;}
        void saveOutlet(Outlet outlet);
        void deleteOutlet(Outlet outlet);
    }
}
