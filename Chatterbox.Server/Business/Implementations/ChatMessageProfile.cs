using AutoMapper;
using Chatterbox.Contracts.Messages;
using Chatterbox.Server.DataAccess.Abstractions;

namespace Chatterbox.Server.Business.Implementations
{
    /// <summary>
    /// This AutoMapper-<see cref="Profile"/> is responsible for converting <see cref="ChatMessage"/>s into other types.
    /// </summary>
    internal class ChatMessageProfile : Profile
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChatMessageProfile"/>.
        /// </summary>
        public ChatMessageProfile()
        {
            CreateMap<MessageEntity, ChatMessage>();
        }
    }
}
