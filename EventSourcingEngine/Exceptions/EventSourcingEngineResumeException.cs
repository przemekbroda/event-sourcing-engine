namespace EventSourcingEngine.Exceptions;

public class EventSourcingEngineResumeException(string message) : Exception(message);