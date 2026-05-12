using System;
using System.Collections.Generic;
using System.Drawing;
using NeighborlyHelp;
using NeighborlyHelp.Services;
using NeighborlyHelp.Models;
using Xunit;

namespace NeighborlyHelp.Tests
{
    public class GameLogicTests
    {
        // ТЕСТЫ ДЛЯ DIALOGUE SERVICE

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

        [Fact]
        public void DialogueService_ReturnsCorrectIntroForOliver()
        {
            var dialogueService = new DialogueService();
            var oliver = new NPC(0, 0, "Оливер", new List<string>(), "sprite2.png", 100, 100);
            var result = dialogueService.GetDialogueFor(oliver, GameState.Quest2_Spawn);

            Assert.NotNull(result);
            Assert.NotEmpty(result.NpcLines);
            Assert.Contains("посылк", result.NpcLines[0].ToLower()); // Проверяем контекст про посылку
        }

        [Fact]
        public void DialogueService_ReturnsCorrectIntroForMelissa()
        {
            var dialogueService = new DialogueService();
            var melissa = new NPC(0, 0, "Мелисса", new List<string>(), "sprite3.png", 100, 100);
            var result = dialogueService.GetDialogueFor(melissa, GameState.Quest3_Spawn);

            Assert.NotNull(result);
            Assert.NotEmpty(result.NpcLines);
            Assert.Contains("цвет", result.NpcLines[0].ToLower()); // Проверяем контекст про цветы
        }

        // ТЕСТЫ ДЛЯ QUEST SERVICE (ПЕРЕХОДЫ МЕЖДУ КВЕСТАМИ)

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

        [Fact]
        public void QuestService_StartsQuest3AfterDeliveringPackage()
        {
            var model = new GameModel();
            model.Initialize();
            model.CurrentGameState = GameState.Quest2_Deliver;
            model.IsDialogueActive = false;
            model.SpawnNPC("Оливер", 0, 0, new List<string>(), "sprite2.png", 100, 100);

            var questService = new QuestService(model);
            questService.HandleDialogueEnd();

            Assert.Equal(GameState.Quest3_Spawn, model.CurrentGameState);
            Assert.DoesNotContain(model.NPCs, n => n.DisplayName == "Оливер");
            Assert.Contains(model.NPCs, n => n.DisplayName == "Мелисса");
        }

        [Fact]
        public void QuestService_StartsQuest4AfterWateringFlowers()
        {
            var model = new GameModel();
            model.Initialize();
            model.CurrentGameState = GameState.Quest3_Completed;
            model.IsDialogueActive = false;
            model.SpawnNPC("Мелисса", 0, 0, new List<string>(), "sprite3.png", 100, 100);

            var questService = new QuestService(model);
            questService.HandleDialogueEnd();

            Assert.Equal(GameState.Quest4_Spawn, model.CurrentGameState);
            Assert.DoesNotContain(model.NPCs, n => n.DisplayName == "Мелисса");
            Assert.Contains(model.NPCs, n => n.DisplayName == "Ричард");
        }

        // ТЕСТЫ ДЛЯ GAME MODEL (МЕХАНИКИ ИГРЫ)

        [Fact]
        public void GameModel_InitializesPlayerCorrectly()
        {
            var model = new GameModel();
            model.Initialize();

            Assert.NotNull(model.Player);
            Assert.Equal(530, model.Player.X);
            Assert.Equal(450, model.Player.Y);
        }

        [Fact]
        public void GameModel_SpawnsKeysCorrectly()
        {
            var model = new GameModel();
            model.Initialize();
            model.SpawnKeys();

            Assert.Single(model.Collectibles);
            Assert.Equal("Ключи", model.Collectibles[0].Item.Name);
        }

        [Fact]
        public void GameModel_RadioGame_WinsWhenFreqIsClose()
        {
            var model = new GameModel();
            model.Initialize();
            model.StartRadioMiniGame(800, 600);
            model.TargetFreq = 95.5f;
            model.RadioFreq = 95.0f; // Разница 0.5 < 0.8

            model.CheckRadioGameWin();

            Assert.False(model.IsRadioGameActive);
            Assert.Equal(GameState.Quest4_Completed, model.CurrentGameState);
        }

        [Fact]
        public void GameModel_RadioGame_DoesNotWinIfFreqIsFar()
        {
            var model = new GameModel();
            model.Initialize();
            model.StartRadioMiniGame(800, 600);
            model.TargetFreq = 95.5f;
            model.RadioFreq = 88.0f; // Разница большая

            model.CheckRadioGameWin();

            Assert.True(model.IsRadioGameActive); // Игра должна продолжаться
            Assert.NotEqual(GameState.Quest4_Completed, model.CurrentGameState);
        }

        [Fact]
        public void GameModel_FlowerGame_WinsWhenAllWatered()
        {
            var model = new GameModel();
            model.Initialize();
            model.StartFlowerMiniGame(800, 600);

            // Поливаем все цветы до максимума
            foreach (var flower in model.Flowers)
            {
                flower.WaterLevel = 100;
            }

            model.CheckFlowerGameWin();

            Assert.False(model.IsFlowerGameActive);
            Assert.Equal(GameState.Quest3_Completed, model.CurrentGameState);
        }

        [Fact]
        public void GameModel_FlowerGame_DoesNotWinIfNotAllWatered()
        {
            var model = new GameModel();
            model.Initialize();
            model.StartFlowerMiniGame(800, 600);

            // Поливаем только один цветок
            if (model.Flowers.Count > 0)
                model.Flowers[0].WaterLevel = 100;

            model.CheckFlowerGameWin();

            Assert.True(model.IsFlowerGameActive);
            Assert.NotEqual(GameState.Quest3_Completed, model.CurrentGameState);
        }

        [Fact]
        public void GameModel_PlayerCannotMoveThroughWalls()
        {
            var model = new GameModel();
            model.Initialize();

            // Пытаемся двигать игрока за пределы поля (слева)
            int initialX = model.Player.X;
            model.MovePlayer(-100, model.Player.Y);

            Assert.Equal(initialX, model.Player.X); // Координата X не должна измениться
        }

        [Fact]
        public void GameModel_RemoveNPCWorksCorrectly()
        {
            var model = new GameModel();
            model.Initialize();
            model.SpawnNPC("TestNPC", 100, 100, new List<string>(), "sprite.png", 50, 50);

            Assert.Single(model.NPCs);

            model.RemoveNPC("TestNPC");

            Assert.Empty(model.NPCs);
        }

        [Fact]
        public void GameModel_MailboxMiniGame_CreatesCorrectNumberOfBoxes()
        {
            var model = new GameModel();
            model.Initialize();
            model.StartMailboxMiniGame(800, 600);

            Assert.Equal(50, model.MailOptions.Count);
            Assert.True(model.IsMiniGameActive);
        }

        [Fact]
        public void GameModel_MailboxMiniGame_HasExactlyOneCorrectBox()
        {
            var model = new GameModel();
            model.Initialize();
            model.StartMailboxMiniGame(800, 600);

            int correctCount = 0;
            foreach (var box in model.MailOptions)
            {
                if (box.IsCorrect) correctCount++;
            }

            Assert.Equal(1, correctCount);
        }
    }
}