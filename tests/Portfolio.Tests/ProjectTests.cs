using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Portfolio.Application.Interfaces.Repositories;
using Portfolio.Application.Features.Projects;
using Portfolio.Application.Features.Projects.DTOs;
using Portfolio.Application.Features.Projects.Commands;
using Portfolio.Domain.Entities;
using Xunit;

namespace Portfolio.Tests;

public class ProjectTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Project>> _projectRepoMock;
    private readonly Mock<IMapper> _mapperMock;

    public ProjectTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _projectRepoMock = new Mock<IRepository<Project>>();
        _mapperMock = new Mock<IMapper>();

        _unitOfWorkMock.Setup(u => u.Repository<Project>()).Returns(_projectRepoMock.Object);
    }

    [Fact]
    public async Task CreateProject_ShouldAddProjectAndSaveChanges()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Title = "Test Project",
            Description = "Test Description",
            Technologies = "C#, Angular",
            DisplayOrder = 1,
            IsFeatured = true
        };

        var expectedDto = new ProjectDto
        {
            Title = command.Title,
            Description = command.Description,
            Technologies = command.Technologies,
            DisplayOrder = command.DisplayOrder,
            IsFeatured = command.IsFeatured
        };

        _mapperMock.Setup(m => m.Map<ProjectDto>(It.IsAny<Project>())).Returns(expectedDto);

        var handler = new CreateProjectCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Title, result.Title);
        
        _projectRepoMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
