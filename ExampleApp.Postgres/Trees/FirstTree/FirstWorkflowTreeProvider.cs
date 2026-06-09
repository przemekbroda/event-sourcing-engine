using EventSourcingEngine;
using ExampleApp.Postgres.Trees.FirstTree.Nodes;

namespace ExampleApp.Postgres.Trees.FirstTree;

public class FirstWorkflowTreeProvider : WorkflowTreeProvider<TestState, FirstTreeEvent>
{
    public override Type InitialEvent => typeof(FirstTreeEvent.AwaitingExecution);

    public override EventNode<TestState, FirstTreeEvent> ProvideTree()
    {
        return EventNode<TestState, FirstTreeEvent>.Create<EventExecutorNode>(new EventNodeData<TestState, FirstTreeEvent>()
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
        if (@event is FirstTreeEvent.AwaitingExecution execution )
        {
            return new TestState
            {
                Balance = execution.Balance,
                AwaitingResult = false,
                ProcessRequestId = execution.ProcessRequestId
            };        
        }

        throw new Exception();
    }
}