using System;
using System.Collections.Generic;
using NeighborlyHelp.Models;
using NeighborlyHelp.Services;
using Xunit;

namespace NeighborlyHelp.Tests
{
    public class QuestServiceTests
    {
        [Fact]
        public void HandleDialogueEnd_Quest4Completed_SwitchesToEnding()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();
            model.CurrentGameState = GameState.Quest4_Completed;
            model.IsDialogueActive = false;

            var questService = new QuestService(model);

            // Act
            questService.HandleDialogueEnd();

            // Assert
            Assert.Equal(GameState.Ending, model.CurrentGameState);
        }

        [Fact]
        public void HandleDialogueEnd_Quest1Return_StartsQuest2()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();
            model.CurrentGameState = GameState.Quest1_Return;
            model.IsDialogueActive = false;

            // Спавним Шарлотту, чтобы StartQuest2 мог её удалить
            model.SpawnNPC("Шарлотта", 0, 0, new List<string>(), "sprite1.png", 100, 100);

            var questService = new QuestService(model);

            // Act
            questService.HandleDialogueEnd();

            // Assert
            Assert.Equal(GameState.Quest2_Spawn, model.CurrentGameState);
            Assert.DoesNotContain(model.NPCs, n => n.DisplayName == "Шарлотта");
            Assert.Contains(model.NPCs, n => n.DisplayName == "Оливер");
        }

        [Fact]
        public void HandleDialogueEnd_Quest2Deliver_StartsQuest3()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();
            model.CurrentGameState = GameState.Quest2_Deliver;
            model.IsDialogueActive = false;
            model.SpawnNPC("Оливер", 0, 0, new List<string>(), "sprite2.png", 100, 100);

            var questService = new QuestService(model);

            // Act
            questService.HandleDialogueEnd();

            // Assert
            Assert.Equal(GameState.Quest3_Spawn, model.CurrentGameState);
            Assert.DoesNotContain(model.NPCs, n => n.DisplayName == "Оливер");
            Assert.Contains(model.NPCs, n => n.DisplayName == "Мелисса");
        }

        [Fact]
        public void HandleDialogueEnd_Quest3Completed_StartsQuest4()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();
            model.CurrentGameState = GameState.Quest3_Completed;
            model.IsDialogueActive = false;
            model.SpawnNPC("Мелисса", 0, 0, new List<string>(), "sprite3.png", 100, 100);

            var questService = new QuestService(model);

            // Act
            questService.HandleDialogueEnd();

            // Assert
            Assert.Equal(GameState.Quest4_Spawn, model.CurrentGameState);
            Assert.DoesNotContain(model.NPCs, n => n.DisplayName == "Мелисса");
            Assert.Contains(model.NPCs, n => n.DisplayName == "Ричард");
        }
    }
}