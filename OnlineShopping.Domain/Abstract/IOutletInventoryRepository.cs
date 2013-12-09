using OnlineShopping.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Domain.Abstract
{
    public interface IOutletInventoryRepository
    {
        IQueryable<OutletInventory> OutletInventories { get;}
        //void saveOutletInventory(OutletInventory outletInventory);
        void quickSaveOutletInventory(OutletInventory outletInventory);
        void quickUpdateOutletInventory(OutletInventory outletInventory);
        void saveContext();
        void deleteOutletInventory(OutletInventory outletInventory);
    }
}
