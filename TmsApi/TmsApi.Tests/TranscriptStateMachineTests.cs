using TmsApi.Application.Transcripts;
using TmsApi.Infrastructure.Transcripts;
using Xunit;

namespace TmsApi.Tests;

public class TranscriptStateMachineTests
{
    [Fact]
    public async Task ValidTransitions_ProgressFromQueuedToProcessingToReady()
    {
        var store = new InMemoryTranscriptStatusStore();
        var reportId = "rep-valid-001";
        var studentId = 42;

        // 1. Create -> Queued
        var status = await store.CreateAsync(reportId, studentId, default);
        Assert.Equal(TranscriptState.Queued, status.State);
        Assert.Null(status.StartedAt);
        Assert.Null(status.CompletedAt);

        // 2. MarkProcessing -> Processing
        await store.MarkProcessingAsync(reportId, default);
        var processing = await store.GetAsync(reportId, default);
        Assert.NotNull(processing);
        Assert.Equal(TranscriptState.Processing, processing.State);
        Assert.NotNull(processing.StartedAt);
        Assert.Null(processing.CompletedAt);

        // 3. MarkReady -> Ready
        var downloadUrl = $"/api/v2/transcripts/{reportId}/download";
        await store.MarkReadyAsync(reportId, downloadUrl, default);
        var ready = await store.GetAsync(reportId, default);
        Assert.NotNull(ready);
        Assert.Equal(TranscriptState.Ready, ready.State);
        Assert.NotNull(ready.CompletedAt);
        Assert.Equal(downloadUrl, ready.DownloadUrl);
    }

    [Fact]
    public async Task IllegalTransition_FromReadyBackToProcessing_ThrowsInvalidOperationException()
    {
        var store = new InMemoryTranscriptStatusStore();
        var reportId = "rep-illegal-001";

        await store.CreateAsync(reportId, 1, default);
        await store.MarkProcessingAsync(reportId, default);
        await store.MarkReadyAsync(reportId, "/download", default);

        // Sanity check: try forcing transition from Ready back to Processing (or Queued)
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            store.MarkProcessingAsync(reportId, default));

        Assert.Contains("Cannot move", ex.Message);
        Assert.Contains("from Ready", ex.Message);
    }

    [Fact]
    public async Task IdempotencyKey_LinksAndResolvesReportId()
    {
        var store = new InMemoryTranscriptStatusStore();
        var idempotencyKey = Guid.NewGuid().ToString();
        var reportId = "rep-idemp-001";

        await store.CreateAsync(reportId, 1, default);
        await store.LinkIdempotencyKeyAsync(idempotencyKey, reportId, default);

        var resolvedId = await store.GetReportIdForIdempotencyKeyAsync(idempotencyKey, default);
        Assert.Equal(reportId, resolvedId);

        var unknownKey = await store.GetReportIdForIdempotencyKeyAsync("unknown-key", default);
        Assert.Null(unknownKey);
    }
}
