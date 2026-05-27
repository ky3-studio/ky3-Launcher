// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.SourceGeneration;

internal static class WellKnownMetadataNames
{
    public const string CommandAttribute = "kyxsan.Core.Annotation.CommandAttribute";
    public const string GeneratedConstructorAttribute = "kyxsan.Core.Annotation.GeneratedConstructorAttribute";
    public const string BindableCustomPropertyProviderAttribute = "kyxsan.Core.Annotation.BindableCustomPropertyProviderAttribute";
    public const string DependencyPropertyAttributeT = "kyxsan.Core.Annotation.DependencyPropertyAttribute`1";
    public const string FieldAccessAttribute = "kyxsan.Core.Annotation.FieldAccessorAttribute";

    public const string HttpClient = "System.Net.Http.HttpClient";
    public const string HttpClientAttribute = "kyxsan.Core.DependencyInjection.Annotation.HttpClient.HttpClientAttribute";
    public const string PrimaryHttpMessageHandlerAttribute = "kyxsan.Core.DependencyInjection.Annotation.HttpClient.PrimaryHttpMessageHandlerAttribute";
    public const string HttpClientConfiguration = "kyxsan.Core.DependencyInjection.Annotation.HttpClient.HttpClientConfiguration.";

    public const string IServiceProvider = "System.IServiceProvider";
    public const string ServiceLifetimeSingleton = "Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton";
    public const string ServiceLifetimeScoped = "Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped";
    public const string ServiceLifetimeTransient = "Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient";

    public const string ServiceAttribute = "kyxsan.Core.DependencyInjection.Annotation.ServiceAttribute";
    public const string FromKeyedServicesAttribute = "kyxsan.Core.DependencyInjection.Annotation.FromKeyedServicesAttribute";

    public const string ExtendedEnumAttribute = "kyxsan.Resource.Localization.ExtendedEnumAttribute";
    public const string LocalizationKeyAttribute = "kyxsan.Resource.Localization.LocalizationKeyAttribute";
    public const string InterceptsLocationAttribute = "System.Runtime.CompilerServices.InterceptsLocationAttribute";
}