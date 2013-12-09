using OnlineShopping.Domain.Abstract;
using OnlineShopping.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Domain.Concrete
{
    public class EFOutletRepository : IOutletRepository
    {
        private EFDbContext context = new EFDbContext();
        public IQueryable<Outlet> Outlets
        {
            get { return context.Outlets; }
        }

        public void saveOutlet(Outlet outlet)
        {
            //
        }

        public void deleteOutlet(Outlet outlet)
        {
            //
        }

        public void deleteTable()
        {
            //
        }
    }
}
