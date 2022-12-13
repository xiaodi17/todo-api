using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Todo.Api.Features.Todo.Models;
using Todo.Tests.Infrastructure;
using Xunit;

namespace Todo.Tests;

public class TodoTests
{
    [Fact]
    public async Task GivenTodoItemDoesNotExist_ThenReturnNotFoundException()
    {
        await using var testContext = new TestContext();
        await testContext.Setup();

        var response = await testContext.Client.GetAsync($"/todo/todoitems/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GivenAddTodoItem_ThenItemShouldBeSavedInDb()
    {
        await using var testContext = new TestContext();
        await testContext.Setup();

        var request = new CreateTodoItemRequest
        {
            Name = "Play",
            IsComplete = true
        };
        var response = await testContext.Client.PostAsJsonAsync($"/todoitems", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}