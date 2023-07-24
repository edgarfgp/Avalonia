﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Avalonia.Data.Core.Plugins;
using Avalonia.Reactive;

namespace Avalonia.Data.Core.ExpressionNodes.Reflection;

[RequiresUnreferencedCode(TrimmingMessages.ExpressionNodeRequiresUnreferencedCodeMessage)]
internal class PluginStreamNode : ExpressionNode
{
    private IDisposable? _subscription;

    override public void BuildString(StringBuilder builder)
    {
        builder.Append('^');
    }

    protected override void OnSourceChanged(object source)
    {
        var reference = new WeakReference<object?>(source);

        if (GetPlugin(reference) is { } plugin &&
            plugin.Start(reference) is { } accessor)
        {
            _subscription = accessor.Subscribe(SetValue);
        }
        else
        {
            SetValue(null);
        }
    }

    protected override void Unsubscribe(object oldSource)
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private static IStreamPlugin? GetPlugin(WeakReference<object?> source)
    {
        if (source is null)
            return null;

        foreach (var plugin in BindingPlugins.StreamHandlers)
        {
            if (plugin.Match(source))
                return plugin;
        }

        return null;
    }
}
