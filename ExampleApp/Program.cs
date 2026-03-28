using EventSourcingEngine;
using ExampleApp.Trees.FirstTree;
using ExampleApp.Trees.FirstTree.Nodes;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterWorkflowTree<TestState, FirstTreeEvent, FirstWorkflowTreeProvider>();
builder.Services.AddTransient<EventExecutorNode>();
builder.Services.AddTransient<ResultSaverNode>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/execute-tree",  async ([FromServices] IWorkflowTreeExecutor<TestState, FirstTreeEvent, FirstWorkflowTreeProvider> firstWorkflowTreeExecutor, CancellationToken cancellationToken) =>
    {
        List<FirstTreeEvent> events =
        [
            // new FirstTreeEvent.ResultSaveError(),
            new FirstTreeEvent.AwaitingExecution(300),
            // new FirstTreeEvent.ResultSaveError(),
            new FirstTreeEvent.AwaitingResult(1),
            new FirstTreeEvent.AwaitingResult(2),
            new FirstTreeEvent.AwaitingResult(3),
            // new FirstTreeEvent.ResultFetched(600),
        ];
        // List<Event> events =
        // [
        //     new("AwaitingExecution", 300),
        //     new("AwaitingResult", 1),
        //     new("AwaitingResult", 2),
        //     new("AwaitingResult", 3),
        // ];
        //
        // List<Event> events =
        // [
        //     new("AwaitingExecution", 300),
        //     new("ResultFetched", 600),
        // ];
        //
        // List<Event> events =
        // [
        //     new("AwaitingExecution", 300),
        // ];
        // List<Event> events =
        // [
        //     new("AwaitingExecution", 300),
        //     new("AwaitingResult", 1),
        //     new("AwaitingResult", 2),
        //     new("AwaitingResult", 3),
        //     new("ResultFetched", 600),
        //     new("ResultSaveError", 600),
        // ];
        // List<Event> events =
        // [
        //     new("AwaitingExecution", 300),
        //     new("AwaitingResult", 1),
        //     new("AwaitingResult", 2),
        //     new("AwaitingResult", 3),
        //     new("ResultFetched", 600),
        //     new("ResultSaveError", 600),
        //     new("ResultSaved", 600),
        // ];

        events.Reverse();
        
        var finishedWithEvent = await firstWorkflowTreeExecutor.ExecuteTree(events, cancellationToken);
        
        Console.WriteLine($"Finished with event {finishedWithEvent}");
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();