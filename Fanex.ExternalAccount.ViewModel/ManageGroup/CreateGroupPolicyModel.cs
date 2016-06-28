// --------------------------------------------------------------------------
// <copyright file="CreateGroupPolicyModel.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.13">Created</change>
// </history>

namespace Fanex.ExternalAccount.ViewModel.ManageGroup
{
    /// <summary>
    /// Create group policy model.
    /// </summary>
    public class CreateGroupPolicyModel
    {
        public int GroupId { get; set; }

        public string GroupPolicy { get; set; }
    }
}