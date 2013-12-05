﻿using OnlineShopping.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OnlineShopping.Domain.Abstract
{
    public interface IOnlineTransactionRepository
    {
        IQueryable<OnlineTransaction> Transactions { get; }
        void saveTransaction(OnlineTransaction Transaction);
        void quickSaveTransaction(OnlineTransaction Transaction);
        void saveContext();
        void deleteTransaction(OnlineTransaction Transaction);
    }
}