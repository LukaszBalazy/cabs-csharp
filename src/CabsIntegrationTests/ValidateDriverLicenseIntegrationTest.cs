using LegacyFighter.Cabs.Entity;
using LegacyFighter.Cabs.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabsIntegrationTests
{
    public class ValidateDriverLicenseIntegrationTest
    {
        private IDriverService DriverService => _app.DriverService;
        private CabsApp _app = default!;

        [SetUp]
        public void InitializeApp()
        {
            _app = CabsApp.CreateInstance();
        }

        [TearDown]
        public async Task DisposeOfApp()
        {
            await _app.DisposeAsync();
        }

        [Test]
        public async Task CannotCreateActiveDriverWithInvalidLicense()
        {
            // Then
            await this.Awaiting(_ => CreateActiveDriverWithLicense("invalidLicense")).Should().ThrowExactlyAsync<ArgumentException>();
        }

        public void CanCreateActiveDriverWithValidLicense()
        {

        }

        public void CanCreateInactiveDriverWithValidLicense()
        {

        }

        public void CanChangeLicenseForValidOne()
        {

        }

        public void CannotChangelicenseForInvalidOne()
        {

        }

        public void CanActivateDriverWithValidLicense()
        {

        }

        public void CannotActivateDriverWithInvalidLicense()
        {

        }

        private async Task<Driver> CreateActiveDriverWithLicense(string license)
        {
            return await DriverService.CreateDriver(
              license,
              "Kowalski",
              "Jan",
              Driver.Types.Regular,
              Driver.Statuses.Active,
              "photo");
        }
    }
}
