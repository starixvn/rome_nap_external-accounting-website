// --------------------------------------------------------------------------
// <copyright file="EditGroupModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.ViewModel.ManageGroup
{
    /// <summary>
    /// Edit group model.
    /// </summary>
    public class EditGroupModel
    {
        public int GroupId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}