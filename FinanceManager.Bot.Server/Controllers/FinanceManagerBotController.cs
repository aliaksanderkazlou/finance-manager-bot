using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Services;
using FinanceManager.Bot.Helpers.Models;
using FinanceManager.Bot.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = FinanceManager.Bot.Helpers.Models.Message;

namespace FinanceManager.Bot.Server.Controllers
{
    [Route("/webhook")]
    public class FinanceManagerBotController : Controller
    {
        private readonly ITelegramBotClient _botClient;

        private readonly CommandService _commandService;

        public FinanceManagerBotController(
            ITelegramBotClient botClient, 
            CommandService commandService)
        {
            _botClient = botClient;
            _commandService = commandService;
        }

        [HttpPost("")]
        public async Task<IActionResult> GetMessage([FromBody]Update update)
        {
            var message = update.Message;

            if (update.Message == null && update.CallbackQuery == null)
            {
                return Ok();
            }

            var messageToProcess = message != null
                ? ControllerHelper.GetMessageFromMessageStructure(message)
                : ControllerHelper.GetMessageFromUpdateStructure(update);
               
            var responseList =
                await _commandService.ExecuteCommand(messageToProcess.Text.Split(' ')[0], messageToProcess);

                foreach (var response in responseList)
                {
                    if (response.StatusCode == StatusCodeEnum.NeedKeyboard)
                    {
                        await _botClient.SendTextMessageAsync(messageToProcess.UserInfo.ChatId,
                            response.Message,
                            replyMarkup: Helpers.ControllerHelper.BuildKeyBoardMarkup((List<string>) response.Helper));
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(messageToProcess.UserInfo.ChatId, response.Message);
                    }
                }

            return Ok();
        }
    }
}