using EventSourcingEngine;
using ExampleApp.Trees.FirstTree.Nodes;

namespace ExampleApp.Trees.FirstTree;

public class FirstWorkflowTreeProvider : WorkflowTreeProvider<TestState, FirstTreeEvent>
{
    public override Type InitialEvent => typeof(FirstTreeEvent.AwaitingExecution);

    public override EventNode<TestState, FirstTreeEvent> ProvideTree()
    {
        return EventNode<TestState, FirstTreeEvent>.Create<EventExecutorNode>(new EventNodeData<TestState, FirstTreeEvent>
        {
            HandlesEvents = [
                typeof(FirstTreeEvent.AwaitingExecution),
                typeof(FirstTreeEvent.AwaitingResult)
            ],
            ProducesEvents = [
                typeof(FirstTreeEvent.AwaitingResult),
                typeof(FirstTreeEvent.ResultFetched)
            ],
            NextExecutors = [
                EventNode<TestState, FirstTreeEvent>.Create<ResultSaverNode>(new EventNodeData<TestState, FirstTreeEvent>
                {
                    HandlesEvents = [
                        typeof(FirstTreeEvent.ResultFetched),
                        typeof(FirstTreeEvent.ResultSaveError)
                    ],
                    ProducesEvents = [
                        typeof(FirstTreeEvent.ResultSaved),
                        typeof(FirstTreeEvent.ResultSaveError),
                    ],
                    NextExecutors = []
                })
            ]
        });
    }

    public override TestState InitializeState(FirstTreeEvent @event)
    {
        return new TestState
        {
            Balance = 500
        };
    }
}