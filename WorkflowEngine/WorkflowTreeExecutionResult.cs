namespace EventSourcingEngine;

public record WorkflowTreeExecutionResult<TState, TEvent>(TState ProducedState, TEvent ProducedEvent) 
    where TState : class 
    where TEvent : class;