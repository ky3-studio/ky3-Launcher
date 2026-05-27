//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using System.Collections.Immutable;

namespace kyxsan.Core.IO.Ini;

internal sealed class IniSection : IniElement, IEquatable<IniSection>
{
    public IniSection(string name, ImmutableArray<IniElement> children)
    {
        Name = name;
        Children = children;
    }

    public string Name { get; }

    public ImmutableArray<IniElement> Children { get; }

    public override string ToString()
    {
        return $"[{Name}]";
    }

    public bool Equals(IniSection? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Name == other.Name && Children.SequenceEqual(other.Children);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IniSection);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Children);
    }

    internal sealed class Builder : IniElement
    {
        private readonly string name;
        private readonly ImmutableArray<IniElement>.Builder children = ImmutableArray.CreateBuilder<IniElement>();

        public Builder(string name)
        {
            this.name = name;
        }

        public void Add(IniElement element)
        {
            children.Add(element);
        }

        public IniSection ToSection()
        {
            return new(name, children.ToImmutable());
        }

        public override string ToString()
        {
            throw kyxsanException.NotSupported();
        }
    }
}