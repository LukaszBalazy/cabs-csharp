using CabsIntegrationTests;
using LegacyFighter.Cabs.Entity;
using LegacyFighter.Cabs.Service;
using System;
using System.Threading.Tasks;

namespace LegacyFighter.CabsTests.Integration
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

        [Test]
        public async Task CanCreateActiveDriverWithValidLicense()
        {
            // when
            var driver = await CreateActiveDriverWithLicense("FARME100165AB5EW");

            // then
            var loaded = await DriverService.LoadDriver(driver.Id);

            loaded.Should().NotBeNull();
            loaded.DriverLicense.Should().Be("FARME100165AB5EW");
            loaded.Status.Should().Be(Driver.Statuses.Active);
        }

        [Test]
        public async Task CanCreateInactiveDriverWithValidLicense()
        {
            // When
            var driver = await CreateInactiveDriverWithLicense("FARME100165AB5EW");

            // then
            var loaded = await DriverService.LoadDriver(driver.Id);

            loaded.Should().NotBeNull();
            loaded.DriverLicense.Should().Be("FARME100165AB5EW");
            loaded.Status.Should().Be(Driver.Statuses.Inactive);
        }

        [Test]
        public async Task CanChangeLicenseForValidOne()
        {
            // Given
            var driver = await CreateActiveDriverWithLicense("FARME100165AB5EW");

            // When
            await DriverService.ChangeLicenseNumber("FARME100165AB1EW", driver.Id);

            // Then
            var loaded = await DriverService.LoadDriver(driver.Id);
            loaded.Should().NotBeNull();
            loaded.DriverLicense.Should().Be("FARME100165AB1EW");
        }

        [Test]
        public async Task CannotChangelicenseForInvalidOne()
        {
            // Given
            var driver = await CreateActiveDriverWithLicense("FARME100165AB5EW");

            // Then
            await this.Awaiting(_ => DriverService.ChangeLicenseNumber("invalid", driver.Id)).Should().ThrowExactlyAsync<ArgumentException>("Illegal new license no = invalid");
        }

        [Test]
        public async Task CanActivateDriverWithValidLicense()
        {
            // Given
            var driver = await CreateInactiveDriverWithLicense("FARME100165AB5EW");

            // When
            await DriverService.ChangeDriverStatus(driver.Id, Driver.Statuses.Active);

            // Then
            var loaded = await DriverService.LoadDriver(driver.Id);
            loaded.Status.Should().Be(Driver.Statuses.Active);
        }

        [Test]
        public async Task CannotActivateDriverWithInvalidLicense()
        {
            // Given
            var driver = await CreateInactiveDriverWithLicense("invalid");

            // Then
            await this.Awaiting(_ => DriverService.ChangeDriverStatus(driver.Id, Driver.Statuses.Active)).Should().ThrowExactlyAsync<InvalidOperationException>("Status cannot be ACTIVE.Illegal license no = invalid");
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

        private async Task<Driver> CreateInactiveDriverWithLicense(string license)
        {
            return await DriverService.CreateDriver(
              license,
              "Kowalski",
              "Jan",
              Driver.Types.Regular,
              Driver.Statuses.Inactive,
              "photo");
        }
    }
}
