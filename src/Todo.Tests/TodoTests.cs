using System;
using System.Collections.Generic;
using System.Linq;
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
    public async Task GivenTodoItemDoesExist_ThenReturnTodoItem()
    {
        // Arrange
        await using var testContext = new TestContext();
        var request = new CreateTodoItemRequest
        {
            Name = "Play",
            IsComplete = true
        };
        
        await testContext.Client.PostAsJsonAsync($"/todoitems", request);
        
        // Act
        var response = await testContext.Client.GetAsync($"/todoitems");
        var todoItems = await response.Content.ReadAsAsync<IEnumerable<TodoItem>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todoItems.Should().HaveCount(1);
        var todoItem = todoItems.Single();
        todoItem.Name.Should().Be("Play");
        todoItem.IsComplete.Should().BeTrue();
    }
    
    [Fact]
    public async Task GivenAddTodoItem_ThenItemShouldBeSavedInDb()
    {
        // Arrange
        await using var testContext = new TestContext();

        // Act
        var request = new CreateTodoItemRequest
        {
            Name = "Play",
            IsComplete = true
        };
        var response = await testContext.Client.PostAsJsonAsync($"/todoitems", request);
        var todoItem = await response.Content.ReadAsAsync<TodoItem>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        todoItem.Name.Should().Be("Play");
        todoItem.IsComplete.Should().BeTrue();
    }
}