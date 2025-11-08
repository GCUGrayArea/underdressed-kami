using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Infrastructure.Persistence;
using SmartScheduler.WebApi.Models.Requests;
using SmartScheduler.WebApi.Models.Responses;
using LocationDto = SmartScheduler.Application.DTOs.LocationDto;

namespace SmartScheduler.IntegrationTests;

/// <summary>
/// Integration tests for the full recommendation and assignment workflow.
/// Tests the complete flow from API to database with real PostgreSQL.
/// </summary>
public class RecommendationFlowTests : IClassFixture<TestFixture>, IAsyncLifetime
{
    private readonly TestFixture _fixture;
    private readonly HttpClient _client;
    private ApplicationDbContext _context = null!;
    private TestDataSeeder _seeder = null!;

    public RecommendationFlowTests(TestFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeDatabaseAsync();
        await _fixture.SeedJobTypesAsync();

        var scope = _fixture.Services.CreateScope();
        _context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        _seeder = new TestDataSeeder(_context);
    }

    public async Task DisposeAsync()
    {
        await _seeder.ClearAllDataAsync();
        await _fixture.CleanupDatabaseAsync();
    }

    [Fact]
    public async Task RecommendationFlow_WithValidJob_ReturnsRankedContractors()
    {
        // Arrange - Create 3 contractors with different characteristics
        var jobTypeId = await _seeder.GetTileInstallerJobTypeIdAsync();

        // Contractor 1: Close (NYC), high rating, available
        var contractor1 = await _seeder.CreateContractorAsync(
            jobTypeId: jobTypeId,
            name: "John Smith",
            latitude: 40.7128, // NYC
            longitude: -74.0060,
            rating: 4.8,
            schedule: _seeder.CreateWeekdaySchedule());

        // Contractor 2: Far (LA), medium rating, available
        var contractor2 = await _seeder.CreateContractorAsync(
            jobTypeId: jobTypeId,
            name: "Jane Doe",
            latitude: 34.0522, // LA
            longitude: -118.2437,
            rating: 4.0,
            schedule: _seeder.CreateWeekdaySchedule());

        // Contractor 3: Medium distance (Philadelphia), high rating, available
        var contractor3 = await _seeder.CreateContractorAsync(
            jobTypeId: jobTypeId,
            name: "Bob Johnson",
            latitude: 39.9526, // Philadelphia
            longitude: -75.1652,
            rating: 4.9,
            schedule: _seeder.CreateWeekdaySchedule());

        // Create recommendation request for NYC location
        var request = new ContractorRecommendationRequest
        {
            JobTypeId = jobTypeId,
            DesiredDate = new DateOnly(2025, 1, 20), // Monday
            DesiredTime = new TimeOnly(10, 0),
            Location = new LocationDto
            {
                Latitude = 40.7589, // Times Square, NYC
                Longitude = -73.9851,
                Address = "Times Square, NYC"
            },
            EstimatedDurationHours = 4.0,
            TopN = 5
        };

        // Act - Call recommendation endpoint
        var response = await _client.PostAsJsonAsync(
            "/api/recommendations/contractors",
            request);

        // Assert - Verify response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var recommendations = await response.Content
            .ReadFromJsonAsync<List<ContractorRecommendationResponse>>();

        recommendations.Should().NotBeNull();
        recommendations!.Should().HaveCountGreaterOrEqualTo(2);

        // Top contractor should be either John Smith (closest) or Bob Johnson
        // (close and highest rating)
        var topContractor = recommendations.First();
        topContractor.Name.Should().BeOneOf("John Smith", "Bob Johnson");

        // Verify score breakdown is present
        topContractor.ScoreBreakdown.Should().NotBeNull();
        topContractor.ScoreBreakdown.AvailabilityScore.Should().BeGreaterThan(0);
        topContractor.ScoreBreakdown.RatingScore.Should().BeGreaterThan(0);
        topContractor.ScoreBreakdown.DistanceScore.Should().BeGreaterThan(0);

        // Verify best available slot is present
        topContractor.BestAvailableSlot.Should().NotBeNull();
    }

    [Fact]
    public async Task AssignmentFlow_WithTopContractor_UpdatesJobStatus()
    {
        // Arrange - Create contractor and job
        var jobTypeId = await _seeder.GetTileInstallerJobTypeIdAsync();

        var contractor = await _seeder.CreateContractorAsync(
            jobTypeId: jobTypeId,
            name: "Test Contractor",
            latitude: 40.7128,
            longitude: -74.0060,
            rating: 4.5,
            schedule: _seeder.CreateWeekdaySchedule());

        var job = await _seeder.CreateJobAsync(
            jobTypeId: jobTypeId,
            latitude: 40.7589,
            longitude: -73.9851,
            desiredDate: new DateOnly(2025, 1, 20),
            desiredTime: new TimeOnly(10, 0),
            estimatedDurationHours: 4.0);

        // Act - Assign contractor to job
        var assignRequest = new AssignContractorRequest
        {
            ContractorId = contractor.Id,
            ScheduledStartTime = DateTime.SpecifyKind(
                new DateTime(2025, 1, 20, 10, 0, 0),
                DateTimeKind.Utc)
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/jobs/{job.Id}/assign",
            assignRequest);

        // Assert - Verify assignment succeeded
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify job status updated in database (create new scope to avoid caching)
        using var verifyScope = _fixture.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var updatedJob = await verifyContext.Jobs.FindAsync(job.Id);
        updatedJob.Should().NotBeNull();
        updatedJob!.Status.Should().Be(JobStatus.Assigned);
        updatedJob.AssignedContractorId.Should().Be(contractor.Id);
        updatedJob.ScheduledStartTime.Should().Be(DateTime.SpecifyKind(
            new DateTime(2025, 1, 20, 10, 0, 0),
            DateTimeKind.Utc));
    }

    [Fact]
    public async Task AssignmentFlow_UpdatesContractorAvailability()
    {
        // Arrange - Create contractor with schedule
        var jobTypeId = await _seeder.GetTileInstallerJobTypeIdAsync();

        var contractor = await _seeder.CreateContractorAsync(
            jobTypeId: jobTypeId,
            name: "Busy Contractor",
            latitude: 40.7128,
            longitude: -74.0060,
            rating: 4.5,
            schedule: _seeder.CreateCustomSchedule(
                DayOfWeek.Monday,
                startHour: 9,
                endHour: 17));

        // Create and assign first job (9 AM - 1 PM)
        var job1 = await _seeder.CreateJobAsync(
            jobTypeId: jobTypeId,
            latitude: 40.7589,
            longitude: -73.9851,
            desiredDate: new DateOnly(2025, 1, 19), // Monday
            desiredTime: new TimeOnly(9, 0),
            estimatedDurationHours: 4.0);

        await _seeder.AssignJobAsync(
            job1,
            contractor.Id,
            DateTime.SpecifyKind(
                new DateTime(2025, 1, 19, 9, 0, 0),
                DateTimeKind.Utc));

        // Act - Request recommendations for another job at same time
        var request = new ContractorRecommendationRequest
        {
            JobTypeId = jobTypeId,
            DesiredDate = new DateOnly(2025, 1, 19), // Same Monday
            DesiredTime = new TimeOnly(10, 0), // Overlaps with first job
            Location = new LocationDto
            {
                Latitude = 40.7589,
                Longitude = -73.9851,
                Address = "NYC"
            },
            EstimatedDurationHours = 2.0,
            TopN = 5
        };

        var response = await _client.PostAsJsonAsync(
            "/api/recommendations/contractors",
            request);

        // Assert - Contractor should not be recommended or have reduced slots
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var recommendations = await response.Content
            .ReadFromJsonAsync<List<ContractorRecommendationResponse>>();

        recommendations.Should().NotBeNull();

        // If contractor appears in recommendations, their best slot should be
        // in the afternoon (after 1 PM when first job ends)
        var contractorRec = recommendations!
            .FirstOrDefault(r => r.ContractorId == contractor.Id);

        if (contractorRec != null && contractorRec.BestAvailableSlot != null)
        {
            contractorRec.BestAvailableSlot.Start.Should()
                .BeOnOrAfter(new TimeOnly(13, 0));
        }
    }

    [Fact]
    public async Task RecommendationFlow_WithNoAvailableContractors_ReturnsEmpty()
    {
        // Arrange - Create contractor not working on target day
        var jobTypeId = await _seeder.GetTileInstallerJobTypeIdAsync();

        await _seeder.CreateContractorAsync(
            jobTypeId: jobTypeId,
            name: "Weekend Worker",
            latitude: 40.7128,
            longitude: -74.0060,
            rating: 4.5,
            schedule: _seeder.CreateCustomSchedule(
                DayOfWeek.Saturday, // Only works Saturday
                startHour: 9,
                endHour: 17));

        // Request recommendations for Monday
        var request = new ContractorRecommendationRequest
        {
            JobTypeId = jobTypeId,
            DesiredDate = new DateOnly(2025, 1, 20), // Monday
            DesiredTime = new TimeOnly(10, 0),
            Location = new LocationDto
            {
                Latitude = 40.7589,
                Longitude = -73.9851,
                Address = "NYC"
            },
            EstimatedDurationHours = 4.0,
            TopN = 5
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/recommendations/contractors",
            request);

        // Assert - Should return empty array (not error)
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var recommendations = await response.Content
            .ReadFromJsonAsync<List<ContractorRecommendationResponse>>();

        recommendations.Should().NotBeNull();
        recommendations!.Should().BeEmpty();
    }

    [Fact]
    public async Task RecommendationFlow_RankingByOverallScore_IsCorrect()
    {
        // Arrange - Create 3 contractors with clear ranking differences
        var jobTypeId = await _seeder.GetTileInstallerJobTypeIdAsync();

        // Perfect contractor: close, high rating, available at exact time
        var perfect = await _seeder.CreateContractorAsync(
            jobTypeId: jobTypeId,
            name: "Perfect Contractor",
            latitude: 40.7589, // Exact job location
            longitude: -73.9851,
            rating: 5.0,
            schedule: _seeder.CreateWeekdaySchedule());

        // Good contractor: medium distance, good rating
        var good = await _seeder.CreateContractorAsync(
            jobTypeId: jobTypeId,
            name: "Good Contractor",
            latitude: 40.7128,
            longitude: -74.0060,
            rating: 4.0,
            schedule: _seeder.CreateWeekdaySchedule());

        // Poor contractor: far, low rating
        var poor = await _seeder.CreateContractorAsync(
            jobTypeId: jobTypeId,
            name: "Poor Contractor",
            latitude: 39.9526, // Philadelphia
            longitude: -75.1652,
            rating: 3.0,
            schedule: _seeder.CreateWeekdaySchedule());

        // Request recommendations
        var request = new ContractorRecommendationRequest
        {
            JobTypeId = jobTypeId,
            DesiredDate = new DateOnly(2025, 1, 20),
            DesiredTime = new TimeOnly(10, 0),
            Location = new LocationDto
            {
                Latitude = 40.7589,
                Longitude = -73.9851,
                Address = "NYC"
            },
            EstimatedDurationHours = 4.0,
            TopN = 5
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/recommendations/contractors",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var recommendations = await response.Content
            .ReadFromJsonAsync<List<ContractorRecommendationResponse>>();

        recommendations.Should().NotBeNull();
        recommendations!.Should().HaveCountGreaterOrEqualTo(3);

        // Verify ranking order: Perfect > Good > Poor
        recommendations[0].Name.Should().Be("Perfect Contractor");
        recommendations[0].ScoreBreakdown.OverallScore.Should().BeGreaterThan(
            recommendations[1].ScoreBreakdown.OverallScore);

        if (recommendations.Count >= 3)
        {
            recommendations[1].ScoreBreakdown.OverallScore.Should().BeGreaterThan(
                recommendations[2].ScoreBreakdown.OverallScore);
        }
    }
}
