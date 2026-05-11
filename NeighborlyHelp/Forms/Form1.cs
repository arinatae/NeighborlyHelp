using System;
using System.Windows.Forms;
using NeighborlyHelp.Views; // ← ДОБАВЛЕНО!

namespace NeighborlyHelp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var model = new GameModel();
            var view = new GameView(null!, model);
            var controller = new GameController(model, view);
            view.Controller = controller;
            controller.StartGame();
            view.ShowDialog();
        }
    }
}