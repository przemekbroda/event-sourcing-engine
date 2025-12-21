namespace EventSourcingEngine.Exceptions;

public class EventSourcingTreeValidationException(string message) : Exception(message);