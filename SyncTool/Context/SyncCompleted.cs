namespace SyncTool.Context;

record SyncCompletedRequest(string ClientName, ulong CurrentVersion);
record SyncCompletedResponse();
