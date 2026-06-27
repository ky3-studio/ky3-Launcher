// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

namespace Launcher.SourceGeneration;

internal static class WellKnownMetadataNames
{
    public const string CommandAttribute = "Launcher.Core.Annotation.CommandAttribute";
    public const string GeneratedConstructorAttribute = "Launcher.Core.Annotation.GeneratedConstructorAttribute";
    public const string BindableCustomPropertyProviderAttribute = "Launcher.Core.Annotation.BindableCustomPropertyProviderAttribute";
    public const string DependencyPropertyAttributeT = "Launcher.Core.Annotation.DependencyPropertyAttribute`1";
    public const string FieldAccessAttribute = "Launcher.Core.Annotation.FieldAccessorAttribute";

    public const string HttpClient = "System.Net.Http.HttpClient";
    public const string HttpClientAttribute = "Launcher.Core.DependencyInjection.Annotation.HttpClient.HttpClientAttribute";
    public const string PrimaryHttpMessageHandlerAttribute = "Launcher.Core.DependencyInjection.Annotation.HttpClient.PrimaryHttpMessageHandlerAttribute";
    public const string HttpClientConfiguration = "Launcher.Core.DependencyInjection.Annotation.HttpClient.HttpClientConfiguration.";

    public const string IServiceProvider = "System.IServiceProvider";
    public const string ServiceLifetimeSingleton = "Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton";
    public const string ServiceLifetimeScoped = "Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped";
    public const string ServiceLifetimeTransient = "Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient";

    public const string ServiceAttribute = "Launcher.Core.DependencyInjection.Annotation.ServiceAttribute";
    public const string FromKeyedServicesAttribute = "Launcher.Core.DependencyInjection.Annotation.FromKeyedServicesAttribute";

    public const string ExtendedEnumAttribute = "Launcher.Resource.Localization.ExtendedEnumAttribute";
    public const string LocalizationKeyAttribute = "Launcher.Resource.Localization.LocalizationKeyAttribute";
    public const string InterceptsLocationAttribute = "System.Runtime.CompilerServices.InterceptsLocationAttribute";
}