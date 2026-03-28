using EventSourcingEngine;
using ExampleApp.Trees.FirstTree.Nodes;

namespace ExampleApp.Trees.FirstTree;

public class FirstWorkflowTreeProvider : WorkflowTreeProvider<TestState, FirstTreeEvent>
{
    public override Type InitialEvent => typeof(FirstTreeEvent.AwaitingExecution);

    public override EventNode<TestState, FirstTreeEvent> ProvideTree()
    {
        return new EventNode<TestState, FirstTreeEvent>
        {
            HandlesEvents = [
                typeof(FirstTreeEvent.AwaitingExecution),
                typeof(FirstTreeEvent.AwaitingResult)
            ],
            Executor = typeof(EventExecutorNode),
            ProducesEvents = [
                typeof(FirstTreeEvent.AwaitingResult),
                typeof(FirstTreeEvent.ResultFetched)
            ],
            NextExecutors = [
                new EventNode<TestState, FirstTreeEvent>
                {
                    HandlesEvents = [
                        typeof(FirstTreeEvent.ResultFetched),
                        typeof(FirstTreeEvent.ResultSaveError)
                    ],
                    Executor = typeof(ResultSaverNode),
                    ProducesEvents = [
                        typeof(FirstTreeEvent.ResultSaved),
                        typeof(FirstTreeEvent.ResultSaveError),
                    ],
                    NextExecutors = []
                }
            ]
        };
    }

    public override TestState InitializeState(FirstTreeEvent @event)
    {
        return new TestState
        {
            Balance = 500
        };
    }
}