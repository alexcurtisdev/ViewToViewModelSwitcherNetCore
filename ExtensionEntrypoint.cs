using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

namespace ViewToViewModelSwitcherNetCore
{
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Extension
    {
        public override ExtensionConfiguration ExtensionConfiguration => new()
        {
            Metadata = new(
                    id: "ViewToViewModelSwitcherNetCore.fdb8f460-25f1-4164-85b2-f81e0a5cbb86",
                    version: this.ExtensionAssemblyVersion,
                    publisherName: "Alex Curtis",
                    displayName: "MVVM View To View Model Switcher",
                    description: "A quick way to switch between a view and a view model"),
        };

        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
            base.InitializeServices(serviceCollection);
        }
    }
}
