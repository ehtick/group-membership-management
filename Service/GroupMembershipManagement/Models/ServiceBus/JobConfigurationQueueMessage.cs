// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace Models.ServiceBus
{
    public class JobConfigurationQueueMessage
    {
        /// <summary>
        /// The unique identifier for the sync job
        /// </summary>
        public required Guid JobId { get; init; }

        /// <summary>
        /// The unique identifier for the group to be configured
        /// </summary>
        public required Guid GroupId { get; init; }

        /// <summary>
        /// Object ID of the requestor that initiated onboarding.
        /// </summary>
        public required string RequestorObjectId { get; init; }

        /// <summary>
        /// Display name of the requestor that initiated onboarding.
        /// </summary>
        public string? RequestorDisplayName { get; init; }

        /// <summary>
        /// Display name if onboarding was submitted on behalf of another user.
        /// </summary>
        public string? ChangedOnBehalfOfDisplayName { get; init; }

        /// <summary>
        /// Object ID if onboarding was submitted on behalf of another user.
        /// </summary>
        public string? ChangedOnBehalfOfObjectId { get; init; }

        /// <summary>
        /// Business justification provided during onboarding.
        /// </summary>
        public string? BusinessJustification { get; init; }
    }
}