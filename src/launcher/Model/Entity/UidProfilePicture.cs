//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Enka.Model;
using kyxsan.Web.Hoyolab;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kyxsan.Model.Entity;

[Table("uid_profile_pictures")]
internal sealed class UidProfilePicture
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public string Uid { get; set; } = default!;

    public uint ProfilePictureId { get; set; }

    public uint AvatarId { get; set; }

    public uint CostumeId { get; set; }

    public DateTimeOffset RefreshTime { get; set; }

    public static UidProfilePicture From(PlayerUid uid, ProfilePicture profilePicture)
    {
        return new()
        {
            Uid = uid.ToString(),
            ProfilePictureId = profilePicture.Id,
            AvatarId = profilePicture.AvatarId,
            CostumeId = profilePicture.CostumeId,
            RefreshTime = DateTimeOffset.Now,
        };
    }
}
