using System.Text.RegularExpressions;

namespace LegacyFighter.Cabs.Entity
{
    public class DriverLicense
    {
        private const string DriverLicenseRegex = "^[A-Z9]{5}\\d{6}[A-Z9]{2}\\d[A-Z]{2}$";

        public string License { get; private set; }

        private DriverLicense(string license)
        {
            License = license;
        }

        public static DriverLicense withLicense(string license)
        {
            if (license == null || !license.Any() || !Regex.IsMatch(license, DriverLicenseRegex))
            {
                throw new InvalidOperationException("Status cannot be ACTIVE. Illegal license no = " + license);
            }

            return new DriverLicense(license);
        }

        public static DriverLicense WithoutValidation(string license) => new DriverLicense(license);
    }
}
