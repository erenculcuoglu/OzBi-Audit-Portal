using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace OzBiPortalCRM.Services
{
    /// <summary>
    /// Static XML repository for ASP.NET Core Data Protection keys.
    /// Ensures all Cloud Run container instances and cold-starts share the exact same encryption key,
    /// preventing session cookie decryption failures, auto-logouts, and multi-attempt login issues.
    /// </summary>
    public class StaticDataProtectionKeyRepository : IXmlRepository
    {
        private static readonly XElement KeyElement = XElement.Parse(@"
<key id=""e4b5c6d7-8a9b-0c1d-2e3f-4a5b6c7d8e9f"" version=""1"">
  <creationDate>2026-01-01T00:00:00.0000000Z</creationDate>
  <activationDate>2026-01-01T00:00:00.0000000Z</activationDate>
  <expirationDate>2099-12-31T23:59:59.9999999Z</expirationDate>
  <descriptor deserializerType=""Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.AuthenticatedEncryptorDescriptorDeserializer, Microsoft.AspNetCore.DataProtection, Version=8.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60"">
    <descriptor>
      <encryption algorithm=""AES_256_CBC"" />
      <validation algorithm=""HMACSHA256"" />
      <masterKey EncryptedSecret=""kX9vW2Z0Y3A0SDFnM2k0bDVtNm83cDhxOXJzdHV2d3h5ejBBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWjAxMjM0NTY3ODk="" />
    </descriptor>
  </descriptor>
</key>");

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return new[] { KeyElement };
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            // Static key repository - ignores runtime dynamic key generation
        }
    }
}
