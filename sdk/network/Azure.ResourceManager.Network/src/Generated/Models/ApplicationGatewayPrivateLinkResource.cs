// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;
using Azure.ResourceManager.Resources.Models;

namespace Azure.ResourceManager.Network.Models
{
    /// <summary> PrivateLink Resource of an application gateway. </summary>
    public partial class ApplicationGatewayPrivateLinkResource : WritableSubResource
    {
        /// <summary> Initializes a new instance of ApplicationGatewayPrivateLinkResource. </summary>
        public ApplicationGatewayPrivateLinkResource()
        {
            RequiredMembers = new ChangeTrackingList<string>();
            RequiredZoneNames = new ChangeTrackingList<string>();
        }

        /// <summary> Initializes a new instance of ApplicationGatewayPrivateLinkResource. </summary>
        /// <param name="id"> The id. </param>
        /// <param name="name"> Name of the private link resource that is unique within an Application Gateway. </param>
        /// <param name="etag"> A unique read-only string that changes whenever the resource is updated. </param>
        /// <param name="type"> Type of the resource. </param>
        /// <param name="groupId"> Group identifier of private link resource. </param>
        /// <param name="requiredMembers"> Required member names of private link resource. </param>
        /// <param name="requiredZoneNames"> Required DNS zone names of the the private link resource. </param>
        internal ApplicationGatewayPrivateLinkResource(string id, string name, string etag, string type, string groupId, IReadOnlyList<string> requiredMembers, IList<string> requiredZoneNames) : base(id)
        {
            Name = name;
            Etag = etag;
            Type = type;
            GroupId = groupId;
            RequiredMembers = requiredMembers;
            RequiredZoneNames = requiredZoneNames;
        }

        /// <summary> Name of the private link resource that is unique within an Application Gateway. </summary>
        public string Name { get; set; }
        /// <summary> A unique read-only string that changes whenever the resource is updated. </summary>
        public string Etag { get; }
        /// <summary> Type of the resource. </summary>
        public string Type { get; }
        /// <summary> Group identifier of private link resource. </summary>
        public string GroupId { get; }
        /// <summary> Required member names of private link resource. </summary>
        public IReadOnlyList<string> RequiredMembers { get; }
        /// <summary> Required DNS zone names of the the private link resource. </summary>
        public IList<string> RequiredZoneNames { get; }
    }
}
