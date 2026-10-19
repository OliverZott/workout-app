using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;

namespace workout_app;

// Tapping the already selected tab pops that tab back to its root page, as iOS does natively.
// MAUI's Android Shell leaves OnTabReselected empty, so the stack would otherwise stay as it is.
public class TabReselectShellRenderer : ShellRenderer
{
    protected override IShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
        => new TabReselectShellItemRenderer(this);

    class TabReselectShellItemRenderer(IShellContext shellContext) : ShellItemRenderer(shellContext)
    {
        protected override void OnTabReselected(ShellSection shellSection)
        {
            if (shellSection.Navigation.NavigationStack.Count > 1)
            {
                _ = shellSection.Navigation.PopToRootAsync();
            }
        }
    }
}
