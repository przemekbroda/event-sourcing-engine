using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ExampleApp.Postgres.Trees.FirstTree;

namespace ExampleApp.Postgres.Models;

public class ProcessRequestEvent
{
    public Guid Id { get; set; }
    [Column(TypeName = "VARCHAR")]
    [StringLength(100)]
    public required string EventName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Index { get; set; }
    [Column(TypeName = "jsonb")] 
    public required ProcessRequestEventPayload ProcessRequestEventPayload { get; set; }

    public ProcessRequest ProcessRequest { get; set; } = null!;
    public Guid ProcessRequestId { get; set; }

    public FirstTreeEvent ToTreeEvent()
    {
        return ProcessRequestEventPayload switch
        {
            AwaitingExecution awaitingExecution => new FirstTreeEvent.AwaitingExecution(awaitingExecution.Balance, Index, ProcessRequestId),
            AwaitingResult => new FirstTreeEvent.AwaitingResult(Index),
            ResultFetched resultFetched => new FirstTreeEvent.ResultFetched(resultFetched.Amount, Index),
            ResultSaved => new FirstTreeEvent.ResultSaved(Index),
            ResultSaveError => new FirstTreeEvent.ResultSaveError(Index),
            _ => throw new ArgumentOutOfRangeException(nameof(ProcessRequestEventPayload))
        };
    }

    public static ProcessRequestEvent FromTreeEvent(FirstTreeEvent e, Guid processRequestId, DateTime createdAt)
    {
        return new ProcessRequestEvent
        {
            EventName = e.GetType().Name,
            ProcessRequestEventPayload = e switch
            {
                FirstTreeEvent.AwaitingExecution execution => new AwaitingExecution(execution.Balance),
                FirstTreeEvent.ResultFetched result => new ResultFetched(result.Amount),
                FirstTreeEvent.ResultSaveError => new ResultSaveError(),
                FirstTreeEvent.ResultSaved => new ResultSaved(),
                FirstTreeEvent.AwaitingResult => new AwaitingResult(),
                _ => throw new ArgumentOutOfRangeException(nameof(e))
            },
            CreatedAt = createdAt,
            Index = e.Index,
            ProcessRequestId = processRequestId
        };
    }
}

[JsonDerivedType(typeof(AwaitingExecution), nameof(AwaitingExecution))]
[JsonDerivedType(typeof(ResultFetched), nameof(ResultFetched))]
[JsonDerivedType(typeof(AwaitingResult), nameof(AwaitingResult))]
[JsonDerivedType(typeof(ResultSaveError), nameof(ResultSaveError))]
[JsonDerivedType(typeof(ResultSaved), nameof(ResultSaved))]
public abstract record ProcessRequestEventPayload;
public record AwaitingExecution(int Balance) : ProcessRequestEventPayload;
public record ResultFetched(int Amount) : ProcessRequestEventPayload;
public record AwaitingResult : ProcessRequestEventPayload;
public record ResultSaveError : ProcessRequestEventPayload;
public record ResultSaved : ProcessRequestEventPayload;