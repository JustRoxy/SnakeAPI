using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SnakeAPI.Controllers;
using SnakeAPI.Model.Game.Abstractions;

namespace SnakeAPI.Tests.Controllers.Tests
{
    [TestFixture]
    internal class DirectionControllerTests
    {
        [SetUp]
        public void Init()
        {
            Init(new DefaultHttpContext());
        }

        private DirectionController _controller;

        private void Init(HttpContext context)
        {
            _controller = new DirectionController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context
                }
            };
        }

        private void Init(string cookie)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["Cookie"] = $"Game={cookie}";
            Init(context);
        }

        [Test]
        public void No_Cookie_Change_Direction()
        {
            Assert.IsInstanceOf(typeof(NotFoundResult), _controller.Post(Direction.Top));
        }

        [Test]
        public void Ok_Cookie_Change_Direction()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["Cookie"] = "Game=test";


            var game = new GameboardController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context
                }
            };
            game.Get();

            Init("test");
            Assert.IsInstanceOf(typeof(OkResult), _controller.Post(Direction.Left),
                "Сервер не применяет direction к правильному cookie игры");
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), _controller.Post(Direction.Right),
                "Сервер не отвечает на обратное направление");
        }

        [Test]
        public void Wrong_Cookie_Change_Direction()
        {
            Init("asdads");
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), _controller.Post(Direction.Top));
        }
    }
}