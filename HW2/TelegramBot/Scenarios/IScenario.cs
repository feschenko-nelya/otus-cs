using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW2.TelegramBot.Scenario;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HW2.TelegramBot.Scenarios
{
    internal enum ScenarioResult
    {
        // Переход к следующему шагу. Сообщение обработано, но сценарий еще не завершен
        Transition,
        // Сценарий завершен
        Completed
    }
    internal interface IScenario
    {
        //Проверяет, может ли текущий сценарий обрабатывать указанный тип сценария.
        //Используется для определения подходящего обработчика в системе сценариев.
        bool CanHandle(ScenarioType scenario);
        //Обрабатывает входящее сообщение от пользователя в рамках текущего сценария.
        //Включает основную бизнес-логику
        Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct);
    }
}
