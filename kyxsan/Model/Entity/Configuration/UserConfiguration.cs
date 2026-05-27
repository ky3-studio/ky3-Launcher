//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using kyxsan.Web.Hoyolab;
using System.Security.Cryptography;
using System.Text;

namespace kyxsan.Model.Entity.Configuration;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    [SuppressMessage("", "SH007")]
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.CookieToken)
            .HasColumnType(SqliteTypeNames.Text)
            .HasConversion(e => Protect(e!.ToString()), e => Cookie.Parse(Unprotect(e)));

        builder.Property(e => e.LToken)
            .HasColumnType(SqliteTypeNames.Text)
            .HasConversion(e => Protect(e!.ToString()), e => Cookie.Parse(Unprotect(e)));

        builder.Property(e => e.SToken)
            .HasColumnType(SqliteTypeNames.Text)
            .HasConversion(e => Protect(e!.ToString()), e => Cookie.Parse(Unprotect(e)));
    }

    private static string Protect(string plainText)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(encryptedBytes);
    }

    private static string Unprotect(string encryptedText)
    {
        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (FormatException)
        {
            return encryptedText;
        }
        catch (CryptographicException)
        {
            return encryptedText;
        }
    }
}