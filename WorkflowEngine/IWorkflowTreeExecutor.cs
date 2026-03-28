namespace EventSourcingEngine;

public interface IWorkflowTreeExecutor<TState, TEvent, TTreeProvider>
    where TState : class
    where TEvent : class
    where TTreeProvider : WorkflowTreeProvider<TState, TEvent>
{
    IReadOnlyList<Type> HandlesEvents { get; }
    
    /// <summary>
    /// Executes workflow tree with given state initializer
    /// </summary>
    /// <param name="events"></param>
    /// <param name="cancellationToken"></param>
    public Task<WorkflowTreeExecutionResult<TState, TEvent>> ExecuteTree(IList<TEvent> events, CancellationToken cancellationToken);

    /// <summary>
    /// Used solely to recreate state based on provided events and state initializer
    /// </summary>
    /// <param name="events">Events for state recreation</param>
    /// <returns>Recreated state</returns>
    public TState RecreateState(IList<TEvent> events);
}