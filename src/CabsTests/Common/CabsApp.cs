﻿using System;
using System.Collections.Generic;
using LegacyFighter.Cabs.Agreements;
using LegacyFighter.Cabs.CarFleet;
using LegacyFighter.Cabs.Contracts.Application.Acme.Dynamic;
using LegacyFighter.Cabs.Contracts.Application.Acme.Straightforward;
using LegacyFighter.Cabs.Contracts.Application.Editor;
using LegacyFighter.Cabs.Contracts.Legacy;
using LegacyFighter.Cabs.Crm.Claims;
using LegacyFighter.Cabs.Crm.TransitAnalyzer;
using LegacyFighter.Cabs.DriverFleet;
using LegacyFighter.Cabs.DriverFleet.DriverReports;
using LegacyFighter.Cabs.DriverFleet.DriverReports.TravelledDistances;
using LegacyFighter.Cabs.Geolocation.Address;
using LegacyFighter.Cabs.Loyalty;
using LegacyFighter.Cabs.Pricing;
using LegacyFighter.Cabs.Repair.Api;
using LegacyFighter.Cabs.Repair.Legacy.Service;
using LegacyFighter.Cabs.Ride;
using LegacyFighter.Cabs.Tracking;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LegacyFighter.CabsTests.Common;

internal class CabsApp : WebApplicationFactory<Program>
{
  private IServiceScope _scope;
  private readonly Action<IServiceCollection> _customization;
  
  /// <summary>
  /// https://stackoverflow.com/questions/66942392/unwanted-unique-constraint-in-many-to-many-relationship
  /// </summary>
  private bool _reuseScope = false;
  private readonly Dictionary<string, string> _configurationOverrides;

  private CabsApp(Action<IServiceCollection> customization, Dictionary<string, string> configurationOverrides)
  {
    _customization = customization;
    _configurationOverrides = configurationOverrides;
    _scope = base.Services.CreateAsyncScope();
  }

  public static CabsApp CreateInstance()
  {
    var cabsApp = new CabsApp(_ => { }, new Dictionary<string, string>());
    return cabsApp;
  }

  public static CabsApp CreateInstance(Dictionary<string, string> configurationOverrides)
  {
    var cabsApp = new CabsApp(_ => { }, configurationOverrides);
    return cabsApp;
  }

  public static CabsApp CreateInstance(Action<IServiceCollection> customization)
  {
    var cabsApp = new CabsApp(customization, new Dictionary<string, string>());
    return cabsApp;
  }

  public static CabsApp CreateInstance(Action<IServiceCollection> customization, Dictionary<string, string> configurationOverrides)
  {
    var cabsApp = new CabsApp(customization, configurationOverrides);
    return cabsApp;
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    base.ConfigureWebHost(builder);
    builder.ConfigureAppConfiguration(configurationBuilder =>
    {
      configurationBuilder.AddInMemoryCollection(_configurationOverrides);
    });
    builder.ConfigureServices(collection =>
    {
      collection.AddTransient<Fixtures>();
      collection.AddTransient<AddressFixture>();
      collection.AddTransient<AwardsAccountFixture>();
      collection.AddTransient<CarTypeFixture>();
      collection.AddTransient<ClaimFixture>();
      collection.AddTransient<ClientFixture>();
      collection.AddTransient<DriverFixture>();
      collection.AddTransient<RideFixture>();
      collection.AddTransient<StubbedTransitPrice>();
      collection.AddTransient<TransitFixture>();
      collection.AddSingleton(Substitute.ForPartsOf<Tariffs>());
    });
    builder.ConfigureServices(_customization);
  }

  public void StartReuseRequestScope()
  {
    _reuseScope = true;
  }

  public void EndReuseRequestScope()
  {
    _reuseScope = false;
  }

  protected override void Dispose(bool disposing)
  {
    _scope?.Dispose();
    base.Dispose(disposing);
  }

  private IServiceScope RequestScope()
  {
    if (!_reuseScope)
    {
      _scope.Dispose();
      _scope = Services.CreateAsyncScope();
    }
    return _scope;
  }

  public Fixtures Fixtures 
    => RequestScope().ServiceProvider.GetRequiredService<Fixtures>();

  public IDriverFeeService DriverFeeService
    => RequestScope().ServiceProvider.GetRequiredService<IDriverFeeService>();

  public IDriverService DriverService
    => RequestScope().ServiceProvider.GetRequiredService<IDriverService>();

  public IRideService RideService
    => RequestScope().ServiceProvider.GetRequiredService<IRideService>();

  public IDriverSessionService DriverSessionService
    => RequestScope().ServiceProvider.GetRequiredService<IDriverSessionService>();

  public IDriverTrackingService DriverTrackingService
    => RequestScope().ServiceProvider.GetRequiredService<IDriverTrackingService>();

  public TransitController TransitController
    => RequestScope().ServiceProvider.GetRequiredService<TransitController>();

  public ICarTypeService CarTypeService
    => RequestScope().ServiceProvider.GetRequiredService<ICarTypeService>();

  public IClaimService ClaimService
    => RequestScope().ServiceProvider.GetRequiredService<IClaimService>();

  public IAwardsService AwardsService
    => RequestScope().ServiceProvider.GetRequiredService<IAwardsService>();

  public IAwardsAccountRepository AwardsAccountRepository
    => RequestScope().ServiceProvider.GetRequiredService<IAwardsAccountRepository>();

  public IContractService ContractService
    => RequestScope().ServiceProvider.GetRequiredService<IContractService>();

  public DriverReportController DriverReportController 
    => RequestScope().ServiceProvider.GetRequiredService<DriverReportController>();

  public IAddressRepository AddressRepository
    => RequestScope().ServiceProvider.GetRequiredService<IAddressRepository>();

  public ITravelledDistanceService TravelledDistanceService
    => RequestScope().ServiceProvider.GetRequiredService<ITravelledDistanceService>();

  public TransitAnalyzerController TransitAnalyzerController
    => RequestScope().ServiceProvider.GetRequiredService<TransitAnalyzerController>();

  public GraphTransitAnalyzer GraphTransitAnalyzer
    => RequestScope().ServiceProvider.GetRequiredService<GraphTransitAnalyzer>();

  public ITransitRepository TransitRepository
    => RequestScope().ServiceProvider.GetRequiredService<ITransitRepository>();

  public IPopulateGraphService PopulateGraphService
    => RequestScope().ServiceProvider.GetRequiredService<IPopulateGraphService>();

  public IJobDoer JobDoer
    => RequestScope().ServiceProvider.GetRequiredService<IJobDoer>();

  public RepairProcess VehicleRepairProcess
    => RequestScope().ServiceProvider.GetRequiredService<RepairProcess>();

  public IContractManager ContractManager
    => RequestScope().ServiceProvider.GetRequiredService<IContractManager>();

  public IDocumentEditor DocumentEditor
    => RequestScope().ServiceProvider.GetRequiredService<IDocumentEditor>();

  public IDocumentResourceManager DocumentResourceManager
    => RequestScope().ServiceProvider.GetRequiredService<IDocumentResourceManager>();

  public IUserRepository UserRepository
    => RequestScope().ServiceProvider.GetRequiredService<IUserRepository>();

  public IAcmeContractProcessBasedOnStraightforwardDocumentModel AcmeContractProcessBasedOnStraightforwardDocumentModel
    => RequestScope().ServiceProvider.GetRequiredService<IAcmeContractProcessBasedOnStraightforwardDocumentModel>();

  public RideFixture RideFixture
    => RequestScope().ServiceProvider.GetRequiredService<RideFixture>();
}