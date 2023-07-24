using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Avalonia.Data.Core.Plugins;

namespace Avalonia.Data.Core.ExpressionNodes.Reflection;

/// <summary>
/// A node in the binding path of an <see cref="UntypedBindingExpression"/> that reads a property
/// via an <see cref="IPropertyAccessorPlugin"/> selected at runtime from the registered
/// <see cref="BindingPlugins.PropertyAccessors"/>.
/// </summary>
[RequiresUnreferencedCode(TrimmingMessages.ExpressionNodeRequiresUnreferencedCodeMessage)]
internal class DynamicPluginPropertyAccessorNode : ExpressionNode, IPropertyAccessorNode
{
    private readonly Action<object?> _onValueChanged;
    private IPropertyAccessor? _accessor;
    private bool _enableDataValidation;

    public DynamicPluginPropertyAccessorNode(string propertyName)
    {
        _onValueChanged = OnValueChanged;
        PropertyName = propertyName;
    }

    public string PropertyName { get; }
    public IPropertyAccessor? Accessor => _accessor;

    override public void BuildString(StringBuilder builder)
    {
        if (builder.Length > 0 && builder[builder.Length - 1] != '!')
            builder.Append('.');
        builder.Append(PropertyName);
    }

    public void EnableDataValidation() => _enableDataValidation = true;

    public override bool WriteValueToSource(object? value, IReadOnlyList<ExpressionNode> nodes)
    {
        return _accessor?.SetValue(value, BindingPriority.LocalValue) ?? false;
    }

    protected override void OnSourceChanged(object source)
    {
        var reference = new WeakReference<object?>(source);

        if (GetPlugin(source) is { } plugin &&
            plugin.Start(reference, PropertyName) is { } accessor)
        {
            if (_enableDataValidation)
            {
                foreach (var validator in BindingPlugins.DataValidators)
                {
                    if (validator.Match(reference, PropertyName))
                        accessor = validator.Start(reference, PropertyName, accessor);
                }
            }

            _accessor = accessor;
            _accessor.Subscribe(_onValueChanged);
        }
        else
        {
            SetError(
                $"Could not find a matching property accessor for '{PropertyName}' on '{source}'");
        }
    }

    protected override void Unsubscribe(object oldSource)
    {
        _accessor?.Dispose();
        _accessor = null;
    }

    private void OnValueChanged(object? newValue)
    {
        SetValue(newValue);
    }

    private IPropertyAccessorPlugin? GetPlugin(object? source)
    {
        if (source is null)
            return null;

        foreach (var plugin in BindingPlugins.PropertyAccessors)
        {
            if (plugin.Match(source, PropertyName))
                return plugin;
        }

        return null;
    }
}
