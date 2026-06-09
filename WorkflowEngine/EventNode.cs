namespace EventSourcingEngine;

public record EventNode<TState, TEvent>
    where TState : class
    where TEvent : class
{
    public required HashSet<Type> HandlesEvents { get; init; }
    public required Type Executor { get; init; }
    public required HashSet<Type> ProducesEvents { get; init; }
    public required List<EventNode<TState, TEvent>> NextExecutors { get; init; }
    
    private EventNode() { }
    
    public static EventNode<TState, TEvent> Create<TExecutor>(
        HashSet<Type> handlesEvents,
        HashSet<Type> producesEvents,
        List<EventNode<TState, TEvent>> nextExecutors)
        where TExecutor : class, INodeExecutor<TState, TEvent>
    {
        return new EventNode<TState, TEvent>
        {
            Executor = typeof(TExecutor),
            HandlesEvents = handlesEvents,
            ProducesEvents = producesEvents,
            NextExecutors = nextExecutors
        };
    }
    
    public static EventNode<TState, TEvent> Create<TExecutor>(EventNodeData<TState, TEvent> data)
        where TExecutor : class, INodeExecutor<TState, TEvent>
    {
        return Create<TExecutor>(
            handlesEvents: data.HandlesEvents,
            producesEvents: data.ProducesEvents,
            nextExecutors: data.NextExecutors
        );
    }
}

public record EventNodeData<TState, TEvent>
    where TState : class
    where TEvent : class
{
    public required HashSet<Type> HandlesEvents { get; init; }
    public required HashSet<Type> ProducesEvents { get; init; }
    public required List<EventNode<TState, TEvent>> NextExecutors { get; init; }

}

internal record EventNodeInst<TState, TEvent>(INodeExecutor<TState, TEvent> Executor, List<EventNodeInst<TState, TEvent>> NextExecutors) 
    where TState : class
    where TEvent : class;