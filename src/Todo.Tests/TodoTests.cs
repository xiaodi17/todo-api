using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
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
}