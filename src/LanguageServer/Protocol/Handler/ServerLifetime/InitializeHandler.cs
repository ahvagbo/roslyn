﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Internal.Log;
using Roslyn.LanguageServer.Protocol;

namespace Microsoft.CodeAnalysis.LanguageServer.Handler;

[Method(Methods.InitializeName)]
internal sealed class InitializeHandler : ILspServiceRequestHandler<InitializeParams, InitializeResult>
{
    public InitializeHandler()
    {
    }

    public bool MutatesSolutionState => true;
    public bool RequiresLSPSolution => false;

    public Task<InitializeResult> HandleRequestAsync(InitializeParams request, RequestContext context, CancellationToken cancellationToken)
    {
        var clientCapabilitiesManager = context.GetRequiredLspService<IInitializeManager>();
        var clientCapabilities = clientCapabilitiesManager.TryGetClientCapabilities();
        if (clientCapabilities != null)
        {
            throw new InvalidOperationException($"{nameof(Methods.InitializeName)} called multiple times");
        }

        clientCapabilities = request.Capabilities;
        clientCapabilitiesManager.SetInitializeParams(request);

        var capabilitiesProvider = context.GetRequiredLspService<ICapabilitiesProvider>();
        var serverCapabilities = capabilitiesProvider.GetCapabilities(clientCapabilities);

        // Record a telemetry event indicating what capabilities are being provided by the server.
        // Useful for figuring out if a particular session is opted into an LSP feature.
        Logger.Log(FunctionId.LSP_Initialize, KeyValueLogMessage.Create(m =>
        {
            m["serverKind"] = context.ServerKind.ToTelemetryString();
            m["capabilities"] = JsonSerializer.Serialize(serverCapabilities, ProtocolConversions.LspJsonSerializerOptions);
        }));

        return Task.FromResult(new InitializeResult
        {
            Capabilities = serverCapabilities,
        });
    }
}
