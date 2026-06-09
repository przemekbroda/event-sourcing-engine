using EventSourcingEngine.UnitTests.TreeProviderTests.Nodes;

namespace EventSourcingEngine.UnitTests.TreeProviderTests.TestingTrees;

public class EmptyProducesEventsWorkflowTreeProvider : WorkflowTreeProvider<TreeState, TreeEvent>
{
    public override Type InitialEvent => typeof(TreeEvent.Event1);

    public override EventNode<TreeState, TreeEvent> ProvideTree()
    {
        return EventNode<TreeState, TreeEvent>.Create<Node1>(new EventNodeData<TreeState, TreeEvent>
        {
            HandlesEvents = [typeof(TreeEvent.Event1)],
            ProducesEvents = [typeof(TreeEvent.Event2), typeof(TreeEvent.Event3)],
            NextExecutors =
            [
                EventNode<TreeState, TreeEvent>.Create<Node2>(new EventNodeData<TreeState, TreeEvent>
                {
                    HandlesEvents = [typeof(TreeEvent.Event2)],
                    ProducesEvents = [typeof(TreeEvent.Event5)],
                    NextExecutors =
                    [
                        EventNode<TreeState, TreeEvent>.Create<Node3>(new EventNodeData<TreeState, TreeEvent>
                        {
                            HandlesEvents = [typeof(TreeEvent.Event5)],
                            ProducesEvents = [typeof(TreeEvent.Event6)],
                            NextExecutors = []
                        })
                    ]
                }),
                EventNode<TreeState, TreeEvent>.Create<Node4>(new EventNodeData<TreeState, TreeEvent>
                {
                    HandlesEvents = [typeof(TreeEvent.Event3)],
                    ProducesEvents = [],
                    NextExecutors =
                    [
                        EventNode<TreeState, TreeEvent>.Create<Node5>(new EventNodeData<TreeState, TreeEvent>
                        {
                            HandlesEvents = [typeof(TreeEvent.Event7), typeof(TreeEvent.Event8)],
                            ProducesEvents = [typeof(TreeEvent.Event10), typeof(TreeEvent.Event11)],
                            NextExecutors = []
                        }), 
                        EventNode<TreeState, TreeEvent>.Create<Node6>(new EventNodeData<TreeState, TreeEvent>
                        {
                            HandlesEvents = [typeof(TreeEvent.Event9)],
                            ProducesEvents = [typeof(TreeEvent.Event12)],
                            NextExecutors = []
                        })
                    ]
                }),
            ]
        });
    }
    
    public override TreeState InitializeState(TreeEvent @event)
    {
        return new TreeState();
    }
}