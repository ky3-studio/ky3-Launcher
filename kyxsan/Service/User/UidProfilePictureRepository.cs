//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity;
using kyxsan.Service.Abstraction;

namespace kyxsan.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IUidProfilePictureRepository))]
internal sealed partial class UidProfilePictureRepository : IUidProfilePictureRepository
{
    [GeneratedConstructor]
    public partial UidProfilePictureRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public UidProfilePicture? SingleUidProfilePictureOrDefaultByUid(string uid)
    {
        try
        {
            return this.Query(query => query.SingleOrDefault(n => n.Uid == uid));
        }
        catch
        {
            this.Delete(n => n.Uid == uid);
            return default;
        }
    }

    public void UpdateUidProfilePicture(UidProfilePicture profilePicture)
    {
        this.Update(profilePicture);
    }

    public void DeleteUidProfilePictureByUid(string uid)
    {
        this.Delete(profilePicture => profilePicture.Uid == uid);
    }
}