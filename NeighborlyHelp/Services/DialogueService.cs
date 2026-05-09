using System;
using System.Collections.Generic;
using System.Drawing;
using NeighborlyHelp.Models; // Убедись, что пространства имен совпадают

namespace NeighborlyHelp.Services
{
    public class DialogueData
    {
        public List<string> NpcLines { get; set; } = new List<string>();
        public List<string> PlayerLines { get; set; } = new List<string>();
        public string SpriteName { get; set; } = "";
    }

    public class DialogueService
    {
        // Метод получает NPC и текущее состояние игры, возвращает данные для диалога
        public DialogueData GetDialogueFor(NPC npc, GameState currentState)
        {
            var data = new DialogueData();

            if (npc.DisplayName == "Мила")
            {
                data.SpriteName = "sprite1.png";
                if (currentState == GameState.Quest1_Return)
                {
                    data.NpcLines = new List<string>
                    {
                        "О, что это? Ты нашла мои ключики! Теперь я могу спокойно зайти домой",
                        "Спасибо тебе большое! Я буду аккуратнее обращаться со своими вещами. Приходи ко мне на чай сегодня вечером!",
                        "Да, посиделки нашей дружной компанией - это прекрасно! Кстати, здесь только что пробегал запыхавшийся Оливер"
                    };
                    data.PlayerLines = new List<string>
                    {
                        "Вот, держи свои ключи! Больше не теряй, будь внимательна и всегда следи за своими вещами!",
                        "С удовольствием приду! Мы можем позвать на чаепитие всех соседей. А пока я найду еще кого-нибудь",
                        "Ха-ха, не удивлена! Он вечно куда-то спешит. Пойду найду его, может быть смогу чем-то помочь"
                    };
                }
                else
                {
                    data.PlayerLines = new List<string>
                    {
                        "Привет, Мила! Да, у меня все прекрасно. Вот вышла на прогулку, подышать свежим воздухом и заняться чем-нибудь интересным. Как твои дела?",
                        "Как же так! Наверняка ты их просто где-то выронила. Давай мы найдем их вместе!"
                    };
                }
            }
            else if (npc.DisplayName == "Оливер")
            {
                data.SpriteName = "sprite2.png";
                if (currentState == GameState.Quest2_Deliver)
                {
                    data.NpcLines = new List<string>
                    {
                        "Ты уже вернулась? Даже забрала мою посылку! Супер, огромное тебе спасибо!",
                        "Ты такая хорошая соседка! Как всегда меня выручила в самый трудный момент. Я обязательно помогу тебе в ответ, когда это потребуется, только скажи!",
                        "Вау, круто! Да, знаешь, кажется я с утра видел Мелиссу. Она сказала мне, что хочет заняться цветами на клумбе"
                    };
                    data.PlayerLines = new List<string>
                    {
                        "Здравствуйте, курьер-соседка к Вашим услугам, ха-ха! Заказ 18046 твой!",
                        "Рада стараться! Сегодня вечером Мила пригласила всех на чаепитие. Может быть ты видел кого-то ещё из наших соседей?",
                        "Конечно, садоводство - её любимое занятие, как я сразу не догадалась! Тогда пррогуляюсь до нашей клумбы"
                    };
                }
                else
                {
                    data.PlayerLines = new List<string> { "Привет, Оливер! Чем могу помочь?", "Без проблем, сейчас схожу на почту." };
                }
            }
            else if (npc.DisplayName == "Мелисса")
            {
                data.SpriteName = "sprite3.png"; // Исправил спрайт
                if (currentState == GameState.Quest3_Completed)
                {
                    data.NpcLines = new List<string>
                    {
                        "Боже мой, клумба просто ожила! Спасибо тебе огромное!",
                        "Ты самая добрая соседка. Хочешь, подарю тебе букет?",
                        "Кстати, Ричард из четвёртого домика ждёт помощи у баков."
                    };
                    data.PlayerLines = new List<string>
                    {
                        "Цветы любят воду, всё просто!",
                        "Спасибо, букет будет кстати!",
                        "Поняла, сейчас найду Ричарда."
                    };
                }
                else
                {
                    data.PlayerLines = new List<string> { "Привет, Мелисса! Красивые цветы.", "Конечно, помогу полить!" };
                }
            }
            else if (npc.DisplayName == "Ричард")
            {
                data.SpriteName = "sprite4.png";
                if (currentState == GameState.Quest4_Spawn)
                {
                    data.NpcLines = new List<string>
                    {
                        "Кто пришел? Ты от Мелиссы? Здорово! Слушай, у меня тут беда...",
                        "Я пытаюсь поймать подкаст о насекомых, но крутилка заела.",
                        "Помоги настроить частоту на 95.5 МГц. Двигай ползунок в зелёную зону!"
                    };
                    data.PlayerLines = new List<string>
                    {
                        "Да, она сказала, что тебе нужна помощь. Что стряслось?",
                        "Старое радио? Попробую починить.",
                        "Сейчас настрою, держись!"
                    };
                }
                else if (currentState == GameState.Quest4_Completed)
                {
                    data.NpcLines = new List<string>
                    {
                        "Спасибо тебе огромное! Подкаст заиграл!",
                        "Ты настоящая волшебница. Наш двор стал уютнее благодаря тебе!"
                    };
                    data.PlayerLines = new List<string>
                    {
                        "Всегда пожалуйста! Приятного прослушивания.",
                        "Рада, что помогла. Береги себя!"
                    };
                }
                else
                {
                    data.PlayerLines = new List<string> { "Привет, Ричард! Чем могу помочь?" };
                }
            }

            return data;
        }
    }
}