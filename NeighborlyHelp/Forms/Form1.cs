using System;
using System.Windows.Forms;

namespace NeighborlyHelp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // 1. Создаём модель
            var model = new GameModel();

            // 2. Создаём вид БЕЗ контроллера (временно null)
            // Чтобы избежать исключения в конструкторе, временно передадим null! 
            // Но сразу после этого установим настоящий контроллер
            var view = new GameView(null!, model);

            // 3. Создаём контроллер, передавая ему модель и вид
            var controller = new GameController(model, view);

            // 4. Устанавливаем контроллер в вид через публичное свойство
            view.Controller = controller;

            // 5. Запускаем игру
            controller.StartGame();

            // 6. Показываем форму как модальное окно (или можно Show(), если хочешь не блокирующее)
            view.ShowDialog();
        }
    }
}