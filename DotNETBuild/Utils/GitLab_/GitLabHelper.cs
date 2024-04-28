#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class GitLabHelper
    {
        /// <inheritdoc cref="IsGitLabCI"/>
        [Obsolete("命名错了，请使用 IsGitLabCI 代替")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsGitlabCI
            => IsGitLabCI;

        /// <summary>
        /// 当前是否运行在 GitLab 的 CI 上，获取环境变量 "GITLAB_CI"
        /// </summary>
        public static bool IsGitLabCI
            => string.Equals(Environment.GetEnvironmentVariable("GITLAB_CI"), "true",
                StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_NAMESPACE"
        /// </summary>
        public static string? ProjectNamespace => Environment.GetEnvironmentVariable("CI_PROJECT_NAMESPACE");

        /// <summary>
        /// 获取环境变量 "CI_RUNNER_VERSION"
        /// </summary>
        public static string? GitLabRunnerVersion => Environment.GetEnvironmentVariable("CI_RUNNER_VERSION");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_NAME"
        /// </summary>
        public static string? ServerName => Environment.GetEnvironmentVariable("CI_SERVER_NAME");

        /// <summary>
        /// 获取环境变量 "CI_RUNNER_DESCRIPTION"
        /// </summary>
        public static string? GitLabRunnerDescription => Environment.GetEnvironmentVariable("CI_RUNNER_DESCRIPTION");

        /// <summary>
        /// 触发流水的用户的邮箱。这不是 commit 的用户的邮箱。获取环境变量 "GITLAB_USER_EMAIL"
        /// </summary>
        public static string? ExecutiveUserEmail => Environment.GetEnvironmentVariable("GITLAB_USER_EMAIL");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_REVISION"
        /// </summary>
        public static string? GitLabServerRevision => Environment.GetEnvironmentVariable("CI_SERVER_REVISION");

        /// <summary>
        /// 获取环境变量 "CI_RUNNER_EXECUTABLE_ARCH"
        /// </summary>
        public static string? GitLabRunnerExecutableArch =>
            Environment.GetEnvironmentVariable("CI_RUNNER_EXECUTABLE_ARCH");

        /// <summary>
        /// 获取环境变量 "CI_PIPELINE_NAME"
        /// </summary>
        public static string? PipelineName => Environment.GetEnvironmentVariable("CI_PIPELINE_NAME");

        /// <summary>
        /// 获取环境变量 "CI_REGISTRY_USER"
        /// </summary>
        public static string? RegistryUser => Environment.GetEnvironmentVariable("CI_REGISTRY_USER");

        /// <summary>
        /// 获取环境变量 "CI_API_V4_URL"
        /// </summary>
        public static string? GitLabApiV4Url => Environment.GetEnvironmentVariable("CI_API_V4_URL");

        /// <summary>
        /// 获取环境变量 "CI_RUNNER_SHORT_TOKEN"
        /// </summary>
        public static string? GitLabRunnerShortToken => Environment.GetEnvironmentVariable("CI_RUNNER_SHORT_TOKEN");

        /// <summary>
        /// 获取环境变量 "CI_JOB_NAME"
        /// </summary>
        public static string? GitLabCIJobName => Environment.GetEnvironmentVariable("CI_JOB_NAME");

        /// <summary>
        /// 获取环境变量 "CI_OPEN_MERGE_REQUESTS"
        /// </summary>
        public static string? GitLabCIOpenMergeRequests => Environment.GetEnvironmentVariable("CI_OPEN_MERGE_REQUESTS");

        /// <summary>
        /// 获取环境变量 "GITLAB_USER_LOGIN"
        /// </summary>
        public static string? GitLabUserLogin => Environment.GetEnvironmentVariable("GITLAB_USER_LOGIN");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_NAME"
        /// </summary>
        public static string? GitLabCIProjectName => Environment.GetEnvironmentVariable("CI_PROJECT_NAME");

        /// <summary>
        /// 获取环境变量 "CI_PIPELINE_SOURCE"
        /// </summary>
        public static string? GitLabCIPipelineSource => Environment.GetEnvironmentVariable("CI_PIPELINE_SOURCE");

        /// <summary>
        /// 获取环境变量 "CI_JOB_STATUS"
        /// </summary>
        public static string? GitLabCIJobStatus => Environment.GetEnvironmentVariable("CI_JOB_STATUS");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_REF_SLUG"
        /// </summary>
        public static string? CommitRefSlug => Environment.GetEnvironmentVariable("CI_COMMIT_REF_SLUG");

        /// <summary>
        /// 获取环境变量 "CI_SERVER"
        /// </summary>
        public static string? GitLabCIServer => Environment.GetEnvironmentVariable("CI_SERVER");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_SHORT_SHA"
        /// </summary>
        public static string? CommitShortSHA => Environment.GetEnvironmentVariable("CI_COMMIT_SHORT_SHA");

        /// <summary>
        /// 获取环境变量 "CI_JOB_NAME_SLUG"
        /// </summary>
        public static string? GitLabCIJobNameSlug => Environment.GetEnvironmentVariable("CI_JOB_NAME_SLUG");

        /// <summary>
        /// 获取环境变量 "CI_DEPENDENCY_PROXY_GROUP_IMAGE_PREFIX"
        /// </summary>
        public static string? GitLabCIDependencyProxyGroupImagePrefix =>
            Environment.GetEnvironmentVariable("CI_DEPENDENCY_PROXY_GROUP_IMAGE_PREFIX");

        /// <summary>
        /// 获取环境变量 "CI_RUNNER_TAGS"
        /// </summary>
        public static string? GitLabCIRunnerTags => Environment.GetEnvironmentVariable("CI_RUNNER_TAGS");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_PATH"
        /// </summary>
        public static string? GitLabCIProjectPath => Environment.GetEnvironmentVariable("CI_PROJECT_PATH");

        /// <summary>
        /// 获取环境变量 "CI_DEPENDENCY_PROXY_DIRECT_GROUP_IMAGE_PREFIX"
        /// </summary>
        public static string? GitLabCIDependencyProxyDirectGroupImagePrefix =>
            Environment.GetEnvironmentVariable("CI_DEPENDENCY_PROXY_DIRECT_GROUP_IMAGE_PREFIX");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_TLS_CA_FILE"
        /// </summary>
        public static string? GitLabCIServerTlsCaFile => Environment.GetEnvironmentVariable("CI_SERVER_TLS_CA_FILE");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_REF_PROTECTED"
        /// </summary>
        public static bool CommitRefProtected => string.Equals(
            Environment.GetEnvironmentVariable("CI_COMMIT_REF_PROTECTED"), "true", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 获取环境变量 "CI_API_GRAPHQL_URL"
        /// </summary>
        public static string? GitLabCIApiGraphqlUrl => Environment.GetEnvironmentVariable("CI_API_GRAPHQL_URL");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_VERSION_MINOR"
        /// </summary>
        public static string? GitLabCIServerVersionMinor =>
            Environment.GetEnvironmentVariable("CI_SERVER_VERSION_MINOR");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_SHA"
        /// </summary>
        public static string? CommitSHA => Environment.GetEnvironmentVariable("CI_COMMIT_SHA");

        /// <summary>
        /// 获取环境变量 "CI_JOB_TIMEOUT"
        /// </summary>
        public static string? GitLabCIJobTimeout => Environment.GetEnvironmentVariable("CI_JOB_TIMEOUT");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_VISIBILITY"
        /// </summary>
        public static string? ProjectVisibility => Environment.GetEnvironmentVariable("CI_PROJECT_VISIBILITY");

        /// <summary>
        /// 获取环境变量 "CI_CONCURRENT_PROJECT_ID"
        /// </summary>
        public static string? ConcurrentProjectId => Environment.GetEnvironmentVariable("CI_CONCURRENT_PROJECT_ID");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_MESSAGE"
        /// </summary>
        public static string? CommitMessage => Environment.GetEnvironmentVariable("CI_COMMIT_MESSAGE");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_SHELL_SSH_PORT"
        /// </summary>
        public static string? GitLabCIServerShellSshPort =>
            Environment.GetEnvironmentVariable("CI_SERVER_SHELL_SSH_PORT");

        /// <summary>
        /// 获取环境变量 "CI_PAGES_DOMAIN"
        /// </summary>
        public static string? GitLabCIPagesDomain => Environment.GetEnvironmentVariable("CI_PAGES_DOMAIN");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_VERSION"
        /// </summary>
        public static string? GitLabCIServerVersion => Environment.GetEnvironmentVariable("CI_SERVER_VERSION");

        /// <summary>
        /// 获取环境变量 "CI_REGISTRY"
        /// </summary>
        public static string? GitLabCIRegistry => Environment.GetEnvironmentVariable("CI_REGISTRY");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_PORT"
        /// </summary>
        public static string? GitLabCIServerPort => Environment.GetEnvironmentVariable("CI_SERVER_PORT");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_NAMESPACE_ID"
        /// </summary>
        public static string? ProjectNamespaceId => Environment.GetEnvironmentVariable("CI_PROJECT_NAMESPACE_ID");

        /// <summary>
        /// 获取环境变量 "CI_SHARED_ENVIRONMENT"
        /// </summary>
        public static string? GitLabCIIsSharedEnvironment =>
            Environment.GetEnvironmentVariable("CI_SHARED_ENVIRONMENT");

        /// <summary>
        /// 获取环境变量 "CI_PAGES_URL"
        /// </summary>
        public static string? GitLabCIPagesUrl => Environment.GetEnvironmentVariable("CI_PAGES_URL");

        /// <summary>
        /// 获取环境变量 "CI_PIPELINE_IID"
        /// </summary>
        public static string? GitLabCIPipelineIid => Environment.GetEnvironmentVariable("CI_PIPELINE_IID");

        /// <summary>
        /// 获取环境变量 "CI_REPOSITORY_URL"
        /// </summary>
        public static string? GitLabCIRepositoryUrl => Environment.GetEnvironmentVariable("CI_REPOSITORY_URL");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_URL"
        /// </summary>
        public static string? GitLabCIServerUrl => Environment.GetEnvironmentVariable("CI_SERVER_URL");

        /// <summary>
        /// 获取环境变量 "GITLAB_FEATURES"
        /// </summary>
        public static string? GitLabFeatures => Environment.GetEnvironmentVariable("GITLAB_FEATURES");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_DESCRIPTION"
        /// </summary>
        public static string? CommitDescription => Environment.GetEnvironmentVariable("CI_COMMIT_DESCRIPTION");

        /// <summary>
        /// 获取环境变量 "CI_TEMPLATE_REGISTRY_HOST"
        /// </summary>
        public static string? GitLabCITemplateRegistryHost =>
            Environment.GetEnvironmentVariable("CI_TEMPLATE_REGISTRY_HOST");

        /// <summary>
        /// 获取环境变量 "CI_JOB_STAGE"
        /// </summary>
        public static string? GitLabCIJobStage => Environment.GetEnvironmentVariable("CI_JOB_STAGE");

        /// <summary>
        /// 获取环境变量 "CI_PIPELINE_URL"
        /// </summary>
        public static string? GitLabCIPipelineUrl => Environment.GetEnvironmentVariable("CI_PIPELINE_URL");

        /// <summary>
        /// 获取环境变量 "CI_DEFAULT_BRANCH"
        /// </summary>
        public static string? DefaultBranch => Environment.GetEnvironmentVariable("CI_DEFAULT_BRANCH");

        /// <summary>
        /// 获取环境变量 "GITLAB_ENV"
        /// </summary>
        public static string? GitLabEnv => Environment.GetEnvironmentVariable("GITLAB_ENV");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_VERSION_PATCH"
        /// </summary>
        public static string? GitLabCIServerVersionPatch =>
            Environment.GetEnvironmentVariable("CI_SERVER_VERSION_PATCH");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_TITLE"
        /// </summary>
        public static string? CommitTitle => Environment.GetEnvironmentVariable("CI_COMMIT_TITLE");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_ROOT_NAMESPACE"
        /// </summary>
        public static string? ProjectRootNamespace => Environment.GetEnvironmentVariable("CI_PROJECT_ROOT_NAMESPACE");

        /// <summary>
        /// 获取环境变量 "GITLAB_USER_NAME"
        /// </summary>
        public static string? GitLabUserName => Environment.GetEnvironmentVariable("GITLAB_USER_NAME");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_DIR"
        /// </summary>
        public static string? GitLabCIProjectDir => Environment.GetEnvironmentVariable("CI_PROJECT_DIR");

        /// <summary>
        /// 获取环境变量 "CI_RUNNER_ID"
        /// </summary>
        public static string? GitLabCIRunnerId => Environment.GetEnvironmentVariable("CI_RUNNER_ID");

        /// <summary>
        /// 获取环境变量 "CI_PIPELINE_CREATED_AT"
        /// </summary>
        public static string? GitLabCIPipelineCreatedAt => Environment.GetEnvironmentVariable("CI_PIPELINE_CREATED_AT");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_TIMESTAMP"
        /// </summary>
        public static string? CommitTimestamp => Environment.GetEnvironmentVariable("CI_COMMIT_TIMESTAMP");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_SHELL_SSH_HOST"
        /// </summary>
        public static string? GitLabCIServerShellSshHost =>
            Environment.GetEnvironmentVariable("CI_SERVER_SHELL_SSH_HOST");

        /// <summary>
        /// 获取环境变量 "CI_REGISTRY_IMAGE"
        /// </summary>
        public static string? GitLabCIRegistryImage => Environment.GetEnvironmentVariable("CI_REGISTRY_IMAGE");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_PROTOCOL"，可选值是 http/https
        /// </summary>
        public static string? GitLabCIServerProtocol => Environment.GetEnvironmentVariable("CI_SERVER_PROTOCOL");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_AUTHOR"
        /// </summary>
        public static string? CommitAuthor => Environment.GetEnvironmentVariable("CI_COMMIT_AUTHOR");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_REF_NAME"
        /// </summary>
        public static string? CommitRefName => Environment.GetEnvironmentVariable("CI_COMMIT_REF_NAME");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_HOST"
        /// </summary>
        public static string? GitLabCIServerHost => Environment.GetEnvironmentVariable("CI_SERVER_HOST");

        /// <summary>
        /// 获取环境变量 "CI_JOB_URL"
        /// </summary>
        public static string? GitLabCIJobUrl => Environment.GetEnvironmentVariable("CI_JOB_URL");

        /// <summary>
        /// 获取环境变量 "CI_JOB_STARTED_AT"
        /// </summary>
        public static string? GitLabCIJobStartedAt => Environment.GetEnvironmentVariable("CI_JOB_STARTED_AT");

        /// <summary>
        /// 获取环境变量 "CI_CONCURRENT_ID"
        /// </summary>
        public static string? GitLabCIConcurrentId => Environment.GetEnvironmentVariable("CI_CONCURRENT_ID");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_DESCRIPTION"
        /// </summary>
        public static string? ProjectDescription => Environment.GetEnvironmentVariable("CI_PROJECT_DESCRIPTION");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_BRANCH"
        /// </summary>
        public static string? CommitBranch => Environment.GetEnvironmentVariable("CI_COMMIT_BRANCH");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_CLASSIFICATION_LABEL"
        /// </summary>
        public static string? GitLabCIProjectClassificationLabel =>
            Environment.GetEnvironmentVariable("CI_PROJECT_CLASSIFICATION_LABEL");

        /// <summary>
        /// 获取环境变量 "CI_RUNNER_REVISION"
        /// </summary>
        public static string? GitLabCIRunnerRevision => Environment.GetEnvironmentVariable("CI_RUNNER_REVISION");

        /// <summary>
        /// 获取环境变量 "CI_DEPENDENCY_PROXY_USER"
        /// </summary>
        public static string? GitLabCIDependencyProxyUser =>
            Environment.GetEnvironmentVariable("CI_DEPENDENCY_PROXY_USER");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_PATH_SLUG"
        /// </summary>
        public static string? GitLabCIProjectPathSlug => Environment.GetEnvironmentVariable("CI_PROJECT_PATH_SLUG");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_ID"
        /// </summary>
        public static string? ProjectId => Environment.GetEnvironmentVariable("CI_PROJECT_ID");

        /// <summary>
        /// 获取环境变量 "CI"
        /// </summary>
        public static bool IsCI => string.Equals(Environment.GetEnvironmentVariable("CI"), "true",
            StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 获取环境变量 "CI_NODE_TOTAL"
        /// </summary>
        public static string? GitLabCINodeTotal => Environment.GetEnvironmentVariable("CI_NODE_TOTAL");

        /// <summary>
        /// 获取环境变量 "CI_JOB_ID"
        /// </summary>
        public static string? GitLabCIJobId => Environment.GetEnvironmentVariable("CI_JOB_ID");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_REPOSITORY_LANGUAGES"
        /// </summary>
        public static string? GitLabCIProjectRepositoryLanguages =>
            Environment.GetEnvironmentVariable("CI_PROJECT_REPOSITORY_LANGUAGES");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_TITLE"
        /// </summary>
        public static string? ProjectTitle => Environment.GetEnvironmentVariable("CI_PROJECT_TITLE");

        /// <summary>
        /// 获取环境变量 "CI_COMMIT_BEFORE_SHA"
        /// </summary>
        public static string? CommitBeforeSha => Environment.GetEnvironmentVariable("CI_COMMIT_BEFORE_SHA");

        /// <summary>
        /// 获取环境变量 "CI_SERVER_VERSION_MAJOR"
        /// </summary>
        public static string? GitLabCIServerVersionMajor =>
            Environment.GetEnvironmentVariable("CI_SERVER_VERSION_MAJOR");

        /// <summary>
        /// 获取环境变量 "CI_CONFIG_PATH"
        /// </summary>
        public static string? GitLabCIConfigPath => Environment.GetEnvironmentVariable("CI_CONFIG_PATH");

        /// <summary>
        /// 获取环境变量 "CI_DEPENDENCY_PROXY_SERVER"
        /// </summary>
        public static string? GitLabCIDependencyProxyServer =>
            Environment.GetEnvironmentVariable("CI_DEPENDENCY_PROXY_SERVER");

        /// <summary>
        /// 获取环境变量 "CI_PROJECT_URL"
        /// </summary>
        public static string? ProjectUrl => Environment.GetEnvironmentVariable("CI_PROJECT_URL");
    }
}