using OnlineShopping.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OnlineShopping.Domain.Abstract
{
    public interface ITransactionRepository
    {
        IQueryable<Transaction> Transactions { get; }
        void saveTransaction(Transaction Transaction);
        void quickSaveTransaction(Transaction Transaction);
        void saveContext();
        void deleteTransaction(Transaction Transaction);
    }
}