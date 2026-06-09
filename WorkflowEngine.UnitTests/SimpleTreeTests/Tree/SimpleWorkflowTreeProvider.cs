using EventSourcingEngine.UnitTests.SimpleTreeTests.Nodes;

namespace EventSourcingEngine.UnitTests.SimpleTreeTests.Tree;

public class SimpleWorkflowTreeProvider : WorkflowTreeProvider<SimpleTreeState, SimpleTreeEvent>
{
    public override Type InitialEvent => typeof(SimpleTreeEvent.AwaitingExecution);

    public override EventNode<SimpleTreeState, SimpleTreeEvent> ProvideTree()
    {
        return EventNode<SimpleTreeState, SimpleTreeEvent>.Create<FirstEventExecutorNode>(new EventNodeData<SimpleTreeState, SimpleTreeEvent>
        {
            HandlesEvents = [
                typeof(SimpleTreeEvent.AwaitingExecution),
                typeof(SimpleTreeEvent.AwaitingResult)
            ],
            ProducesEvents = [
                typeof(SimpleTreeEvent.AwaitingResult),
                typeof(SimpleTreeEvent.ResultFetched)
            ],
            NextExecutors = [
                EventNode<SimpleTreeState, SimpleTreeEvent>.Create<ResultSaverNode>(new EventNodeData<SimpleTreeState, SimpleTreeEvent>
                {
                    HandlesEvents = [
                        typeof(SimpleTreeEvent.ResultFetched),
                        typeof(SimpleTreeEvent.ResultSaveError)
                    ],
                    ProducesEvents = [
                        typeof(SimpleTreeEvent.ResultSaved),
                        typeof(SimpleTreeEvent.ResultSaveError),
                    ],
                    NextExecutors = []
                })
            ]
        });
    }

    public override SimpleTreeState InitializeState(SimpleTreeEvent @event)
    {
        if (@event is SimpleTreeEvent.AwaitingExecution e)
        {
            return new SimpleTreeState
            {
                Balance = e.Balance
            };
        }
        
        throw new Exception();
    }
}