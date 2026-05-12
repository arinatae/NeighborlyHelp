using System;
using System.Collections.Generic;
using NeighborlyHelp.Models;
using NeighborlyHelp.Services;
using Xunit;

namespace NeighborlyHelp.Tests
{
    public class DialogueServiceTests
    {
        [Fact]
        public void GetDialogueFor_Charlotte_Quest1Talk_ReturnsCorrectIntro()
        {
            // Arrange
            var dialogueService = new DialogueService();
            var charlotte = new NPC(0, 0, "Шарлотта", new List<string>(), "sprite1.png", 100, 100);

            // Act
            var result = dialogueService.GetDialogueFor(charlotte, GameState.Quest1_Talk);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.NpcLines);
            Assert.Contains("привет", result.NpcLines[0].ToLower());
        }

        [Fact]
        public void GetDialogueFor_Richard_Quest4Spawn_ReturnsRadioContext()
        {
            // Arrange
            var dialogueService = new DialogueService();
            var richard = new NPC(0, 0, "Ричард", new List<string>(), "sprite4.png", 100, 100);

            // Act
            var result = dialogueService.GetDialogueFor(richard, GameState.Quest4_Spawn);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.NpcLines);
            Assert.Contains("радио", result.NpcLines[0].ToLower());
        }

        [Fact]
        public void GetDialogueFor_Oliver_Quest2Spawn_ReturnsPackageContext()
        {
            // Arrange
            var dialogueService = new DialogueService();
            var oliver = new NPC(0, 0, "Оливер", new List<string>(), "sprite2.png", 100, 100);

            // Act
            var result = dialogueService.GetDialogueFor(oliver, GameState.Quest2_Spawn);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.NpcLines);
            Assert.Contains("посылк", result.NpcLines[0].ToLower());
        }

        [Fact]
        public void GetDialogueFor_Melissa_Quest3Spawn_ReturnsFlowerContext()
        {
            // Arrange
            var dialogueService = new DialogueService();
            var melissa = new NPC(0, 0, "Мелисса", new List<string>(), "sprite3.png", 100, 100);

            // Act
            var result = dialogueService.GetDialogueFor(melissa, GameState.Quest3_Spawn);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.NpcLines);
            Assert.Contains("цвет", result.NpcLines[0].ToLower());
        }
    }
}
