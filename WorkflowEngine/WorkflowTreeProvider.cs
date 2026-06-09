using EventSourcingEngine.Exceptions;

namespace EventSourcingEngine;

public abstract class WorkflowTreeProvider<TState, TEvent> 
    where TState : class
    where TEvent : class
{
    internal HashSet<Type> HandledEvents { get; } = [];
    public abstract Type InitialEvent { get; } 
    
    protected WorkflowTreeProvider()
    {
        ValidateTree();
    }

    public abstract EventNode<TState, TEvent> ProvideTree();
    public abstract TState InitializeState(TEvent @event);
    
    private void ValidateTree()
    {
        var eventNode = ProvideTree();

        if (!eventNode.HandlesEvents.Contains(InitialEvent))
        {
            throw new WorkflowEngineTreeValidationException($"Initial Node must handle event {InitialEvent}");
        }
        
        ValidateNodeType(eventNode);
    }
    
    private void ValidateNodeType(EventNode<TState, TEvent> eventNode)
    {
        if (eventNode.HandlesEvents.Count == 0)
        {
            throw new WorkflowEngineTreeValidationException("Node must handle at least one event");
        }

        if (eventNode.ProducesEvents.Count == 0)
        {
            throw new WorkflowEngineTreeValidationException("Node must produce at least one event");
        }

        CheckForDuplicatedHandledEventsInNextExecutor(eventNode);
        CheckNextExecutorsHandleProducedEvents(eventNode);
        
        HandledEvents.UnionWith(eventNode.HandlesEvents);
        
        foreach (var nextExecutor in eventNode.NextExecutors)
        {
            ValidateNodeType(nextExecutor);
        }
    }

    private static void CheckForDuplicatedHandledEventsInNextExecutor(EventNode<TState, TEvent> eventNode)
    {
        var childNodesHandledEvents = new HashSet<Type>();
        foreach (var handledEvent in eventNode.NextExecutors.Select(ne => ne.HandlesEvents).SelectMany(x => x))
        {
            if (!childNodesHandledEvents.Add(handledEvent))
            {
                throw new WorkflowEngineTreeValidationException($"Child node handles same event ({handledEvent}) as other node with the same parent node");
            }
        }

        foreach (var childNodeHandledEventType in childNodesHandledEvents)
        {
            if (eventNode.HandlesEvents.Contains(childNodeHandledEventType))
            {
                throw new WorkflowEngineTreeValidationException($"Child node handles same event ({childNodeHandledEventType}) as parent node");
            }
        }
    }
    

    private static void CheckNextExecutorsHandleProducedEvents(EventNode<TState, TEvent> parentNode)
    {
        var parentProducedEvents = parentNode.ProducesEvents;
        
        foreach (var childNode in parentNode.NextExecutors)
        {
            foreach (var childNodeHandledEvent in childNode.HandlesEvents)
            {
                if (!parentProducedEvents.Remove(childNodeHandledEvent) && !childNode.ProducesEvents.Contains(childNodeHandledEvent))
                {
                    throw new WorkflowEngineTreeValidationException($"Node with an executor {childNode.Executor} handles event {childNodeHandledEvent} that is not produced by parent node with an executor {parentNode.Executor} or by itself");
                }
            }
        }
    }
}