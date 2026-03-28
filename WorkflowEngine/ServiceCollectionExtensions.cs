using Microsoft.Extensions.DependencyInjection;

namespace EventSourcingEngine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterWorkflowTree<TState, TEvent, TTreeProvider>(this IServiceCollection serviceCollection, ServiceLifetime eventSourceTreeLifetime = ServiceLifetime.Scoped)
        where TState : class
        where TEvent : class
        where TTreeProvider : WorkflowTreeProvider<TState, TEvent>
    {
        serviceCollection.AddSingleton<TTreeProvider>();
        
        var descriptor = new ServiceDescriptor(typeof(IWorkflowTreeExecutor<TState, TEvent, TTreeProvider>), typeof(WorkflowTreeExecutor<TState, TEvent, TTreeProvider>), eventSourceTreeLifetime);
        serviceCollection.Add(descriptor);
        
        return serviceCollection;
    }
}