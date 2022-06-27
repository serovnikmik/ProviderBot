using bottest2;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("5434083624:AAHYAHbckvlGq6PmOeODwNFyljy13ghVWdQ");

using var cts = new CancellationTokenSource();

ReplyKeyboardRemove keyboardRemove = new ReplyKeyboardRemove();
ReplyKeyboardMarkup BasicIn = new(new[]
{
    new KeyboardButton[] {"Логин", "Тарифы"},
})
{
    ResizeKeyboard = true
};
ReplyKeyboardMarkup UserIn = new(new[]
{
    new KeyboardButton[] {"Баланс", "Текущий тариф"},
    new KeyboardButton[] {"Адрес", "Сменить тариф"},
    new KeyboardButton[] {"Сменить пользователя"},
    new KeyboardButton[] {"Поддержка"}
})
{
    ResizeKeyboard = true
};

ReplyKeyboardMarkup AdminIn = new(new[]
{
    new KeyboardButton[] {"Посмотреть заявки"},
    new KeyboardButton[] {"Сменить пользователя"}
})
{
    ResizeKeyboard = true
};

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};

botClient.StartReceiving(
    HandleUpdatesAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Начал прослушку @{me.Username}");
Console.ReadLine();

cts.Cancel();

async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message && update?.Message?.Text != null)
    {
        await HandleMessage(botClient, update.Message);
        return;
    }
}

async Task HandleMessage(ITelegramBotClient botClient, Message message)
{
    if (message.Text == "/start")
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Привет", replyMarkup: BasicIn);
        return;
    }

    if (message.Text == "Логин")
    {
        bool e = false;
        using (BotContext db = new BotContext())
        {
            var Users = db.Users.ToList();
            foreach (bottest2.User u in Users)
            {
                if (u.ChatId == message.Chat.Id)
                {
                    if(u.Login == "ADMIN")
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Вы уже авторизованы {u.Login}", replyMarkup: AdminIn);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Вы уже авторизованы {u.Login}", replyMarkup: UserIn);
                    }
                    e = true;
                }
            }
        }
        if (!e)
        {

            await botClient.SendTextMessageAsync(message.Chat.Id, "Введите ваш хэш", replyMarkup: keyboardRemove);
            return;
        }
    }

    if (message.Text.StartsWith('#'))
    {
        bool e = false;
        using (BotContext db = new BotContext())
        {
            var Users = db.Users.ToList();
            foreach (bottest2.User u in Users)
            {
                if (u.ChatId == message.Chat.Id)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Вы вышли из аккауна {u.Login}");
                    var change = db.Users.SingleOrDefault(b => b == u);
                    change.ChatId = 0;
                    db.SaveChanges();
                }
            }
            foreach (bottest2.User u in Users)
            {
                if (u.Password == message.Text)
                {
                    e = true;
                    if (u.Login == "ADMIN")
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"С возвращением, {u.Login}", replyMarkup: AdminIn);
                        var change1 = db.Users.SingleOrDefault(b => b == u);
                        change1.ChatId = message.Chat.Id;
                        db.SaveChanges();
                        return;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Вы успешно вошли в аккаунт {u.Login}", replyMarkup: UserIn);
                        var change2 = db.Users.SingleOrDefault(b => b == u);
                        change2.ChatId = message.Chat.Id;
                        db.SaveChanges();
                        return;
                    }
                }
            }
        }
        if (!e)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Неверный хэш", replyMarkup: BasicIn);
            return;
        }
    }
    if (message.Text == "Баланс")
    {
        bool e = false;
        using (BotContext db = new BotContext())
        {
            var Users = db.Users.ToList();
            foreach (bottest2.User u in Users)
            {
                if (u.ChatId == message.Chat.Id)
                {
                    e = true;
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш баланс: {u.Balance} руб.", replyMarkup: UserIn);
                }
            }
        }
        if (!e)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы не авторизованы!", replyMarkup: BasicIn);
            return;
        }
    }
    if (message.Text == "Текущий тариф")
    {
        bool e = false;
        using (BotContext db = new BotContext())
        {
            var Users = db.Users.ToList();
            foreach (bottest2.User u in Users)
            {
                if (u.ChatId == message.Chat.Id)
                {
                    e = true;
                    var Tarifs = db.Tarifs.ToList();
                    var UsersTarifs = db.UserTarifs.ToList();
                    long tId = 0;
                    foreach (UserTarif ut in UsersTarifs)
                    {
                        if (ut.UId == u.Id)
                        {
                            tId = ut.TId;
                        }
                    }
                    foreach (Tarif t in Tarifs)
                    {
                        if (t.Id == tId)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{t.Id}. {t.Name}\n" +
                            $"Стоимость: {t.Price}\n" +
                            $"Скорость: {t.Speed} МБ/с\n" +
                            $"Дата списания: {u.Date}", replyMarkup: UserIn);
                        }
                    }
                }
            }
        }
        if (!e)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы не авторизованы!", replyMarkup: BasicIn);
            return;
        }
    }
    if (message.Text == "Адрес")
    {
        bool e = false;
        using (BotContext db = new BotContext())
        {
            var Users = db.Users.ToList();
            foreach (bottest2.User u in Users)
            {
                if (u.ChatId == message.Chat.Id)
                {
                    e = true;
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш адрес: {u.Ip}", replyMarkup: UserIn);
                }
            }
        }
        if (!e)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы не авторизованы!", replyMarkup: BasicIn);
            return;
        }
    }

    if (message.Text == "Сменить тариф")
    {
        bool e = false;
        using (BotContext db = new BotContext())
        {
            var Users = db.Users.ToList();
            foreach (bottest2.User u in Users)
            {
                if (u.ChatId == message.Chat.Id)
                {
                    e = true;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Доступные тарифы");
                    var Tarifs = db.Tarifs.ToList();
                    foreach (Tarif t in Tarifs)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{t.Id}. {t.Name}\n" +
                            $"Стоимость: {t.Price}\n" +
                            $"Скорость: {t.Speed} МБ/с", replyMarkup: BasicIn);
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Отправьте сообщение, которое начинается с '/change' и после укажите номер тарифа", replyMarkup: UserIn);
                    return;
                }
            }
        }
        if (!e)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы не авторизованы!", replyMarkup: BasicIn);
            return;
        }
    }

    if (message.Text == "Тарифы")
    {
        using (BotContext db = new BotContext())
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Доступные тарифы");
            var Tarifs = db.Tarifs.ToList();
            foreach (Tarif t in Tarifs)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"{t.Id}. {t.Name}\n" +
                    $"Стоимость: {t.Price}\n" +
                    $"Скорость: {t.Speed} МБ/с", replyMarkup: BasicIn);
            }
        }
    }
    if (message.Text == "Сменить пользователя")
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Введите ваш хэш", replyMarkup: keyboardRemove);
        return;
    }
    if (message.Text == "Посмотреть заявки")
    {
        bool e = false;
        using (BotContext db = new BotContext())
        {
            var Users = db.Users.ToList();
            foreach (bottest2.User u in Users)
            {
                if (u.ChatId == message.Chat.Id && u.Login == "ADMIN")
                {
                    e = true;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Заявки:\n", replyMarkup: AdminIn);
                    var Requests = db.Reqs.ToList();
                    int i = 0;
                    foreach (Req r in Requests)
                    {
                        if (r.Res == null)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{r.RId}. {r.Content}\nПользователь: {r.UId}");
                            i++;
                        }
                    }
                    if (i == 0)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Необработанных заявок нет");
                    }
                }
            }
        }
        if (!e)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "У Вас нет доступа!", replyMarkup: BasicIn);
            return;
        }
    }
    if (message.Text == "Поддержка")
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Отправьте сообщение, которое начинается с '/help' и после укажите Вашу проблему", replyMarkup: UserIn);
        return;
    }
    if (message.Text.StartsWith("/help"))
    {
        bool e = false;
        using (BotContext db = new BotContext())
        {
            var Users = db.Users.ToList();
            foreach (bottest2.User u in Users)
            {
                if (u.ChatId == message.Chat.Id)
                {
                    string s = message.Text.Remove(0, 6);
                    e = true;
                    var Requests = db.Reqs.ToList();
                    Req r = new Req { Content = s, UId = u.Id };
                    db.Reqs.Add(r);
                    db.SaveChanges();
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ваша заявка принята", replyMarkup: UserIn);
                }
            }
        }
        if (!e)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы не авторизованы!", replyMarkup: BasicIn);
            return;
        }
    }
    if (message.Text.StartsWith("/change"))
    {
        bool e = false;
        using (BotContext db = new BotContext())
        {
            var Users = db.Users.ToList();
            foreach (bottest2.User u in Users)
            {
                if (u.ChatId == message.Chat.Id)
                {
                    string s = message.Text.Remove(0, 8);
                    e = true;
                    var Tarifs = db.Tarifs.ToList();
                    var UserTarifs = db.UserTarifs.ToList();
                    int sc;
                    bool bb = false;
                    if(Int32.TryParse(s, out sc))
                    {
                        foreach(Tarif t in Tarifs)
                        {
                            if(sc == t.Id)
                            {
                                bb = true;
                                if(u.Balance >= t.Price)
                                {
                                    foreach(UserTarif ut in UserTarifs)
                                    {
                                        if(ut.UId == u.Id)
                                        {
                                            var change1 = db.Users.SingleOrDefault(b => b == u);
                                            change1.Balance -= t.Price;
                                            var change2 = db.UserTarifs.SingleOrDefault(c => c == ut);
                                            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы успешно сменили тариф!", replyMarkup: UserIn);
                                            change2.TId = t.Id;
                                            change1.Date = DateTime.Today.ToString().Substring(0,2);
                                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Каждый месяц {change1.Date} числа у Вас будет списываться новая стоимость тарифа\n" +
                                                $"Если в месяце не будет такого числа, то оплата спишется 1-го", replyMarkup: UserIn);
                                        }
                                    }
                                    db.SaveChanges();
                                    return;
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(message.Chat.Id, "У Вас недостаточный баланс!\nПерейдите на наш сайт для оплаты и попробуйте еще раз\ntest.test/payment.html", replyMarkup: UserIn);
                                }
                            }
                        }
                        if (!bb)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Такого тарифа не существует!", replyMarkup: UserIn);
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Некорректно введен тариф!", replyMarkup: UserIn);
                    }
                }
            }
        }
        if (!e)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы не авторизованы!", replyMarkup: BasicIn);
            return;
        }
    }
}

Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Ошибка телеграм АПИ:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

