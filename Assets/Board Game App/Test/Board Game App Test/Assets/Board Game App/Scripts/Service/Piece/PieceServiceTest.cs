using ECS.EntityView.Piece;
using Moq;
using NUnit.Framework;
using Svelto.ECS;
// Using NUnit and Moq
// https://github.com/Moq/moq4/wiki/Quickstart

namespace Service.Piece
{
    [TestFixture]
    class PieceServiceTest
    {
        [Test]
        public void TestFindPieceEV()
        {
            int egid = 1234;

            var entitiesDB = new Mock<IEntitiesDB>();
            uint index = 0;
            entitiesDB.Setup(x => x.QueryEntitiesAndIndex<PieceEV>(It.IsAny<EGID>(), out index)).Returns(new PieceEV[] {
                new PieceEV { ID = new EGID(egid) }
            });

            var result = PieceService.FindPieceEV(1, entitiesDB.Object);

            entitiesDB.Verify(x => x.QueryEntitiesAndIndex<PieceEV>(It.IsAny<EGID>(), out index), Times.Once);

            Assert.AreEqual(egid, result.ID.entityID);
        }
    }
}
