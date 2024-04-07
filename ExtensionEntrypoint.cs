using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;
using System.Runtime.ConstrainedExecution;

namespace ViewToViewModelSwitcher
{
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Extension
    {
        public override ExtensionConfiguration ExtensionConfiguration => new()
        {
            Metadata = new(
                    id: "ViewToViewModelSwitcher.fb2787f7-8b89-4cee-9e76-02ff6486f4f5",
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
