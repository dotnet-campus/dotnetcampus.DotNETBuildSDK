#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class GitLabHelper
    {
        public static bool IsGitlabCI
            => string.Equals(Environment.GetEnvironmentVariable("GITLAB_CI"), "true",
                StringComparison.OrdinalIgnoreCase);

        public static string? ProjectNamespace => Environment.GetEnvironmentVariable("CI_PROJECT_NAMESPACE");

        public static string? GitLabRunnerVersion => Environment.GetEnvironmentVariable("CI_RUNNER_VERSION");

        public static string? ServerName => Environment.GetEnvironmentVariable("CI_SERVER_NAME");

        public static string? GitLabRunnerDescription => Environment.GetEnvironmentVariable("CI_RUNNER_DESCRIPTION");

        /// <summary>
        /// 触发流水的用户的邮箱。这不是 commit 的用户的邮箱
        /// </summary>
        public static string? ExecutiveUserEmail => Environment.GetEnvironmentVariable("GITLAB_USER_EMAIL");

        public static string? GitLabServerRevision => Environment.GetEnvironmentVariable("CI_SERVER_REVISION");

        public static string? GitLabRunnerExecutableArch =>
            Environment.GetEnvironmentVariable("CI_RUNNER_EXECUTABLE_ARCH");

        public static string? PipelineName => Environment.GetEnvironmentVariable("CI_PIPELINE_NAME");

        public static string? RegistryUser => Environment.GetEnvironmentVariable("CI_REGISTRY_USER");

        public static string? GitLabApiV4Url => Environment.GetEnvironmentVariable("CI_API_V4_URL");

        public static string? GitLabRunnerShortToken => Environment.GetEnvironmentVariable("CI_RUNNER_SHORT_TOKEN");

        public static string? GitLabCIJobName => Environment.GetEnvironmentVariable("CI_JOB_NAME");

        public static string? GitLabCIOpenMergeRequests => Environment.GetEnvironmentVariable("CI_OPEN_MERGE_REQUESTS");

        public static string? GitLabUserLogin => Environment.GetEnvironmentVariable("GITLAB_USER_LOGIN");

        public static string? GitLabCIProjectName => Environment.GetEnvironmentVariable("CI_PROJECT_NAME");

        public static string? GitLabCIPipelineSource => Environment.GetEnvironmentVariable("CI_PIPELINE_SOURCE");

        public static string? GitLabCIJobStatus => Environment.GetEnvironmentVariable("CI_JOB_STATUS");

        public static string? CommitRefSlug => Environment.GetEnvironmentVariable("CI_COMMIT_REF_SLUG");

        public static string? GitLabCIServer => Environment.GetEnvironmentVariable("CI_SERVER");

        public static string? CommitShortSHA => Environment.GetEnvironmentVariable("CI_COMMIT_SHORT_SHA");

        public static string? GitLabCIJobNameSlug => Environment.GetEnvironmentVariable("CI_JOB_NAME_SLUG");

        public static string? GitLabCIDependencyProxyGroupImagePrefix =>
            Environment.GetEnvironmentVariable("CI_DEPENDENCY_PROXY_GROUP_IMAGE_PREFIX");

        public static string? GitLabCIRunnerTags => Environment.GetEnvironmentVariable("CI_RUNNER_TAGS");

        public static string? GitLabCIProjectPath => Environment.GetEnvironmentVariable("CI_PROJECT_PATH");

        public static string? GitLabCIDependencyProxyDirectGroupImagePrefix =>
            Environment.GetEnvironmentVariable("CI_DEPENDENCY_PROXY_DIRECT_GROUP_IMAGE_PREFIX");

        public static string? GitLabCIServerTlsCaFile => Environment.GetEnvironmentVariable("CI_SERVER_TLS_CA_FILE");

        public static bool CommitRefProtected => string.Equals(
            Environment.GetEnvironmentVariable("CI_COMMIT_REF_PROTECTED"), "true", StringComparison.OrdinalIgnoreCase);

        public static string? GitLabCIApiGraphqlUrl => Environment.GetEnvironmentVariable("CI_API_GRAPHQL_URL");

        public static string? GitLabCIServerVersionMinor =>
            Environment.GetEnvironmentVariable("CI_SERVER_VERSION_MINOR");

        public static string? CommitSHA => Environment.GetEnvironmentVariable("CI_COMMIT_SHA");

        public static string? GitLabCIJobTimeout => Environment.GetEnvironmentVariable("CI_JOB_TIMEOUT");

        public static string? ProjectVisibility => Environment.GetEnvironmentVariable("CI_PROJECT_VISIBILITY");

        public static string? ConcurrentProjectId => Environment.GetEnvironmentVariable("CI_CONCURRENT_PROJECT_ID");

        public static string? CommitMessage => Environment.GetEnvironmentVariable("CI_COMMIT_MESSAGE");

        public static string? GitLabCIServerShellSshPort =>
            Environment.GetEnvironmentVariable("CI_SERVER_SHELL_SSH_PORT");

        public static string? GitLabCIPagesDomain => Environment.GetEnvironmentVariable("CI_PAGES_DOMAIN");

        public static string? GitLabCIServerVersion => Environment.GetEnvironmentVariable("CI_SERVER_VERSION");

        public static string? GitLabCIRegistry => Environment.GetEnvironmentVariable("CI_REGISTRY");

        public static string? GitLabCIServerPort => Environment.GetEnvironmentVariable("CI_SERVER_PORT");

        public static string? ProjectNamespaceId => Environment.GetEnvironmentVariable("CI_PROJECT_NAMESPACE_ID");

        public static string? GitLabCIIsSharedEnvironment =>
            Environment.GetEnvironmentVariable("CI_SHARED_ENVIRONMENT");

        public static string? GitLabCIPagesUrl => Environment.GetEnvironmentVariable("CI_PAGES_URL");

        public static string? GitLabCIPipelineIid => Environment.GetEnvironmentVariable("CI_PIPELINE_IID");

        public static string? GitLabCIRepositoryUrl => Environment.GetEnvironmentVariable("CI_REPOSITORY_URL");

        public static string? GitLabCIServerUrl => Environment.GetEnvironmentVariable("CI_SERVER_URL");

        public static string? GitLabFeatures => Environment.GetEnvironmentVariable("GITLAB_FEATURES");

        public static string? CommitDescription => Environment.GetEnvironmentVariable("CI_COMMIT_DESCRIPTION");

        public static string? GitLabCITemplateRegistryHost =>
            Environment.GetEnvironmentVariable("CI_TEMPLATE_REGISTRY_HOST");

        public static string? GitLabCIJobStage => Environment.GetEnvironmentVariable("CI_JOB_STAGE");

        public static string? GitLabCIPipelineUrl => Environment.GetEnvironmentVariable("CI_PIPELINE_URL");

        public static string? DefaultBranch => Environment.GetEnvironmentVariable("CI_DEFAULT_BRANCH");

        public static string? GitLabEnv => Environment.GetEnvironmentVariable("GITLAB_ENV");

        public static string? GitLabCIServerVersionPatch =>
            Environment.GetEnvironmentVariable("CI_SERVER_VERSION_PATCH");

        public static string? CommitTitle => Environment.GetEnvironmentVariable("CI_COMMIT_TITLE");

        public static string? ProjectRootNamespace => Environment.GetEnvironmentVariable("CI_PROJECT_ROOT_NAMESPACE");

        public static string? GitLabUserName => Environment.GetEnvironmentVariable("GITLAB_USER_NAME");

        public static string? GitLabCIProjectDir => Environment.GetEnvironmentVariable("CI_PROJECT_DIR");

        public static string? GitLabCIRunnerId => Environment.GetEnvironmentVariable("CI_RUNNER_ID");

        public static string? GitLabCIPipelineCreatedAt => Environment.GetEnvironmentVariable("CI_PIPELINE_CREATED_AT");

        public static string? CommitTimestamp => Environment.GetEnvironmentVariable("CI_COMMIT_TIMESTAMP");

        public static string? GitLabCIServerShellSshHost =>
            Environment.GetEnvironmentVariable("CI_SERVER_SHELL_SSH_HOST");

        public static string? GitLabCIRegistryImage => Environment.GetEnvironmentVariable("CI_REGISTRY_IMAGE");

        /// <summary>
        /// http/https
        /// </summary>
        public static string? GitLabCIServerProtocol => Environment.GetEnvironmentVariable("CI_SERVER_PROTOCOL");

        public static string? CommitAuthor => Environment.GetEnvironmentVariable("CI_COMMIT_AUTHOR");

        public static string? CommitRefName => Environment.GetEnvironmentVariable("CI_COMMIT_REF_NAME");

        public static string? GitLabCIServerHost => Environment.GetEnvironmentVariable("CI_SERVER_HOST");

        public static string? GitLabCIJobUrl => Environment.GetEnvironmentVariable("CI_JOB_URL");

        public static string? GitLabCIJobStartedAt => Environment.GetEnvironmentVariable("CI_JOB_STARTED_AT");

        public static string? GitLabCIConcurrentId => Environment.GetEnvironmentVariable("CI_CONCURRENT_ID");

        public static string? ProjectDescription => Environment.GetEnvironmentVariable("CI_PROJECT_DESCRIPTION");

        public static string? CommitBranch => Environment.GetEnvironmentVariable("CI_COMMIT_BRANCH");

        public static string? GitLabCIProjectClassificationLabel =>
            Environment.GetEnvironmentVariable("CI_PROJECT_CLASSIFICATION_LABEL");

        public static string? GitLabCIRunnerRevision => Environment.GetEnvironmentVariable("CI_RUNNER_REVISION");

        public static string? GitLabCIDependencyProxyUser =>
            Environment.GetEnvironmentVariable("CI_DEPENDENCY_PROXY_USER");

        public static string? GitLabCIProjectPathSlug => Environment.GetEnvironmentVariable("CI_PROJECT_PATH_SLUG");

        public static string? ProjectId => Environment.GetEnvironmentVariable("CI_PROJECT_ID");

        public static bool IsCI => string.Equals(Environment.GetEnvironmentVariable("CI"), "true",
            StringComparison.OrdinalIgnoreCase);


        public static string? GitLabCINodeTotal => Environment.GetEnvironmentVariable("CI_NODE_TOTAL");
        public static string? GitLabCIJobId => Environment.GetEnvironmentVariable("CI_JOB_ID");

        public static string? GitLabCIProjectRepositoryLanguages =>
            Environment.GetEnvironmentVariable("CI_PROJECT_REPOSITORY_LANGUAGES");


        public static string? ProjectTitle => Environment.GetEnvironmentVariable("CI_PROJECT_TITLE");
        public static string? CommitBeforeSha => Environment.GetEnvironmentVariable("CI_COMMIT_BEFORE_SHA");

        public static string? GitLabCIServerVersionMajor =>
            Environment.GetEnvironmentVariable("CI_SERVER_VERSION_MAJOR");

        public static string? GitLabCIConfigPath => Environment.GetEnvironmentVariable("CI_CONFIG_PATH");

        public static string? GitLabCIDependencyProxyServer =>
            Environment.GetEnvironmentVariable("CI_DEPENDENCY_PROXY_SERVER");

        public static string? ProjectUrl => Environment.GetEnvironmentVariable("CI_PROJECT_URL");
    }
}