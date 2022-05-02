using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;
using DatingApp.API.Helpers.Pagination;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public MessagesController(IUserRepository userRepository, 
                                  IMessageRepository messageRepository, 
                                  IMapper mapper)
        {
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> AddMessage(CreateMessageDTO createMessageDTO)
        {
            var currentUsername = User.GetUsername();

            if(currentUsername == createMessageDTO.RecipientUsername.Trim().ToLower())
            {
                return BadRequest("You cannot send message to yourself.");
            }

            //this is not option because senderPhotoUrl is not present during mapping
            //var currentUserId = User.GetUserId();
            //var currentUser = await userRepository.GetUserByIdAsync(currentUserId);
            var currentUser = await userRepository.GetUserByUsernameAsync(currentUsername);
            var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if(recipient == null)
            {
                return NotFound();
            }

            var message = new Message()
            {
                SenderUsername = currentUser.UserName,
                //Sender = currentUser,
                RecipientUsername = recipient.UserName,
                //Recipient = recipient,
                Content = createMessageDTO.Content,
                SenderId = currentUser.Id,
                RecipientId = recipient.Id
            };

            messageRepository.AddMessage(message);

            if(await messageRepository.SaveAllChangesAsync())
            {
                return mapper.Map<MessageDTO>(message); //it should be createdAtRoute
            }

            return BadRequest("Failed sending message.");
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessages([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await messageRepository.GetMessagesForUser(messageParams);

            var pgHeader = new PaginationHeader()
            {
                CurrentPage = messages.CurrentPage,
                TotalPages = messages.TotalPages,
                PageSize = messages.PageSize,
                TotalCount = messages.TotalCount
            };

            Response.AddPaginationHeader(pgHeader);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        {
            return Ok(await messageRepository.GetMessageThread(User.GetUsername(), username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await messageRepository.GetMessage(id);

            if(message == null)
            {
                return NotFound();
            }

            if(message.RecipientUsername != username && message.SenderUsername != username)
            {
                return Unauthorized();
            }

            if(message.SenderUsername == username)
            {
                message.SenderDeleted = true;
            }

            if (message.RecipientUsername == username)
            {
                message.RecipientDeleted = true;
            }

            if(message.SenderDeleted && message.RecipientDeleted)
            {
                messageRepository.DeleteMessage(message);
            }

            if(await messageRepository.SaveAllChangesAsync())
            {
                return Ok();
            }

            return BadRequest("Problem deleting message");
        }
    }
}
