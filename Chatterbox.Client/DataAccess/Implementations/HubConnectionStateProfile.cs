using AutoMapper;
using Chatterbox.Client.DataAccess.Abstractions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chatterbox.Client.DataAccess.Implementations
{
    /// <summary>
    /// This AutoMapper-<see cref="Profile"/> is responsible for converting <see cref="HubConnectionState"/>s into other types.
    /// </summary>
    internal class HubConnectionStateProfile : Profile
    {
        /// <summary>
        /// Creates a new instance of <see cref="HubConnectionStateProfile"/>.
        /// </summary>
        public HubConnectionStateProfile()
        {
            CreateMap<HubConnectionState, ConnectionState>();
        }
    }
}
