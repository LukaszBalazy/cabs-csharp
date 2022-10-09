﻿using LegacyFighter.Cabs.Service;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabsIntegrationTests
{
    internal class CabsApp : WebApplicationFactory<Program>
    {
        private IServiceScope _scope;

        private CabsApp()
        {
            _scope = base.Services.CreateAsyncScope();
        }

        public static CabsApp CreateInstance()
        {
            var cabsApp = new CabsApp();
            return cabsApp;
        }

        protected override void Dispose(bool disposing)
        {
            _scope.Dispose();
            base.Dispose(disposing);
        }

        private IServiceScope NewRequestScope()
        {
            _scope.Dispose();
            _scope = Services.CreateAsyncScope();
            return _scope;
        }

        public IDriverService DriverService
          => NewRequestScope().ServiceProvider.GetRequiredService<IDriverService>();
    }
}
