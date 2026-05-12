using System;
using System.Collections.Generic;
using NeighborlyHelp.Models;
using Xunit;

namespace NeighborlyHelp.Tests
{
    public class GameModelTests
    {
        [Fact]
        public void Initialize_PlayerCreatedWithCorrectCoordinates()
        {
            // Arrange & Act
            var model = new GameModel();
            model.Initialize();

            // Assert
            Assert.NotNull(model.Player);
            Assert.Equal(530, model.Player.X);
            Assert.Equal(450, model.Player.Y);
        }

        [Fact]
        public void SpawnKeys_AddsExactlyOneKeyItem()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();

            // Act
            model.SpawnKeys();

            // Assert
            Assert.Single(model.Collectibles);
            Assert.Equal("Ключи", model.Collectibles[0].Item.Name);
        }

        [Fact]
        public void RadioMiniGame_WinsWhenFrequencyIsClose()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();
            model.StartRadioMiniGame(800, 600);
            model.TargetFreq = 95.5f;
            model.RadioFreq = 95.0f; // Разница 0.5 < 0.8

            // Act
            model.CheckRadioGameWin();

            // Assert
            Assert.False(model.IsRadioGameActive);
            Assert.Equal(GameState.Quest4_Completed, model.CurrentGameState);
        }

        [Fact]
        public void RadioMiniGame_DoesNotWinIfFrequencyIsFar()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();
            model.StartRadioMiniGame(800, 600);
            model.TargetFreq = 95.5f;
            model.RadioFreq = 88.0f; // Большая разница

            // Act
            model.CheckRadioGameWin();

            // Assert
            Assert.True(model.IsRadioGameActive);
            Assert.NotEqual(GameState.Quest4_Completed, model.CurrentGameState);
        }

        [Fact]
        public void FlowerMiniGame_WinsWhenAllWatered()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();
            model.StartFlowerMiniGame(800, 600);

            // Поливаем все цветы до максимума
            foreach (var flower in model.Flowers)
            {
                flower.WaterLevel = 100;
            }

            // Act
            model.CheckFlowerGameWin();

            // Assert
            Assert.False(model.IsFlowerGameActive);
            Assert.Equal(GameState.Quest3_Completed, model.CurrentGameState);
        }

        [Fact]
        public void FlowerMiniGame_DoesNotWinIfNotAllWatered()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();
            model.StartFlowerMiniGame(800, 600);

            // Поливаем только один цветок
            if (model.Flowers.Count > 0)
                model.Flowers[0].WaterLevel = 100;

            // Act
            model.CheckFlowerGameWin();

            // Assert
            Assert.True(model.IsFlowerGameActive);
            Assert.NotEqual(GameState.Quest3_Completed, model.CurrentGameState);
        }

        [Fact]
        public void PlayerMovement_CannotMoveThroughWalls()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();

            int initialX = model.Player.X;

            // Act: Пытаемся двигать игрока за пределы поля (слева)
            model.MovePlayer(-100, model.Player.Y);

            // Assert: Координата X не должна измениться
            Assert.Equal(initialX, model.Player.X);
        }

        [Fact]
        public void MailboxMiniGame_CreatesCorrectNumberOfBoxes()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();

            // Act
            model.StartMailboxMiniGame(800, 600);

            // Assert
            Assert.Equal(50, model.MailOptions.Count);
            Assert.True(model.IsMiniGameActive);
        }

        [Fact]
        public void MailboxMiniGame_HasExactlyOneCorrectBox()
        {
            // Arrange
            var model = new GameModel();
            model.Initialize();
            model.StartMailboxMiniGame(800, 600);

            int correctCount = 0;
            foreach (var box in model.MailOptions)
            {
                if (box.IsCorrect) correctCount++;
            }

            // Assert
            Assert.Equal(1, correctCount);
        }
    }
}