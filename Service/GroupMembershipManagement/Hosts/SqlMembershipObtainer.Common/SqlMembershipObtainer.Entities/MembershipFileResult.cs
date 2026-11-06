// Copyright(c) Microsoft Corporation.
// Licensed under the MIT license.

using Models;
using System;

namespace SqlMembershipObtainer.Entities
{
    public class MembershipFileResult
    {
        public SyncStatus Status { get; set; }
        public string FilePath { get; set; }
        public Guid? AdfRunId { get; set; }
    }
}