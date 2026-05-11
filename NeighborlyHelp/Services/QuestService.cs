using System;
using System.Collections.Generic;
using NeighborlyHelp.Models;

namespace NeighborlyHelp.Services
{
	public class QuestService
	{
		private readonly GameModel _model;

		public QuestService(GameModel model)
		{
			_model = model ?? throw new ArgumentNullException(nameof(model));
		}

		public void HandleDialogueEnd()
		{
			// Этот метод должен вызываться ТОЛЬКО когда диалог только что завершился
			if (!_model.IsDialogueActive)
			{
				switch (_model.CurrentGameState)
				{
					case GameState.Quest1_Talk:
						_model.CurrentGameState = GameState.Quest1_Find;
						_model.SpawnKeys();
						break;

					case GameState.Quest1_Return:
						StartQuest2();
						break;

					case GameState.Quest2_Deliver:
						StartQuest3();
						break;

					case GameState.Quest3_Completed:
						StartQuest4();
						break;

					case GameState.Quest4_Spawn:
						_model.CurrentGameState = GameState.Quest4_Talk;
						break;

					case GameState.Quest4_Completed:
						// === ВОТ ГЛАВНОЕ ИЗМЕНЕНИЕ ДЛЯ КОНЦОВКИ ===
						_model.CurrentGameState = GameState.Ending;
						break;
				}
			}
		}

		private void StartQuest2()
		{
			_model.RemoveNPC("Шарлотта"); // Или "Мила", если ты вернула старое имя
			_model.CurrentGameState = GameState.Quest2_Spawn;
			_model.SpawnNPC("Оливер", 600, 400, new List<string>
			{
				"Привет, соседка! Ты сегодня просто сияешь ярче солнышка! Я правда очень рад тебя видеть",
				"Слушай, мне неловко тебя просить, но... Не могла бы ты оказать мне одну услугу? Дело в том, что мне нужно срочно забрать посылку с почты. Но я сейчас очень занят, бегу по делам!",
				"Забери, пожалуйста, мой заказ с почтового пункта. Номер коробки - 18046. С меня шоколадка ха-ха!"
			}, "sprite2.png", 250, 250, "portrait2.png");
		}

		private void StartQuest3()
		{
			_model.RemoveNPC("Оливер");
			_model.CurrentGameState = GameState.Quest3_Spawn;
			_model.SpawnNPC("Мелисса", 150, 400, new List<string>
			{
				"Добрый денек, моя любимая соседка! Только посмотри, какие цветочки я сегодня посадила! Очень красивые, правда? Тебе нравится",
				"Я очень рада! Садоводство - это прекрасно, хоть и очень выматывает. Фух, так устала... Не могла бы ты мне помочь?",
				"Смотри, ничего сложного! Нужно просто полить каждый цветочек водой из лейки. Убедись, что воды достаточно! Я пока присяду и чуток отдохну"
			}, "sprite3.png", 160, 180, "portrait3.png");
		}

		private void StartQuest4()
		{
			_model.RemoveNPC("Мелисса");
			_model.CurrentGameState = GameState.Quest4_Spawn;
			_model.GameObjects.Add(new Radio(800, 400));
			_model.SpawnNPC("Ричард", 950, 400, new List<string>
			{
				"Ой, это ты! Спасибо, что пришла. Я помню что мы должны были сегодня слушать музыку, но у меня тут некая проблема с радио...",
				"Ты видишь, оно совсем не хочет ловить нужную частоту. Ты случайно не разбираешься в радиотехнике?",
				"О, класс, то что нужно! Помоги настроить его на 95.5 МГц! Я уверен, что ты справишься. Просто нажми на радио"
			}, "sprite4.png", 160, 180, "portrait4.png");
		}

		public void StartStory()
		{
			_model.CurrentGameState = GameState.Quest1_Talk;
			_model.SpawnNPC("Шарлотта", 1400, 500, new List<string>
			{
				"Ох, привет! Давно не виделись, соседка! Как у тебя дела, все в порядке?",
				"Знаешь, по правде говоря, у меня произошла одна неприятность. Я гуляла во дворе, и, кажется, где-то потеряла свои ключи... Теперь я не могу вернуться домой!",
				"Что же мне теперь делать? Кажется, я уже везде их посмотрела. Если тебе не сложно, помоги мне в поисках! Они такие маленькие и блестящие. Возможно, они где-то недалеко..."
			}, "sprite1.png", 270, 270, "portrait1.png");
		}
	}
}