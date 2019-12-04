using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SnakeAPI.Controllers;

namespace SnakeAPI.Tests.Controllers.Tests
{
    [TestFixture]
    internal class GameboardControllerTests
    {
        [SetUp]
        public void Init()
        {
            Init(new DefaultHttpContext());
        }

        private GameboardController _controller;
        private string _token;

        private string GetCookieValueFromResponse(HttpResponse response, string cookieName)
        {
            return (from headers in response.Headers.Values
                    from header in headers
                    where header.StartsWith($"{cookieName}=")
                    let p1 = header.IndexOf('=')
                    let p2 = header.IndexOf(';')
                    select header.Substring(p1 + 1, p2 - p1 - 1)
                ).FirstOrDefault();
        }

        private void Init(HttpContext context)
        {
            _controller = new GameboardController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context
                }
            };
        }

        [Test]
        public void FirstGetTest()
        {
            //Check for generation of cookie by server
            var result = _controller.Get().Value;
            _token = GetCookieValueFromResponse(_controller.Response, "Game");
            Assert.IsNotNull(_token, "Сервер не создает cookie игры");
            if (_token == null) throw new ArgumentNullException();
            Assert.IsNotNull(result);
        }

        [Test]
        public void SecondGetTest()
        {
            if (_token == null) FirstGetTest();
            //Check for not generation of another cookie by server
            var context = new DefaultHttpContext();
            context.Request.Headers["Cookie"] = $"Game={_token}";
            Init(context);

            Assert.IsNull(GetCookieValueFromResponse(_controller.Response, "Game"),
                "При повторном обращении с cookie к серверу, он создает новую");
            Thread.Sleep(5000);
            var result = _controller.Get().Value;

            Assert.AreEqual(1, result.turnNumber, "Сервер не играет :(");
        }
    }
}