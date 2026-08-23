using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Tasks;

public class TaskOwnershipTests
{
    [Fact]
    public async Task User_ShouldOnlyAccessTasksInsideOwnProjects()
    {
        await using var dbContext = TestDbContextFactory.Create();

        var userA = new User
        {
            FirstName = "User",
            LastName = "A",
            Email = "a@test.com",
            PasswordHash = "hash"
        };

        var userB = new User
        {
            FirstName = "User",
            LastName = "B",
            Email = "b@test.com",
            PasswordHash = "hash"
        };

        var projectA = new Project
        {
            Name = "Project A",
            UserId = userA.Id,
            User = userA
        };

        var projectB = new Project
        {
            Name = "Project B",
            UserId = userB.Id,
            User = userB
        };

        dbContext.Users.AddRange(userA, userB);
        dbContext.Projects.AddRange(projectA, projectB);

        dbContext.TaskItems.AddRange(
            new TaskFlow.Domain.Entities.TaskItem
            {
                Title = "User A Task",
                Project = projectA
            },
            new TaskFlow.Domain.Entities.TaskItem
            {
                Title = "User B Task",
                Project = projectB
            });

        await dbContext.SaveChangesAsync();

        var tasks = await dbContext.TaskItems
            .Where(x => x.Project.UserId == userA.Id)
            .ToListAsync();

        tasks.Should().HaveCount(1);
        tasks.Single().Title.Should().Be("User A Task");
    }
}
