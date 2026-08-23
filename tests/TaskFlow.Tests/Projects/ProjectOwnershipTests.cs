using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Projects;

public class ProjectOwnershipTests
{
    [Fact]
    public async Task User_ShouldOnlySeeOwnProjects()
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

        dbContext.Users.AddRange(userA, userB);

        dbContext.Projects.AddRange(
            new Project
            {
                Name = "User A Project",
                UserId = userA.Id
            },
            new Project
            {
                Name = "User B Project",
                UserId = userB.Id
            });

        await dbContext.SaveChangesAsync();

        var projects = await dbContext.Projects
            .Where(x => x.UserId == userA.Id)
            .ToListAsync();

        projects.Should().HaveCount(1);
        projects.Single().Name.Should().Be("User A Project");
    }
}
