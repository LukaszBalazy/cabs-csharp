using LegacyFighter.Cabs.Entity;
using System;

namespace LegacyFighter.CabsTests.Unit
{
    public class DriverLicenseTests
    {
        [Test]
        public void CannotCreateInvalidLicense()
        {
            this.Invoking(_ => DriverLicense.withLicense("invalid"))
                .Should()
                .ThrowExactly<ArgumentException>("Status cannot be ACTIVE. Illegal license no = invalid");

            this.Invoking(_ => DriverLicense.withLicense(""))
                .Should()
                .ThrowExactly<ArgumentException>("Status cannot be ACTIVE. Illegal license no = ");
        }

        [Test]
        public void CanCreateValidLicense()
        {
            // When
            var driverLicense = DriverLicense.withLicense("FARME100165AB5EW");

            // Then
            driverLicense.License.Should().Be("FARME100165AB5EW");
        }

        [Test]
        public void CanCreateInvalidLicenseExplicitly()
        {
            // When
            var license = DriverLicense.WithoutValidation("invalid");

            // Then
            license.License.Should().Be("invalid");
        }
    }
}
