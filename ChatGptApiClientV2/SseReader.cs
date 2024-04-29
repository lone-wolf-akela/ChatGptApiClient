using System;
using System.IO;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ChatGptApiClientV2;
// from https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/openai/Azure.AI.OpenAI/src/Helpers/SseLine.cs
// and https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/openai/Azure.AI.OpenAI/src/Helpers/SseReader.cs

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// SSE specification: https://html.spec.whatwg.org/multipage/server-sent-events.html#parsing-an-event-stream

internal readonly struct SseLine
{
    private readonly string _original;
    private readonly int _colonIndex;
    private readonly int _valueIndex;

    public static SseLine Empty { get; } = new(string.Empty, 0, false);

    internal SseLine(string original, int colonIndex, bool hasSpaceAfterColon)
    {
        _original = original;
        _colonIndex = colonIndex;
        _valueIndex = colonIndex + (hasSpaceAfterColon ? 2 : 1);
    }

    public bool IsEmpty => _original.Length == 0;
    public bool IsComment => !IsEmpty && _original[0] == ':';

    // TODO: we should not expose UTF16 publicly
    public ReadOnlyMemory<char> FieldName => _original.AsMemory(0, _colonIndex);
    public ReadOnlyMemory<char> FieldValue => _original.AsMemory(_valueIndex);

    public override string ToString() => _original;
}

internal sealed class SseReader(Stream stream) : IDisposable
{
    private readonly StreamReader _reader = new(stream);
    private bool _disposedValue;

    public SseLine? TryReadSingleFieldEvent()
    {
        while (true)
        {
            var line = TryReadLine();
            if (line == null)
                return null;
            if (line.Value.IsEmpty)
                throw new InvalidDataException("event expected.");
            var empty = TryReadLine();
            if (empty is { IsEmpty: false })
                throw new NotSupportedException("Multi-filed events not supported.");
            if (!line.Value.IsComment)
                return line; // skip comment lines
        }
    }

    // TODO: we should support cancellation tokens, but StreamReader does not in NS2
    public async Task<SseLine?> TryReadSingleFieldEventAsync()
    {
        while (true)
        {
            var line = await TryReadLineAsync().ConfigureAwait(false);
            if (line == null)
                return null;
            if (line.Value.IsEmpty)
                throw new InvalidDataException("event expected.");
            var empty = await TryReadLineAsync().ConfigureAwait(false);
            if (empty is { IsEmpty: false })
                throw new NotSupportedException("Multi-filed events not supported.");
            if (!line.Value.IsComment)
                return line; // skip comment lines
        }
    }

    public SseLine? TryReadLine()
    {
        var lineText = _reader.ReadLine();
        if (lineText == null)
            return null;
        if (lineText.Length == 0)
            return SseLine.Empty;
        if (TryParseLine(lineText, out var line))
            return line;
        return null;
    }

    // TODO: we should support cancellation tokens, but StreamReader does not in NS2
    public async Task<SseLine?> TryReadLineAsync()
    {
        var lineText = await _reader.ReadLineAsync().ConfigureAwait(false);
        if (lineText == null)
            return null;
        if (lineText.Length == 0)
            return SseLine.Empty;
        if (TryParseLine(lineText, out var line))
            return line;
        return null;
    }

    private static bool TryParseLine(string lineText, out SseLine line)
    {
        if (lineText.Length == 0)
        {
            line = default;
            return false;
        }

        var lineSpan = lineText.AsSpan();
        var colonIndex = lineSpan.IndexOf(':');
        var fieldValue = lineSpan[(colonIndex + 1)..];

        var hasSpace = fieldValue.Length > 0 && fieldValue[0] == ' ';
        line = new SseLine(lineText, colonIndex, hasSpace);
        return true;
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _reader.Dispose();
            stream.Dispose();
        }

        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
    }
}