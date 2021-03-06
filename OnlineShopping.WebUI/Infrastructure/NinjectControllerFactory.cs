﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using System.Collections.Generic;
using OnlineShopping.Domain.Abstract;
using OnlineShopping.Domain.Entities;
using System.Linq;
using OnlineShopping.Domain.Concrete;
using System.Configuration;


namespace OnlineShopping.WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;
        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }
        protected override IController GetControllerInstance(RequestContext requestContext,
        Type controllerType)
        {
            return controllerType == null
            ? null
            : (IController)ninjectKernel.Get(controllerType);
        }
        private void AddBindings()
        {
            // put additional bindings here 
            ninjectKernel.Bind<IProductRepository>().To<EFProductRepository>();
            ninjectKernel.Bind<IManufacturerRepository>().To<EFManufacturerRepository>();
            ninjectKernel.Bind<ICategoryRepository>().To<EFCategoryRepository>();
            ninjectKernel.Bind<IOnlineTransactionDetailRepository>().To<EFOnlineTransactionDetailRepository>();
            ninjectKernel.Bind<IOnlineTransactionRepository>().To<EFOnlineTransactionRepository>();
            ninjectKernel.Bind<IOutletRepository>().To<EFOutletRepository>();
            ninjectKernel.Bind<IOutletInventoryRepository>().To<EFOutletInventoryRepository>();
        }
    }
}