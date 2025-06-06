﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Microsoft.CodeAnalysis.MSBuild;

[DataContract]
internal sealed record PackageReference(
    [property: DataMember(Order = 0)] string Name,
    [property: DataMember(Order = 1)] string VersionRange
);
