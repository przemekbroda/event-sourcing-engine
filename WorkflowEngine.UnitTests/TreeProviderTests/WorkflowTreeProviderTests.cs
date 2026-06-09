using EventSourcingEngine.Exceptions;
using EventSourcingEngine.UnitTests.TreeProviderTests.Nodes;
using EventSourcingEngine.UnitTests.TreeProviderTests.TestingTrees;

namespace EventSourcingEngine.UnitTests.TreeProviderTests;

public class WorkflowTreeProviderTests
{
    [Fact]
    public void TreeProviderConstructor_ShouldThrowEventSourcingEngineTreeValidationException_WhenNodesOnSameLevelHandleSameEvent()
    {
        // Act & Assert
        var exception = Assert.Throws<WorkflowEngineTreeValidationException>(() =>
            new SameEventsWorkflowTreeProvider());
        
        Assert.Equal($"Child node handles same event ({typeof(TreeEvent.Event8)}) as other node with the same parent node", exception.Message);
    }

    [Fact]
    public void TreeProviderConstructor_ShouldThrowEventSourcingEngineTreeValidationException_WhenNextExecutorHandlesEventThatIsNotProducedByParent()
    {
        // Act & Assert
        var exception = Assert.Throws<WorkflowEngineTreeValidationException>(() =>
            new NotValidProducedEventWorkflowTreeProvider());
        
        Assert.Equal($"Node with an executor {typeof(Node6)} handles event {typeof(TreeEvent.WeirdEvent)} that is not produced by parent node with an executor {typeof(Node4)} or by itself", exception.Message);
    }

    [Fact]
    public void TreeProviderConstructor_ShouldThrowEventSourcingEngineTreeValidationException_WhenHandlesEventsSetIsEmpty()
    {
        // Act & Assert
        var exception = Assert.Throws<WorkflowEngineTreeValidationException>(() =>
            new EmptyHandlesEventsWorkflowTreeProvider());
        
        Assert.Equal("Node must handle at least one event", exception.Message);
    }
    
    [Fact]
    public void TreeProviderConstructor_ShouldThrowEventSourcingEngineTreeValidationException_WhenProducesEventsSetIsEmpty()
    {
        // Act & Assert
        var exception = Assert.Throws<WorkflowEngineTreeValidationException>(() =>
            new EmptyProducesEventsWorkflowTreeProvider());
        
        Assert.Equal("Node must produce at least one event", exception.Message);
    }

    [Fact]
    public void TreeProviderConstructor_ShouldNotThrow_WhenTreeIsValid()
    {
        // Arrange
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>())).Returns(new object());
        
        // Act
        _ = new ValidWorkflowTreeProvider();
    }

    [Fact]
    public void TreeProviderConstructor_ShouldThrowWorkflowEngineTreeValidationException_WhenInitialNodeDoesntHandleInitialEvent()
    {
        var exception = Assert.Throws<WorkflowEngineTreeValidationException>(() => new NotInitialEventHandledWorkflowTreeProvider());
        
        Assert.Equal($"Initial Node must handle event {typeof(TreeEvent.Event1)}", exception.Message);
    }
    
    [Fact]
    public void TreeProviderConstructor_ShouldThrowWorkflowEngineTreeValidationException_WhenChildNodeHandlesSameEventAsParentNode()
    {
        var exception = Assert.Throws<WorkflowEngineTreeValidationException>(() => new SameEventsAsParentNodeWorkflowTreeProvider());
        
        Assert.Equal($"Child node handles same event ({typeof(TreeEvent.Event3)}) as parent node", exception.Message);
    }
}
