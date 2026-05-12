using System;
using System.Collections.Generic;
using NeighborlyHelp;
using NeighborlyHelp.Services;
using NeighborlyHelp.Models;
using Xunit;

namespace NeighborlyHelp.Tests
{
    public class GameLogicTests
    {
        // Тест проверяет, что сервис диалогов возвращает корректную вводную фразу для персонажа Шарлотта.
        // Ожидается, что первая реплика содержит слово "привет".
        [Fact]
        public void DialogueService_ReturnsCorrectIntroForCharlotte()
        {
            var dialogueService = new DialogueService();
            var charlotte = new NPC(0, 0, "Шарлотта", new List<string>(), "sprite1.png", 100, 100);

            var result = dialogueService.GetDialogueFor(charlotte, GameState.Quest1_Talk);

            Assert.NotNull(result);
            Assert.NotEmpty(result.NpcLines);
            Assert.Contains("привет", result.NpcLines[0].ToLower());
        }

        // Тест проверяет, что сервис диалогов возвращает корректную вводную фразу для персонажа Ричард.
        // Ожидается, что первая реплика содержит слово "радио", так как это связано с квестом.
        [Fact]
        public void DialogueService_ReturnsCorrectIntroForRichard()
        {
            var dialogueService = new DialogueService();
            var richard = new NPC(0, 0, "Ричард", new List<string>(), "sprite4.png", 100, 100);

            var result = dialogueService.GetDialogueFor(richard, GameState.Quest4_Spawn);

            Assert.NotNull(result);
            Assert.NotEmpty(result.NpcLines);
            Assert.Contains("радио", result.NpcLines[0].ToLower());
        }

        // Тест проверя логику перехода состояния игры: после завершения 4-го квеста и окончания диалога
        // игра должна переключиться в состояние "Концовка" (Ending).
        [Fact]
        public void QuestService_SwitchesToEndingAfterQuest4Completed()
        {
            var model = new GameModel();
            model.Initialize();
            model.CurrentGameState = GameState.Quest4_Completed;
            model.IsDialogueActive = false;

            var questService = new QuestService(model);

            questService.HandleDialogueEnd();

            Assert.Equal(GameState.Ending, model.CurrentGameState);
        }

        // Тест проверяет переход от 1-го квеста ко 2-му после возвращения ключей.
        // После окончания диалога с Шарлоттой она должна исчезнуть, должен появиться Оливер,
        // а состояние игры должно измениться на Quest2_Spawn.
        [Fact]
        public void QuestService_StartsQuest2AfterReturningKeys()
        {
            var model = new GameModel();
            model.Initialize();
            model.CurrentGameState = GameState.Quest1_Return;
            model.IsDialogueActive = false;

            model.SpawnNPC("Шарлотта", 0, 0, new List<string>(), "sprite1.png", 100, 100);

            var questService = new QuestService(model);

            questService.HandleDialogueEnd();

            Assert.Equal(GameState.Quest2_Spawn, model.CurrentGameState);
            Assert.DoesNotContain(model.NPCs, n => n.DisplayName == "Шарлотта");
            Assert.Contains(model.NPCs, n => n.DisplayName == "Оливер");
        }

        // Тест проверяет корректную инициализацию игрока в модели игры.
        // Проверяются начальные координаты X и Y, а также наличие объекта игрока.
        [Fact]
        public void GameModel_InitializesPlayerCorrectly()
        {
            var model = new GameModel();
            model.Initialize();

            Assert.NotNull(model.Player);
            Assert.Equal(530, model.Player.X);
            Assert.Equal(450, model.Player.Y);
        }

        // Тест проверяет корректное создание предмета "Ключи" в мире игры.
        // После вызова SpawnKeys в списке collectibles должен появиться ровно один предмет с именем "Ключи".
        [Fact]
        public void GameModel_SpawnsKeysCorrectly()
        {
            var model = new GameModel();
            model.Initialize();

            model.SpawnKeys();

            Assert.Single(model.Collectibles);
            Assert.Equal("Ключи", model.Collectibles[0].Item.Name);
        }

        // Тест проверяет логику мини-игры с радио.
        // Если текущая частота близка к целевой (в пределах допустимой погрешности),
        // мини-игра должна завершиться успешно, флаг активности сброситься,
        // а состояние игры перейти в Quest4_Completed.
        [Fact]
        public void GameModel_RadioGame_WinsWhenFreqIsClose()
        {
            var model = new GameModel();
            model.Initialize();
            model.StartRadioMiniGame(800, 600);
            model.TargetFreq = 95.5f;
            model.RadioFreq = 95.0f;

            model.CheckRadioGameWin();

            Assert.False(model.IsRadioGameActive);
            Assert.Equal(GameState.Quest4_Completed, model.CurrentGameState);
        }
    }
}