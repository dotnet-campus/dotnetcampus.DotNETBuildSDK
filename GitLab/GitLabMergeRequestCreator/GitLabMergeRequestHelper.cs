﻿using System;
using System.Net;
using System.Threading.Tasks;
using dotnetCampus.GitCommand;
using GitLabApiClient;
using GitLabApiClient.Models.MergeRequests.Requests;

namespace dotnetCampus.GitLabMergeRequestCreator
{
    public static class GitLabMergeRequestHelper
    {
        /// <summary>
        /// 尝试创建 MergeRequest 如果当前的分支有变更的话
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task TryCreateMergeRequest(Options options)
        {
            if (string.IsNullOrEmpty(options.GitLabUrl)
                || string.IsNullOrEmpty(options.GitLabToken)
                || string.IsNullOrEmpty(options.ProjectId))
            {
                Console.WriteLine($"Can not find GitLabUrl or GitLabToken or ProjectId.");
                return;
            }

            // "[Bot] Automated PR to fix formatting errors"
            var title = options.Title ?? "[Bot] Automated PR to fix formatting errors";

            var targetBranch = options.TargetBranch ?? "dev";

            var git = new Git();
            var currentBranch = options.SourceBranch;
            if (string.IsNullOrEmpty(currentBranch))
            {
                currentBranch = git.GetCurrentBranch();
            }

            var (success, output) = git.ExecuteCommand($"diff origin/{targetBranch} --name-only");
            if (success is false || string.IsNullOrEmpty(output))
            {
                Console.WriteLine(
                    $"There is no difference CurrentBranch({currentBranch}) and TargetBranch({targetBranch}).");
                return;
            }

            var gitLabClient = new GitLabClient(options.GitLabUrl, options.GitLabToken);

            Console.WriteLine($"Create MergeRequest: {currentBranch} to {targetBranch}");
            try
            {
                await gitLabClient.MergeRequests.CreateAsync(options.ProjectId,
                    new CreateMergeRequest(currentBranch, targetBranch, title)
                    {

                    });
            }
            catch (GitLabException e)
            {
                if (e.HttpStatusCode == HttpStatusCode.Conflict)
                {
                    // {"message":["Another open merge request already exists for this source branch: !10"]}
                    // 已经创建了 MR 过了，此时啥都不做
                    Console.WriteLine(e.Message);
                    return;
                }

                throw;
            }
        }
    }
}