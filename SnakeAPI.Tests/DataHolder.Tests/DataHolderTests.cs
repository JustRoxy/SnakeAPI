using System;
using NUnit.Framework;
using SnakeAPI.Model;
using SnakeAPI.Model.Auth.DataHolder;
using SnakeAPI.Model.Game;

namespace SnakeAPI.Tests.DataHolder.Tests
{
    [TestFixture]
    public class DataHolderTests
    {
        [SetUp]
        public void Init()
        {
            _holder = new DictionaryHolder(); //oh, I don't love noDI there
            _testGuid = Guid.NewGuid().ToString();
            _testData = new Snapshot(new GameRule(new GameBoard(30, 10), 1000, 2));
        }

        private IHolder<string> _holder;
        private string _testGuid;
        private Snapshot _testData;

        [Test]
        public void Create() //is equals to [Test]Get()
        {
            _holder.Create(_testGuid, _testData);
            Assert.AreEqual(_testData, _holder.Get(_testGuid));
        }

        [Test]
        public void Delete()
        {
            _holder.Create(_testGuid, _testData);
            _holder.Delete(_testGuid);
            Assert.Throws<Exception>(() => _holder.Get(_testGuid));
        }

        [Test]
        public void Edit()
        {
            var newData = new Snapshot(new GameRule(new GameBoard(10, 100), 5000, 4));
            _holder.Create(_testGuid, _testData);

            _holder.Edit(_testGuid, newData);
            Assert.AreEqual(newData, _holder.Get(_testGuid));
        }
    }
}