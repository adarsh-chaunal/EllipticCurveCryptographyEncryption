using Domain.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Infrastructure;

public static class EncryptColumnExtension
{
    public static void UseEncryption(this ModelBuilder modelBuilder)
    {
        var converter = new EncryptionConvertor();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string) && !IsDiscriminator(property))
                {
                    var attributes = property.PropertyInfo?.GetCustomAttributes(typeof(EncryptColumnAttribute), false);
                    if (attributes != null && attributes.Any())
                    {
                        property.SetValueConverter(converter);
                    }
                }
            }
        }
    }

    // A helper function to ignore EF Core Discriminator
    private static bool IsDiscriminator(IMutableProperty property)
    {
        return property.Name == "Discriminator" || property.PropertyInfo == null;
    }
}

public class EncryptionConvertor : ValueConverter<string, string>
{
    public EncryptionConvertor(ConverterMappingHints mappingHints = null)
    : base(x => EncryptData(x), x => DecryptData(x), mappingHints)
    { }

    private static string EncryptData(string dataToEncrypt)
    {
        var encryptedData = EncryptColumnHelper.Encrypt(dataToEncrypt);
        return Convert.ToBase64String(encryptedData);
    }

    private static string DecryptData(string dataToDecrypt)
    {
        byte[] encryptedDataWithIv = Convert.FromBase64String(dataToDecrypt);
        return EncryptColumnHelper.Decrypt(encryptedDataWithIv);
    }
}