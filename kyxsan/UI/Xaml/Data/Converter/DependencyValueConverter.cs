//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using kyxsan.Core.ExceptionService;

namespace kyxsan.UI.Xaml.Data.Converter;

internal abstract class DependencyValueConverter<TFrom, TTo> : DependencyObject, IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        try
        {
            return Convert((TFrom)value);
        }
        catch
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        try
        {
            return ConvertBack((TTo)value);
        }
        catch
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public abstract TTo Convert(TFrom from);

    public virtual TFrom ConvertBack(TTo to)
    {
        throw kyxsanException.NotSupported();
    }
}