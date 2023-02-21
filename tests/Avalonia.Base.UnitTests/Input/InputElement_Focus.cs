using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.UnitTests;
using Xunit;

namespace Avalonia.Base.UnitTests.Input
{
    public class InputElement_Focus
    {
        [Fact]
        public void Focus_Should_Set_FocusManager_Current()
        {
            Button target;

            var root = new TestRoot { Child = target = new Button() };

            target.Focus();

            Assert.Same(target, FocusManager.Instance.Current);
        }

        [Fact]
        public void Focus_Should_Not_Get_Restored_To_Enabled_Control()
        {
            var sp = new StackPanel();
            Button target = new Button();
            Button target1 = new Button();
            target.Click += (s, e) => target.IsEnabled = false;
            target1.Click += (s, e) => target.IsEnabled = true;
            sp.Children.Add(target);
            sp.Children.Add(target1);
            var root = new TestRoot { Child = sp };

            target.Focus();
            target.RaiseEvent(new RoutedEventArgs(AccessKeyHandler.AccessKeyPressedEvent));
            Assert.False(target.IsEnabled);
            Assert.False(target.IsFocused);
            target1.RaiseEvent(new RoutedEventArgs(AccessKeyHandler.AccessKeyPressedEvent));
            Assert.True(target.IsEnabled);
            Assert.False(target.IsFocused);
        }

        [Fact]
        public void Focus_Should_Be_Cleared_When_Control_Is_Removed_From_VisualTree()
        {
            Button target;

            var root = new TestRoot { Child = target = new Button() };

            target.Focus();
            root.Child = null;

            Assert.Null(FocusManager.Instance.Current);
        }

        [Fact]
        public void Focus_Pseudoclass_Should_Be_Applied_On_Focus()
        {
            var target1 = new Decorator();
            var target2 = new Decorator();
            var root = new TestRoot { Child = new StackPanel { Children = { target1, target2 } } };

            target1.ApplyTemplate();
            target2.ApplyTemplate();


            FocusManager.Instance?.Focus(target1);
            Assert.True(target1.IsFocused);
            Assert.True(target1.Classes.Contains(":focus"));
            Assert.False(target2.IsFocused);
            Assert.False(target2.Classes.Contains(":focus"));

            FocusManager.Instance?.Focus(target2, NavigationMethod.Tab);
            Assert.False(target1.IsFocused);
            Assert.False(target1.Classes.Contains(":focus"));
            Assert.True(target2.IsFocused);
            Assert.True(target2.Classes.Contains(":focus"));
        }

        [Fact]
        public void Control_FocusVsisible_Pseudoclass_Should_Be_Applied_On_Tab_And_DirectionalFocus()
        {
            var target1 = new Decorator();
            var target2 = new Decorator();
            var root = new TestRoot { Child = new StackPanel { Children = { target1, target2 } } };

            target1.ApplyTemplate();
            target2.ApplyTemplate();

            FocusManager.Instance?.Focus(target1);
            Assert.True(target1.IsFocused);
            Assert.False(target1.Classes.Contains(":focus-visible"));
            Assert.False(target2.IsFocused);
            Assert.False(target2.Classes.Contains(":focus-visible"));

            FocusManager.Instance?.Focus(target2, NavigationMethod.Tab);
            Assert.False(target1.IsFocused);
            Assert.False(target1.Classes.Contains(":focus-visible"));
            Assert.True(target2.IsFocused);
            Assert.True(target2.Classes.Contains(":focus-visible"));

            FocusManager.Instance?.Focus(target1, NavigationMethod.Directional);
            Assert.True(target1.IsFocused);
            Assert.True(target1.Classes.Contains(":focus-visible"));
            Assert.False(target2.IsFocused);
            Assert.False(target2.Classes.Contains(":focus-visible"));
        }

        [Fact]
        public void Control_FocusWithin_PseudoClass_Should_Be_Applied()
        {
            var target1 = new Decorator();
            var target2 = new Decorator();
            var root = new TestRoot { Child = new StackPanel { Children = { target1, target2 } } };

            target1.ApplyTemplate();
            target2.ApplyTemplate();

            FocusManager.Instance?.Focus(target1);
            Assert.True(target1.IsFocused);
            Assert.True(target1.Classes.Contains(":focus-within"));
            Assert.True(target1.IsKeyboardFocusWithin);
            Assert.True(root.Child.Classes.Contains(":focus-within"));
            Assert.True(root.Child.IsKeyboardFocusWithin);
            Assert.True(root.Classes.Contains(":focus-within"));
            Assert.True(root.IsKeyboardFocusWithin);
        }

        [Fact]
        public void Control_FocusWithin_PseudoClass_Should_Be_Applied_and_Removed()
        {
            var target1 = new Decorator();
            var target2 = new Decorator();
            var panel1 = new Panel { Children = { target1 } };
            var panel2 = new Panel { Children = { target2 } };
            var root = new TestRoot { Child = new StackPanel { Children = { panel1, panel2 } } };

            target1.ApplyTemplate();
            target2.ApplyTemplate();

            FocusManager.Instance?.Focus(target1);
            Assert.True(target1.IsFocused);
            Assert.True(target1.Classes.Contains(":focus-within"));
            Assert.True(target1.IsKeyboardFocusWithin);
            Assert.True(panel1.Classes.Contains(":focus-within"));
            Assert.True(panel1.IsKeyboardFocusWithin);
            Assert.True(root.Child.Classes.Contains(":focus-within"));
            Assert.True(root.Child.IsKeyboardFocusWithin);
            Assert.True(root.Classes.Contains(":focus-within"));
            Assert.True(root.IsKeyboardFocusWithin);

            FocusManager.Instance?.Focus(target2);

            Assert.False(target1.IsFocused);
            Assert.False(target1.Classes.Contains(":focus-within"));
            Assert.False(target1.IsKeyboardFocusWithin);
            Assert.False(panel1.Classes.Contains(":focus-within"));
            Assert.False(panel1.IsKeyboardFocusWithin);
            Assert.True(root.Child.Classes.Contains(":focus-within"));
            Assert.True(root.Child.IsKeyboardFocusWithin);
            Assert.True(root.Classes.Contains(":focus-within"));
            Assert.True(root.IsKeyboardFocusWithin);

            Assert.True(target2.IsFocused);
            Assert.True(target2.Classes.Contains(":focus-within"));
            Assert.True(target2.IsKeyboardFocusWithin);
            Assert.True(panel2.Classes.Contains(":focus-within"));
            Assert.True(panel2.IsKeyboardFocusWithin);
        }

        [Fact]
        public void Control_FocusWithin_Pseudoclass_Should_Be_Removed_When_Removed_From_Tree()
        {
            var target1 = new Decorator();
            var target2 = new Decorator();
            var root = new TestRoot { Child = new StackPanel { Children = { target1, target2 } } };

            target1.ApplyTemplate();
            target2.ApplyTemplate();

            FocusManager.Instance?.Focus(target1);
            Assert.True(target1.IsFocused);
            Assert.True(target1.Classes.Contains(":focus-within"));
            Assert.True(target1.IsKeyboardFocusWithin);
            Assert.True(root.Child.Classes.Contains(":focus-within"));
            Assert.True(root.Child.IsKeyboardFocusWithin);
            Assert.True(root.Classes.Contains(":focus-within"));
            Assert.True(root.IsKeyboardFocusWithin);

            Assert.Equal(KeyboardDevice.Instance.FocusedElement, target1);

            root.Child = null;

            Assert.Null(KeyboardDevice.Instance.FocusedElement);

            Assert.False(target1.IsFocused);
            Assert.False(target1.Classes.Contains(":focus-within"));
            Assert.False(target1.IsKeyboardFocusWithin);
            Assert.False(root.Classes.Contains(":focus-within"));
            Assert.False(root.IsKeyboardFocusWithin);
        }

        [Fact]
        public void Control_FocusWithin_Pseudoclass_Should_Be_Removed_Focus_Moves_To_Different_Root()
        {
            var target1 = new Decorator();
            var target2 = new Decorator();

            var root1 = new TestRoot { Child = new StackPanel { Children = { target1, } } };

            var root2 = new TestRoot { Child = new StackPanel { Children = { target2, } } };

            target1.ApplyTemplate();
            target2.ApplyTemplate();

            FocusManager.Instance?.Focus(target1);
            Assert.True(target1.IsFocused);
            Assert.True(target1.Classes.Contains(":focus-within"));
            Assert.True(target1.IsKeyboardFocusWithin);
            Assert.True(root1.Child.Classes.Contains(":focus-within"));
            Assert.True(root1.Child.IsKeyboardFocusWithin);
            Assert.True(root1.Classes.Contains(":focus-within"));
            Assert.True(root1.IsKeyboardFocusWithin);

            Assert.Equal(KeyboardDevice.Instance.FocusedElement, target1);

            FocusManager.Instance?.Focus(target2);

            Assert.False(target1.IsFocused);
            Assert.False(target1.Classes.Contains(":focus-within"));
            Assert.False(target1.IsKeyboardFocusWithin);
            Assert.False(root1.Child.Classes.Contains(":focus-within"));
            Assert.False(root1.Child.IsKeyboardFocusWithin);
            Assert.False(root1.Classes.Contains(":focus-within"));
            Assert.False(root1.IsKeyboardFocusWithin);

            Assert.True(target2.IsFocused);
            Assert.True(target2.Classes.Contains(":focus-within"));
            Assert.True(target2.IsKeyboardFocusWithin);
            Assert.True(root2.Child.Classes.Contains(":focus-within"));
            Assert.True(root2.Child.IsKeyboardFocusWithin);
            Assert.True(root2.Classes.Contains(":focus-within"));
            Assert.True(root2.IsKeyboardFocusWithin);
        }

        [Fact]
        public void Can_Clear_Focus()
        {
            Button target;

            var root = new TestRoot { Child = target = new Button() };

            target.Focus();
            FocusManager.Instance.Focus(null);

            Assert.Null(FocusManager.Instance.Current);
        }
    }
}
