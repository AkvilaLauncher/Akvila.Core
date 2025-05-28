using System;
using System.Collections.Generic;

namespace AkvilaCore.Interfaces.User {
    public interface IUser {
        string Name { get; set; }
        string AccessToken { get; set; }
        string Uuid { get; set; }
        string? TextureSkinUrl { get; set; }
        string? TextureCloakUrl { get; set; }
        string? TextureSkinGuid { get; set; }
        string? TextureCloakGuid { get; set; }
        public string ServerUuid { get; set; }
        bool IsBanned { get; set; }
        public DateTime ServerExpiredDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        List<ISession> Sessions { get; set; }
        IAkvilaManager Manager { get; set; }
    }
}
